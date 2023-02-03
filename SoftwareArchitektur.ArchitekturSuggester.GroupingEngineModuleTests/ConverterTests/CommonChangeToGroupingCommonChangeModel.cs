using Moq;
using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Converter;
using SoftwareArchitektur.ArchitekturSuggester.TestUtility;
using SoftwareArchitektur.Utility.Interface;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester._GroupingEngine.ConverterTests;

public class CommonChangeToGroupingCommonChangeModel
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
        var serviceFactory = new TestServiceModelFactory();
        var s1 = serviceFactory.CreateServiceModel("S1");
        var s2 = serviceFactory.CreateServiceModel("S2");
      
        var s3 = serviceFactory.CreateServiceModel("S3");

        var commonChange = new List<CommonChangeRelationServiceModel>
        {
            new(s1, s2, 1),
            new(s1, s3, 1)
        };

        
        s1.ChangedWith.Add(new(s1, s2, 1));
        s1.ChangedWith.Add(new(s1, s3, 1));


        var packages = new List<PackageModel>();

        var package1 = new PackageModel("P1");
        package1.AddService(s1);
        packages.Add(package1);

        var package2 = new PackageModel("P2");
        package2.AddService(s2);
        package2.AddService(s3);

        //Arrange
        _dataProvider.Setup(s => s.GetServices()).Returns(serviceFactory.ServiceModels);

        var converter = new GroupingEngine.Converter.CommonChangeToGroupingCommonChangeConverter(_dataProvider.Object);
        //Act
        var result = converter.CreateGroupingCommonChangeModelsList(commonChange);

        //Assert

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.First().NumberOfChanges, Is.EqualTo(2));
    }

    [Test]
    [MaxTime(2000)]
    public void CommonChangeToCcpCommonChangeConverterTests_3PackagesWith3Services_Returns1CcpScoreWithValue3()
    {
        //Arrange
        var serviceFactory = new TestServiceModelFactory();
        var s1 = serviceFactory.CreateServiceModel("S1");
        var s2 = serviceFactory.CreateServiceModel("S2");
        var s3 = serviceFactory.CreateServiceModel("S3");
        
        var commonChange = new List<CommonChangeRelationServiceModel>
        {
            new(s1, s2, 1),
            new(s1, s3, 1)
        };

        
        s1.ChangedWith.Add(new(s1, s2, 1));
        s1.ChangedWith.Add(new(s1, s3, 1));

        var packages = new List<PackageModel>();

        var package1 = new PackageModel("P1");
        package1.AddService(s1);
        packages.Add(package1);

        var package2 = new PackageModel("P2");
        package2.AddService(s2);
        
        var package3 = new PackageModel("P3");
        package3.AddService(s3);


        _dataProvider.Setup(s => s.GetServices()).Returns(serviceFactory.ServiceModels);

        var converter = new GroupingEngine.Converter.CommonChangeToGroupingCommonChangeConverter(_dataProvider.Object);
        //Act
        var result = converter.CreateGroupingCommonChangeModelsList(commonChange);

        //Assert

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.First().NumberOfChanges, Is.EqualTo(1));
    }
}