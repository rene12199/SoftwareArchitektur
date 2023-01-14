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

    private readonly IList<ServiceModel> _services;

    public ArchitectureSuggester(IContainer container)
    {
        _container = container;
        _dataProvider = container.Resolve<IDataProvider>();
        _services = _dataProvider.GetServices();
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
        throw new NotImplementedException();
    }

    private void GroupPackages(List<PackageModel> packageModels)
    {
        throw new NotImplementedException();
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
        foreach (var package in packages) dupCounter = GetAndDeleteServicesFromPackage(package, dupCounter);
    }

    private int GetAndDeleteServicesFromPackage(PackageModel package, int dupCounter)
    {
        foreach (var service in package.GetServices())
            if (CheckIfServiceIsStillRegistered(service))
            {
                dupCounter++;
                _services.Remove(service);
            }

        return dupCounter;
    }

    private bool CheckIfServiceIsStillRegistered(ServiceModel service)
    {
        return _services.Any(s => s.Name == service.Name);
    }

    private void CreateOPackage(List<PackageModel> packageModels)
    {
        var oPackage = new PackageModel("O-Package");
        var oServices = _services.Where(s => s.IsIsolated).ToList();

        foreach (var oService in oServices) _services.Remove(oService);

        oPackage.AddServiceRange(oServices);
        packageModels.Add(oPackage);
    }
}