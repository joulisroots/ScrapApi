using ScrapApi.Utils;

namespace ScrapApi.Models
{
    public class ExceptionModel
    {
        /// <summary>
        /// Default constructor for initialize the message and parameters.
        /// </summary>
        /// <param name="scrapException">The exception</param>
        public ExceptionModel(ScrapException scrapException)
        {
            this.CodeMessage = scrapException.CodeMessage;
            this.Message = scrapException.Message;
        }

        /// <summary>
        /// The code of the message in resources.resx.
        /// </summary>
        public string CodeMessage { get; set; }

        /// <summary>
        /// The message of the exception.
        /// </summary>
        public string Message { get; set; }

    }
}
