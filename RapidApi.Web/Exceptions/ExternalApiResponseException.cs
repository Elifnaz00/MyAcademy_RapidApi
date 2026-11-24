namespace RapidApi.Web.Exceptions
{
    public class ExternalApiResponseException : ExternalApiException
    {
        public ExternalApiResponseException(string message, Exception innerException)
           : base(message, innerException)
        {
        }

    }
}
