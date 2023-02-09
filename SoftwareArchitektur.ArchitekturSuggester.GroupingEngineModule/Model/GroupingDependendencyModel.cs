using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;

public class GroupingDependendencyModel
{
    public PackageModel Caller { get; private set; }

    public PackageModel Callee { get; private set; }

    public long NumberOfCalls { get; private set; }

    public bool HasBeenLookedAt { get; set; } = false;

    public void AddCalls(long numberOfChanges)
    {
        NumberOfCalls += numberOfChanges;
    }

    public GroupingDependendencyModel(PackageModel ownPackage, PackageModel? differentPackage, long numberOfCalls)
    {
        Caller = ownPackage;
        Callee = differentPackage;
        NumberOfCalls = numberOfCalls;
    }


    public bool Equals(GroupingDependendencyModel? other)
    {
        return other != null && Caller.Equals(other.Callee) && Callee.Equals(other.Callee);
    }

    public bool Equals(string ownPackage, string otherPackage)
    {
        return Caller.Equals(ownPackage) && Callee.Equals(otherPackage);
    }
}