namespace SoftwareArchitektur.Utility.Extensions;

public static class ListExtensions
{
    public static double StandardDeviation(this IEnumerable<double> values)
    {
        if (!values.Any()) return 0;
        double avg = values.Average();
        return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
    }
}