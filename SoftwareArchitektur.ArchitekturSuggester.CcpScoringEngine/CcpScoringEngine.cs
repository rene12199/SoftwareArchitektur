using System.Collections.ObjectModel;
using SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine.Converter;
using SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine.Interfaces;
using SoftwareArchitektur.Utility.Interface;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine;

public class CcpScoringEngine : ICcpScoringEngine
{

    private ReadOnlyCollection<PackageModel> _packageModels = null!;

    private readonly CommonChangeToCcpCommonChangeConverter _converter;


    public CcpScoringEngine(IDataProvider dataProvider)
    {
        _converter = new CommonChangeToCcpCommonChangeConverter(dataProvider);
    }

    public void SetPossiblePackages(IList<PackageModel> packagesModels)
    {
        _packageModels = packagesModels.ToList().AsReadOnly();
    }

    public IList<PackageModel> DistributePackages(IList<ServiceModel> remainingServices)
    {
        if (_packageModels == null || _packageModels.Count == 0) throw new ApplicationException("No Packagemodel List set");

        foreach (var remainingService in remainingServices)
        {
            var bestPackage = _converter.CreateCcpCommonChangeList(remainingService.ChangedWith).OrderByDescending(d => d.NumberOfChanges).First();
            _packageModels.Single(s => s.PackageName == bestPackage.OtherPackage).AddService(remainingService);
        }

        return _packageModels;
    }
}