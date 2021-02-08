using System;
using System.Threading;
using System.Threading.Tasks;
using DP.MessageQueue.Extensions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace DP.MessageQueue.Impl
{
    public sealed class AzureServiceBusQueue : IBaseQueueSend , IBaseQueueReceive
    {
        private bool _isDisposed = false;

        private readonly string _connectionString;

        private QueueClient _queueClient;

        private MessageReceiver _messageReceiver;

        private static readonly Random _random = new Random();

        public AzureServiceBusQueue(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            _connectionString = connectionString;
        }

        public void InitializeOutbound(QueueDescription queueDescription)
        {
            var queueDesc = queueDescription ?? throw new ArgumentNullException(nameof(queueDescription));

            _queueClient = new QueueClient( _connectionString, queueDesc.QueueName );
        }

        public void InitializeInbound(QueueDescription queueDescription)
        {
            var queueDesc = queueDescription ?? throw new ArgumentNullException(nameof(queueDescription));
            
                        _messageReceiver = new MessageReceiver(_connectionString, queueDesc.QueueName, ReceiveMode.PeekLock);

                    }

        public async Task SendAsync(EventMessage eventMessage)
        {
            var message = new Message(eventMessage.ToData())
            {
                ContentType = "application/json",
                Label       = eventMessage.GetType().FullName,
                MessageId   = eventMessage.CorrelationId
            };

            await _queueClient.SendAsync(message).ConfigureAwait(false);
        }

        public async Task ReceiveAsync<TMessage>(Action<TMessage> onMessageReceived, CancellationToken cancellationToken)
            where TMessage : EventMessage
        {

            var doneReceiving = new TaskCompletionSource<bool>();

            var messageHandlerOptions = new MessageHandlerOptions( args =>
                {
                    Console.WriteLine(args.Exception);
                    return Task.CompletedTask;
                }){
                    MaxConcurrentCalls = 1,
                    AutoComplete = false,
                    MaxAutoRenewDuration = TimeSpan.FromSeconds(30)
                };


            cancellationToken.Register(
                async () =>
                {
                    await _messageReceiver.CloseAsync();
                    doneReceiving.SetResult(true);
                });
            
            
            _messageReceiver.RegisterMessageHandler(async (message, token) =>
            {
                var eventMessage = message.Body.FromData<TMessage>();
                onMessageReceived(eventMessage);
                
                /* emulate work time */
                                
                await _messageReceiver.CompleteAsync(message.SystemProperties.LockToken);

            }, messageHandlerOptions);

                        await Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        

        private void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
            }
            
            _isDisposed = true;
        }
    }
}
