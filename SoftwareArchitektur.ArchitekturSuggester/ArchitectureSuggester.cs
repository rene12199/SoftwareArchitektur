using MNCD.CommunityDetection.SingleLayer;
using MNCD.Core;
using MNCD.Writers;
using Newtonsoft.Json;
using SoftwareArchitektur.ArchitekturSuggester.Models;
using SoftwareArchitektur.ArchitekturSuggester.Scoring;
using SoftwareArchitektur.ArchitekturSuggester.Scoring.ScorerClasses;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester;

public class ArchitectureSuggester
{
    private readonly List<ServiceModel> _services;
    private readonly List<DependencyRelationModel> _dependencyRelations;
    private List<CommonChangeRelationModel> _commonChangeRelations;
    private readonly List<PackageModel> _packageModels = new List<PackageModel>();
    private readonly DependencyScorer _dependencyScorer;
    private List<ServiceModel> _indipendantServices;

    public ArchitectureSuggester(string completeDataFileAddress, string dependencyFileAddress, string changeFileAddress)
    {
        _services = ReadData<List<ServiceModel>>(completeDataFileAddress);
        _dependencyRelations = ReadData<List<DependencyRelationModel>>(dependencyFileAddress);
        _commonChangeRelations = ReadData<List<CommonChangeRelationModel>>(changeFileAddress);
        _dependencyScorer = new DependencyScorer(_packageModels);
        CheckIfServiceIsLeafOrRoot();
    }

    public void CalculateArchitecture(int numberOfPackages)
    {

        //CreateOPackage();


        CreatePackages();
        var possibleMoves = new List<Move>();
        foreach (var service in _services)
        {
            // CleanMoves(possibleMoves, service.Name);
            // foreach (var move in possibleMoves)
            // {
            //     if (!service.IsIndependent)
            //     {
            //         _dependencyScorer.ScoreByDependency(move, service);
            //     }
            // }
        }
    }
    private void CreateOPackage()
    {

        var oPackage = new PackageModel("O-Package");
        var oServices = _services.Where(s => s.ChangedWith.Count == 0 && s.IsIndependent).ToList();

        foreach (var oService in oServices)
        {
            _services.Remove(oService);
        }

        oPackage.Services.AddRange(oServices);
        _packageModels.Add(oPackage);
    }

    private T ReadData<T>(string fileName)
    {
        var file = File.ReadAllText(fileName);
        return JsonConvert.DeserializeObject<T>(file)!;
    }

    private void CleanMoves(List<Move> moves, string serviceName)
    {
        moves.Clear();
        for (int i = 0; i < _packageModels.Count; i++)
        {
            moves.Add(new Move(packageName: _packageModels[i].Name, serviceName: serviceName));
        }
    }

    // private void CreatePackages()
    // {
    //     // _indipendantServices = _services.Where(s => s.IsIndependent)
    //     //     .OrderBy(s => s.ChangedWith.Select(se => se.NumberOfChanges).Max()).ToList();
    //     //
    //     // var network = CreateNetWork(_indipendantServices);
    //     //
    //     // var louvain = new Louvain();
    //     // var communities = louvain.Apply(network);
    //     //
    //     // int counter = 0;
    //     //
    //     // foreach (var community in communities)
    //     // {
    //     //     counter++;
    //     //     
    //     //     _packageModels.Add(new PackageModel($"Package: {counter}")
    //     //     {
    //     //         Services = ConvertActorsIntoServices(community.Actors)
    //     //     });
    //     // }
    //     //
    //     // VisualizeCommunities(network, communities);
    //
    // }

    private void CreatePackages()
    {
        
    }
    private List<ServiceModel> ConvertActorsIntoServices(List<Actor> communityActors)
    {
        var serviceList = new List<ServiceModel>();
        foreach (var actor in communityActors)
        {
            var service = _services.First(s => s.Name == actor.Name);
           serviceList.Add(service);
           _services.Remove(service);
        }

        return serviceList;
    }
    private Network CreateNetWork(List<ServiceModel> isolatedServices)
    {

        Network network = new Network();
        var actors = new List<Actor>();
        var edges = new List<Edge>();
        
        CreateActors(isolatedServices, actors);

        CreateEdges(isolatedServices, edges, actors);

        network.Actors = actors;

        network.Layers.Add(new Layer(edges));

        return network;
    }
    private void CreateEdges(List<ServiceModel> isolatedServices, List<Edge> edges, List<Actor> actors)
    {

        foreach (var isolatedService in isolatedServices)
        {
            foreach (var changeRelation in isolatedService.ChangedWith)
            {
                edges.Add(new Edge(actors.First(a => a.Name == changeRelation.NameOfCurrentService),
                    actors.First(a => a.Name == changeRelation.NameOfCurrentService), changeRelation.NumberOfChanges));
            }
        }
    }
    private void CreateActors(List<ServiceModel> isolatedServices, List<Actor> actors)
    {
        foreach (var isolatedService in isolatedServices)
        {
            var newActor = new Actor(isolatedService.Name);
            actors.Add(newActor);
        }
    }

    private void CheckIfServiceIsLeafOrRoot()
    {
        _services.Where(s => s.DependsOn.Count == 0).ToList().ForEach(i => i.IsRoot = true);

        var allCallers = _dependencyRelations.Select(d => d.Callee).Distinct();

        _services.Where(s => allCallers.Any(c => c != s.Name)).ToList().ForEach(s => s.IsLeaf = true);
    }

    public void VisualizeCommunities(Network network, List<Community> communities)
    {
        var writer = new EdgeListWriter();
        var edge_list = writer.ToString(network, true);
        var communityWriter = new ActorCommunityListWriter();
        var community_list = communityWriter.ToString(network.Actors, communities, true);
        var body = new
        {
            edge_list = edge_list,
            community_list = community_list,
            image_format = "svg"
        };
        var json = JsonConvert.SerializeObject(body);
        var content = new StringContent(json);
    }
}