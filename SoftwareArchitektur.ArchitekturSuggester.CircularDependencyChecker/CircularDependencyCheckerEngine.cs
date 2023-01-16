using SoftwareArchitektur.ArchitekturSuggester.CircularDependencyChecker.Interfaces;
using SoftwareArchitektur.ArchitekturSuggester.CircularDependencyChecker.Models;
using SoftwareArchitektur.Utility.Interface;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.CircularDependencyChecker;

public class CircularDependencyChecker : ICircularDependencyChecker
{
    private readonly List<CircularDependencyCheckerModel> _dependentServices;

    private readonly IList<string> _packageNameLookUp;

    private readonly IList<CircularDependencyCheckerModel> _packageModels;

    public CircularDependencyChecker(IDataProvider dataProvider)
    {
        var dependentServices = dataProvider.GetServices().Where(s => !s.IsIndependent).ToList();
        _dependentServices = dependentServices.Select(s => new CircularDependencyCheckerModel(s)).ToList();
        _packageNameLookUp = dependentServices.Select(s => s.Name).ToList();
        _packageModels = new List<CircularDependencyCheckerModel>();
    }

    public List<PackageModel> CreatePackages()
    {
        while (_dependentServices.Any())
        {
            var origin = new CircularDependencyTrackingModel(GetServiceWithMostNumberOfCalls());
            Console.WriteLine("Checking Dependencies for Service" + origin.GetBaseModelName);
            var newPackage = CreatePackage(origin);

            _packageModels.Add(newPackage.GetBaseModel);
            RemoveServicesContainedInPackage(newPackage.GetBaseModel);
        }

        return _packageModels.Select(cd => cd.ToPackageModel()).ToList();
    }

    private CircularDependencyCheckerModel? GetServiceWithMostNumberOfCalls()
    {
        var origin = _dependentServices.MaxBy(d => d.DependsOn.Sum(dr => dr.NumberOfCalls));
        return origin;
    }

    private CircularDependencyTrackingModel CreatePackage(CircularDependencyTrackingModel checkerModel)
    {
        while (checkerModel.DependsOn.Count > 0)
        {
            var dependency = checkerModel.DependsOn[0];
            checkerModel.DependsOn.RemoveAt(0);
            Console.WriteLine($"Check Dependency: {dependency.Callee},  {checkerModel.DependsOn.Count} Dependencies Left");

            var calledService = _dependentServices.FirstOrDefault(s => s.BaseServiceModel.Name == dependency.Callee);

            if (calledService == null)
            {
                if (_packageNameLookUp.Any(s => s == dependency.Callee))
                    calledService = _packageModels.First(c => c.Contains.Any(p => p.BaseServiceModel.Name == dependency.Callee));
                else
                    continue;
            }

            checkerModel.AddToVisited(calledService);

            if (checkerModel.HasDuplicate)
            {
                var duplicateSlice = checkerModel.GetDuplicateSlice();

                foreach (var dependencyCheckerModel in duplicateSlice)
                {
                    checkerModel.GetBaseModel.EatDifferentModels(dependencyCheckerModel);
                    checkerModel.Visited.RemoveAt(checkerModel.Visited.Count - 1);
                }
            }
            else
            {
                CreatePackage(checkerModel);
            }
        }

        if (checkerModel.Visited.Count > 1) checkerModel.Visited.RemoveAt(checkerModel.Visited.Count - 1);


        return checkerModel;
    }

    private void RemoveServicesContainedInPackage(CircularDependencyCheckerModel newPackage)
    {
        foreach (var service in newPackage.Contains)
        {
            _dependentServices.Remove(
                _dependentServices.First(s =>
                    s.BaseServiceModel.Name == service.BaseServiceModel.Name));
        }
    }


    private void ConsumePackageWithCircularDependency(CircularDependencyCheckerModel checkerModel,
        CircularDependencyRelationModel dependency)
    {
        var packageToEat =
            _packageModels.First(p => p.Contains.Any(s => s.BaseServiceModel.Name == dependency.Callee));
        Console.WriteLine(
            $"Possible Circular Dependency For Package {checkerModel.PackageName} With Package {packageToEat.PackageName}, From Service {dependency.Caller} To {dependency.Callee}");

        checkerModel.EatDifferentModels(packageToEat);
        _packageModels.Remove(packageToEat);
    }

    private bool CheckIfDependencyIsAlreadyInternal(CircularDependencyCheckerModel checkerModel,
        CircularDependencyRelationModel dependency)
    {
        return checkerModel.Contains.Any(s => s.BaseServiceModel.Name == dependency.Callee);
    }

    private bool CheckIfDependencyIsAlreadyAdded(CircularDependencyRelationModel dependency)
    {
        return _packageNameLookUp.FirstOrDefault(s => s == dependency.Callee) != null;
    }
}