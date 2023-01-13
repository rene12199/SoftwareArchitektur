using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ExcelDataReader;

public class DataHolder
{
    public static readonly List<ServiceModel> ServiceList = new();
    public static readonly List<DependencyRelationModel> DependencyList = new();
    public static readonly List<CommonChangeRelationModel> ChangedWithList = new();
}