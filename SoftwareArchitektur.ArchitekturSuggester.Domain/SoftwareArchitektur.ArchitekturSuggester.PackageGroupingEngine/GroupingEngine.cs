

using SoftwareArchitektur.Utility.Interface;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingModule;

public class GroupingEngine
{
    private IList<PackageModel> _packages = null!;

    private IDataProvider _dataProvider = null!;

    public GroupingEngine(IDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }


    public void SetPackages(IList<PackageModel> packages)
    {
        _packages = packages;
    }
    
    public IList<PackageModel> GroupPackages()
    {
        return _packages;
    }
}