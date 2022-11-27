using MNCD.Core;
using MNCD.Writers;
using Newtonsoft.Json;
using SoftwareArchitektur.ArchitekturSuggester.Models;
using SoftwareArchitektur.ArchitekturSuggester.Scoring;
using SoftwareArchitektur.Utility.Models;


namespace SoftwareArchitektur.ArchitekturSuggester;

public class ArchitectureSuggester
{
    private readonly List<ServiceModel> _services;
    private readonly List<DependencyRelationModel> _dependencyRelations;
    private List<CommonChangeRelationModel> _commonChangeRelations;

    public ArchitectureSuggester(string completeDataFileAddress, string dependencyFileAddress, string changeFileAddress)
    {
        _services = ReadData<List<ServiceModel>>(completeDataFileAddress);
        _dependencyRelations = ReadData<List<DependencyRelationModel>>(dependencyFileAddress);
        _commonChangeRelations = ReadData<List<CommonChangeRelationModel>>(changeFileAddress);
        CheckIfServiceIsLeafOrRoot();
    }

    public List<PackageModel> CalculateArchitecture()
    {
        var packages = CreateInitalPackageModels();

        CreateOPackage(packages);

        var ausPackage = new PackageModel("Aux");
        
        while (_services.Count > 0)
        {
            Console.WriteLine($"Judging CCP moves for Service{_services[0]}, {_services.Count} remaining");
            // ausPackage.AddService(_services[0]);
            // _services.Remove(_services[0]);
            
            Move bestMove = new Move(_services[0]);
           
            foreach (var package in packages)
            {
                var newScore =
                    Math.Abs(Math.Sqrt(Math.Pow(package.StandardDeviationOfChangeRate, 2) +
                                       Math.Pow(bestMove.Service.StandardDeviationChangeRate, 2)) -
                             package.StandardDeviationOfChangeRate);
                if (newScore < bestMove.Score)
                {
                    bestMove.SetNewBestPackage(package, newScore);
                }
            }
            
            ExecuteMove(bestMove);
        }

        packages.Add(ausPackage);
        return packages;
    }

    private void ExecuteMove(Move bestMove)
    {
        bestMove.BestPackage.AddService(bestMove.Service);
        _services.Remove(bestMove.Service);
    }

    private List<PackageModel> CreateInitalPackageModels()
    {
        //todo check Whether Root and Leaf Packages should be Included
        var nonIndependentServices = _services.Where(s => !s.IsIndependent).ToList();
        var circularChecker = new CircularDependencyChecker(nonIndependentServices);
        var packages = circularChecker.CreatePackages();
        var dupCounter = -nonIndependentServices.Count;
        foreach (var package in packages)
        {
            foreach (var service in package.GetServices())
            {
              
                if (_services.Any(s => s.Name == service.Name))
                {
                    dupCounter++;
                    _services.Remove(service);
                }
                    
            }
        }

        packages.CreateDependenciesToPackages();
        return packages;
    }

    private void CreateOPackage(List<PackageModel> packageModels)
    {
        var oPackage = new PackageModel("O-Package");
        var oServices = _services.Where(s => s.IsIsolated).ToList();

        foreach (var oService in oServices)
        {
            _services.Remove(oService);
        }

        oPackage.AddServiceRange(oServices);
        packageModels.Add(oPackage);
    }

    private T ReadData<T>(string fileName)
    {
        var file = File.ReadAllText(fileName);
        return JsonConvert.DeserializeObject<T>(file)!;
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

        var allCallees = _dependencyRelations.Select(d => d.Callee).Distinct().ToList();

        foreach (var callee in allCallees)
        {
            _services.First(s => s.Name == callee).IsLeaf = false;
        }
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