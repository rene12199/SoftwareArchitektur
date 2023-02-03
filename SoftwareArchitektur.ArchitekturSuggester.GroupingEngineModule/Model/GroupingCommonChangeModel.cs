
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;

public class GroupingCommonChangeModel
{
    public GroupingCommonChangeModel(PackageModel ownPackage, PackageModel differentPackage, long numberOfChanges)
    {
        ThisPackage = ownPackage;
        OtherPackage = differentPackage;
        NumberOfChanges = numberOfChanges;
    }
    
    
    public PackageModel ThisPackage { get; private set; }

    public PackageModel OtherPackage { get; private set; }

    public long NumberOfChanges { get; private set; }

    public void AddChanges(long numberOfChanges)
    {
        NumberOfChanges += numberOfChanges;
    }


    public bool Equals(GroupingCommonChangeModel? other)
    {
        return other != null && ThisPackage.Equals(other.ThisPackage) && OtherPackage.Equals(other.OtherPackage);
    }

    public bool Equals(string ownPackage, string otherPackage)
    {
        return ThisPackage.Equals(ownPackage) && OtherPackage.Equals(otherPackage);
    }
}