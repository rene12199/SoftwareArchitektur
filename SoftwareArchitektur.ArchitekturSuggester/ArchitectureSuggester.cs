using Autofac;
using SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine.Interfaces;
using SoftwareArchitektur.ArchitekturSuggester.CircularDependencyChecker.Interfaces;
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
        ;
    }

    public List<PackageModel> CalculateArchitecture()
    {
        var packages = CreateInitialPackageModels();

        CreateOPackage(packages);

        DistributeRemainingPackagesByCcpScore(packages);

        //todo Create Grouping Algorithm with focus on balancing
        GroupPackages(packages);


        return packages;
    }

    private void DistributeRemainingPackagesByCcpScore(List<PackageModel> packages)
    {
        _ccpScoringEngine.SetPossiblePackages(packages);
        
        _ccpScoringEngine.DistributeRemainingServices(_dataProvider.GetServices().Where(s => s.InPackage == String.Empty).ToList());
    }

    private void GroupPackages(List<PackageModel> packageModels)
    {
        
    }

    private List<PackageModel> CreateInitialPackageModels()
    {
        var circularChecker = new CircularDependencyChecker.CircularDependencyChecker(_dataProvider);
        var packages = circularChecker.CreatePackages();

        DeleteAddedServicesFromGlobalServicePool(_dataProvider.GetServices().ToList(), packages);
        packages.CreateDependenciesToPackages();
        return packages;
    }

    private void DeleteAddedServicesFromGlobalServicePool(List<ServiceModel> nonIndependentServices, List<PackageModel> packages)
    {
        var dupCounter = -nonIndependentServices.Count;
        var registeredServices = _dataProvider.GetServices().Where(s => s.InPackage != String.Empty).ToList();
        foreach (var package in packages) dupCounter = GetAndDeleteServicesFromPackage(package, dupCounter, registeredServices);
    }

    private int GetAndDeleteServicesFromPackage(PackageModel package, int dupCounter, IList<ServiceModel> registeredServices)
    {
    
        foreach (var service in package.GetServices())
            if (CheckIfServiceIsStillRegistered(service , registeredServices))
            {
                dupCounter++;
            }

        return dupCounter;
    }

    private bool CheckIfServiceIsStillRegistered(ServiceModel service, IList<ServiceModel>registeredServices)
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