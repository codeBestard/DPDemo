using Newtonsoft.Json;
using System.Text;

namespace DP.MessageQueue.Extensions
{
    public static class MessageExtensions
    {
        public static byte[] ToData<TMessage>(this TMessage message)
            where TMessage : EventMessage
        {
            var json = JsonConvert.SerializeObject(message);
            return Encoding.Unicode.GetBytes(json);
        }

        public static TMessage FromData<TMessage>(this byte[] data)
            where TMessage : EventMessage
        {
            var json = Encoding.Unicode.GetString(data);
            return (TMessage)JsonConvert.DeserializeObject<TMessage>(json);
        }

    }
}
