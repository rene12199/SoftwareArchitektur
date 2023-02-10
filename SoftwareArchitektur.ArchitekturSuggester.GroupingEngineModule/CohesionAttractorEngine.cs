using LouvainCommunityPL;
using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine;

public class CohesionAttractorEngine
{
    private IList<GroupingPackageModel> _packageLookup = new List<GroupingPackageModel>();

    private readonly Dictionary<int, GroupingPackageModel> _packageToNodeDict = new();

    private int _counter;
    public void SetPackageLookup(IList<GroupingPackageModel> packageLookup)
    {
        _packageLookup = packageLookup;
    }

    public IList<MergeRequestModel> GroupPackages(IList<GroupingPackageModel> models)
    {
        if (_packageLookup.Count == 0)
        {
            throw new Exception("Package Lookup is not set");
        }

        var graph = new Graph();

        foreach (var groupingPackageModel in models)
        {
            _packageToNodeDict.Add(_counter, groupingPackageModel);
            graph.AddNode(_counter);
            _counter++;
        }

        var commonChanges = models.SelectMany(m => m.ChangesWith).Where(c => _packageToNodeDict.Any(n => n.Value.PackageName == c.OtherPackage.PackageName));

        NormalizeChangeCallsToWeights(commonChanges, graph);

        var partition = Community.BestPartition(graph).GroupBy(x => x.Value);
        var mergeRequests = new List<MergeRequestModel>();

        foreach (var community in partition)
        {
            mergeRequests.AddRange(CreateMergeRequestFromCommunity(community));
        }

        ResetState();
        
        return mergeRequests;
    }

    private void ResetState()
    {
        _packageToNodeDict.Clear();
        _counter = 0;
    }

    private void NormalizeChangeCallsToWeights(IEnumerable<GroupingCommonChangeModel> changes, Graph graph)
    {
        var highestCallNumber = changes.Max(m => m.NumberOfChanges);

        foreach (var commonChangeModel in changes)
        {
            graph.AddEdge(
                _packageToNodeDict.First(x => x.Value.PackageName == commonChangeModel.ThisPackage.PackageName).Key,
                _packageToNodeDict.First(x => x.Value.PackageName == commonChangeModel.OtherPackage.PackageName).Key,
                highestCallNumber - commonChangeModel.NumberOfChanges + 1);
        }
    }

    private IList<MergeRequestModel> CreateMergeRequestFromCommunity(IGrouping<int, KeyValuePair<int, int>> community)
    {
        var mergeRequests = new List<MergeRequestModel>();

        var baseActor = _packageToNodeDict[community.First().Key];

        foreach (var package in community)
        {
            var toBeMerged = _packageToNodeDict[package.Key];

            if (toBeMerged != baseActor)
            {
                mergeRequests.Add(new MergeRequestModel(baseActor, toBeMerged));
            }
        }

        return mergeRequests;
    }
}