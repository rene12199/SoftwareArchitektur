using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingModule;

public class GroupingEngine
{
    private readonly IList<PackageModel> _packages = null!;
    private readonly IList<ServiceModel> _serviceLookUp;
    private readonly IList<CommonChangeRelationModel> _commonChangeRelations;

    public GroupingEngine(IList<ServiceModel> serviceLookUp, IList<CommonChangeRelationModel> commonChangeRelations)
    {
        _serviceLookUp = serviceLookUp;
        _commonChangeRelations = commonChangeRelations;
    }


    public IList<PackageModel> GroupPackages()
    {
        return _packages;
    }
}