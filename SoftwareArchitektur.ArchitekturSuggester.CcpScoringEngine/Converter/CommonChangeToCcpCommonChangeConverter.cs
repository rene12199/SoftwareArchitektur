using SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine.Models;
using SoftwareArchitektur.Utility.Interface;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine.Converter;

public class CommonChangeToCcpCommonChangeConverter
{
    private readonly IList<ServiceModel> _serviceModelLookup;

    public CommonChangeToCcpCommonChangeConverter(IDataProvider dataProvider)
    {
        _serviceModelLookup = dataProvider.GetServices().ToList().AsReadOnly();
    }

    public IList<CcpScoringCommonChangeClass> CreateCcpCommonChangeList(IList<CommonChangeRelationModel> commonChangeList)
    {
        var commonCcpChangeList = new List<CcpScoringCommonChangeClass>();

        ConvertCommonChangeModelToCcpCommonChangeModel(commonChangeList, commonCcpChangeList);

        return commonCcpChangeList;
    }

    private void ConvertCommonChangeModelToCcpCommonChangeModel(IList<CommonChangeRelationModel> commonChangeList, List<CcpScoringCommonChangeClass> commonCcpChangeList)
    {
        foreach (var commonChange in commonChangeList)
        {
            if (CcpCommonChangeExists(commonChange, commonCcpChangeList))
            {
                var existingCommonChange = commonCcpChangeList.Single(cc => GetServiceFromLookUp(commonChange.NameOfOtherService).InPackage == cc.OtherPackage);

                existingCommonChange.AddChanges(commonChange.NumberOfChanges);
            }
            else
            {
                var commonCcpChange = new CcpScoringCommonChangeClass(
                    GetServiceFromLookUp(commonChange.NameOfCurrentService),
                    GetServiceFromLookUp(commonChange.NameOfOtherService),
                    commonChange.NumberOfChanges);
                commonCcpChangeList.Add(commonCcpChange);
            }
        }
    }

    private bool CcpCommonChangeExists(CommonChangeRelationModel commonChange, IList<CcpScoringCommonChangeClass> commonCcpChangeList)
    {
        return commonCcpChangeList.Any(cc =>
            cc.Equals(GetServiceFromLookUp(commonChange.NameOfCurrentService).InPackage, GetServiceFromLookUp(commonChange.NameOfOtherService).InPackage));
    }

    private ServiceModel GetServiceFromLookUp(string commonChangeNameOfCurrentService)
    {
        return _serviceModelLookup.SingleOrDefault(s => s.Name == commonChangeNameOfCurrentService);
    }
}