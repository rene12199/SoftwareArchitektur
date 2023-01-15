namespace SoftwareArchitektur.ArchitekturSuggester.Tests;

public class ManualTesting
{
    //todo Test if Louvian Network gets YOu The same as the shit you Programmed
    [Test]
    public void bla()
    {
        var _services = new List<string>();
        _services.Add("1");
        _services.Add("2");
        var allCallers = new List<string>();
        allCallers.Add("1");

        var tes = _services.Where(s => allCallers.Any(c => c != s)).ToList();
    }
}