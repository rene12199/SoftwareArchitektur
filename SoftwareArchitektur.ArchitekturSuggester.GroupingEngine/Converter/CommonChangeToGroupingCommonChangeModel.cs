using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;
using SoftwareArchitektur.Utility.Interface;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Converter;

public class CommonChangeToGroupingCommonChangeModel
{
    private readonly IReadOnlyCollection<ServiceModel> _serviceModelLookUp;

    public CommonChangeToGroupingCommonChangeModel(IDataProvider dataProvider)
    {
        _serviceModelLookUp = dataProvider.GetServices().ToList().AsReadOnly();
    }

    public IList<GroupingCommonChangeModel> CreateGroupingCommonChangeModelsList(IList<CommonChangeRelationModel> commonChangeList)
    {
        var groupingPackageModels = new List<GroupingCommonChangeModel>();

        ConvertCommonChangeModelToCcpCommonChangeModel(commonChangeList, groupingPackageModels);

        return groupingPackageModels;
    }

    private void ConvertCommonChangeModelToCcpCommonChangeModel(IList<CommonChangeRelationModel> commonChangeList, List<GroupingCommonChangeModel> commonGroupingChangeList)
    {
        foreach (var commonChange in commonChangeList)
        {
            if (CcpCommonChangeExists(commonChange, commonGroupingChangeList))
            {
                var existingCommonChange = commonGroupingChangeList.Single(cc => GetServiceFromLookUp(commonChange.NameOfOtherService).InPackage == cc.OtherPackage);

                existingCommonChange.AddChanges(commonChange.NumberOfChanges);
            }
            else
            {
                var commonCcpChange = new GroupingCommonChangeModel(
                    GetServiceFromLookUp(commonChange.NameOfCurrentService),
                    GetServiceFromLookUp(commonChange.NameOfOtherService),
                    commonChange.NumberOfChanges);
                commonGroupingChangeList.Add(commonCcpChange);
            }
        }
    }

    private bool CcpCommonChangeExists(CommonChangeRelationModel commonChange, IList<GroupingCommonChangeModel> commonCcpChangeList)
    {
        return commonCcpChangeList.Any(cc =>
            cc.Equals(GetServiceFromLookUp(commonChange.NameOfCurrentService).InPackage, GetServiceFromLookUp(commonChange.NameOfOtherService).InPackage));
    }

    private ServiceModel GetServiceFromLookUp(string commonChangeNameOfCurrentService)
    {
        return _serviceModelLookUp.SingleOrDefault(s => s.Name == commonChangeNameOfCurrentService);
    }
}