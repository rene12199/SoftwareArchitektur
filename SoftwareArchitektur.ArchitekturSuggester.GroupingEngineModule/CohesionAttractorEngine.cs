using MNCD.CommunityDetection.SingleLayer;
using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;
using MNCD.Core;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine;

public class CohesionAttractorEngine
{
    private IList<GroupingPackageModel> _packageLookup;

    public IList<MergeRequestModel> GroupPackages(IList<GroupingPackageModel> models)
    {
        _packageLookup = models;
        var packageName = models.Select(m => m.PackageName);
        var commonChanges = models.SelectMany(m => m.ChangesWith).Where(c => packageName.Any(n => n == c.OtherPackage.PackageName));


        var vertices = models.Select(package => new Actor(package.PackageName));

        var edge = commonChanges.Select(change => new Edge(vertices.First(m => m.Name == change.ThisPackage.PackageName),
            vertices.First(m => m.Name == change.OtherPackage.PackageName), change.NumberOfChanges));

        var layer = new List<Layer>
        {
            new("Layer0") { Edges = edge.ToList() },
        };

        var network = new Network(layer.ToList(), vertices.ToList());

        //todo chech if Louvian is really best here
        var louvain = new Louvain();
        var communities = louvain.Apply(network);

        var mergeRequests = new List<MergeRequestModel>();

        foreach (var community in communities)
        {
            mergeRequests.AddRange(CreateMergeRequestFromCommunity(community));
        }

        return mergeRequests;
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