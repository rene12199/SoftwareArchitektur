using Autofac;
using QuikGraph;
using QuikGraph.Algorithms;
using SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine.Interfaces;
using SoftwareArchitektur.ArchitekturSuggester.CircularDependencyChecker.Interfaces;
using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine;
using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Converter;
using SoftwareArchitektur.Utility.Interface;
using SoftwareArchitektur.Utility.Models;

//todo Create Basic Interface for Engines
namespace SoftwareArchitektur.ArchitekturSuggester;

public class ArchitectureSuggester
{
    private readonly IContainer _container;
    private readonly IDataProvider _dataProvider;
    private readonly ICircularDependencyChecker _circularDependencyChecker;
    private readonly ICcpScoringEngine _ccpScoringEngine;


    public ArchitectureSuggester(IContainer container)
    {
        _container = container;
        _dataProvider = container.Resolve<IDataProvider>();
        _circularDependencyChecker = container.Resolve<ICircularDependencyChecker>();
        _ccpScoringEngine = container.Resolve<ICcpScoringEngine>();
    }

    public List<PackageModel> CalculateArchitecture()
    {
        var packages = CreateInitialPackageModels();

        CreateOPackage(packages);

        CheckIfPackagesHaveCycle(packages);

        DistributePackagesByCcpScore(packages);

        CheckIfPackagesHaveCycle(packages);

        GroupPackages(packages);

        CheckIfPackagesHaveCycle(packages);
        
        DistributeRemainingPackages(_dataProvider.GetServices().Where(s => s.InPackage == null), packages);

        ValidateArchitecture();
        
        return packages;
    }

    private void DistributeRemainingPackages(IEnumerable<ServiceModel> serviceModels, IList<PackageModel> packageModels)
    {
        foreach (var service in serviceModels)
        {
            packageModels.OrderBy(p => p.Services.Count);
            packageModels.First().AddService(service);
        }
    }

    private void CheckIfPackagesHaveCycle(IList<PackageModel> models)
    {
        var edges = new List<SEquatableEdge<string>>();

        foreach (var package in models)
        {
            foreach (var dependencyRelation in package.DependsOn) edges.Add(new SEquatableEdge<string>(dependencyRelation.Caller, dependencyRelation.Callee));
        }

        var tmp = edges.ToUndirectedGraph();

        if (tmp.IsUndirectedAcyclicGraph()) throw new Exception("e");
    }

    private void ValidateArchitecture()
    {
        if (_dataProvider.GetServices().Any(s => s.InPackage == null)) throw new Exception("Not Every Service in Package");
    }

    private void DistributePackagesByCcpScore(List<PackageModel> packages)
    {
        _ccpScoringEngine.SetPossiblePackages(packages);

        _ccpScoringEngine.DistributeRemainingServices(_dataProvider.GetServices().Where(s => s.InPackage == null).ToList());
    }

    private void GroupPackages(List<PackageModel> packageModels)
    {
        Console.WriteLine("Starting GroupingOperation");
        var groupingEngine =
            new GroupingEngine.GroupingEngine(
                new GroupingPackageModelFactory(new DependencyModelToGroupingDependencyConverter(), new CommonChangeToGroupingCommonChangeConverter()), new LayeringEngine(),
                new CohesionAttractorEngine());
        groupingEngine.GroupPackages(packageModels);
    }

    private List<PackageModel> CreateInitialPackageModels()
    {
        var circularChecker = new CircularDependencyChecker.CircularDependencyChecker(_dataProvider);
        var packages = circularChecker.CreatePackages();

        DeleteAddedServicesFromGlobalServicePool(_dataProvider.GetServices().ToList(), packages);
        return packages;
    }

    private void DeleteAddedServicesFromGlobalServicePool(List<ServiceModel> nonIndependentServices, List<PackageModel> packages)
    {
        var dupCounter = -nonIndependentServices.Count;
        var registeredServices = _dataProvider.GetServices().Where(s => s.InPackage != null).ToList();
        foreach (var package in packages) dupCounter = GetAndDeleteServicesFromPackage(package, dupCounter, registeredServices);
    }

    private int GetAndDeleteServicesFromPackage(PackageModel package, int dupCounter, IList<ServiceModel> registeredServices)
    {
        foreach (var service in package.GetServices())
        {
            if (CheckIfServiceIsStillRegistered(service, registeredServices))
                dupCounter++;
        }

        return dupCounter;
    }

    private bool CheckIfServiceIsStillRegistered(ServiceModel service, IList<ServiceModel> registeredServices)
    {
        return registeredServices.Any(s => s.Name == service.Name);
    }

    private void CreateOPackage(List<PackageModel> packageModels)
    {
        var oPackage = new PackageModel("O-Package");
        var oServices = _dataProvider.GetServices().Where(s => s.IsIsolated).ToList();

        oPackage.AddServiceRange(oServices);
        packageModels.Add(oPackage);
    }
}