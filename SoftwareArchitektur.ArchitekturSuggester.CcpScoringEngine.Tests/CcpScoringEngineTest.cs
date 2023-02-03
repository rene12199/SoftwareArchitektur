using Moq;
using SoftwareArchitektur.ArchitekturSuggester.TestUtility;
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
    [MaxTime(2000)]
    public void CcpScoringEngineTest_NoPackagesSet_ThrowsApplicationException()
    {
        //Arrange
        _dataProvider.Setup(d => d.GetServices()).Returns(new List<ServiceModel>());
        var ccpScoringEngine = new CcpScoringEngine(_dataProvider.Object);
        var remainingServices = new List<ServiceModel>();

        //Act / Assert
        Assert.Throws<ApplicationException>(() => ccpScoringEngine.DistributeRemainingServices(remainingServices.ToList()));
    }


    [Test]
    [MaxTime(2000)]
    public void CcpScoringEngineTest_2PackagesWithDifferentCallNumber_AddedToPackageWithHigherCallNumber()
    {
        var serviceModelFactory = new TestServiceModelFactory();
        var s1 = serviceModelFactory.CreateServiceModel("S1");
        var s2 = serviceModelFactory.CreateServiceModel("S2");
        var rs1 = serviceModelFactory.CreateServiceModel("RS1");

        //Arrange
        var packages = new List<PackageModel>();
        for (int i = 1; i < 3; i++) packages.Add(new PackageModel($"P{i}"));


        packages.First().AddService(s1);
        packages.Last().AddService(s2);

        var remainingServices = new List<ServiceModel>();

        remainingServices.Add(serviceModelFactory.CreateServiceModel(rs1.Name, sm =>
        {
            sm.ChangedWith.AddRange(new List<CommonChangeRelationServiceModel>
            {
                new(rs1, s1, 1),
                new(rs1, s2, 2)
            });
            return 0;
        }));

        _dataProvider.Setup(d => d.GetServices()).Returns(serviceModelFactory.ServiceModels);
        var ccpScoringEngine = new CcpScoringEngine(_dataProvider.Object);
        ccpScoringEngine.SetPossiblePackages(packages);
        //Act 
        var result = ccpScoringEngine.DistributeRemainingServices(remainingServices);

        //Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.First().PackageName, Is.EqualTo("P1"));
        Assert.That(result.First().GetServices().Count, Is.EqualTo(1));

        Assert.That(result.Last().PackageName, Is.EqualTo("P2"));
        Assert.That(result.Last().GetServices().Count, Is.EqualTo(2));
    }

    [Test]
    [MaxTime(2000)]
    public void CcpScoringEngineTest_2PackagesWithMultipleServicesDifferentCallNumber_AddedToPackageWithHigherCallNumber()
    {
        //Arrange
        var serviceModelFactory = new TestServiceModelFactory();
        var s1 = serviceModelFactory.CreateServiceModel("S1");
        var s2 = serviceModelFactory.CreateServiceModel("S2");
        var s3 = serviceModelFactory.CreateServiceModel("S4");
        var rs1 = serviceModelFactory.CreateServiceModel("RS1");

        var packages = new List<PackageModel>();
        for (int i = 1; i < 3; i++) packages.Add(new PackageModel($"P{i}"));


        packages.First().AddService(s1);
        packages.Last().AddService(s2);
        packages.Last().AddService(s3);

        s1.ChangedWith.Add(new CommonChangeRelationServiceModel(rs1, s1, 1));
        s1.ChangedWith.Add(new CommonChangeRelationServiceModel(rs1, s2, 2));
        s1.ChangedWith.Add(new CommonChangeRelationServiceModel(rs1, s3, 2));

        var remainingServices = new List<ServiceModel>();

        remainingServices.Add(s1);

        _dataProvider.Setup(d => d.GetServices()).Returns(serviceModelFactory.ServiceModels);
        var ccpScoringEngine = new CcpScoringEngine(_dataProvider.Object);
        ccpScoringEngine.SetPossiblePackages(packages);
        //Act 
        var result = ccpScoringEngine.DistributeRemainingServices(remainingServices);

        //Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.First().PackageName, Is.EqualTo("P1"));
        Assert.That(result.First().GetServices().Count, Is.EqualTo(1));

        Assert.That(result.Last().PackageName, Is.EqualTo("P2"));
        Assert.That(result.Last().GetServices().Count, Is.EqualTo(3));
    }


    [Test]
    [MaxTime(2000)]
    public void CcpScoringEngineTest_ManyServicesOnly1InPackage_AllAddedTo1Package()
    {
        //Arrange
        var serviceModelFactory = new TestServiceModelFactory();

        var packages = new List<PackageModel>();
        for (int i = 1; i < 3; i++) packages.Add(new PackageModel($"P{i}"));
        var s1 = serviceModelFactory.CreateServiceModel("S1");
        packages.First().AddService(s1);

        var remainingServices = new List<ServiceModel>();
        var rs1 = serviceModelFactory.CreateServiceModel("RS1");
        var rs2 = serviceModelFactory.CreateServiceModel("RS2");
        var rs3 = serviceModelFactory.CreateServiceModel("RS3");

        rs1.ChangedWith.AddRange(new[]
        {
            new CommonChangeRelationServiceModel(rs1, s1, 1)
        });

        rs2.ChangedWith.AddRange(new[]
        {
            new CommonChangeRelationServiceModel(rs2, rs1, 1)
        });

        rs3.ChangedWith.AddRange(new[]
        {
            new CommonChangeRelationServiceModel(rs3, rs2, 1)
        });


        remainingServices.Add(rs1);
        remainingServices.Add(rs3);
        remainingServices.Add(rs2);
        remainingServices.Reverse();

        _dataProvider.Setup(d => d.GetServices()).Returns(serviceModelFactory.ServiceModels);
        var ccpScoringEngine = new CcpScoringEngine(_dataProvider.Object);
        ccpScoringEngine.SetPossiblePackages(packages);

        //Act 
        var result = ccpScoringEngine.DistributeRemainingServices(remainingServices);

        //Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.First().PackageName, Is.EqualTo("P1"));
        Assert.That(result.First().GetServices().Count, Is.EqualTo(4));
    }


    [Test]
    [MaxTime(2000)]
    public void CcpScoringEngineTest_ServiceWithOutCommonChangesAdded_RemovedWithOutBeingAdded()
    {
        //Arrange
        var serviceModelFactory = new TestServiceModelFactory();
        var packages = new List<PackageModel>();
        for (int i = 1; i < 3; i++) packages.Add(new PackageModel($"P{i}"));


        packages.First().AddService(serviceModelFactory.CreateServiceModel("S1"));

        var remainingServices = new List<ServiceModel>();

        remainingServices.Add(serviceModelFactory.CreateServiceModel("IS"));


        remainingServices.Reverse();
        _dataProvider.Setup(d => d.GetServices()).Returns(serviceModelFactory.ServiceModels);
        var ccpScoringEngine = new CcpScoringEngine(_dataProvider.Object);
        ccpScoringEngine.SetPossiblePackages(packages);
        //Act 
        var result = ccpScoringEngine.DistributeRemainingServices(remainingServices);

        //Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.First().PackageName, Is.EqualTo("P1"));
        Assert.That(result.First().GetServices().Count, Is.EqualTo(1));
    }

    [Test]
    [MaxTime(2000)]
    public void CcpScoringEngineTest_ServiceWithOutAnyConnectionAdded_NotAddedToAnything()
    {
        //Arrange
        var packages = new List<PackageModel>();
        for (int i = 1; i < 2; i++) packages.Add(new PackageModel($"P{i}"));

        var serviceModelFactory = new TestServiceModelFactory();
        var s1 = serviceModelFactory.CreateServiceModel("S1");
        packages.First().AddService(s1);

        var iS = serviceModelFactory.CreateServiceModel("IS");
        iS.ChangedWith.Add(new CommonChangeRelationServiceModel(iS, new ServiceModel("Unkown"), 1));

        var rs1 = serviceModelFactory.CreateServiceModel("RS1");
        rs1.ChangedWith.Add(new CommonChangeRelationServiceModel(rs1, s1, 1));


        var remainingServices = new List<ServiceModel>();
        var rs2 = serviceModelFactory.CreateServiceModel("RS2");
        rs2.ChangedWith.Add(new CommonChangeRelationServiceModel(rs2, rs1, 1));

        var rs3 = serviceModelFactory.CreateServiceModel("RS3");
        rs3.ChangedWith.Add(new CommonChangeRelationServiceModel(rs3, rs2, 1));


        remainingServices.Add(rs1);
        remainingServices.Add(rs3);
        remainingServices.Add(rs2);
        remainingServices.Reverse();
        _dataProvider.Setup(d => d.GetServices()).Returns(serviceModelFactory.ServiceModels);
        var ccpScoringEngine = new CcpScoringEngine(_dataProvider.Object);
        ccpScoringEngine.SetPossiblePackages(packages);

        //Act 
        var result = ccpScoringEngine.DistributeRemainingServices(remainingServices);

        //Assert
        Assert.That(result.First().PackageName, Is.EqualTo("P1"));
        Assert.That(result.First().GetServices().Count, Is.EqualTo(4));
    }

    [Test]
    [MaxTime(2000)]
    public void CcpScoringEngineTest_ServiceWith2CommonChangesWithNeededLenientMode_AddedAfterActivatingLenientMode()
    {
        //Arrange
        var serviceModelFactory = new TestServiceModelFactory();
        var s1 = serviceModelFactory.CreateServiceModel("S1");
        var s2 = serviceModelFactory.CreateServiceModel("S2");
        var rs1 = serviceModelFactory.CreateServiceModel("RS1");
        var packages = new List<PackageModel>();
        for (int i = 1; i < 2; i++) packages.Add(new PackageModel($"P{i}"));

        rs1.ChangedWith.Add(
            new CommonChangeRelationServiceModel(rs1, s2, 2));
        rs1.ChangedWith.Add(new CommonChangeRelationServiceModel(rs1, s1, 1));

        packages.First().AddService(s1);

        var remainingServices = new List<ServiceModel>();
        var rs2 = serviceModelFactory.CreateServiceModel("RS2");
        rs2.ChangedWith.Add(new CommonChangeRelationServiceModel(rs2, rs1, 1));

        var rs3 = serviceModelFactory.CreateServiceModel("RS3");
        rs3.ChangedWith.Add(new CommonChangeRelationServiceModel(rs3, rs2, 1));


        remainingServices.Add(rs1);
        remainingServices.Add(rs3);
        remainingServices.Add(rs2);
        remainingServices.Reverse();
        _dataProvider.Setup(d => d.GetServices()).Returns(serviceModelFactory.ServiceModels);
        var ccpScoringEngine = new CcpScoringEngine(_dataProvider.Object);
        ccpScoringEngine.SetPossiblePackages(packages);

        //Act 
        var result = ccpScoringEngine.DistributeRemainingServices(remainingServices);

        //Assert
        Assert.That(result.First().PackageName, Is.EqualTo("P1"));
        Assert.That(result.First().GetServices().Count, Is.EqualTo(4));
    }
}