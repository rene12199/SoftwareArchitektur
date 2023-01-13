using System.Collections.ObjectModel;
using SoftwareArchitektur.ArchitekturSuggester.CCPScoringEngine.Converter;
using SoftwareArchitektur.ArchitekturSuggester.CCPScoringEngine.Models;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.CCPScoringEngine;

public class CcpScoringEngine
{
    private readonly ReadOnlyCollection<ServiceModel> _serviceModelLookup;

    private ReadOnlyCollection<PackageModel> _packageModels;

    private CommonChangeToCcpCommonChageConverter _converter;


    public CcpScoringEngine(IList<PackageModel> packageModelList, ReadOnlyCollection<ServiceModel> serviceModelLookup)
    {
        _serviceModelLookup = serviceModelLookup;
        _packageModels = packageModelList.ToList().AsReadOnly();
        _converter = new CommonChangeToCcpCommonChageConverter(_serviceModelLookup);
    }

    public IList<PackageModel> DistributePackages(IList<ServiceModel> remainingServices)
    {
        foreach (var remainingService in remainingServices)
        {
            var bestPackage = _converter.CreateCcpCommonChangeList(remainingService.ChangedWith);
        }

        return _packageModels;
    }

    private ServiceModel GetServiceFromLookUp(string serviceName)
    {
        return _serviceModelLookup.Single(s => s.Name == serviceName);
    }

}