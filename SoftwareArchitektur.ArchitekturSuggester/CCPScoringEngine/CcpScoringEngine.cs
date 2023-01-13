using System.Collections.ObjectModel;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.CCPScoringEngine;

public class CcpScoringEngine
{
    private readonly ReadOnlyCollection<ServiceModel> _serviceModelLookup;

    private readonly IList<CcpScoringCommonChangeClass> _commonCcpChangeList;

    private ReadOnlyCollection<PackageModel> _packageModelLookUp;
    
    private readonly IList<ServiceModel> _remainingServices;

    public CcpScoringEngine(IList<PackageModel> packageModelList, IList<ServiceModel> serviceModelList,ReadOnlyCollection<ServiceModel> serviceModelLookup, IList<CommonChangeRelationModel> changeRelationModelList)
    {
        _serviceModelLookup = serviceModelLookup;
        _packageModelLookUp = packageModelList.ToList().AsReadOnly();
        _commonCcpChangeList = CreateCcpCommonChangeList(changeRelationModelList);
        _remainingServices = serviceModelList;
    }

    private IList<CcpScoringCommonChangeClass> CreateCcpCommonChangeList(IList<CommonChangeRelationModel> commonChangeList)
    {
        var commonCcpChangeList = new List<CcpScoringCommonChangeClass>();
        
        foreach (var commonChange in commonChangeList)
        {
            if (CcpCommonChangeExists(commonChange))
            {
                commonCcpChangeList.Add(new CcpScoringCommonChangeClass(_serviceModelLookup.Single(s => s.Name == commonChange.NameOfCurrentService), _serviceModelLookup.Single(s => s.Name == commonChange.NameOfCurrentService), commonChange.NumberOfChanges));
            }
        }

        return commonCcpChangeList;
    }

    private bool CcpCommonChangeExists(CommonChangeRelationModel commonChange)
    {
        return _commonCcpChangeList.Any(cc => cc.Equals(commonChange));
    }


    class CcpScoringCommonChangeClass : IEquatable<CommonChangeRelationModel>
    {
        public ServiceModel ServiceModel1 { get; private set; }
        
        public ServiceModel ServiceModel2 { get; private set; }

        public long NumberOfChanges  { get; private set; }

        public CcpScoringCommonChangeClass( ServiceModel serviceModel1, ServiceModel serviceModel2, long numberOfChanges)
        {
            ServiceModel2 = serviceModel2;
            ServiceModel1 = serviceModel1;
            NumberOfChanges = numberOfChanges;
        }
        
        protected bool Equals(CcpScoringCommonChangeClass other)
        {
            return (ServiceModel1.Equals(other.ServiceModel1) && ServiceModel2.Equals(other.ServiceModel2)  || ServiceModel1.Equals(other.ServiceModel2) && ServiceModel2.Equals(other.ServiceModel1)) && other.NumberOfChanges == NumberOfChanges;
        }

        public bool Equals(CommonChangeRelationModel? other)
        {
            return other != null && (ServiceModel1.Name.Equals(other.NameOfCurrentService) && ServiceModel2.Name.Equals(other.NameOfOtherService)  || ServiceModel1.Name.Equals(other.NameOfOtherService) && ServiceModel2.Name.Equals(other.NameOfCurrentService) ) && other.NumberOfChanges == NumberOfChanges;

        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ServiceModel1, ServiceModel2, NumberOfChanges);
        }
    }
}