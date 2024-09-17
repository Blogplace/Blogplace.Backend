using Blogplace.Tests.Integration;
using FluentAssertions;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Blogplace.Tests.Unit;
public class LoggerTests : TestBase
{
    [Test]
    public async Task Serilog_Logging()
    {
        //Arrange
        var logEventSink = new TestSink();
        var logger = new LoggerConfiguration()
            .WriteTo.Sink(logEventSink)
            .CreateLogger();

        //Act
        var sampleMessage = "Sample message.";
        logger.Information(sampleMessage);

        //Assert
        logEventSink.LogEvents.Should().ContainSingle()
            .Which.MessageTemplate.Text.Should().Be(sampleMessage);

        //Clean up
        Log.CloseAndFlush();
    }

    private class TestSink : ILogEventSink
    {
        public List<LogEvent> LogEvents { get; } = new List<LogEvent>();

        public void Emit(LogEvent logEvent)
        {
            this.LogEvents.Add(logEvent);
        }
    }
}
