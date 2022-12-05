using SoftwareArchitektur.ArchitekturSuggester.Models;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester;

public class CircularDependencyChecker
{
    private readonly List<CircularDependencyModel> _dependentServices;

    private readonly List<string> _packageNameLookUp;

    private readonly List<CircularDependencyModel> _packageModels;


    public CircularDependencyChecker(List<ServiceModel> dependentServices)
    {
        _dependentServices = dependentServices.Select(s => new CircularDependencyModel(s)).ToList();
        _packageNameLookUp = dependentServices.Select(s => s.Name).ToList();
        _packageModels = new List<CircularDependencyModel>();
    }

    public List<PackageModel> CreatePackages()
    {
        while (_dependentServices.Any())
        {
            var origin = GetServiceWithMostNumberOfCalls();
            Console.WriteLine("Checking Dependencies for Service" + origin.BaseServiceModel.Name);
            var newPackage = CreatePackageRecursive(origin!);

            //todo check if removing this changes anything
            newPackage.Eaten.Add(newPackage);
            _packageModels.Add(newPackage);
            RemoveServicesContainedInPackage(newPackage);
        }

        return _packageModels.Select(cd => cd.ToPackageModel()).ToList();
    }

    private void RemoveServicesContainedInPackage(CircularDependencyModel newPackage)
    {
        foreach (var service in newPackage.Eaten)
        {
            _dependentServices.Remove(_dependentServices.FirstOrDefault(s =>
                s.BaseServiceModel.Name == service.BaseServiceModel.Name));
        }
    }

    private CircularDependencyModel? GetServiceWithMostNumberOfCalls()
    {
        var origin = _dependentServices.MaxBy(d => d.DependsOn.Sum(dr => dr.NumberOfCalls));
        return origin;
    }

    //Todo Create Tracking Model instead of using 
    private CircularDependencyModel CreatePackageRecursive(CircularDependencyModel model)
    {
        while (model.DependsOn.Count > 0)
        {
            var dependency = model.DependsOn[0];
            Console.WriteLine($"Check Dependency: {dependency.Callee},  {model.DependsOn.Count} Dependencies Left");
            var calledService = _dependentServices.FirstOrDefault(s => s.BaseServiceModel.Name == dependency.Callee);

            ProcessDependency(model, calledService, dependency);

            model.DependsOn.Remove(dependency);
        }

        return model;
    }

    private void ProcessDependency(CircularDependencyModel model, CircularDependencyModel? calledService,
        CircularDependencyRelationModel dependency)
    {
        if (calledService == null)
        {
            ProcessDependencyInDifferentPackage(model, dependency);
        }
        else if (model.Visited.Any(d => dependency.Callee == d.BaseServiceModel.Name))
        {
            model.EatDifferentModels(calledService);
        }
        else
        {
            AddToVisited(model, calledService);
        }
    }

    private void ProcessDependencyInDifferentPackage(CircularDependencyModel model,
        CircularDependencyRelationModel dependency)
    {
        if (CheckIfDependencyIsAlreadyAdded(dependency) &&
            !CheckIfDependencyIsAlreadyInternal(model, dependency))
        {
            ConsumePackageWithCircularDependency(model, dependency);
        }
    }

    private void ConsumePackageWithCircularDependency(CircularDependencyModel model,
        CircularDependencyRelationModel dependency)
    {
        var packageToEat =
            _packageModels.First(p => p.Contains.Any(s => s.BaseServiceModel.Name == dependency.Callee));
        Console.WriteLine(
            $"Possible Circular Dependency For Package {model.PackageName} With Package {packageToEat.PackageName}, From Service {dependency.Caller} To {dependency.Callee}");

        model.EatDifferentModels(packageToEat);
        _packageModels.Remove(packageToEat);
    }

    private static bool CheckIfDependencyIsAlreadyInternal(CircularDependencyModel model,
        CircularDependencyRelationModel dependency)
    {
        return model.Contains.Any(s => s.BaseServiceModel.Name == dependency.Callee);
    }

    private bool CheckIfDependencyIsAlreadyAdded(CircularDependencyRelationModel dependency)
    {
        return _packageNameLookUp.FirstOrDefault(s => s == dependency.Callee) != null;
    }

    private static void AddToVisited(CircularDependencyModel model, CircularDependencyModel calledService)
    {
        model.Visited.Add(calledService);
        model.DependsOn.AddRange(calledService.DependsOn);
    }

    internal class CircularDependencyModel : IEquatable<CircularDependencyModel>
    {
        public bool Equals(CircularDependencyModel? other)
        {
            if (other == null)
            {
                return false;
            }

            return BaseServiceModel.Name == other.BaseServiceModel.Name;
        }

        public string PackageName { get; set; } = string.Empty;

        public List<CircularDependencyModel> Contains => GetAllContained();

        public readonly List<CircularDependencyModel> Visited = new List<CircularDependencyModel>();

        public readonly List<CircularDependencyModel> Eaten = new List<CircularDependencyModel>();

        private static int _counter = 0;

        private List<CircularDependencyModel> GetAllContained()
        {
            var newList = new List<CircularDependencyModel>();
            newList.AddRange(Eaten);
            newList.Add(this);
            return newList;
        }

        public ServiceModel BaseServiceModel { get; private set; }

        //todo Create Limit on minimum number of calls that it has to think about
        public readonly List<CircularDependencyRelationModel> DependsOn = new List<CircularDependencyRelationModel>();

        public CircularDependencyModel(ServiceModel model)
        {
            BaseServiceModel = model;
            _counter++;
            PackageName = $"Package{_counter}";
            DependsOn.AddRange(model.DependsOn.Select(d => new CircularDependencyRelationModel(d)).ToList());
        }

        public PackageModel ToPackageModel()
        {
            var packageModel = new PackageModel(PackageName);
            foreach (var eatenService in Eaten)
            {
                packageModel.AddService(eatenService.BaseServiceModel);
            }

            if (packageModel.GetServices().Count == 0)
            {
                packageModel.AddService(BaseServiceModel);
            }

            return packageModel;
        }

        public void EatDifferentModels(CircularDependencyModel eatenModel)
        {
            if (BaseServiceModel == eatenModel.BaseServiceModel)
            {
                return;
            }

            eatenModel.Visited.ForEach(v => v.PackageName = this.PackageName);
            eatenModel.Eaten.ForEach(v => v.PackageName = this.PackageName);

            DigestDependencies(eatenModel);

            ConsumeModelAndVisited(eatenModel);

            ClearDataInEatenModel(eatenModel);
        }

        private static void ClearDataInEatenModel(CircularDependencyModel eatenModel)
        {
            eatenModel.Eaten.Clear();
            eatenModel.Visited.Clear();
            eatenModel.DependsOn.Clear();
        }

        private void ConsumeModelAndVisited(CircularDependencyModel eatenModel)
        {
            Eaten.AddRange(eatenModel.Contains.Except(Contains));
            Eaten.AddRange(Visited.Except(Eaten));
            Eaten.Remove(this);
        }

        private void DigestDependencies(CircularDependencyModel eatenModel)
        {
            var newDependencies = eatenModel.Visited.SelectMany(d => d.DependsOn)
                .UnionBy(eatenModel.Contains.SelectMany(d => d.DependsOn), d => d.Callee);
            DependsOn.UnionBy(newDependencies, d => d.Callee);
            DependsOn.DistinctBy(d => d.Callee);
        }
    }

    internal class CircularDependencyRelationModel
    {
        public readonly string Caller;
        public readonly string Callee;
        public readonly long NumberOfCalls;

        public CircularDependencyRelationModel(DependencyRelationModel dependencyRelationModel)
        {
            Callee = dependencyRelationModel.Callee;
            Caller = dependencyRelationModel.Caller;
            NumberOfCalls = dependencyRelationModel.NumberOfCalls;
        }
    }
}