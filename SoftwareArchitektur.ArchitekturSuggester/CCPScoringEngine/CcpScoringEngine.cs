using System.Collections.ObjectModel;
using SoftwareArchitektur.ArchitekturSuggester.CCPScoringEngine.Scoring;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.CCPScoringEngine;

public class CcpScoringEngine
{
    private readonly ReadOnlyCollection<ServiceModel> _serviceModelLookup;

    private readonly IList<CcpScoringCommonChangeClass> _commonCcpChangeList;

    private ReadOnlyCollection<PackageModel> _packageModels;

    private readonly IList<ServiceModel> _remainingServices;

    public CcpScoringEngine(IList<PackageModel> packageModelList, IList<ServiceModel> serviceModelList, ReadOnlyCollection<ServiceModel> serviceModelLookup,
        IList<CommonChangeRelationModel> changeRelationModelList)
    {
        _serviceModelLookup = serviceModelLookup;
        _packageModels = packageModelList.ToList().AsReadOnly();
        _commonCcpChangeList = CreateCcpCommonChangeList(changeRelationModelList);
        _remainingServices = serviceModelList;
    }

    public IList<PackageModel> DistributePackages()
    {
        foreach (var remainingService in _remainingServices)
        {
            var changes = CreateCcpCommonChangeList(remainingService.ChangedWith);
            Move bestMove = new Move(remainingService);

            CalculateBestPackageForMove(_packageModels.ToList(), bestMove);

            ExecuteMove(bestMove);
        }

        return _packageModels;
    }
    
    private void CalculateBestPackageForMove(List<PackageModel> packages, Move bestMove)
    {
        var mostChangedWith = bestMove.Service.ChangedWith.OrderBy(c => c.NumberOfChanges);

        foreach (var changeRelation in mostChangedWith)
        {
            foreach (var package in packages)
            {
                if (package.GetServices().Any(s => s.Name == changeRelation.NameOfOtherService))
                {
                    bestMove.BestPackage = package;
                    return;
                }
            }
        }

        if (bestMove.BestPackage == null)
        {
            bestMove.BestPackage = packages
                .OrderBy(p => Math.Abs(p.AverageChangeRate - bestMove.Service.AverageChange))
                .FirstOrDefault();
        }
    }

    private double CalculateDifferenceInStandardDeviation(PackageModel package, Move bestMove)
    {
        var newScore =
            Math.Abs(Math.Sqrt(Math.Pow(package.StandardDeviationOfChangeRate, 2) +
                               Math.Pow(bestMove.Service.StandardDeviationChangeRate, 2)) -
                     package.StandardDeviationOfChangeRate);
        return newScore;
    }

    private void ExecuteMove(Move bestMove)
    {
        bestMove.BestPackage.AddService(bestMove.Service);
        _services.Remove(bestMove.Service);
    }

    private void DistributeRemainingPackagesByCcpScore(List<PackageModel> packages)
    {
        
       

        
    }

    private IList<CcpScoringCommonChangeClass> CreateCcpCommonChangeList(IList<CommonChangeRelationModel> commonChangeList)
    {
        var commonCcpChangeList = new List<CcpScoringCommonChangeClass>();

        ConvertCommonChangeModelToCcpCommonChangeModel(commonChangeList, commonCcpChangeList);

        return commonCcpChangeList;
    }

    private void ConvertCommonChangeModelToCcpCommonChangeModel(IList<CommonChangeRelationModel> commonChangeList, List<CcpScoringCommonChangeClass> commonCcpChangeList)
    {
        foreach (var commonChange in commonChangeList)
        {
            if (CcpCommonChangeExists(commonChange))
            {
                commonCcpChangeList.Add(new CcpScoringCommonChangeClass(GetServiceFromLookUp(commonChange.NameOfCurrentService),
                    GetServiceFromLookUp(commonChange.NameOfOtherService), commonChange.NumberOfChanges));
            }
            else
            {
                var existingCommonChange = commonCcpChangeList.Single(cc =>
                    cc.Equals(GetServiceFromLookUp(commonChange.NameOfCurrentService).InPackage, GetServiceFromLookUp(commonChange.NameOfOtherService).InPackage));

                existingCommonChange.AddChanges(commonChange.NumberOfChanges);
            }
        }
    }

    private ServiceModel GetServiceFromLookUp(string serviceName)
    {
        return _serviceModelLookup.Single(s => s.Name == serviceName);
    }

    private bool CcpCommonChangeExists(CommonChangeRelationModel commonChange)
    {
        return _commonCcpChangeList.Any(cc =>
            cc.Equals(GetServiceFromLookUp(commonChange.NameOfCurrentService).InPackage, GetServiceFromLookUp(commonChange.NameOfOtherService).InPackage));
    }


    class CcpScoringCommonChangeClass
    {
        public string Package1 { get; private set; }

        public string Package2 { get; private set; }

        public long NumberOfChanges { get; private set; }

        public void AddChanges(long numberOfChanges) => NumberOfChanges += numberOfChanges;

        public CcpScoringCommonChangeClass(ServiceModel serviceModel1, ServiceModel serviceModel2, long numberOfChanges)
        {
            Package1 = serviceModel2.InPackage;
            Package2 = serviceModel1.InPackage;
            NumberOfChanges = numberOfChanges;
        }

        protected bool Equals(CcpScoringCommonChangeClass? other)
        {
            return other != null && (Package1.Equals(other.Package1) && Package2.Equals(other.Package2) || Package1.Equals(other.Package2) && Package2.Equals(other.Package1));
        }

        public bool Equals(string package1, string package2)
        {
            return this.Package1.Equals(package1) && Package2.Equals(package2) || this.Package1.Equals(package2) && Package2.Equals(package1);
        }
    }
}