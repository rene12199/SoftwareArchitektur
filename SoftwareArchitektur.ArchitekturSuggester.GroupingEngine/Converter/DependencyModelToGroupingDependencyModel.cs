using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;
using SoftwareArchitektur.Utility.Interface;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Converter;

public class DependencyModelToGroupingDependencyModel
{
    private readonly IReadOnlyCollection<ServiceModel> _serviceModelLookUp;

    public DependencyModelToGroupingDependencyModel(IDataProvider dataProvider)
    {
        _serviceModelLookUp = dataProvider.GetServices().ToList().AsReadOnly();
    }

    public IList<GroupingDependendencyModel> CreateGroupingDependencyModelsList(IList<DependencyRelationModel> dependencyRelationModels)
    {
        var groupingPackageModels = new List<GroupingDependendencyModel>();

        ConvertDependencyRelationModelToGroupingRelationshipModel(dependencyRelationModels, groupingPackageModels);

        return groupingPackageModels;
    }

    private void ConvertDependencyRelationModelToGroupingRelationshipModel(IList<DependencyRelationModel> dependencyRelationModels, List<GroupingDependendencyModel> groupingDependencyList)
    {
        foreach (var dependencyRelation in dependencyRelationModels)
        {
            if (GroupingDependencyExists(dependencyRelation, groupingDependencyList))
            {
                var existingRelationDependency = groupingDependencyList.Single(cc => GetServiceFromLookUp(dependencyRelation.Callee).InPackage == cc.Callee);

                existingRelationDependency.AddCalls(dependencyRelation.NumberOfCalls);
            }
            else
            {
                var groupingDependencyRelation = new GroupingDependendencyModel(
                    GetServiceFromLookUp(dependencyRelation.Caller),
                    GetServiceFromLookUp(dependencyRelation.Callee),
                    dependencyRelation.NumberOfCalls);
                groupingDependencyList.Add(groupingDependencyRelation);
            }
        }
    }

    private bool GroupingDependencyExists(DependencyRelationModel commonChange, IList<GroupingDependendencyModel> commonCcpChangeList)
    {
        return commonCcpChangeList.Any(cc =>
            cc.Equals(GetServiceFromLookUp(commonChange.Caller).InPackage, GetServiceFromLookUp(commonChange.Callee).InPackage));
    }

    private ServiceModel GetServiceFromLookUp(string commonChangeNameOfCurrentService)
    {
        return _serviceModelLookUp.SingleOrDefault(s => s.Name == commonChangeNameOfCurrentService);
    }
}