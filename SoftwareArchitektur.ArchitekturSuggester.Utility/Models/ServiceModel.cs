using Newtonsoft.Json;
using SoftwareArchitektur.Utility.Extensions;

namespace SoftwareArchitektur.Utility.Models;

public class ServiceModel
{
    private sealed class ServiceModelEqualityComparer : IEqualityComparer<ServiceModel>
    {
        public bool Equals(ServiceModel x, ServiceModel y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.IsLeaf == y.IsLeaf && x.IsRoot == y.IsRoot && x.HasChangeRelation == y.HasChangeRelation && Equals(x.InPackage, y.InPackage) && x.Name == y.Name;
        }

        public int GetHashCode(ServiceModel obj)
        {
            return HashCode.Combine(obj.IsLeaf, obj.IsRoot, obj.HasChangeRelation, obj.InPackage, obj.Name);
        }
    }

    public static IEqualityComparer<ServiceModel> ServiceModelComparer { get; } = new ServiceModelEqualityComparer();

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
    
    public readonly List<DependencyRelationServiceModel> DependsOn = new();

    public readonly List<CommonChangeRelationServiceModel> ChangedWith = new();

    public bool IsStatic => ChangedWith.Count == 0;

    public bool HasChangeRelation = false;

    public bool IsIndependent => IsLeaf && IsRoot;

    public PackageModel? InPackage = null!;

    public bool IsIsolated => IsIndependent && IsStatic && !HasChangeRelation;

    public double AverageChange => ChangedWith.Count > 1 ? ChangedWith.Average(s => s.NumberOfChanges) : 0;

    public double StandardDeviationChangeRate => ChangedWith.Select(ch => (double)ch.NumberOfChanges).StandardDeviation();
}