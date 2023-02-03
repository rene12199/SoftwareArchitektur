using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Converter;

public class DependencyModelToGroupingDependencyConverter
{
    public IList<GroupingDependendencyModel> CreateGroupingDependencyModelsList(IList<DependencyRelationPackageModel> dependencyRelations)
    {
        var groupingPackageModels = new List<GroupingDependendencyModel>();
        foreach (var commonChange in dependencyRelations)
        {
            var commonCcpChange = new GroupingDependendencyModel(
                commonChange.CallerService,
                commonChange.CalleeService,
                commonChange.NumberOfCalls);
            groupingPackageModels.Add(commonCcpChange);
        }

        return groupingPackageModels;
    }
}