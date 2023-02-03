using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Converter;

public class CommonChangeToGroupingCommonChangeConverter
{
    public IList<GroupingCommonChangeModel> CreateGroupingCommonChangeModelsList(IEnumerable<CommonChangeRelationPackageModel> commonChanges)
    {
        var groupingPackageModels = new List<GroupingCommonChangeModel>();
        foreach (var commonChange in commonChanges)
        {
            var commonCcpChange = new GroupingCommonChangeModel(
                commonChange.CurrentPackage,
                commonChange.OtherPackage,
                commonChange.NumberOfChanges);
            groupingPackageModels.Add(commonCcpChange);
        }

        return groupingPackageModels;
    }
}