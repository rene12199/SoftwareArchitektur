using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.CircularDependencyChecker.Models;

internal class CircularDependencyCheckerModel : IEquatable<CircularDependencyCheckerModel>
{
    public string PackageName { get; set; } = string.Empty;

    public List<CircularDependencyCheckerModel> Contains => GetAllContained();

    public readonly List<CircularDependencyCheckerModel> Eaten = new();

    private static int _counter;

    public ServiceModel BaseServiceModel { get; private set; }

    //todo Create Limit on minimum number of calls that it has to think about

    public readonly List<CircularDependencyRelationModel> DependsOn = new();

    public bool Equals(CircularDependencyCheckerModel? other)
    {
        if (other == null) return false;

        return BaseServiceModel.Name == other.BaseServiceModel.Name;
    }

    private List<CircularDependencyCheckerModel> GetAllContained()
    {
        var newList = new List<CircularDependencyCheckerModel>();
        newList.AddRange(Eaten);
        newList.Add(this);
        return newList;
    }

    public CircularDependencyCheckerModel(ServiceModel model)
    {
        BaseServiceModel = model;
        _counter++;
        PackageName = $"Package{_counter}";
        DependsOn.AddRange(model.DependsOn.Select(d => new CircularDependencyRelationModel(d)).ToList());
    }

    public PackageModel ToPackageModel()
    {
        var packageModel = new PackageModel(PackageName);
        foreach (var eatenService in Contains) packageModel.AddService(eatenService.BaseServiceModel);

        return packageModel;
    }

    public void EatDifferentModels(CircularDependencyCheckerModel eatenCheckerModel)
    {
        if (BaseServiceModel == eatenCheckerModel.BaseServiceModel) return;

        eatenCheckerModel.Eaten.ForEach(v => v.PackageName = PackageName);

        ConsumeModelAndVisited(eatenCheckerModel);

        DigestDependencies(eatenCheckerModel);

        ClearDataInEatenModel(eatenCheckerModel);
    }

    private static void ClearDataInEatenModel(CircularDependencyCheckerModel eatenCheckerModel)
    {
        eatenCheckerModel.Eaten.Clear();
        eatenCheckerModel.DependsOn.Clear();
    }

    private void ConsumeModelAndVisited(CircularDependencyCheckerModel eatenCheckerModel)
    {
        Eaten.AddRange(eatenCheckerModel.Contains.Except(Contains));
        Eaten.Remove(this);
    }

    private void DigestDependencies(CircularDependencyCheckerModel eatenCheckerModel)
    {
        var newDependencies = eatenCheckerModel.DependsOn;
        //DependsOn.Where(d => !Contains.Any(m => m.PackageName == d.Callee)).UnionBy(newDependencies, d => d.Callee);
        DependsOn.AddRange(newDependencies);
        DependsOn.DistinctBy(d => d.Callee);
    }
}