using SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine.Models;
using SoftwareArchitektur.ArchitekturSuggester.TestUtility;

namespace SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine.Tests.Models;

public class CcpScoringCommonChangeClassTesting
{

    [Test, MaxTime(2000)]
    [TestCase("S1", "S2", "S1", "S2",true )]
    [TestCase("S1", "S2", "S2", "S1",true )]
    [TestCase("S1", "S2", "S2", "",false )]
    [TestCase("S1", "S2", "S2", "S2",false )]
    [TestCase("S2", "S2", "S2", "S2",true )]
    [TestCase("S2", "", "", "S2",true )]
    [TestCase("", "S2", "", "S2",false )]
    public void EqualsTesting(string commonChange1OwnService, string commonChange1OtherService, string commonChange2OwnService, string commonChange2OtherService, bool expected)
    {
        var serviceFactory = new TestServiceModelFactory();
        var commonChange1 =
            new CcpScoringCommonChangeClass(serviceFactory.CreateServiceModel("",commonChange1OwnService), serviceFactory.CreateServiceModel("",commonChange1OtherService), 1);
        var commonChange2 =
            new CcpScoringCommonChangeClass(serviceFactory.CreateServiceModel("",commonChange2OwnService), serviceFactory.CreateServiceModel("",commonChange2OtherService), 1);
        
        Assert.That(commonChange1.Equals(commonChange2), Is.EqualTo(expected));
        
    }
}