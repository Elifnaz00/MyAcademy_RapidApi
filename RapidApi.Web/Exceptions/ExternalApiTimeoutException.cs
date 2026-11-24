namespace RapidApi.Web.Exceptions
{
    public class ExternalApiTimeoutException : ExternalApiException
    {
        public ExternalApiTimeoutException(string message, Exception innerException)
          : base(message, innerException)
        {
        }
    }
}
