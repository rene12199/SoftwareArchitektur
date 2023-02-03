using SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine.Models;
using SoftwareArchitektur.ArchitekturSuggester.TestUtility;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine.Tests.Models;

public class CcpScoringCommonChangeClassTesting
{
    ///[Test, MaxTime(2000)]
    [TestCase("S1", "S2", "S1", "S2", true)]
    [TestCase("S1", "S2", "S2", "S1", false)]
    [TestCase("S1", "S2", "S2", "", false)]
    [TestCase("S1", "S2", "S2", "S2", false)]
    [TestCase("S2", "S2", "S2", "S2", true)]
    [TestCase("S2", "", "", "S2", false)]
    [TestCase("", "S2", "", "S2", true)]
    public void EqualsTesting(string commonChange1OwnService, string commonChange1OtherService, string commonChange2OwnService, string commonChange2OtherService, bool expected)
    {
        var serviceFactory = new TestServiceModelFactory();
        var commonChange1 =
            new CcpScoringCommonChangeClass(serviceFactory.CreateServiceModel(null, new PackageModel(commonChange1OwnService)),
                serviceFactory.CreateServiceModel(null, new PackageModel(commonChange1OtherService)), 1);
        var commonChange2 =
            new CcpScoringCommonChangeClass(serviceFactory.CreateServiceModel(null, new PackageModel(commonChange2OwnService)),
                serviceFactory.CreateServiceModel(null, new PackageModel(commonChange2OtherService)), 1);

        Assert.That(commonChange1.Equals(commonChange2), Is.EqualTo(expected));
    }
}