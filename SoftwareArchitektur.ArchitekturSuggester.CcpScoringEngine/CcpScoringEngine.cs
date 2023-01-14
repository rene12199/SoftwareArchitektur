using System.Collections.ObjectModel;
using SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine.Converter;
using SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine.Interfaces;
using SoftwareArchitektur.Utility.Interface;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine;

public class CcpScoringEngine : ICcpScoringEngine
{
    private readonly ReadOnlyCollection<ServiceModel> _serviceModelLookup;

    private ReadOnlyCollection<PackageModel> _packageModels = null!;

    private readonly CommonChangeToCcpCommonChangeConverter _converter;


    public CcpScoringEngine(IDataProvider dataProvider)
    {
        _serviceModelLookup = dataProvider.GetServices().ToList().AsReadOnly();
        _converter = new CommonChangeToCcpCommonChangeConverter(dataProvider);
    }

    public void SetPossiblePackages(IList<PackageModel> packagesModels)
    {
        _packageModels = packagesModels.ToList().AsReadOnly();
    }

    public IList<PackageModel> DistributePackages(IList<ServiceModel> remainingServices)
    {
        if (_packageModels == null) throw new ApplicationException("No Packagemodel List set");

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