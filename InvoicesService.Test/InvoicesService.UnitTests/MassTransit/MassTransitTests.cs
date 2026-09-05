using FluentAssertions;
using InvoicesService.Shared.Contracts.Events;
using InvoicesServiceTest.Consumers;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace InvoicesServiceTest.InvoicesService.UnitTests.MassTransit;

public class MassTransitTests
{

    [Fact]
    public async Task PublishEndpoint_ShouldPublishInvoiceCreatedEvent_WhenPublishEndpointIsInvoked()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness()
            .BuildServiceProvider();
        
        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();
        
        var publisher = provider.GetRequiredService<IPublishEndpoint>();
        var createdEvent = new InvoiceCreatedEvent(Guid.NewGuid(), 123, 400, DateTime.UtcNow);
        
        await publisher.Publish(createdEvent);
        
        Assert.True(harness.Published.Select<InvoiceCreatedEvent>().Any());
        
        var published = harness.Published.Select<InvoiceCreatedEvent>().FirstOrDefault();
        
        published.Should().NotBeNull();
        published.Context.Message.Should().BeEquivalentTo(createdEvent);
    }

    [Fact]
    public async Task CreatedInvoiceConsumer_ShouldConsumeCreatedInvoiceEvent_WhenInvoiceCreated()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<CreatedInvoiceConsumerTest>();
            })
            .BuildServiceProvider();
        
        var harness = provider.GetRequiredService<ITestHarness>();
        
        await harness.Start();
        
        var publishEndpoint = provider.GetRequiredService<IPublishEndpoint>();
        
        var createdEvent = new InvoiceCreatedEvent(Guid.NewGuid(), 123, 400, DateTime.UtcNow);
        await publishEndpoint.Publish(createdEvent);

        var consumed = await harness.Consumed.SelectAsync<InvoiceCreatedEvent>().FirstOrDefault();
        
        consumed.Should().NotBeNull();
        consumed.Context.Message.Should().BeEquivalentTo(createdEvent);
        CreatedInvoiceConsumerTest.InvoiceId.Should().NotBeEmpty();
        CreatedInvoiceConsumerTest.InvoiceId.Should().Be(createdEvent.InvoiceId);
    }

    [Fact]
    public async Task ConsumerShouldFail_WhenConsumeInvoiceCreatedEvent()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<FailingCreatedInvoiceConsumerTest>();
            })
            .BuildServiceProvider();
        
        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();
        
        var invoiceEvent = new InvoiceCreatedEvent(Guid.NewGuid(), 123, 400, DateTime.UtcNow);
        
        await provider.GetRequiredService<IPublishEndpoint>()
            .Publish(invoiceEvent);
        
        Assert.False(await harness.Consumed.Any<FailingCreatedInvoiceConsumerTest>());
    }

    [Fact]
    public async Task ConsumerShouldConsumeOnlyOneEvent_WhenEventAreDuplicated()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x => 
                x.AddConsumer<CreatedInvoiceConsumerTest>())
            .BuildServiceProvider();
        
        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();
        
        var createdEvent = new InvoiceCreatedEvent(Guid.NewGuid(), 123, 400, DateTime.UtcNow);
        
        await provider.GetRequiredService<IPublishEndpoint>()
            .Publish(createdEvent);
        
        await provider.GetRequiredService<IPublishEndpoint>()
            .Publish(createdEvent);
        
        await provider.GetRequiredService<IPublishEndpoint>()
            .Publish(createdEvent);
        
        var consumed = await harness.Consumed.SelectAsync<InvoiceCreatedEvent>().CountAsync();
        
        consumed.Should().Be(3);
        CreatedInvoiceConsumerTest.CustomerIds.Count.Should().Be(1);
    }
}