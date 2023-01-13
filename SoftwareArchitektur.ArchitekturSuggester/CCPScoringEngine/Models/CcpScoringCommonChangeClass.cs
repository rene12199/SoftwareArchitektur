using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.CCPScoringEngine.Models;

public class CcpScoringCommonChangeClass
{
    public string ThisPackage { get; private set; }

    public string OtherPackage { get; private set; }

    public long NumberOfChanges { get; private set; }

    public void AddChanges(long numberOfChanges)
    {
        NumberOfChanges += numberOfChanges;
    }

    public CcpScoringCommonChangeClass(ServiceModel serviceModel1, ServiceModel serviceModel2, long numberOfChanges)
    {
        ThisPackage = serviceModel2.InPackage;
        OtherPackage = serviceModel1.InPackage;
        NumberOfChanges = numberOfChanges;
    }

    protected bool Equals(CcpScoringCommonChangeClass? other)
    {
        return other != null && ((ThisPackage.Equals(other.ThisPackage) && OtherPackage.Equals(other.OtherPackage)) ||
                                 (ThisPackage.Equals(other.OtherPackage) && OtherPackage.Equals(other.ThisPackage)));
    }

    public bool Equals(string package1, string package2)
    {
        return (ThisPackage.Equals(package1) && OtherPackage.Equals(package2)) || (ThisPackage.Equals(package2) && OtherPackage.Equals(package1));
    }
}