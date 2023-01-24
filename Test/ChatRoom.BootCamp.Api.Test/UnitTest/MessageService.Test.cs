using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using ChatRoom.Core.Application.Services.Implementations;
using ChatRoom.Core.Domain.Models;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ChatRoom.BootCamp.Api.Test.UnitTest;

public class MessageServiceTest
{
    private readonly IFixture _fixture;

    public MessageServiceTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    #region AutoMoqDataAttribute
    public class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute() : base(() => 
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var configurationSection = fixture.Freeze<Mock<IConfigurationSection>>();
            configurationSection.Setup(x => x.Value).Returns("myDb");

            var configuration = fixture.Freeze<Mock<IConfiguration>>();
            configuration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(() => configurationSection.Object);

            fixture.Inject(configuration.Object);
            return fixture;
        })
        {
            
        }
    }
    #endregion

    #region SetupParameter
    public class SetupParameter : TheoryData<HttpStatusCode, bool>
    {
        public SetupParameter()
        {
            Add(HttpStatusCode.Created, true);
            Add(HttpStatusCode.BadRequest, false);
        }
    }
    #endregion

    [Theory]
    //[MemberData(nameof(SetupClassData.Parameters), MemberType = typeof(SetupClassData))]
    //[ClassData(typeof(SetupClassData))]
    [ClassData(typeof(SetupParameter))]
    public async Task Try_UpsertMessage(HttpStatusCode httpStatusCode, bool expectedReslt)
    {
        #region Arrange
        var configurationSection = _fixture.Freeze<Mock<IConfigurationSection>>();
        configurationSection.Setup(x => x.Value).Returns("myDb");
        var configuration = _fixture.Freeze<Mock<IConfiguration>>();

        configuration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(() => configurationSection.Object);

        var container = _fixture.Freeze<Mock<Container>>();

        var itemResponse = _fixture.Freeze<Mock<ItemResponse<Message>>>();
        itemResponse.Setup(x => x.StatusCode).Returns(() => httpStatusCode);

        container.Setup(x => x.UpsertItemAsync(
            It.IsAny<Message>(), It.IsAny<PartitionKey>(),
            It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => itemResponse.Object);

        var cosmosClient = _fixture.Freeze<Mock<CosmosClient>>();
        cosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(() => container.Object);

        var messageService = new MessageService(cosmosClient.Object, configuration.Object);

        #endregion

        #region Act

        var result = await messageService.CreateMessage(new Message());

        #endregion

        #region Assert

        result.Should().Be(expectedReslt);

        #endregion
    }

    [Theory, AutoMoqData]
    public async Task Try_UpsertMessage_WithParameter(
        [Frozen] Mock<IConfiguration> configuration, [Frozen] Mock<Container> container,
        [Frozen] Mock<ItemResponse<Message>> itemResponse, [Frozen] Mock<CosmosClient> cosmosClient)
    {
        #region Arrange
        itemResponse.Setup(x => x.StatusCode).Returns(() => HttpStatusCode.Created);

        container.Setup(x => x.UpsertItemAsync(
            It.IsAny<Message>(), It.IsAny<PartitionKey>(),
            It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => itemResponse.Object);

        cosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(() => container.Object);

        var messageService = new MessageService(cosmosClient.Object, configuration.Object);

        #endregion

        #region Act

        var result = await messageService.CreateMessage(new Message());

        #endregion

        #region Assert

        result.Should().BeTrue();

        #endregion
    }

}