using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RapidApi.Web.Exceptions;

namespace RapidApi.Web.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("/Error")]
        public IActionResult Index()
        {
            var exceptionFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            var exception = exceptionFeature?.Error;

            if (exception == null)
            {
                return View();
            }

            int statusCode;
            string message;

            switch (exception)
            {
                case ExternalApiTimeoutException:
                    statusCode = StatusCodes.Status504GatewayTimeout;
                    message = "Bağlantı zaman aşımına uğradı. Lütfen tekrar deneyin.";
                    break;

                case ExternalApiException:
                    statusCode = StatusCodes.Status502BadGateway;
                    message = "Harici servisle iletişim sırasında bir sorun oluştu.";
                    break;

                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    message = "Beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
                    break;
            }

            _logger.LogError(
                exception,
                "Uygulama hatası. İstek yolu: {Path}",
                exceptionFeature?.Path);

            Response.StatusCode = statusCode;

            ViewBag.ErrorMessage = message;
            ViewBag.StatusCode = statusCode;

            return View();
        }
    }
}
