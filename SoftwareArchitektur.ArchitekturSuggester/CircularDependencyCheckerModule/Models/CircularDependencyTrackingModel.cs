namespace SoftwareArchitektur.ArchitekturSuggester.CircularDependencyCheckerModule.Models;

internal class CircularDependencyTrackingModel
{
    private CircularDependencyChecker.CircularDependencyCheckerModel BaseModel { get; set; }

    public string GetBaseModelName => BaseModel.PackageName;

    public CircularDependencyChecker.CircularDependencyCheckerModel GetBaseModel => BaseModel;

    public List<CircularDependencyRelationModel> DependsOn => Visited.LastOrDefault()?.DependsOn;

    public bool HasDuplicate => Visited.Count != Visited.Distinct().Count();

    public readonly List<CircularDependencyChecker.CircularDependencyCheckerModel> Visited = new();

    public CircularDependencyTrackingModel(CircularDependencyChecker.CircularDependencyCheckerModel baseModel)
    {
        BaseModel = baseModel;
        Visited.Add(baseModel);
    }

    public void AddToVisited(CircularDependencyChecker.CircularDependencyCheckerModel visitedModel)
    {
        Visited.Add(visitedModel);
        DigestDependencies(visitedModel);
    }

    private void DigestDependencies(CircularDependencyChecker.CircularDependencyCheckerModel eatenCheckerModel)
    {
        var newDependencies = Visited.SelectMany(d => d.DependsOn)
            .UnionBy(eatenCheckerModel.Contains.SelectMany(d => d.DependsOn), d => d.Callee);
        BaseModel.DependsOn.UnionBy(newDependencies, d => d.Callee);
        BaseModel.DependsOn.DistinctBy(d => d.Callee);
    }

    public List<CircularDependencyChecker.CircularDependencyCheckerModel> GetDuplicateSlice()
    {
        var duplicates = Visited
            .GroupBy(s => s.PackageName)
            .OrderByDescending(g => g.Count())
            .First();

        var startIndex = Visited.FindIndex(d => duplicates.First().PackageName == d.PackageName);
        var lastIndex = Visited.FindLastIndex(d => duplicates.Last().PackageName == d.PackageName);
        return Visited.GetRange(startIndex, lastIndex-startIndex);
    }
}