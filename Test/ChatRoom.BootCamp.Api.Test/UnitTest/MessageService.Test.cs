using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using ChatRoom.Core.Application.Services.Implementations;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using ChatRoom.Core.Domain.Models;

namespace ChatRoom.BootCamp.Api.Test.UnitTest;

public class MessageServiceTest
{
    private readonly IFixture _fixture;

    public MessageServiceTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    [Fact]
    public async Task Try_UpsertMessage()
    {
        #region Arrange

        var configurationSection = _fixture.Freeze<Mock<IConfigurationSection>>();
        configurationSection.Setup(x => x.Value).Returns("myDb");

        var configuration = _fixture.Freeze<Mock<IConfiguration>>();

        configuration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(() => configurationSection.Object);

        var container = _fixture.Freeze<Mock<Container>>();

        var itemResponse = _fixture.Freeze<Mock<ItemResponse<Message>>>();
        itemResponse.Setup(x => x.StatusCode).Returns(() => HttpStatusCode.Created);

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

        result.Should().BeTrue();

        #endregion
    }

    [Fact]
    public async Task Try_UpsertMessage_Without_Success()
    {
        #region Arrange

        var configurationSection = _fixture.Freeze<Mock<IConfigurationSection>>();
        configurationSection.Setup(x => x.Value).Returns("myDb");

        var configuration = _fixture.Freeze<Mock<IConfiguration>>();

        configuration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(() => configurationSection.Object);

        var container = _fixture.Freeze<Mock<Container>>();

        var itemResponse = _fixture.Freeze<Mock<ItemResponse<Message>>>();
        itemResponse.Setup(x => x.StatusCode).Returns(() => HttpStatusCode.BadRequest);

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

        result.Should().BeFalse();

        #endregion
    }

    // [Theory]
    // public async Task Try_UpsertMessage()
    // {
    //     
    // }
}