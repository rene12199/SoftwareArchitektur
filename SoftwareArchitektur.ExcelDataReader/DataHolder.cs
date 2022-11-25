using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ExcelDataReader;

public class DataHolder
{
    public static readonly List<ServiceModel> ServiceList= new List<ServiceModel>();
    public static readonly List<DependencyRelationModel> DependencyList= new List<DependencyRelationModel>();
    public static readonly List<CommonChangeRelationModel> ChangedWithList= new List<CommonChangeRelationModel>();
}