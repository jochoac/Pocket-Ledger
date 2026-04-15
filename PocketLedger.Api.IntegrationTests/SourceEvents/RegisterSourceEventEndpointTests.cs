using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using PocketLedger.Api.SourceEvents.RegisterSourceEvent;

namespace PocketLedger.Api.IntegrationTests.SourceEvents;

public sealed class RegisterSourceEventEndpointTests(PocketLedgerApiFactory factory) :
    IClassFixture<PocketLedgerApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Post_Manual_GivenValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new RegisterSourceEventRequest(
            RawPayload: """
                        { "description": "Lunch", "amount": 25000 }
                        """,
            ReceivedAt: new DateTimeOffset(2026, 4, 15, 15, 0, 0, TimeSpan.Zero),
            ExternalId: "manual-001");

        // Act
        var response = await _client.PostAsJsonAsync("/source-events/manual", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<RegisterSourceEventResponse>();
        body.Should().NotBeNull();
        body!.SourceEventId.Should().NotBeEmpty();

        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString()
            .Should().Be($"/source-events/{body.SourceEventId}");
    }
    
    [Fact]
    public async Task Post_GivenInvalidSourceType_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterSourceEventRequest(
            RawPayload: """
                        { "description": "Lunch", "amount": 25000 }
                        """,
            ReceivedAt: new DateTimeOffset(2026, 4, 15, 15, 0, 0, TimeSpan.Zero),
            ExternalId: "manual-001");

        // Act
        var response = await _client.PostAsJsonAsync("/source-events/not-a-valid-type", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Title.Should().Be("Invalid source type");
        problem.Detail.Should().Be("Unsupported source type 'not-a-valid-type'.");
        problem.Status.Should().Be((int)HttpStatusCode.BadRequest);
    }
}