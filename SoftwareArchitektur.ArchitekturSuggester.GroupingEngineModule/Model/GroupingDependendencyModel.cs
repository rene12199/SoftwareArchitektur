using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;

public class GroupingDependendencyModel
{
    public string Caller { get; private set; }

    public string Callee { get; private set; }

    public long NumberOfCalls { get; private set; }

    public void AddCalls(long numberOfChanges)
    {
        NumberOfCalls += numberOfChanges;
    }

    public GroupingDependendencyModel(PackageModel ownPackage, PackageModel? differentPackage, long numberOfCalls)
    {
        Caller = ownPackage.PackageName;
        Callee = differentPackage.PackageName;
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