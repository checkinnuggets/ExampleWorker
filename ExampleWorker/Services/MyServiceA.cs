using System;
using System.Threading;
using System.Threading.Tasks;
using ExampleWorker.Services.Config;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExampleWorker.Services
{
    // One option is to override BackgroundService, where you just have to implement one method...
    public class MyServiceA : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ServiceACfg _config;

        // <-- Inject strongly typed config.  There are different IOptions implementations.
        // This one, for example, refreshed the config on each request.
        public MyServiceA(ILogger<MyServiceA> logger, IOptionsSnapshot<ServiceACfg> config)
        {
            _logger = logger;
            _config = config.Value;
        }
                       
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("MyServiceA is starting.");

            cancellationToken.Register(() => _logger.LogDebug("MyServiceA is stopping."));

            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation(_config.Message);

                await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            }

            _logger.LogDebug("MyServiceA background task is stopping.");
        }
    }
}
