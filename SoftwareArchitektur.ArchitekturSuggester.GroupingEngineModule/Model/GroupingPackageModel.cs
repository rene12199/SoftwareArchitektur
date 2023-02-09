using System.Collections.ObjectModel;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;

public class GroupingPackageModel
{
    private readonly PackageModel _packageModel;

    public string PackageName => _packageModel.PackageName;

    public int? Layer { get; private set; } = null;

    public ReadOnlyCollection<GroupingCommonChangeModel> ChangesWith { get;  }

    public ReadOnlyCollection<GroupingDependendencyModel> DependsOn { get;  }

    public List<GroupingDependendencyModel> CalledBy { get; } = new();

    public GroupingPackageModel(PackageModel packageModel, IList<GroupingDependendencyModel> dependsOn, IList<GroupingCommonChangeModel> changesWith)
    {
        _packageModel = packageModel;
        DependsOn = dependsOn.Where(p => p.Callee != null && p.Callee.PackageName != PackageName  ).ToList().AsReadOnly();
        ChangesWith = changesWith.Where(p => p.OtherPackage != null && p.OtherPackage.PackageName != PackageName  ).ToList().AsReadOnly();
    }
    
    public void SetLayer(int layer)
    {
        Layer = layer;
    }
}