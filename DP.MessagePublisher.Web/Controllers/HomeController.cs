using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DP.Common;
using Microsoft.AspNetCore.Mvc;
using DP.MessagePublisher.Web.Models;
using DP.MessagePublisher.Web.Services;
using DP.MessageQueue;
using Microsoft.AspNetCore.Http;

namespace DP.MessagePublisher.Web.Controllers
{
    public class HomeController : Controller
    {
        private static IBaseQueueSend _jobQueue;

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        
        public IActionResult SendJob(int id)
        {
            var mqFactory = MessageQueueFactory.GetMessageQueueFactory();
            _jobQueue     = mqFactory.BuildOutboundQueue("pubJob");
            var sendJobTask = SendJobAsync(id);

            return Content($"sending {id}");
        }
        private static readonly Random _rand = new Random();

        private static async Task SendJobAsync(int jobcount)
        {
                        var sleepDuration = TimeSpan.FromSeconds(_rand.Next(10, 20));

            foreach (var _ in Enumerable.Range(0, jobcount))
            {
                var message = new JobAssignmentEvent
                {
                    JobDetails = new JobDetails[_rand.Next(1, 5)]
                };

                GracePeriodManagerService._jobStatues.TryAdd(message.CorrelationId, "");

                Console.WriteLine(
                    $"{DateTime.UtcNow.ToLongTimeString()} sending job {message.CorrelationId}");

                await _jobQueue.SendAsync(message);
            }

            await Task.Delay(sleepDuration);


        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
