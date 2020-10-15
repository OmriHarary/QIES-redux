using System;
using System.Runtime.Serialization;

namespace QIES.Core.Users
{
    [Serializable]
    public class AgentLimitExceededException : Exception
    {
        public AgentLimitExceededException() { }
        public AgentLimitExceededException(string message) : base(message) { }
        public AgentLimitExceededException(string message, Exception inner) : base(message, inner) { }
        protected AgentLimitExceededException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
}
