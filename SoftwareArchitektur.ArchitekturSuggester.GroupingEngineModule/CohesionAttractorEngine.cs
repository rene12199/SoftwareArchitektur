using MNCD.CommunityDetection.SingleLayer;
using MNCD.Core;
using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine;

public class CohesionAttractorEngine
{
    private IList<GroupingPackageModel> _packageLookup = new List<GroupingPackageModel>();

    public IList<MergeRequestModel> GroupPackages(IList<GroupingPackageModel> models)
    {
        _packageLookup = models;
        var packageName = models.Select(m => m.PackageName);
        var commonChanges = models.SelectMany(m => m.ChangesWith).Where(c => packageName.Any(n => n == c.OtherPackage.PackageName));

        var vertices = models.Select(package => new Actor(package.PackageName));

        var edge = NormalizeChangeCallsToWeights(commonChanges, vertices);
        ;

        var layer = new List<Layer>
        {
            new("Layer0") { Edges = edge.ToList() }
        };

        var network = new Network(layer.ToList(), vertices.ToList());

        var louvain = new FluidC();
        var communities = louvain.Compute(network, 20);

        var mergeRequests = new List<MergeRequestModel>();

        foreach (var community in communities) mergeRequests.AddRange(CreateMergeRequestFromCommunity(community));

        return mergeRequests;
    }

    private IEnumerable<Edge> NormalizeChangeCallsToWeights(IEnumerable<GroupingCommonChangeModel> changes, IEnumerable<Actor> vertices)
    {
        var highestCallNumber = changes.Max(m => m.NumberOfChanges);

        return changes.Select(change => new Edge(vertices.First(m => m.Name == change.ThisPackage.PackageName),
            vertices.First(m => m.Name == change.OtherPackage.PackageName), highestCallNumber - change.NumberOfChanges));
    }

    private IList<MergeRequestModel> CreateMergeRequestFromCommunity(Community community)
    {
        var mergeRequests = new List<MergeRequestModel>();
        var baseActor = _packageLookup.First(m => m.PackageName == community.Actors.First().Name);

        foreach (var actor in community.Actors)
        {
            var package = _packageLookup.First(m => m.PackageName == actor.Name);
            if (package != baseActor) mergeRequests.Add(new MergeRequestModel(baseActor, package));
        }

        return mergeRequests;
    }
}