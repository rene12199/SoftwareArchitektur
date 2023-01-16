using Moq;
using SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine.Converter;
using SoftwareArchitektur.Utility.Interface;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine.Tests.ConverterTests;

public class CommonChangeToCcpCommonChangeConverterTests
{
    private Mock<IDataProvider> _dataProvider;

    [SetUp]
    public void Setup()
    {
        _dataProvider = new Mock<IDataProvider>();
    }

    [Test]
    [MaxTime(2000)]
    public void CommonChangeToCcpCommonChangeConverterTests_2PackagesWith3Services_Returns1CcpScoreWithValue2()
    {
        var commonChange = new List<CommonChangeRelationModel>
        {
            new()
            {
                NameOfCurrentService = "S1",
                NameOfOtherService = "S2",
                NumberOfChanges = 1
            },
            new()
            {
                NameOfCurrentService = "S1",
                NameOfOtherService = "S3",
                NumberOfChanges = 1
            }
        };

        var serviceModels = new List<ServiceModel>();
        var s1 = new ServiceModel("S1")
        {
            InPackage = "P1",
            ChangedWith =
            {
                new CommonChangeRelationModel
                {
                    NameOfCurrentService = "S1",
                    NameOfOtherService = "S2",
                    NumberOfChanges = 1
                },
                new CommonChangeRelationModel
                {
                    NameOfCurrentService = "S1",
                    NameOfOtherService = "S3",
                    NumberOfChanges = 1
                }
            }
        };

        serviceModels.Add(s1);

        var packages = new List<PackageModel>();

        var package1 = new PackageModel("P1");
        package1.AddService(s1);
        packages.Add(package1);

        var package2 = new PackageModel("P2");

        for (int i = 2; i < 4; i++)
        {
            var newService = new ServiceModel($"S{i}")
            {
                InPackage = "P2"
            };
            serviceModels.Add(newService);
            package2.AddService(newService);
        }

        //Arrange
        _dataProvider.Setup(s => s.GetServices()).Returns(serviceModels);

        var converter = new CommonChangeToCcpCommonChangeConverter(_dataProvider.Object);
        //Act
        var result = converter.CreateCcpCommonChangeList(commonChange);

        //Assert

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.First().NumberOfChanges, Is.EqualTo(2));
    }

    [Test]
    [MaxTime(2000)]
    public void CommonChangeToCcpCommonChangeConverterTests_3PackagesWith3Services_Returns1CcpScoreWithValue3()
    {
        //Arrange
        var commonChange = new List<CommonChangeRelationModel>
        {
            new()
            {
                NameOfCurrentService = "S1",
                NameOfOtherService = "S2",
                NumberOfChanges = 1
            },
            new()
            {
                NameOfCurrentService = "S1",
                NameOfOtherService = "S3",
                NumberOfChanges = 1
            }
        };

        var serviceModels = new List<ServiceModel>();
        var s1 = new ServiceModel("S1")
        {
            InPackage = "P1",
            ChangedWith =
            {
                new CommonChangeRelationModel
                {
                    NameOfCurrentService = "S1",
                    NameOfOtherService = "S2",
                    NumberOfChanges = 1
                },
                new CommonChangeRelationModel
                {
                    NameOfCurrentService = "S1",
                    NameOfOtherService = "S3",
                    NumberOfChanges = 1
                }
            }
        };

        serviceModels.Add(s1);

        var packages = new List<PackageModel>();

        var package1 = new PackageModel("P1");
        package1.AddService(s1);
        packages.Add(package1);


        for (int i = 2; i < 4; i++)
        {
            var package = new PackageModel($"P{i}");
            var newService = new ServiceModel($"S{i}")
            {
                InPackage = $"P{i}"
            };
            serviceModels.Add(newService);
            package.AddService(newService);
            packages.Add(package);
        }


        _dataProvider.Setup(s => s.GetServices()).Returns(serviceModels);

        var converter = new CommonChangeToCcpCommonChangeConverter(_dataProvider.Object);
        //Act
        var result = converter.CreateCcpCommonChangeList(commonChange);

        //Assert

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.First().NumberOfChanges, Is.EqualTo(1));
    }
}