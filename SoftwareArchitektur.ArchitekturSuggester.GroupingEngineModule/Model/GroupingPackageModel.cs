using System.Collections.ObjectModel;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;

public class GroupingPackageModel
{
    
    private readonly PackageModel _packageModel;

    public string PackageName => _packageModel.PackageName;
    
    public int Layer { get; private set; }
    
    private ReadOnlyCollection<GroupingCommonChangeModel> ChangesWith { get; set; } 
    
    private ReadOnlyCollection<GroupingDependendencyModel> DependsOn { get; set; }
    
    private ReadOnlyCollection<GroupingDependendencyModel> CalledBy { get; set; }
        
    public GroupingPackageModel(PackageModel packageModel, ReadOnlyCollection<GroupingDependendencyModel> calledBy, ReadOnlyCollection<GroupingDependendencyModel> dependsOn, ReadOnlyCollection<GroupingCommonChangeModel> changesWith)
    {
        _packageModel = packageModel;
        CalledBy = calledBy;
        DependsOn = dependsOn;
        ChangesWith = changesWith;
    }
}