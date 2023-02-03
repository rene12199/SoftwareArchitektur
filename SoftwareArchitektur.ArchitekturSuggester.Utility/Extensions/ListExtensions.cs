namespace SoftwareArchitektur.Utility.Extensions;

public static class ListExtensions
{
    public static double StandardDeviation(this IEnumerable<double> values)
    {
        if (!values.Any()) return 0;
        double avg = values.Average();
        return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
    }

    public static IEnumerable<T> Slice<T>(this IList<T> values, int from, int to)
    {
        try
        {
            var newList = new List<T>();

            int counter = from;
            while (counter < to)
            {
                if (counter >= values.Count) return newList;
                newList.Add(values[counter]);
                counter++;
            }

            return newList;
        }
        catch
        {
            return null;
        }
    }
}