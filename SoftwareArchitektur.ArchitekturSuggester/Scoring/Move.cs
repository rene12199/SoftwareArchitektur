namespace SoftwareArchitektur.ArchitekturSuggester.Scoring;

public class Move
{
    public Move(string serviceName, string packageName)
    {
        ServiceName = serviceName;
        PackageName = packageName;
    }
    
    public string ServiceName { get; set; }
    
    public string PackageName { get; set; }
    public long DependencyScore { get; set; } = 0;
    public long ChangeScore { get; set; } = 0;
    public long CircularScore { get; set; } = 0;

    public long TotalScore
    {
        get => DependencyScore + ChangeScore + CircularScore;
    }
}