using Newtonsoft.Json;
using SoftwareArchitektur.Utility.Extensions;

namespace SoftwareArchitektur.Utility.Models;

public class ServiceModel
{
    public ServiceModel(string name)
    {
        Name = name;
        IsLeaf = true;
    }
    
    public string Name { get; private set; }

    //Leaf = Depends On Something but things dont depend on it
    [JsonIgnore] public bool IsLeaf = true;

    //Root = Doesnt Depend on Anything but things Depend on it
    [JsonIgnore] public bool IsRoot = false;
    public readonly List<DependencyRelationModel> DependsOn = new List<DependencyRelationModel>();
    
    public readonly List<CommonChangeRelationModel> ChangedWith = new List<CommonChangeRelationModel>();
    
    public bool IsIndependent => IsLeaf && IsRoot;

    public string InPackage = String.Empty;

    public bool IsIsolated => IsIndependent && ChangedWith.Count == 0;
    
    public double AverageChange => ChangedWith.Count > 1 ? ChangedWith.Average(s => s.NumberOfChanges) : double.NaN;
    
    public double StandardDeviationChangeRate => ChangedWith.Select(ch => (double)ch.NumberOfChanges).StandardDeviation();
    
}