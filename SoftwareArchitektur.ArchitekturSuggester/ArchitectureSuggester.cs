using System.Collections.ObjectModel;
using Newtonsoft.Json;
using SoftwareArchitektur.ArchitekturSuggester.CircularDependencyCheckerModule;
using SoftwareArchitektur.ArchitekturSuggester.GroupingModule;
using SoftwareArchitektur.Utility.Models;


//todo Create LookUp Service in Utility
//todo turn Modules into Seperate Projects
//todo Create Basic Interface for Engines
namespace SoftwareArchitektur.ArchitekturSuggester;

public class ArchitectureSuggester
{
    private readonly List<ServiceModel> _services;
    private readonly ReadOnlyCollection<ServiceModel> _servicesLookUp;
    private readonly List<DependencyRelationModel> _dependencyRelations;
    private readonly List<CommonChangeRelationModel> _changeRelations;

    public ArchitectureSuggester(string completeDataFileAddress, string dependencyFileAddress, string changeFileAddress)
    {
        _services = ReadData<List<ServiceModel>>(completeDataFileAddress);
        _servicesLookUp = ReadData<ReadOnlyCollection<ServiceModel>>(completeDataFileAddress);
        _dependencyRelations = ReadData<List<DependencyRelationModel>>(dependencyFileAddress);
        _changeRelations = ReadData<List<CommonChangeRelationModel>>(changeFileAddress);
        CheckIfServiceIsLeafOrRoot();
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
        var groupingEngine = new GroupingEngine(_servicesLookUp, _changeRelations);
    }

    private List<PackageModel> CreateInitialPackageModels()
    {
        var nonIndependentServices = _services.Where(s => !s.IsIndependent).ToList();
        var circularChecker = new CircularDependencyChecker(nonIndependentServices);
        var packages = circularChecker.CreatePackages();

        DeleteAddedServicesFromGlobalServicePool(nonIndependentServices, packages);
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

    private T ReadData<T>(string fileName)
    {
        var file = File.ReadAllText(fileName);
        return JsonConvert.DeserializeObject<T>(file)!;
    }

    private void CheckIfServiceIsLeafOrRoot()
    {
        CheckIfRoot();

        CheckIfLeaf();
    }

    private void CheckIfRoot()
    {
        _services.Where(s => s.DependsOn.Count == 0).ToList().ForEach(i => i.IsRoot = true);
    }

    private void CheckIfLeaf()
    {
        var allCallees = _dependencyRelations.Select(d => d.Callee).Distinct().ToList();

        foreach (var callee in allCallees) _services.First(s => s.Name == callee).IsLeaf = false;
    }
}