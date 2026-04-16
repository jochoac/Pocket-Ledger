using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PocketLedger.Api.SourceEvents.GetSourceEventById;
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
}