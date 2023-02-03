using Moq;
using SoftwareArchitektur.ArchitekturSuggester.TestUtility;
using SoftwareArchitektur.Utility.Interface;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.Tests;

public class CircularDependencyCheckerTests
{
    private Mock<IDataProvider> _dataProvider;

    [SetUp]
    public void SetUp()
    {
        _dataProvider = new Mock<IDataProvider>();
    }

    [Test]
    [MaxTime(2000)]
    public void CircularDependencyCheckerTests_Only2Dependencies_ReturnsPackageWith2Services()
    {
        //Arrange
        var serviceFactory = new TestServiceModelFactory();
        var s1 = serviceFactory.CreateServiceModel("S1");
        var s2 = serviceFactory.CreateServiceModel("S2");

        s1.DependsOn.Add(new DependencyRelationServiceModel(s1, s2, 1));

        _dataProvider.Setup(s => s.GetServices()).Returns(serviceFactory.ServiceModels);

        var checker = new CircularDependencyChecker.CircularDependencyChecker(_dataProvider.Object);
        //Assert
        var result = checker.CreatePackages();

        //Act
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.NotNull(result.First().GetServices().FirstOrDefault(s => s.Name == "S1"));
    }

    [Test]
    [MaxTime(2000)]
    public void CircularDependencyCheckerTests_Only3DependenciesWithCircularDependency_Returns4PackageWith1Services()
    {
        //Arrange
        var serviceFactory = new TestServiceModelFactory();
        var s1 = serviceFactory.CreateServiceModel("S1");
        var s2 = serviceFactory.CreateServiceModel("S2");
        var s3 = serviceFactory.CreateServiceModel("S3");
        s1.DependsOn.Add(new DependencyRelationServiceModel(s1, s2, 10));


        s2.DependsOn.Add(new DependencyRelationServiceModel(s2, s3, 1));
        s3.DependsOn.Add(new DependencyRelationServiceModel(s3, s1, 1));

        _dataProvider.Setup(s => s.GetServices()).Returns(serviceFactory.ServiceModels);


        var checker = new CircularDependencyChecker.CircularDependencyChecker(_dataProvider.Object);
        //Assert
        var result = checker.CreatePackages();

        //Act
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.NotNull(result.First().GetServices().FirstOrDefault(s => s.Name == "S1"));
        Assert.NotNull(result.First().GetServices().FirstOrDefault(s => s.Name == "S2"));
        Assert.NotNull(result.First().GetServices().FirstOrDefault(s => s.Name == "S3"));
    }

    [Test]
    [MaxTime(2000)]
    public void CircularDependencyCheckerTests_Only4DependenciesWithNoCircularDependency_Returns2PackageWith2Services()
    {
        //Arrange
        var serviceFactory = new TestServiceModelFactory();
        var s1 = serviceFactory.CreateServiceModel("S1");
        var s2 = serviceFactory.CreateServiceModel("S2");
        var s3 = serviceFactory.CreateServiceModel("S3");
        var s4 = serviceFactory.CreateServiceModel("S4");
        s1.DependsOn.Add(new DependencyRelationServiceModel(s1, s2, 10));


        s3.DependsOn.Add(new DependencyRelationServiceModel(s3, s4, 1));


        _dataProvider.Setup(s => s.GetServices()).Returns(serviceFactory.ServiceModels);


        var checker = new CircularDependencyChecker.CircularDependencyChecker(_dataProvider.Object);
        //Assert
        var result = checker.CreatePackages();

        //Act
        Assert.That(result.Count, Is.EqualTo(4));
    }

    [Test]
    [MaxTime(2000)]
    public void CircularDependencyCheckerTests_Only4DependenciesWithCircularDependency_Returns2Packages()
    {
        //Arrange
        var serviceFactory = new TestServiceModelFactory();
        var s1 = serviceFactory.CreateServiceModel("S1");
        var s2 = serviceFactory.CreateServiceModel("S2");
        var s3 = serviceFactory.CreateServiceModel("S3");
        var s4 = serviceFactory.CreateServiceModel("S4");

        s1.DependsOn.Add(new DependencyRelationServiceModel(s1, s2, 10));
        s2.DependsOn.Add(new DependencyRelationServiceModel(s2, s3, 10));
        s3.DependsOn.Add(new DependencyRelationServiceModel(s3, s1, 1));
        s4.DependsOn.Add(new DependencyRelationServiceModel(s3, s1, 1));


        _dataProvider.Setup(s => s.GetServices()).Returns(serviceFactory.ServiceModels);

        var checker = new CircularDependencyChecker.CircularDependencyChecker(_dataProvider.Object);
        //Assert
        var result = checker.CreatePackages();

        //Act
        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    [MaxTime(2000)]
    public void CircularDependencyCheckerTests_DependencyToANonRegisteredService_ReturnOnePackage()
    {
        //Arrange
        var serviceFactory = new TestServiceModelFactory();
        var s1 = serviceFactory.CreateServiceModel("S1");
        var s2 = new ServiceModel("Unknown");

        s1.DependsOn.Add(new DependencyRelationServiceModel(s1, s2, 10));

        _dataProvider.Setup(s => s.GetServices()).Returns(serviceFactory.ServiceModels);

        var checker = new CircularDependencyChecker.CircularDependencyChecker(_dataProvider.Object);
        //Assert
        var result = checker.CreatePackages();

        //Act
        Assert.That(result.Count, Is.EqualTo(1));
    }

    [Test]
    [MaxTime(2000)]
    public void CircularDependencyCheckerTests_2CircularDependenciesInATree_Returns1Package()
    {
        //Arrange
        var serviceFactory = new TestServiceModelFactory();
        var s1 = serviceFactory.CreateServiceModel("S1");
        var s2 = serviceFactory.CreateServiceModel("S2");
        var s3 = serviceFactory.CreateServiceModel("S3");
        var s4 = serviceFactory.CreateServiceModel("S4");
        var s5 = serviceFactory.CreateServiceModel("S5");

        s1.DependsOn.Add(new DependencyRelationServiceModel(s1, s2, 222));
        s2.DependsOn.Add(new DependencyRelationServiceModel(s2, s3, 20));
        s2.DependsOn.Add(new DependencyRelationServiceModel(s2, s4, 20));
        s3.DependsOn.Add(new DependencyRelationServiceModel(s3, s1, 20));
        s4.DependsOn.Add(new DependencyRelationServiceModel(s4, s5, 20));
        s5.DependsOn.Add(new DependencyRelationServiceModel(s5, s1, 20));

        _dataProvider.Setup(s => s.GetServices()).Returns(serviceFactory.ServiceModels);


        var checker = new CircularDependencyChecker.CircularDependencyChecker(_dataProvider.Object);
        //act
        var result = checker.CreatePackages();

        //Asster
        Assert.That(result.Count, Is.EqualTo(1));
    }

    [Test]
    [MaxTime(2000)]
    public void CircularDependencyCheckerTests_2CircularDependenciesInATree()
    {
        //Arrange
        var serviceFactory = new TestServiceModelFactory();
        var s1 = serviceFactory.CreateServiceModel("S1");
        var s2 = serviceFactory.CreateServiceModel("S2");
        var s3 = serviceFactory.CreateServiceModel("S3");
        var s4 = serviceFactory.CreateServiceModel("S4");
        var s5 = serviceFactory.CreateServiceModel("S5");
        var s6 = serviceFactory.CreateServiceModel("S6");

        s1.DependsOn.Add(new DependencyRelationServiceModel(s1, s2, 10));
        s2.DependsOn.Add(new DependencyRelationServiceModel(s2, s3, 110));
        s2.DependsOn.Add(new DependencyRelationServiceModel(s2, s4, 110));
        s3.DependsOn.Add(new DependencyRelationServiceModel(s3, s4, 110));
        s4.DependsOn.Add(new DependencyRelationServiceModel(s4, s5, 110));
        s5.DependsOn.Add(new DependencyRelationServiceModel(s5, s1, 110));
        s5.DependsOn.Add(new DependencyRelationServiceModel(s5, s6, 110));


        _dataProvider.Setup(s => s.GetServices()).Returns(serviceFactory.ServiceModels);


        var checker = new CircularDependencyChecker.CircularDependencyChecker(_dataProvider.Object);
        //Assert
        var result = checker.CreatePackages();

        //Act
        Assert.That(result.Count, Is.EqualTo(2));
    }
}