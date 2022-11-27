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

        while (_dependentServices.Any())
        {
            var origin = _dependentServices.MaxBy(d => d.DependsOn.Sum(dr => dr.NumberOfCalls));
            Console.WriteLine("Checking Dependencies for Service" + origin.BaseServiceModel.Name);
            var newPackage = CreatePackageRecursive(origin!);
            _packageModels.Add(newPackage);
            newPackage.Eaten.Add(newPackage);

            foreach (var service in newPackage.Eaten)
            {
                _dependentServices.Remove(_dependentServices.FirstOrDefault(s =>
                    s.BaseServiceModel.Name == service.BaseServiceModel.Name));
            }
        }

        return _packageModels.Select(cd => cd.ToPackageModel()).ToList();
    }

    private CircularDependencyModel CreatePackageRecursive(CircularDependencyModel model)
    {
        var counter = 0;
        while (model.DependsOn.Count > 0)
        {
            var dependency = model.DependsOn[0];
            counter++;
            Console.WriteLine($"Check Dependency: " + dependency.Callee + " " + counter + " " + model.DependsOn.Count);
            var calledService = _dependentServices.FirstOrDefault(s => s.BaseServiceModel.Name == dependency.Callee);

            if (calledService == null)
            {
                var isAlreadyAdded =
                    _packageNameLookUp.FirstOrDefault(s => s == dependency.Callee) != null;
                
                var isInSelf = model.Contains.Any(s=> s.BaseServiceModel.Name == dependency.Callee);

                if (isAlreadyAdded && !isInSelf)
                {

                    var packageToEat =
                        _packageModels.First(p => p.Contains.Any(s => s.BaseServiceModel.Name == dependency.Callee));
                    Console.WriteLine(
                        $"Possible Circular Dependency For Package {model.PackageName} With Package {packageToEat.PackageName}, From Service {dependency.Caller} To {dependency.Callee}");

                    model.EatDifferentModels(packageToEat);
                    _packageModels.Remove(packageToEat);
                }
            }

            else if (model.Visited.Any(d => dependency.Callee == d.BaseServiceModel.Name))
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

    internal class CircularDependencyModel : IEquatable<CircularDependencyModel>
    {
        public bool Equals(CircularDependencyModel? other)
        {
            return this.BaseServiceModel.Name == other.BaseServiceModel.Name;
        }

        public static int counter = 0;

        public string PackageName
        {
            get;
            set;
        } = string.Empty;

        public readonly List<CircularDependencyModel> Visited = new List<CircularDependencyModel>();

        public readonly List<CircularDependencyModel> Eaten = new List<CircularDependencyModel>();

        public List<CircularDependencyModel> Contains => GetAllContained();

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
            counter++;
            PackageName = $"Package{counter}";
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

            var list = new List<CircularDependencyRelationModel>();

            var newDependencies = eatenModel.Visited.SelectMany(d => d.DependsOn)
                .UnionBy(eatenModel.Contains.SelectMany(d => d.DependsOn), d => d.Callee);
            DependsOn.UnionBy(newDependencies, d => d.Callee);
            DependsOn.DistinctBy(d => d.Callee);

            this.Eaten.AddRange(eatenModel.Contains.Except(Contains));
            this.Eaten.AddRange(Visited.Except(Eaten));

            Eaten.Remove(this);

            eatenModel.Eaten.Clear();
            eatenModel.Visited.Clear();
            eatenModel.DependsOn.Clear();
        }

        private bool CheckIfDependsOnEatenService(CircularDependencyRelationModel dependencyRelation)
        {
            return !Eaten.Any(e => e.BaseServiceModel.Name == dependencyRelation.Callee);
        }

        private bool CheckIfDuplicateDependency(CircularDependencyRelationModel dependencyRelation)
        {
            return DependsOn.FirstOrDefault(d => d.Callee == dependencyRelation.Callee) != null;
        }
    }

    internal class CircularDependencyRelationModel
    {
        public readonly string Caller;
        public readonly string Callee;
        public readonly long NumberOfCalls;

        public static int counte = 0;

        public CircularDependencyRelationModel(DependencyRelationModel dependencyRelationModel)
        {
            counte++;
            Callee = dependencyRelationModel.Callee;
            Caller = dependencyRelationModel.Caller;
            NumberOfCalls = dependencyRelationModel.NumberOfCalls;
        }
    }
}