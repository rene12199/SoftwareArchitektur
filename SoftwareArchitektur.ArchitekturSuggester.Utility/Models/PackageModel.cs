﻿using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace SoftwareArchitektur.Utility.Models;

public class PackageModel
{
    public double AverageChangeRate => _services.Where(s => !double.IsNaN(s.AverageChange)).Sum(s => s.AverageChange) / _services.Count;

    public ReadOnlyCollection<ServiceModel> Services => _services.ToList().AsReadOnly();
    //Sourcehttps://socratic.org/statistics/random-variables/addition-rules-for-variances
    public double StandardDeviationOfChangeRate =>
        Math.Sqrt(_services.Sum(sd => sd.StandardDeviationChangeRate * sd.StandardDeviationChangeRate));

    public List<DependencyRelationPackageModel> DependsOn => GroupDependencies().ToList();

    public List<CommonChangeRelationPackageModel> ChangesWith => GroupChanges().ToList();

    public bool HasServices => GetServices().Count > 0;

    [JsonIgnore] public List<PackageModel> PackageDependencies { get; } = new();

    public string PackageName { get; set; }

    [JsonIgnore] private readonly List<ServiceModel> _services = new List<ServiceModel>();

    public PackageModel(string packageName)
    {
        PackageName = packageName;
    }

    public void AddService(ServiceModel service)
    {
        _services.Add(service);
        service.InPackage = this;
    }

    public void AddServiceRange(IEnumerable<ServiceModel> service)
    {
        foreach (var s in service)
        {
            _services.Add(s);
            s.InPackage = this;
        }
    }

    public List<ServiceModel> GetServices()
    {
        return _services.ToList();
    }

    private IEnumerable<DependencyRelationPackageModel> GroupDependencies()
    {
        var allDependencies = _services.SelectMany(s => s.DependsOn);
        var grouped = allDependencies.GroupBy(d => d.CalleeService.InPackage).Select(gr =>
            new DependencyRelationPackageModel(gr.First().CallerService.InPackage, gr.First().CalleeService.InPackage, gr.Sum(g => g.NumberOfCalls)));
        return grouped;
    }


    private IEnumerable<CommonChangeRelationPackageModel> GroupChanges()
    {
        var allChanges = _services.SelectMany(s => s.ChangedWith);
        var grouped = allChanges.GroupBy(d => d.OtherService.InPackage).Select(gr =>
            new CommonChangeRelationPackageModel(gr.First().CurrentService.InPackage, gr.First().OtherService.InPackage, gr.Sum(g => g.NumberOfChanges)));
        return grouped;
    }
    
    public void Merge(PackageModel packageModelByPackageName)
    {
        this.AddServiceRange(packageModelByPackageName._services);
        packageModelByPackageName._services.Clear();
    }
}