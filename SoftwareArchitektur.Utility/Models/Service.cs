namespace SoftwareArchitektur.Utility.Models;

public class Service
{
    public string? Name;
    public bool isLeaf = false;
    public List<DependencyRelation> DependsOn = new List<DependencyRelation>();
    public List<CommonChangeRelation> ChangedWith = new List<CommonChangeRelation>();

}