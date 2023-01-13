using SoftwareArchitektur.ArchitekturSuggester.CircularDependencyCheckerModule;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.Tests;

public class CircularDependencyCheckerTests
{
    [Test]
    public void CircularDependencyCheckerTests_Only2Dependencies_ReturnsPackageWith2Services()
    {
        //Arrange
        var serviceModels = new List<ServiceModel>();
        serviceModels.Add(new ServiceModel("S1")
        {
            DependsOn =
            {
                new DependencyRelationModel()
                {
                    Caller = "S1",
                    Callee = "S2"
                }
            }
        });
        serviceModels.Add(new ServiceModel("S2"));
        var checker = new CircularDependencyChecker(serviceModels);
        //Assert
        var result = checker.CreatePackages();

        //Act
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.NotNull(result.First().GetServices().FirstOrDefault(s => s.Name == "S1"));
    }

    [Test]
    public void CircularDependencyCheckerTests_Only3DependenciesWithCiruclarDependency_Returns4PackageWith1Services()
    {
        //Arrange
        var serviceModels = new List<ServiceModel>();
        serviceModels.Add(new ServiceModel("S1")
        {
            DependsOn =
            {
                new DependencyRelationModel()
                {
                    Caller = "S1",
                    Callee = "S2",
                    NumberOfCalls = 10,
                }
            }
        });
        serviceModels.Add(new ServiceModel("S2")
        {
            DependsOn =
            {
                new DependencyRelationModel()
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
                new DependencyRelationModel()
                {
                    Caller = "S3",
                    Callee = "S1"
                }
            }
        });

        var checker = new CircularDependencyChecker(serviceModels);
        //Assert
        var result = checker.CreatePackages();

        //Act
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.NotNull(result.First().GetServices().FirstOrDefault(s => s.Name == "S1"));
        Assert.NotNull(result.First().GetServices().FirstOrDefault(s => s.Name == "S2"));
        Assert.NotNull(result.First().GetServices().FirstOrDefault(s => s.Name == "S3"));
    }

    [Test]
    public void CircularDependencyCheckerTests_Only4DependenciesWithNoCiruclarDependency_Returns2PackageWith2Services()
    {
        //Arrange
        var serviceModels = new List<ServiceModel>();
        serviceModels.Add(new ServiceModel("S1")
        {
            DependsOn =
            {
                new DependencyRelationModel()
                {
                    Caller = "S1",
                    Callee = "S2",
                    NumberOfCalls = 10,
                }
            }
        });
        serviceModels.Add(new ServiceModel("S2"));

        serviceModels.Add(new ServiceModel("S3")
        {
            DependsOn =
            {
                new DependencyRelationModel()
                {
                    Caller = "S3",
                    Callee = "S4"
                }
            }
        });

        serviceModels.Add(new ServiceModel("S4"));

        var checker = new CircularDependencyChecker(serviceModels);
        //Assert
        var result = checker.CreatePackages();

        //Act
        Assert.That(result.Count, Is.EqualTo(4));
    }

    [Test]
    public void CircularDependencyCheckerTests_Only4DependenciesWithCiruclarDependency_Returns2Packages()
    {
        //Arrange
        var serviceModels = new List<ServiceModel>();
        serviceModels.Add(new ServiceModel("S1")
        {
            DependsOn =
            {
                new DependencyRelationModel()
                {
                    Caller = "S1",
                    Callee = "S2",
                    NumberOfCalls = 10,
                }
            }
        });

        serviceModels.Add(new ServiceModel("S2")
        {
            DependsOn =
            {
                new DependencyRelationModel()
                {
                    Caller = "S2",
                    Callee = "S3",
                    NumberOfCalls = 10,
                }
            }
        });


        serviceModels.Add(new ServiceModel("S3")
        {
            DependsOn =
            {
                new DependencyRelationModel()
                {
                    Caller = "S3",
                    Callee = "S1"
                }
            }
        });

        serviceModels.Add(new ServiceModel("S4"));

        var checker = new CircularDependencyChecker(serviceModels);
        //Assert
        var result = checker.CreatePackages();

        //Act
        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public void CircularDependencyCheckerTests_DependencyToANonRegisteredService_ReturnOnePackage()
    {
        //Arrange
        var serviceModels = new List<ServiceModel>();
        serviceModels.Add(new ServiceModel("S1")
        {
            DependsOn =
            {
                new DependencyRelationModel()
                {
                    Caller = "S1",
                    Callee = "S2",
                    NumberOfCalls = 10,
                }
            }
        });

        var checker = new CircularDependencyChecker(serviceModels);
        //Assert
        var result = checker.CreatePackages();

        //Act
        Assert.That(result.Count, Is.EqualTo(1));
    }

    [Test]
    public void CircularDependencyCheckerTests_2CircularDependenciesInATree()
    {
        //Arrange
        var serviceModels = new List<ServiceModel>();
        serviceModels.Add(new ServiceModel("S1")
        {
            DependsOn =
            {
                new DependencyRelationModel()
                {
                    Caller = "S1",
                    Callee = "S2",
                    NumberOfCalls = 20,
                }
            }
        });       
        
        serviceModels.Add(new ServiceModel("S2")
        {
            DependsOn =
            {
                new DependencyRelationModel()
                {
                    Caller = "S2",
                    Callee = "S3",
                    NumberOfCalls = 10,
                },
                new DependencyRelationModel()
                {
                    Caller = "S2",
                    Callee = "S4",
                    NumberOfCalls = 10,
                }
            }
        });
        
        serviceModels.Add(new ServiceModel("S3")
        {
            DependsOn =
            {
                new DependencyRelationModel()
                {
                    Caller = "S3",
                    Callee = "S1",
                    NumberOfCalls = 20,
                }
            }
        });
        
        serviceModels.Add(new ServiceModel("S4")
        {
            DependsOn =
            {
                new DependencyRelationModel()
                {
                    Caller = "S4",
                    Callee = "S5",
                    NumberOfCalls = 10,
                }
            }
        });        
        serviceModels.Add(new ServiceModel("S5")
        {
            DependsOn =
            {
                new DependencyRelationModel()
                {
                    Caller = "S5",
                    Callee = "S1",
                    NumberOfCalls = 10,
                }
            }
        });

        var checker = new CircularDependencyChecker(serviceModels);
        //Assert
        var result = checker.CreatePackages();

        //Act
        Assert.That(result.Count, Is.EqualTo(1));
    }
}