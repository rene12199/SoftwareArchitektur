using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;

public class GroupingCommonChangeModel
{
    public string NameOfCurrentService { get; private set; } = null!;
    
    public string OtherService { get; private set; } = null!;
    
    public long NumberOfChanges { get; set; }

    public GroupingCommonChangeModel(CommonChangeRelationModel model)
    {
        NameOfCurrentService = model.NameOfCurrentService;
        NameOfCurrentService = model.NameOfOtherService;
        NumberOfChanges = model.NumberOfChanges;
    }
}