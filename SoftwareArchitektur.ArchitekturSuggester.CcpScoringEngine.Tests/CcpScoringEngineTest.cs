using Moq;
using SoftwareArchitektur.Utility.Interface;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine.Tests;

public class CcpScoringEngineTest
{
    private Mock<IDataProvider> _dataProvider;

    [SetUp]
    public void SetUp()
    {
        _dataProvider = new Mock<IDataProvider>();
    }

    [Test]
    public void CcpScoringEngineTest_NoPackagesSet_ThrowsApplicationException()
    {
        //Arrange
        _dataProvider.Setup(d => d.GetServices()).Returns(new List<ServiceModel>());
        var ccpScoringEngine = new CcpScoringEngine(_dataProvider.Object);
        var remainingServices = new List<ServiceModel>();

        //Act / Assert
        Assert.Throws<ApplicationException>(() => ccpScoringEngine.DistributePackages(remainingServices));
    }


    [Test]
    public void CcpScoringEngineTest_2PackagesWithDifferentCallNumber_AddedToPackageWithHigherCallNumber()
    {
        //Arrange
        var packages = new List<PackageModel>();
        for (int i = 1; i < 3; i++)
        {
            packages.Add(new PackageModel($"P{i}"));
        }
        
        
        
        packages.First().AddService(new ServiceModel("S1"));
        packages.Last().AddService(new ServiceModel("S2"));

        //todo add Services to LookUp
        _dataProvider.Setup(d => d.GetServices()).Returns(new List<ServiceModel>());
        var ccpScoringEngine = new CcpScoringEngine(_dataProvider.Object);

        var remainingServices = new List<ServiceModel>();

        remainingServices.Add(new ServiceModel("RS1")
        {
            ChangedWith =
            {
                new CommonChangeRelationModel()
                {
                    NameOfCurrentService = "RS1",
                    NameOfOtherService = "S1",
                    NumberOfChanges = 1
                },
                new CommonChangeRelationModel()
                {
                    NameOfCurrentService = "RS1",
                    NameOfOtherService = "S2",
                    NumberOfChanges = 2
                }
            }
        });

        ccpScoringEngine.SetPossiblePackages(packages);
        //Act 
        var result = ccpScoringEngine.DistributePackages(remainingServices);

        //Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.First().PackageName, Is.EqualTo("P1"));
        Assert.That(result.First().GetServices().Count, Is.EqualTo(1)); 
        
        Assert.That(result.Last().PackageName, Is.EqualTo("P1"));
        Assert.That(result.Last().GetServices().Count, Is.EqualTo(2));
    }
}