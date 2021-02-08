using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DP.Common;
using DP.MessageQueue;

namespace DP.MessageHandler
{
    public class Program
    {
        private static readonly Random   _rand = new Random();
        private static ManualResetEvent  _manualResetEvent = new ManualResetEvent(false);
        private static readonly Guid     _workerId = Guid.NewGuid();

        private static IBaseQueueReceive _jobQueue;
        private static IBaseQueueSend    _responseQueue;


        public static void Main(string[] args)
        {
            Console.WriteLine($"{Environment.MachineName} Worker {_workerId }");
            

            try
            {
                Run().GetAwaiter().GetResult();

                _manualResetEvent.WaitOne();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                _jobQueue?.Dispose();
                _responseQueue?.Dispose();
            }

        }

        public static async Task Run()
        {
            var cts        = new CancellationTokenSource();
            var mqFactory = MessageQueueFactory.GetMessageQueueFactory();
            _responseQueue = mqFactory.BuildOutboundQueue("pubResponse");

            _jobQueue      = mqFactory.BuildInboundQueue("subJob");
            await _jobQueue.ReceiveAsync<JobAssignmentEvent>(HandleJobEvent, cts.Token);
        }


        private static void HandleJobEvent(EventMessage message)
        {
            if (!(message is JobAssignmentEvent eventMessage))
            {
                return;
            }

            var jobDetails = eventMessage.JobDetails;
            Console.WriteLine(
                $"{DateTime.UtcNow.ToLongTimeString()} job received: {eventMessage.CorrelationId} and data count is {jobDetails.Count()}");


                                                
            Stopwatch stopWatch = Stopwatch.StartNew();
            DoWork();
            stopWatch.Stop();
            Console.WriteLine($"work hard for {stopWatch.ElapsedMilliseconds} milliseconds");

            Console.WriteLine($"{DateTime.UtcNow.ToLongTimeString()} completed {eventMessage.CorrelationId}.");

            var responseMessage = new JobResponseEvent
            {
                CorrelationId = eventMessage.CorrelationId,
                JobStatus = new JobStatus { Status = "Done" },
                WorkerId = _workerId
            };
            _responseQueue.SendAsync(responseMessage);
        }

        private static void DoWork()
        {
            var percentage = _rand.Next(5500, 6500);

            Stopwatch watch = new Stopwatch();
            watch.Start();
            while (true)
            {
                                                if (watch.ElapsedMilliseconds > percentage)
                {
                    Thread.Sleep(7000 - percentage);
                                                                                return;
                }
            }

        }


    }
}
