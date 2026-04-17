using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PocketLedger.Api.SourceEvents.GetSourceEvents;
using PocketLedger.Api.SourceEvents.RegisterSourceEvent;

namespace PocketLedger.Api.IntegrationTests.SourceEvents;

public sealed class GetSourceEventByIdEndpointTests(PocketLedgerApiFactory factory) :
    IClassFixture<PocketLedgerApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Get_GivenExistingSourceEventId_ReturnsOk()
    {
        // Arrange
        var createRequest = new RegisterSourceEventRequest(
            RawPayload: """
                        { "description": "Lunch", "amount": 25000 }
                        """,
            ReceivedAt: new DateTimeOffset(2026, 4, 15, 15, 0, 0, TimeSpan.Zero),
            ExternalId: "manual-001");

        var createResponse = await _client.PostAsJsonAsync("/source-events/manual", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdBody = await createResponse.Content.ReadFromJsonAsync<RegisterSourceEventResponse>();
        createdBody.Should().NotBeNull();

        var sourceEventId = createdBody.SourceEventId;

        // Act
        var response = await _client.GetAsync($"/source-events/{sourceEventId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<GetSourceEventByIdResponse>();
        body.Should().NotBeNull();

        body.SourceEventId.Should().Be(sourceEventId);
        body.SourceType.Should().Be("manual");
        body.RawPayload.Should().Contain("""description": "Lunch""");
        body.ReceivedAt.Should().Be(new DateTimeOffset(2026, 4, 15, 15, 0, 0, TimeSpan.Zero));
        body.ExternalId.Should().Be("manual-001");
    }

    [Fact]
    public async Task Get_GivenNonExistingSourceEventId_ReturnsNotFound()
    {
        // Arrange
        var sourceEventId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/source-events/{sourceEventId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_GivenSourceTypeFilter_ReturnsMatchingSourceEvents()
    {
        // Arrange
        var olderWalletRequest = new RegisterSourceEventRequest(
            RawPayload: """
                        { "description": "Wallet older", "amount": 10000 }
                        """,
            ReceivedAt: new DateTimeOffset(2026, 4, 15, 15, 0, 0, TimeSpan.Zero),
            ExternalId: "wallet-older");

        var smsRequest = new RegisterSourceEventRequest(
            RawPayload: """
                        { "description": "Sms event", "amount": 20000 }
                        """,
            ReceivedAt: new DateTimeOffset(2026, 4, 15, 15, 10, 0, TimeSpan.Zero),
            ExternalId: "sms-001");

        var newerWalletRequest = new RegisterSourceEventRequest(
            RawPayload: """
                        { "description": "Wallet newer", "amount": 30000 }
                        """,
            ReceivedAt: new DateTimeOffset(2026, 4, 15, 15, 20, 0, TimeSpan.Zero),
            ExternalId: "wallet-newer");

        var olderWalletCreateResponse =
            await _client.PostAsJsonAsync("/source-events/wallet", olderWalletRequest);
        olderWalletCreateResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var smsCreateResponse =
            await _client.PostAsJsonAsync("/source-events/sms", smsRequest);
        smsCreateResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var newerWalletCreateResponse =
            await _client.PostAsJsonAsync("/source-events/wallet", newerWalletRequest);
        newerWalletCreateResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var olderWalletBody =
            await olderWalletCreateResponse.Content.ReadFromJsonAsync<RegisterSourceEventResponse>();
        var newerWalletBody =
            await newerWalletCreateResponse.Content.ReadFromJsonAsync<RegisterSourceEventResponse>();

        olderWalletBody.Should().NotBeNull();
        newerWalletBody.Should().NotBeNull();

        // Act
        var response = await _client.GetAsync("/source-events?sourceType=wallet");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<List<GetSourceEventByIdResponse>>();
        body.Should().NotBeNull();
        body.Should().HaveCount(2);

        body.Select(x => x.SourceEventId).Should().ContainInOrder(
            newerWalletBody!.SourceEventId,
            olderWalletBody!.SourceEventId);

        body.Should().OnlyContain(x => x.SourceType == "wallet");
    }
}