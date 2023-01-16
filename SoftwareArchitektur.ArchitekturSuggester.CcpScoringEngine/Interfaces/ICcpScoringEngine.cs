using SoftwareArchitektur.Utility.Interface;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine.Interfaces;

public interface ICcpScoringEngine
{
    public void SetPossiblePackages(IList<PackageModel> packagesModels);

    public IList<PackageModel> DistributeRemainingServices(IList<ServiceModel> remainingServices);
}