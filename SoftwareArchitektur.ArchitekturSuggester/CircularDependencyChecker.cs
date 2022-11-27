using SoftwareArchitektur.ArchitekturSuggester.Models;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester;

public class CircularDependencyChecker
{
    private readonly List<CircularDependencyModel> _dependentServices;
  
    
    public CircularDependencyChecker(List<ServiceModel> dependentServices)
    {
       _dependentServices = dependentServices.Select(s => new CircularDependencyModel(s)).ToList();
    }

    public List<PackageModel> CreatePackages()
    {
        var result = new List<PackageModel>();
        //todo ForEach until limit or no change

        while (_dependentServices.Any())
        {
            var origin = _dependentServices.MaxBy(d => d.DependsOn.Sum(dr => dr.NumberOfCalls));
            var newPackage = CreatePackageRecursive(origin!).ToPackageModel();
            result.Add(newPackage);
            foreach (var service in newPackage.GetServices())
            {
                _dependentServices.Remove(_dependentServices.First( s=> s.BaseServiceModel.Name == service.Name));
            }
        }
       
       
        return result;
    }

    private CircularDependencyModel CreatePackageRecursive(CircularDependencyModel model)
    {

        while (model.DependsOn.Count > 0)
        {
            var dependency = model.DependsOn[0];
            var calledService = _dependentServices.FirstOrDefault(s => s.BaseServiceModel.Name == dependency.Callee);

            if (calledService == null)
            {
                model.DependsOn.Remove(dependency);
                continue;
            }
            
            if (model.Visited.Any(d => dependency.Callee == d.BaseServiceModel.Name))
            {
                model.EatDifferentModels(calledService);
            }
            else
            {
                model.Visited.Add(calledService);
                model.DependsOn.AddRange(calledService.DependsOn);
            }

            model.DependsOn.Remove(dependency);
        }

        return model;
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
            Visited.Add(this);
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
            Visited.Select(v => v.PackageName = this.PackageName);

            foreach (var visited in Visited)
            {
                if (Eaten.FirstOrDefault(s => s.BaseServiceModel.Name == visited.BaseServiceModel.Name) == null)
                {
                    Eaten.Add(visited);
                }
            }
            DigestDependencies(eatenModel);
        }

        private void DigestDependencies(CircularDependencyModel eatenModel)
        {
            for (int i = 0; i< eatenModel.DependsOn.Count; i++)
            {
                var dependencyRelation = eatenModel.DependsOn[i];
                if (!Eaten.Any(e => e.BaseServiceModel.Name == dependencyRelation.Callee) && DependsOn.FirstOrDefault(d => d.Callee == dependencyRelation.Callee) == null )
                {
                    DependsOn.Add(dependencyRelation);
                }
            }
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