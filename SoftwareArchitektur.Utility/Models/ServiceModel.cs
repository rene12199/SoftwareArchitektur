namespace SoftwareArchitektur.Utility.Models;

public class ServiceModel
{

    public ServiceModel(string name)
    {
        Name = name;
    }
    public string Name { get; private set; }
    //Leaf = Depends On Something but things dont depend on it
    public bool IsLeaf = false;
    //Root = Doesnt Depend on Anything but things Depend on it
    public bool IsRoot = false;
    public readonly List<DependencyRelationModel> DependsOn = new List<DependencyRelationModel>();
    public readonly List<CommonChangeRelationModel> ChangedWith = new List<CommonChangeRelationModel>();
    public bool IsIndependent  => IsLeaf && IsRoot;

    public bool IsIsolated => IsIndependent && ChangedWith.Count == 0;

}