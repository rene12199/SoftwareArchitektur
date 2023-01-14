using SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine.Models;
using SoftwareArchitektur.Utility.Interface;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine.Converter;

public class CommonChangeToCcpCommonChageConverter
{
    public CommonChangeToCcpCommonChageConverter(IDataProvider _dataProvider)
    {
        _serviceModelLookup = _dataProvider.GetServices().ToList().AsReadOnly();
    }

    private readonly IList<ServiceModel> _serviceModelLookup;

    public CommonChangeToCcpCommonChageConverter(IList<ServiceModel> serviceModelLookup)
    {
        _serviceModelLookup = serviceModelLookup;
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
            if (CcpCommonChangeExists(commonChange, commonCcpChangeList))
            {
                commonCcpChangeList.Add(new CcpScoringCommonChangeClass(
                    GetServiceFromLookUp(commonChange.NameOfCurrentService),
                    GetServiceFromLookUp(commonChange.NameOfCurrentService),
                    commonChange.NumberOfChanges));
            }
            else
            {
                var existingCommonChange = commonCcpChangeList.Single(cc =>
                    cc.Equals(
                        GetServiceFromLookUp(commonChange.NameOfCurrentService).InPackage,
                        GetServiceFromLookUp(commonChange.NameOfCurrentService).InPackage));

                existingCommonChange.AddChanges(commonChange.NumberOfChanges);
            }
    }

    private bool CcpCommonChangeExists(CommonChangeRelationModel commonChange, IList<CcpScoringCommonChangeClass> commonCcpChangeList)
    {
        return commonCcpChangeList.Any(cc =>
            cc.Equals(GetServiceFromLookUp(commonChange.NameOfCurrentService).InPackage, GetServiceFromLookUp(commonChange.NameOfOtherService).InPackage));
    }

    private ServiceModel GetServiceFromLookUp(string commonChangeNameOfCurrentService)
    {
        return _serviceModelLookup.Single(s => s.Name == commonChangeNameOfCurrentService);
    }
}