using SoftwareArchitektur.ArchitekturSuggester.Models;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester;

public class CircularDependencyChecker
{
    private readonly List<CircularDependencyModel> _dependentServices;

    private readonly List<string> _packageNameLookUp;

    private List<CircularDependencyModel> _packageModels;


    public CircularDependencyChecker(List<ServiceModel> dependentServices)
    {
        _dependentServices = dependentServices.Select(s => new CircularDependencyModel(s)).ToList();
        _packageNameLookUp = dependentServices.Select(s => s.Name).ToList();
    }

    public List<PackageModel> CreatePackages()
    {
        _packageModels = new List<CircularDependencyModel>();
        //todo ForEach until limit or no change

        while (_dependentServices.Any())
        {
            var origin = _dependentServices.MaxBy(d => d.DependsOn.Sum(dr => dr.NumberOfCalls));
            Console.WriteLine("Checking Dependencies for Service" + origin.BaseServiceModel.Name);
            var newPackage = CreatePackageRecursive(origin!);
            _packageModels.Add(newPackage);
            newPackage.Eaten.Add(newPackage);
            foreach (var service in newPackage.Eaten)
            {
                if (newPackage.Eaten.Count < 1)
                {
                    _dependentServices.Remove(_dependentServices.First(s =>
                        s.BaseServiceModel.Name == newPackage.BaseServiceModel.Name));
                }

                _dependentServices.Remove(_dependentServices.FirstOrDefault(s =>
                    s.BaseServiceModel.Name == service.BaseServiceModel.Name));
            }
        }

        var packageLookUp = _packageModels.Select(p => p.PackageName);

        foreach (var models in _packageModels)
        {
            var origin = models;
            Console.WriteLine("Checking Dependencies for Service" + origin.BaseServiceModel.Name);
            var newPackage = CreatePackageRecursive(origin!);
        }

        Console.WriteLine("Finished Circle Check");

        return _packageModels.Select(cd => cd.ToPackageModel()).ToList();
    }

    private CircularDependencyModel CreatePackageRecursive(CircularDependencyModel model)
    {
        var counter = 0;
        while (model.DependsOn.Count > 0)
        {
            
            var dependency = model.DependsOn[0];
            counter++;
            Console.WriteLine($"Check Dependency: " + dependency.Callee+" " + counter);
            var calledService = _dependentServices.FirstOrDefault(s => s.BaseServiceModel.Name == dependency.Callee);

            if (calledService == null)
            {
                var isAlreadyAdded =
                    _packageNameLookUp.FirstOrDefault(s => s == dependency.Callee) != null;

                if (isAlreadyAdded)
                {
                    var packageToEat =
                        _packageModels.First(p => p.Eaten.Any(s => s.BaseServiceModel.Name == dependency.Callee));
                    Console.WriteLine(
                        $"Possible Circular Dependency For Package {model.PackageName} With Package {packageToEat.PackageName}, From Service {dependency.Caller} To {dependency.Callee}");

                    model.EatDifferentModels(packageToEat);
                }

                model.DependsOn.Remove(dependency);
                continue;
            }

            if (model.Visited.Any(d => dependency.Callee == d.BaseServiceModel.Name))
            {
                model.EatDifferentModels(calledService);
            }
            else
            {
                AddToVisited(model, calledService);
            }

            model.DependsOn.Remove(dependency);
        }

        return model;
    }

    private static void AddToVisited(CircularDependencyModel model, CircularDependencyModel calledService)
    {
        model.Visited.Add(calledService);
        model.DependsOn.AddRange(calledService.DependsOn);
    }

    internal class CircularDependencyModel
    {
        public static int counter = 0;

        public string PackageName = "";

        public readonly List<CircularDependencyModel> Visited = new List<CircularDependencyModel>();

        public readonly List<CircularDependencyModel> Eaten = new List<CircularDependencyModel>();

        public ServiceModel BaseServiceModel { get; private set; }

        //todo Create Limit on minimum number of calls that it has to think about
        public readonly List<CircularDependencyRelationModel> DependsOn = new List<CircularDependencyRelationModel>();

        public CircularDependencyModel(ServiceModel model)
        {
            BaseServiceModel = model;
            counter++;
            PackageName = $"Package{counter}";
            DependsOn.AddRange(model.DependsOn.Select(d => new CircularDependencyRelationModel(d)).ToList());
            
            Eaten.Add(this);
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

            Visited.Select(v => v.PackageName = this.PackageName);
            
            EatVisitedModel();
            
            DigestDependencies(eatenModel);
        }

        private void EatVisitedModel()
        {
            foreach (var visited in Visited)
            {
                CheckForDuplicatesInEatenModel(visited);
            }
        }

        private void CheckForDuplicatesInEatenModel(CircularDependencyModel visited)
        {
            if (Eaten.FirstOrDefault(s => s.BaseServiceModel.Name == visited.BaseServiceModel.Name) == null)
            {
                Eaten.Add(visited);
            }
        }

        private void DigestDependencies(CircularDependencyModel eatenModel)
        {
            for (int i = 0; i < eatenModel.DependsOn.Count; i++)
            {
                var dependencyRelation = eatenModel.DependsOn[i];
                if (CheckIfDependsOnEatenService(dependencyRelation))
                {
                    this.DependsOn.Add(dependencyRelation);
                }
            }
        }

        private bool CheckIfDependsOnEatenService(CircularDependencyRelationModel dependencyRelation)
        {
            return !Eaten.Any(e => e.BaseServiceModel.Name == dependencyRelation.Callee);
        }

        private bool CheckIfDuplicateDependency(CircularDependencyRelationModel dependencyRelation)
        {
            return DependsOn.FirstOrDefault(d => d.Callee == dependencyRelation.Callee) == null;
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