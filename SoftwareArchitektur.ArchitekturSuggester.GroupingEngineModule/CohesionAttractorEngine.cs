using MNCD.CommunityDetection.SingleLayer;
using MNCD.Core;
using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine;

public class CohesionAttractorEngine
{
    private IList<GroupingPackageModel> _packageLookup = new List<GroupingPackageModel>();
    
    public void SetPackageLookup(IList<GroupingPackageModel> packageLookup)
    {
        _packageLookup = packageLookup;
    }
    
    public IList<MergeRequestModel> GroupPackages(IList<GroupingPackageModel> models)
    {
        if(_packageLookup.Count == 0)
        {
            throw new Exception("Package Lookup is not set");
        }
        var packageName = models.Select(m => m.PackageName);
        var commonChanges = models.SelectMany(m => m.ChangesWith).Where(c => packageName.Any(n => n == c.OtherPackage.PackageName));

        var vertices = models.Select(package => new Actor(package.PackageName));

        var edge = NormalizeChangeCallsToWeights(commonChanges, ref vertices);
        
        var layer = new List<Layer>
        {
            new("Layer0") { Edges = edge.ToList() }
        };

        var network = new Network(layer.ToList(), vertices.ToList());

        var louvain = new FluidC();
        var communities = louvain.Compute(network, 1);

        if (communities.ToList().Count == vertices.ToList().Count)
        {
            throw new Exception("No Communities found");
        }
        var mergeRequests = new List<MergeRequestModel>();

        foreach (var community in communities)
        {
            mergeRequests.AddRange(CreateMergeRequestFromCommunity(community));
        }

        return mergeRequests;
    }
    
    private IEnumerable<Edge> NormalizeChangeCallsToWeights(IEnumerable<GroupingCommonChangeModel> changes, ref IEnumerable<Actor> vertices)
    {
        var highestCallNumber = changes.Max(m => m.NumberOfChanges);

        var edgeList = new List<Edge>();
        foreach (var change in changes)
        {
            var current = vertices.First(m => m.Name == change.ThisPackage.PackageName);
            var other = vertices.First(m => m.Name == change.OtherPackage.PackageName);
            edgeList.Add(new Edge(current,other, highestCallNumber - change.NumberOfChanges + 1));
        }
        
        return edgeList;
    }

    private IList<MergeRequestModel> CreateMergeRequestFromCommunity(Community community)
    {
        var mergeRequests = new List<MergeRequestModel>();
        var baseActor = _packageLookup.First(m => m.PackageName == community.Actors.First().Name);

        foreach (var actor in community.Actors)
        {
            var package = _packageLookup.First(m => m.PackageName == actor.Name);
            if (package != baseActor)
            {
                mergeRequests.Add(new MergeRequestModel(baseActor, package));
            }
        }

        return mergeRequests;
    }
}