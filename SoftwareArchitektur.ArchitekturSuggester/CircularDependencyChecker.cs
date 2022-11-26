using SoftwareArchitektur.ArchitekturSuggester.Models;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester;

public class CircularDependencyChecker
{
    private readonly List<CircularDependencyModel> _dependentServices = new List<CircularDependencyModel>();

    public CircularDependencyChecker(List<ServiceModel> dependentServices)
    {

        _dependentServices = dependentServices.Select(s => new CircularDependencyModel(s)).ToList();
    }

    private PackageModel TransformInternalToExternalModel(CircularDependencyModel internalModel)
    {

        return internalModel.ToPackageModel();
    }

    public List<PackageModel> CreatePackages()
    {
        var result = new List<PackageModel>();
        var origin = _dependentServices.MaxBy(d => d.DependsOn.Sum(dr => dr.NumberOfCalls));
        result.Add(CreatePackageRecursive(origin!).ToPackageModel());
        return result;
    }

    private CircularDependencyModel CreatePackageRecursive(CircularDependencyModel model)
    {

        while (model.DependsOn.Count > 0)
        {
            var dependency = model.DependsOn[0];
            var calledService = _dependentServices.FirstOrDefault(s => s.BaseServiceModel.Name == dependency.Callee);

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

        public readonly List<CircularDependencyRelationModel> DependsOn = new List<CircularDependencyRelationModel>();

        public CircularDependencyModel(ServiceModel model)
        {
            BaseServiceModel = model;
            counter++;
            PackageName = $"Package{counter}";
            DependsOn = model.DependsOn.Select(d => new CircularDependencyRelationModel(d)).ToList();
            Visited.Add(this);
        }

        public PackageModel ToPackageModel()
        {
            var packageModel = new PackageModel(PackageName);
            packageModel.Services.Add(BaseServiceModel);
            foreach (var eatenService in Eaten)
            {
                packageModel.Services.Add(eatenService.BaseServiceModel);
            }

            return packageModel;
        }

        public void EatDifferentModels(CircularDependencyModel model)
        {
            Visited.Select(v => v.PackageName = this.PackageName);
            Eaten.AddRange(Visited);
            DigestDependencies(model);
        }

        private void DigestDependencies(CircularDependencyModel model)
        {
            foreach (var dependencyRelation in model.DependsOn)
            {
                if (!Eaten.Any(e => e.BaseServiceModel.Name == dependencyRelation.Callee))
                {
                    model.DependsOn.Add(dependencyRelation);
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