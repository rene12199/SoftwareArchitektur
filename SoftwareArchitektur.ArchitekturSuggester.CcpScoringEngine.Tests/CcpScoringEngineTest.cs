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

    [Test,MaxTime(2000)]
    public void CcpScoringEngineTest_NoPackagesSet_ThrowsApplicationException()
    {
        //Arrange
        _dataProvider.Setup(d => d.GetServices()).Returns(new List<ServiceModel>());
        var ccpScoringEngine = new CcpScoringEngine(_dataProvider.Object);
        var remainingServices = new List<ServiceModel>();

        //Act / Assert
        Assert.Throws<ApplicationException>(() => ccpScoringEngine.DistributeRemainingServices(remainingServices.Cast<ServiceModel>().ToList()));
    }


    [Test,MaxTime(2000)]
    public void CcpScoringEngineTest_2PackagesWithDifferentCallNumber_AddedToPackageWithHigherCallNumber()
    {
        var serviceModelFactory = new TestServiceModelFactory();

        //Arrange
        var packages = new List<PackageModel>();
        for (int i = 1; i < 3; i++)
        {
            packages.Add(new PackageModel($"P{i}"));
        }


        packages.First().AddService(serviceModelFactory.CreateServiceModel("S1"));
        packages.Last().AddService(serviceModelFactory.CreateServiceModel("S2"));

        var remainingServices = new List<ServiceModel>();

        remainingServices.Add(serviceModelFactory.CreateServiceModel("RS1", (sm) =>
        {
            sm.ChangedWith.AddRange(new List<CommonChangeRelationModel>()
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
            });
            sm.InPackage = "P3";
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

    [Test,MaxTime(2000)]
    public void CcpScoringEngineTest_2PackagesWithMultipleServicesDifferentCallNumber_AddedToPackageWithHigherCallNumber()
    {
        //Arrange
        var serviceModelFactory = new TestServiceModelFactory();
        var packages = new List<PackageModel>();
        for (int i = 1; i < 3; i++)
        {
            packages.Add(new PackageModel($"P{i}"));
        }


        packages.First().AddService(serviceModelFactory.CreateServiceModel("S1"));
        packages.Last().AddService(serviceModelFactory.CreateServiceModel("S2"));
        packages.Last().AddService(serviceModelFactory.CreateServiceModel("S3"));


        var remainingServices = new List<ServiceModel>();

        remainingServices.Add(serviceModelFactory.CreateServiceModel("RS1", sm =>
            {
                sm.InPackage = "P3";
                sm.ChangedWith.AddRange(new CommonChangeRelationModel[]
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
                    },
                    new CommonChangeRelationModel()
                    {
                        NameOfCurrentService = "RS1",
                        NameOfOtherService = "S3",
                        NumberOfChanges = 2
                    }
                });
                return 0;
            })
        );

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


    [Test,MaxTime(2000)]
    public void CcpScoringEngineTest_ManyServicesOnly1InPackage_AllAddedTo1Package()
    {
        //Arrange
        var serviceModelFactory = new TestServiceModelFactory();
        var packages = new List<PackageModel>();
        for (int i = 1; i < 3; i++)
        {
            packages.Add(new PackageModel($"P{i}"));
        }


        packages.First().AddService(serviceModelFactory.CreateServiceModel("S1"));

        var remainingServices = new List<ServiceModel>();

        remainingServices.Add(serviceModelFactory.CreateServiceModel("RS1", sm =>
            {
                sm.ChangedWith.AddRange(new CommonChangeRelationModel[]
                {
                    new CommonChangeRelationModel()
                    {
                        NameOfCurrentService = "RS1",
                        NameOfOtherService = "S1",
                        NumberOfChanges = 1
                    },
                });
                return 0;
            })
        );

        for (int i = 2; i < 4; i++)
        {
            remainingServices.Add(serviceModelFactory.CreateServiceModel($"RS{i}", sm =>
                {
                    sm.ChangedWith.AddRange(new CommonChangeRelationModel[]
                    {
                        new CommonChangeRelationModel()
                        {
                            NameOfCurrentService = $"RS{i}",
                            NameOfOtherService = $"RS{i - 1}",
                            NumberOfChanges = 1
                        },
                    });
                    return 0;
                })
            );
        }


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


    [Test,MaxTime(2000)]
    public void CcpScoringEngineTest_ServiceWithOutCommonChangesAdded_RemovedWithOutBeingAdded()
    {
        //Arrange
        var serviceModelFactory = new TestServiceModelFactory();
        var packages = new List<PackageModel>();
        for (int i = 1; i < 3; i++)
        {
            packages.Add(new PackageModel($"P{i}"));
        }


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

    [Test,MaxTime(2000)]
    public void CcpScoringEngineTest_ServiceWithOutAnyConnectionAdded_NotAddedToAnything()
    {
        //Arrange
        var serviceModelFactory = new TestServiceModelFactory();
        var packages = new List<PackageModel>();
        for (int i = 1; i < 2; i++)
        {
            packages.Add(new PackageModel($"P{i}"));
        }


        packages.First().AddService(serviceModelFactory.CreateServiceModel("S1"));

        var remainingServices = new List<ServiceModel>();

        remainingServices.Add(serviceModelFactory.CreateServiceModel("IS", sm =>
        {
            sm.ChangedWith.AddRange(new CommonChangeRelationModel[]
            {
                new CommonChangeRelationModel()
                {
                    NameOfCurrentService = sm.Name,
                    NameOfOtherService = $"Unknown",
                    NumberOfChanges = 1
                },
            });
            return 0;
        }));

        for (int i = 2; i < 4; i++)
        {
            remainingServices.Add(serviceModelFactory.CreateServiceModel($"RS{i}", sm =>
                {
                    sm.ChangedWith.AddRange(new CommonChangeRelationModel[]
                    {
                        new CommonChangeRelationModel()
                        {
                            NameOfCurrentService = sm.Name,
                            NameOfOtherService = "S1",
                            NumberOfChanges = 1
                        },
                    });
                    return 0;
                })
            );
        }

        remainingServices.Reverse();
        _dataProvider.Setup(d => d.GetServices()).Returns(serviceModelFactory.ServiceModels);
        var ccpScoringEngine = new CcpScoringEngine(_dataProvider.Object);
        ccpScoringEngine.SetPossiblePackages(packages);
        //Act 
        var result = ccpScoringEngine.DistributeRemainingServices(remainingServices);

        //Assert
        Assert.That(result.First().PackageName, Is.EqualTo("P1"));
        Assert.That(result.First().GetServices().Count, Is.EqualTo(3));
    }
    
    [Test,MaxTime(2000)]
    public void CcpScoringEngineTest_ServiceWith2CommonChangesWithNeededLenientMode_AddedAfterActivatingLenientMode()
    {
        //Arrange
        var serviceModelFactory = new TestServiceModelFactory();
        var packages = new List<PackageModel>();
        for (int i = 1; i < 2; i++)
        {
            packages.Add(new PackageModel($"P{i}"));
        }


        packages.First().AddService(serviceModelFactory.CreateServiceModel("S1"));

        var remainingServices = new List<ServiceModel>();

        remainingServices.Add(serviceModelFactory.CreateServiceModel("RS1", sm =>
        {
            sm.ChangedWith.AddRange(new CommonChangeRelationModel[]
            {
                new CommonChangeRelationModel()
                {
                    NameOfCurrentService = sm.Name,
                    NameOfOtherService = $"Unknown",
                    NumberOfChanges = 2
                }, 
                new CommonChangeRelationModel()
                {
                    NameOfCurrentService = sm.Name,
                    NameOfOtherService = $"S1",
                    NumberOfChanges = 1
                },
            });
            return 0;
        }));

        for (int i = 2; i < 4; i++)
        {
            remainingServices.Add(serviceModelFactory.CreateServiceModel($"RS{i}", sm =>
                {
                    sm.ChangedWith.AddRange(new CommonChangeRelationModel[]
                    {
                        new CommonChangeRelationModel()
                        {
                            NameOfCurrentService = sm.Name,
                            NameOfOtherService = "S1",
                            NumberOfChanges = 1
                        },
                    });
                    return 0;
                })
            );
        }

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