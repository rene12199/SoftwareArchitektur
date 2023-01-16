using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine.Models;

public class CcpScoringCommonChangeClass
{
    public string ThisPackage { get; private set; }

    public string OtherPackage { get; private set; }

    public long NumberOfChanges { get; private set; }

    public void AddChanges(long numberOfChanges)
    {
        NumberOfChanges += numberOfChanges;
    }

    public CcpScoringCommonChangeClass(ServiceModel ownPackage, ServiceModel? differentPackage, long numberOfChanges)
    {
        ThisPackage = ownPackage.InPackage;
        OtherPackage = differentPackage != null ? differentPackage.InPackage : String.Empty;
        NumberOfChanges = numberOfChanges;
    }
    
    
    public bool Equals(CcpScoringCommonChangeClass? other)
    {
        return other != null && ThisPackage.Equals(other.ThisPackage) && OtherPackage.Equals(other.OtherPackage);
    }

    public bool Equals(string ownPackage, string otherPackage)
    {
        return ThisPackage.Equals(ownPackage) && OtherPackage.Equals(otherPackage);
    }
}