using ScrapApi.Properties;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace ScrapApi.Utils
{
    [Serializable]
    public class ScrapException : Exception
    {
        /// <summary>
        /// The code of the message. It will be
        /// replaced for a full message from resource file
        /// with corresponding identifier
        /// </summary>
        public string CodeMessage { get; set; }

        /// <summary>
        /// The parameters for the message
        /// </summary>
        public object[] Parameters { get; set; }

        /// <summary>
        /// Initiate the exception with the code
        /// </summary>
        /// <param name="codMessage">The code of the message</param>
        public ScrapException(string codMessage) : base(SrsExceptionUtils.GetMessage(codMessage))
        {
            this.CodeMessage = codMessage;
        }

        /// <summary>
        /// Initiate the exception with the code and parameters
        /// </summary>
        /// <param name="codMessage">The code of the message</param>
        /// <param name="args">The parameters</param>
        public ScrapException(string codMessage, params object[] args) : base(SrsExceptionUtils.GetMessage(codMessage, args))
        {
            this.Parameters = args;
            this.CodeMessage = codMessage;
        }
        /// <summary>
        /// The serialization info
        /// </summary>
        /// <param name="info">Info</param>
        /// <param name="context">Context</param>
        protected ScrapException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// Utilities for SRS exception
    /// </summary>
    public static class SrsExceptionUtils
    {
        /// <summary>
        /// Gets the full message from resource using the code
        /// and format it with the parameters
        /// </summary>
        /// <param name="codMessage">The code of the message</param>
        /// <param name="args">The parameters</param>
        /// <returns>The message formated</returns>
        public static string GetMessage(string codMessage, params object[] args)
        {
            string msg = Resources.ResourceManager.GetString(codMessage);

            if (args != null && args.Any() && msg.Contains("{"))
            {
                msg = string.Format(msg, args);
            }
            return msg;
        }
    }
}
