namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;

public class MergeRequestModel
{
    public MergeRequestModel(GroupingPackageModel basePackageModel, GroupingPackageModel toBeMergedModel)
    {
        if (basePackageModel == toBeMergedModel)
        {
            throw new ArgumentException("Cant  Merge and ToBeMerged can not be the same");
        }
        BasePackageModel = basePackageModel;
        ToBeMergedModel = toBeMergedModel;
    }

    public GroupingPackageModel BasePackageModel { get; set; }
    
    public GroupingPackageModel ToBeMergedModel { get; set; }
    
}