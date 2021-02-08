using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DP.MessagePublisher.Web.Hub;
using DP.MessageQueue;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DP.MessagePublisher.Web.Services
{
                
    
        
    public abstract class BackgroundService : IHostedService, IDisposable
    {
        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts =
            new CancellationTokenSource();

        protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
                        _executingTask = ExecuteAsync(_stoppingCts.Token);

                                    if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

                        return Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
                        if (_executingTask == null)
            {
                return;
            }

            try
            {
                                _stoppingCts.Cancel();
            }
            finally
            {
                                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite,
                    cancellationToken));
            }

        }

        public virtual void Dispose()
        {
            _stoppingCts.Cancel();
        }
    }

    public class GracePeriodManagerService : BackgroundService
    {
        private readonly IHubContext<FollowHub> _followHub;

        private readonly ILogger<GracePeriodManagerService> _logger;

        private static IBaseQueueReceive _responseQueue;
        
        
        public static ConcurrentDictionary<string, string> _jobStatues = new ConcurrentDictionary<string, string>();

        public GracePeriodManagerService(
                                    IHubContext<FollowHub> followHub,
            ILogger<GracePeriodManagerService> logger)
        {
            _followHub = followHub;
            _logger = logger;
            
            var cts = new CancellationTokenSource();
            var mqFactory = MessageQueueFactory.GetMessageQueueFactory();
            _responseQueue = mqFactory.BuildInboundQueue("subResponse");
            var checkResponseTask = _responseQueue.ReceiveAsync<JobResponseEvent>(HandleResponseEvent, cts.Token);
        }

        private void HandleResponseEvent(EventMessage message)
        {
            if (!(message is JobResponseEvent eventMessage))
            {
                return;
            }


            if (eventMessage.JobStatus.Status == "Done")
            {
                lock (Console.Out)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    
                    var msg = $"{DateTime.UtcNow.ToLongTimeString()} Job {eventMessage.CorrelationId} was completed by worker {eventMessage.WorkerId}";

                    _followHub.Clients.All.SendAsync("ReceiveMessage", "test", msg);

                    Console.ResetColor();
                }

                _jobStatues.TryRemove(eventMessage.CorrelationId, out var _);
            }

            if (_jobStatues.IsEmpty)
            {
                                _followHub.Clients.All.SendAsync("ReceiveMessage", "test", "job queue is empty");
                            }

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"GracePeriodManagerService is starting.");

            stoppingToken.Register(() =>
                _logger.LogDebug($" GracePeriod background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"GracePeriod task doing background work.");

                                                
                await Task.Delay(5000, stoppingToken);

                            }

            _logger.LogDebug($"GracePeriod background task is stopping.");

        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
                        await Task.CompletedTask;
        }
    }

}
