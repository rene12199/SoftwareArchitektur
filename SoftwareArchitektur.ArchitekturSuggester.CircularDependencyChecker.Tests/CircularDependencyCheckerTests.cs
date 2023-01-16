using Moq;
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

    [Test,MaxTime(2000)]
    public void CircularDependencyCheckerTests_Only2Dependencies_ReturnsPackageWith2Services()
    {
        //Arrange
        var serviceModels = new List<ServiceModel>();
        serviceModels.Add(new ServiceModel("S1")
        {
            DependsOn =
            {
                new DependencyRelationModel
                {
                    Caller = "S1",
                    Callee = "S2"
                }
            }
        });

        _dataProvider.Setup(s => s.GetServices()).Returns(serviceModels);

        serviceModels.Add(new ServiceModel("S2"));
        var checker = new CircularDependencyChecker.CircularDependencyChecker(_dataProvider.Object);
        //Assert
        var result = checker.CreatePackages();

        //Act
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.NotNull(result.First().GetServices().FirstOrDefault(s => s.Name == "S1"));
    }

    [Test,MaxTime(2000)]
    public void CircularDependencyCheckerTests_Only3DependenciesWithCircularDependency_Returns4PackageWith1Services()
    {
        //Arrange
        var serviceModels = new List<ServiceModel>();
        serviceModels.Add(new ServiceModel("S1")
        {
            DependsOn =
            {
                new DependencyRelationModel
                {
                    Caller = "S1",
                    Callee = "S2",
                    NumberOfCalls = 10
                }
            }
        });
        serviceModels.Add(new ServiceModel("S2")
        {
            DependsOn =
            {
                new DependencyRelationModel
                {
                    Caller = "S2",
                    Callee = "S3"
                }
            }
        });
        serviceModels.Add(new ServiceModel("S3")
        {
            DependsOn =
            {
                new DependencyRelationModel
                {
                    Caller = "S3",
                    Callee = "S1"
                }
            }
        });

        _dataProvider.Setup(s => s.GetServices()).Returns(serviceModels);


        var checker = new CircularDependencyChecker.CircularDependencyChecker(_dataProvider.Object);
        //Assert
        var result = checker.CreatePackages();

        //Act
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.NotNull(result.First().GetServices().FirstOrDefault(s => s.Name == "S1"));
        Assert.NotNull(result.First().GetServices().FirstOrDefault(s => s.Name == "S2"));
        Assert.NotNull(result.First().GetServices().FirstOrDefault(s => s.Name == "S3"));
    }

    [Test,MaxTime(2000)]
    public void CircularDependencyCheckerTests_Only4DependenciesWithNoCircularDependency_Returns2PackageWith2Services()
    {
        //Arrange
        var serviceModels = new List<ServiceModel>();
        serviceModels.Add(new ServiceModel("S1")
        {
            DependsOn =
            {
                new DependencyRelationModel
                {
                    Caller = "S1",
                    Callee = "S2",
                    NumberOfCalls = 10
                }
            }
        });
        serviceModels.Add(new ServiceModel("S2"));

        serviceModels.Add(new ServiceModel("S3")
        {
            DependsOn =
            {
                new DependencyRelationModel
                {
                    Caller = "S3",
                    Callee = "S4"
                }
            }
        });

        serviceModels.Add(new ServiceModel("S4"));
        _dataProvider.Setup(s => s.GetServices()).Returns(serviceModels);


        var checker = new CircularDependencyChecker.CircularDependencyChecker(_dataProvider.Object);
        //Assert
        var result = checker.CreatePackages();

        //Act
        Assert.That(result.Count, Is.EqualTo(4));
    }

    [Test,MaxTime(2000)]
    public void CircularDependencyCheckerTests_Only4DependenciesWithCircularDependency_Returns2Packages()
    {
        //Arrange
        var serviceModels = new List<ServiceModel>();
        serviceModels.Add(new ServiceModel("S1")
        {
            DependsOn =
            {
                new DependencyRelationModel
                {
                    Caller = "S1",
                    Callee = "S2",
                    NumberOfCalls = 10
                }
            }
        });

        serviceModels.Add(new ServiceModel("S2")
        {
            DependsOn =
            {
                new DependencyRelationModel
                {
                    Caller = "S2",
                    Callee = "S3",
                    NumberOfCalls = 10
                }
            }
        });


        serviceModels.Add(new ServiceModel("S3")
        {
            DependsOn =
            {
                new DependencyRelationModel
                {
                    Caller = "S3",
                    Callee = "S1"
                }
            }
        });

        serviceModels.Add(new ServiceModel("S4"));
        _dataProvider.Setup(s => s.GetServices()).Returns(serviceModels);

        var checker = new CircularDependencyChecker.CircularDependencyChecker(_dataProvider.Object);
        //Assert
        var result = checker.CreatePackages();

        //Act
        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test,MaxTime(2000)]
    public void CircularDependencyCheckerTests_DependencyToANonRegisteredService_ReturnOnePackage()
    {
        //Arrange
        var serviceModels = new List<ServiceModel>();
        serviceModels.Add(new ServiceModel("S1")
        {
            DependsOn =
            {
                new DependencyRelationModel
                {
                    Caller = "S1",
                    Callee = "S2",
                    NumberOfCalls = 10
                }
            }
        });


        _dataProvider.Setup(s => s.GetServices()).Returns(serviceModels);

        var checker = new CircularDependencyChecker.CircularDependencyChecker(_dataProvider.Object);
        //Assert
        var result = checker.CreatePackages();

        //Act
        Assert.That(result.Count, Is.EqualTo(1));
    }

    [Test,MaxTime(2000)]
    public void CircularDependencyCheckerTests_2CircularDependenciesInATree_Returns1Package()
    {
        //Arrange
        var serviceModels = new List<ServiceModel>();
        serviceModels.Add(new ServiceModel("S1")
        {
            DependsOn =
            {
                new DependencyRelationModel
                {
                    Caller = "S1",
                    Callee = "S2",
                    NumberOfCalls = 20
                }
            }
        });

        serviceModels.Add(new ServiceModel("S2")
        {
            DependsOn =
            {
                new DependencyRelationModel
                {
                    Caller = "S2",
                    Callee = "S3",
                    NumberOfCalls = 10
                },
                new DependencyRelationModel
                {
                    Caller = "S2",
                    Callee = "S4",
                    NumberOfCalls = 10
                }
            }
        });

        serviceModels.Add(new ServiceModel("S3")
        {
            DependsOn =
            {
                new DependencyRelationModel
                {
                    Caller = "S3",
                    Callee = "S1",
                    NumberOfCalls = 20
                }
            }
        });

        serviceModels.Add(new ServiceModel("S4")
        {
            DependsOn =
            {
                new DependencyRelationModel
                {
                    Caller = "S4",
                    Callee = "S5",
                    NumberOfCalls = 10
                }
            }
        });
        serviceModels.Add(new ServiceModel("S5")
        {
            DependsOn =
            {
                new DependencyRelationModel
                {
                    Caller = "S5",
                    Callee = "S1",
                    NumberOfCalls = 10
                }
            }
        });
        _dataProvider.Setup(s => s.GetServices()).Returns(serviceModels);


        var checker = new CircularDependencyChecker.CircularDependencyChecker(_dataProvider.Object);
        //Assert
        var result = checker.CreatePackages();

        //Act
        Assert.That(result.Count, Is.EqualTo(1));
    }

    [Test,MaxTime(2000)]
    public void CircularDependencyCheckerTests_2CircularDependenciesInATree()
    {
        //Arrange
        var serviceModels = new List<ServiceModel>();
        serviceModels.Add(new ServiceModel("S1")
        {
            DependsOn =
            {
                new DependencyRelationModel
                {
                    Caller = "S1",
                    Callee = "S2",
                    NumberOfCalls = 20
                }
            }
        });

        serviceModels.Add(new ServiceModel("S2")
        {
            DependsOn =
            {
                new DependencyRelationModel
                {
                    Caller = "S2",
                    Callee = "S3",
                    NumberOfCalls = 10
                },
                new DependencyRelationModel
                {
                    Caller = "S2",
                    Callee = "S4",
                    NumberOfCalls = 10
                }
            }
        });

        serviceModels.Add(new ServiceModel("S3")
        {
            DependsOn =
            {
                new DependencyRelationModel
                {
                    Caller = "S3",
                    Callee = "S1",
                    NumberOfCalls = 20
                }
            }
        });

        serviceModels.Add(new ServiceModel("S4")
        {
            DependsOn =
            {
                new DependencyRelationModel
                {
                    Caller = "S4",
                    Callee = "S5",
                    NumberOfCalls = 10
                }
            }
        });
        serviceModels.Add(new ServiceModel("S5")
        {
            DependsOn =
            {
                new DependencyRelationModel
                {
                    Caller = "S5",
                    Callee = "S1",
                    NumberOfCalls = 10
                },

                new DependencyRelationModel
                {
                    Caller = "S5",
                    Callee = "S6",
                    NumberOfCalls = 10
                }
            }
        });

        serviceModels.Add(new ServiceModel("S6"));
        _dataProvider.Setup(s => s.GetServices()).Returns(serviceModels);


        var checker = new CircularDependencyChecker.CircularDependencyChecker(_dataProvider.Object);
        //Assert
        var result = checker.CreatePackages();

        //Act
        Assert.That(result.Count, Is.EqualTo(2));
    }
}