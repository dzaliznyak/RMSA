using System;
using System.Runtime.Serialization;

namespace Rmsa.Core.Utils
{
    public class ChannelException : Exception
    {
        public ChannelException()
        {
        }

        public ChannelException(string message) : base(message)
        {
        }

        public ChannelException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ChannelException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
