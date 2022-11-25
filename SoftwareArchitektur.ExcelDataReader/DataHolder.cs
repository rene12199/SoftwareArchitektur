using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ExcelDataReader;

public class DataHolder
{
    public static readonly List<Service> ServiceList= new List<Service>();
    public static readonly List<DependencyRelation> DependencyList= new List<DependencyRelation>();
    public static readonly List<CommonChangeRelation> ChangedWithList= new List<CommonChangeRelation>();
}