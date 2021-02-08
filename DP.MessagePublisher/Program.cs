using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DP.Common;
using DP.MessagePublisher.Configurations;
using DP.MessageQueue;

namespace DP.MessagePublisher
{
    public class Program
    {
        private static readonly Random                      _rand = new Random();
        private static ManualResetEvent                     _manualResetEvent = new ManualResetEvent(false);

        private static ConcurrentDictionary<string, string> _jobStatues = new ConcurrentDictionary<string, string>();


        private static IBaseQueueSend                       _jobQueue;
        private static IBaseQueueReceive                    _responseQueue;

        private static int _batches;
        private static int _batchSize;

        public static void Main(string[] args)
        {
            Console.WriteLine($"{Environment.MachineName} Publisher {Guid.NewGuid()}");

            int.TryParse( Config.Batches, out _batches);
            int.TryParse( Config.BatchSize, out _batchSize);

            Console.WriteLine($"batches   : {_batches}");
            Console.WriteLine($"batch size: {_batchSize}");


            try
            {
                Run().GetAwaiter().GetResult();
                _manualResetEvent.WaitOne();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                _jobQueue?.Dispose();
                _responseQueue?.Dispose();
                Console.WriteLine("cleanup");
            }


            
                        
        }

        public static async Task Run()
        {
            var cts               = new CancellationTokenSource();
            var mqFactory         = MessageQueueFactory.GetMessageQueueFactory();
            _responseQueue        = mqFactory.BuildInboundQueue("subResponse");
            var checkResponseTask = _responseQueue.ReceiveAsync<JobResponseEvent>(HandleResponseEvent, cts.Token);


            _jobQueue             = mqFactory.BuildOutboundQueue("pubJob");
            var sendJobTask       = SendJobAsync();

            await Task.WhenAll(
                checkResponseTask,
                sendJobTask);
        }


        private static async Task SendJobAsync()
        {
                        var sleepDuration = TimeSpan.FromSeconds(_rand.Next(10, 20));

            for (var b=0; b < _batches; b++)
            {
                foreach (var _ in Enumerable.Range(0, _batchSize))
                {
                    var message = new JobAssignmentEvent
                    {
                        JobDetails = new JobDetails[_rand.Next(1, 5)]
                    };

                    _jobStatues.TryAdd(message.CorrelationId, "");

                    Console.WriteLine($"{DateTime.UtcNow.ToLongTimeString()}  batch #{b+1} sending job {message.CorrelationId}");

                    await _jobQueue.SendAsync(message);
                }

                await Task.Delay(sleepDuration);
            }

        }

        private static void HandleResponseEvent(EventMessage message)
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
                    Console.WriteLine($"{DateTime.UtcNow.ToLongTimeString()} Job {eventMessage.CorrelationId} was completed by worker {eventMessage.WorkerId}");
                    Console.ResetColor();
                }

                _jobStatues.TryRemove(eventMessage.CorrelationId, out var _);
            }

            if (_jobStatues.IsEmpty)
            {
                Console.WriteLine("job queue is empty");
                _manualResetEvent.Set();
            }

        }


    }
}
