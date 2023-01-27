namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;

public class GroupingPackageModel
{
    public string PackageName { get; }
    
    public List<GroupingCommonChangeModel> ChangedWith { get; }
    
    public List<GroupingDependendencyModel> DependsOn { get; }
    
    
    public GroupingPackageModel(string packageName, List<GroupingCommonChangeModel> changedWith, List<GroupingDependendencyModel> dependsOn)
    {
        PackageName = packageName;
        ChangedWith = changedWith;
        DependsOn = dependsOn;
    }


}