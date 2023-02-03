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

    public IList<CcpScoringCommonChangeClass> CreateCcpCommonChangeList(IList<CommonChangeRelationServiceModel> commonChangeList)
    {
        var commonCcpChangeList = new List<CcpScoringCommonChangeClass>();

        ConvertCommonChangeModelToCcpCommonChangeModel(commonChangeList, commonCcpChangeList);

        return commonCcpChangeList;
    }

    private void ConvertCommonChangeModelToCcpCommonChangeModel(IList<CommonChangeRelationServiceModel> commonChangeList, List<CcpScoringCommonChangeClass> commonCcpChangeList)
    {
        foreach (var commonChange in commonChangeList)
            if (CcpCommonChangeExists(commonChange, commonCcpChangeList))
            {
                var existingCommonChange = commonCcpChangeList.Single(cc => commonChange.OtherService.InPackage?.PackageName == cc.OtherPackage);

                existingCommonChange.AddChanges(commonChange.NumberOfChanges);
            }
            else
            {
                var commonCcpChange = new CcpScoringCommonChangeClass(
                    commonChange.CurrentService,
                    commonChange.OtherService,
                    commonChange.NumberOfChanges);
                commonCcpChangeList.Add(commonCcpChange);
            }
    }

    private bool CcpCommonChangeExists(CommonChangeRelationServiceModel commonChange, IList<CcpScoringCommonChangeClass> commonCcpChangeList)
    {
        return commonCcpChangeList.Any(cc =>
            cc.Equals(commonChange.CurrentService.InPackage?.PackageName, commonChange.OtherService.InPackage?.PackageName));
    }
}