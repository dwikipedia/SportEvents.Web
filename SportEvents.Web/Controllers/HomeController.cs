using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportEvents.Domain.Repositories;
using static SportEvents.Web.Constants;

namespace SportEvents.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITokenProvider _tokenProvider;

        public HomeController(ILogger<HomeController> logger, ITokenProvider tokenProvider)
        {
            _logger = logger;
            _tokenProvider = tokenProvider; 
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            if(!_tokenProvider.IsAuthenticated())
            {
                _logger.LogWarning(LogMessages.TokenExpiresMessage);
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }
    }
}
