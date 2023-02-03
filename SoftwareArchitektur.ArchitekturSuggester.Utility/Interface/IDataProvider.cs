using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.Utility.Interface;

public interface IDataProvider
{
    IList<ServiceModel> GetServices();
    IList<DependencyRelationServiceModel> GetDependencyRelation();
    IList<CommonChangeRelationServiceModel> GetCommonChangeRelation();
}