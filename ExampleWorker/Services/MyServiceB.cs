using System;
using System.Threading;
using System.Threading.Tasks;
using ExampleWorker.Services.Config;
using MessageProvider;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExampleWorker.Services
{
    // Another option is to implement IHostedService yourself....
    public class MyServiceB : IHostedService, IDisposable
    {

        // State...
        private bool _stopping;
        private Task _backgroundTask;

        // Configuration...
        private readonly ServiceBCfg _config;

        // Dependencies...
        private readonly ILogger _logger;
        private readonly IMessageProvider _messageProvider;


        public MyServiceB(
            IOptions<ServiceBCfg> config, 
            ILogger<MyServiceB> logger, 
            IMessageProvider messageProvider)   // <-- Inject config
        {
            _config = config.Value;
            _logger = logger;
            _messageProvider = messageProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("MyServiceB is starting.");
            _backgroundTask = BackgroundTask();
            return Task.CompletedTask;
        }

        private async Task BackgroundTask()
        {
            while (!_stopping)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                _logger.LogInformation($"MyServiceB says: {_messageProvider.GetMessage()}");
            }

            _logger.LogDebug("MyServiceB background task is stopping.");
        }
 
        // Rather than just having the process killed, this gives us a chance to shutdown gracefully
        // Or at least even just log "I'm shutting down now".
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("MyServiceB is stopping.");
            _stopping = true;

            if (_backgroundTask != null)
            {
                // TODO: cancellation
                await _backgroundTask;
            }
        }

        public void Dispose()
        {
            _logger.LogDebug("MyServiceB is disposing.");
        }
    }
}
