using NATS.Client;
using System;
using System.Threading;
using System.Threading.Tasks;
using DP.MessageQueue.Extensions;

namespace DP.MessageQueue.Impl
{
    public class NatsQueue : IBaseQueueSend, IBaseQueueReceive
    {
        private readonly string _connectionString;
        private bool _isDisposed = false;

        private static readonly Random _random = new Random();
        private IConnection _connection { get; set; }

        private NatsQueueDescription _inBoundQueueDescription;
        private NatsQueueDescription _outBoundQueueDescription;

        ~NatsQueue()
        {
            Dispose(false);
        }


        public NatsQueue(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            _connectionString = connectionString;

        }
        public void InitializeInbound(QueueDescription queueDescription)
        {
            if (!(queueDescription is NatsQueueDescription inBoundQueueDescription))
            {
                throw new ArgumentException(nameof(queueDescription));
            }

            _inBoundQueueDescription = inBoundQueueDescription;
           _connection = new ConnectionFactory().CreateConnection(_connectionString);
        }
        public void InitializeOutbound(QueueDescription queueDescription)
        {
            if (!(queueDescription is NatsQueueDescription outBoundQueueDescription))
            {
                throw new ArgumentException(nameof(queueDescription));
            }

            _outBoundQueueDescription = outBoundQueueDescription;
            _connection = new ConnectionFactory().CreateConnection(_connectionString);
        }

        public async Task ReceiveAsync<TMessage>(Action<TMessage> onMessageReceived, CancellationToken _)
            where TMessage : EventMessage
        {

            var subscription = _connection.SubscribeAsync(_inBoundQueueDescription.Subject, _inBoundQueueDescription.QueueName);

            subscription.MessageHandler += OnSubscriptionOnMessageHandler;

            subscription.Start();

            void OnSubscriptionOnMessageHandler(object sender, MsgHandlerEventArgs args)
            {
                var eventMessage = args.Message.Data.FromData<TMessage>();

                onMessageReceived(eventMessage);
            }


            await Task.CompletedTask;
        }


        public async Task SendAsync(EventMessage eventMessage)
        {
            var data  = eventMessage.ToData();

            _connection.Publish( _outBoundQueueDescription.Subject, data );

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

            if (_connection != null && !_connection.IsClosed())
            {
                _connection.Dispose();
            }

            _isDisposed = true;
        }


    }
}
