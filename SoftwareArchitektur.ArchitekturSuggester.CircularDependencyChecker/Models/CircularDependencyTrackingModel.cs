namespace SoftwareArchitektur.ArchitekturSuggester.CircularDependencyChecker.Models;

internal class CircularDependencyTrackingModel
{
    private CircularDependencyCheckerModel BaseModel { get; set; }

    public string GetBaseModelName => BaseModel.PackageName;

    public CircularDependencyCheckerModel GetBaseModel => BaseModel;

    public List<CircularDependencyRelationModel> DependsOn => Visited.LastOrDefault()?.DependsOn;

    public bool HasDuplicate => Visited.Count != Visited.Distinct().Count();

    public readonly List<CircularDependencyCheckerModel> Visited = new();

    public CircularDependencyTrackingModel(CircularDependencyCheckerModel baseModel)
    {
        BaseModel = baseModel;
        Visited.Add(baseModel);
    }

    public void AddToVisited(CircularDependencyCheckerModel visitedModel)
    {
        Visited.Add(visitedModel);
    }

    private void DigestDependencies(CircularDependencyCheckerModel eatenCheckerModel)
    {
        var newDependencies = Visited
            .SelectMany(d => d.DependsOn)
            .UnionBy(eatenCheckerModel.Contains.SelectMany(d => d.DependsOn), d => d.Callee)
            .ToList();

        BaseModel.DependsOn.AddRange(newDependencies);

        BaseModel.DependsOn.DistinctBy(d => d.Callee);
    }

    public List<CircularDependencyCheckerModel> GetDuplicateSlice()
    {
        var duplicates = Visited
            .GroupBy(s => s.PackageName)
            .OrderByDescending(g => g.Count())
            .First();

        var startIndex = Visited.FindIndex(d => duplicates.First().PackageName == d.PackageName);
        var lastIndex = Visited.FindLastIndex(d => duplicates.Last().PackageName == d.PackageName);
        return Visited.GetRange(startIndex, lastIndex - startIndex);
    }
}