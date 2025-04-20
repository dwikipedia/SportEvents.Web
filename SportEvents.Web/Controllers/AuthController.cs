using Microsoft.AspNetCore.Mvc;
using SportEvents.Core.Models.Auth;
using SportEvents.Core.Models.Exceptions;
using SportEvents.Domain.Repositories;
using static SportEvents.Web.Constants;

namespace SportEvents.Web.Controllers
{
    public class AuthController(IAuthRepository authService,
        ITokenProvider tokenProvider) : Controller
    {
        private readonly IAuthRepository _authService = authService;
        private readonly ITokenProvider _tokenProvider = tokenProvider;

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterRequest());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            string action = "Login";
            string controller = "Auth";

            if (!string.IsNullOrEmpty(_tokenProvider.GetToken()))
            {
                action = "Index";
                controller = "Users";
            }

            return await HandleAuthFlowAsync(
                request,
                () => _authService.RegisterAsync(request),
                successAction: action,
                successController: controller
            );
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            return await HandleAuthFlowAsync(
                request,
                () => _authService.LoginAsync(request),
                successController: "Home",
                successAction: "Index"
            );
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Login", "Auth");
        }

        private async Task<IActionResult> HandleAuthFlowAsync<TRequest>(
            TRequest request,
            Func<Task> authAction,
            string successController,
            string successAction,
            string? successMessage = null) where TRequest : class
        {
            if (!ModelState.IsValid)
                return View(request);

            try
            {
                await authAction();
                if (successAction.Equals("login", StringComparison.CurrentCultureIgnoreCase))
                {
                    successMessage = LogMessages.SuccessRegistration;
                }
                else if (successAction.Equals("index", StringComparison.CurrentCultureIgnoreCase)
                    && successController.Equals("users", StringComparison.CurrentCultureIgnoreCase))
                {
                    successMessage = LogMessages.SuccessCreateNewUser;
                }
                else
                {
                    successMessage = string.Empty;
                }

                if (!string.IsNullOrEmpty(successMessage))
                    TempData["SuccessMessage"] = successMessage;

                return RedirectToAction(successAction, successController);
            }
            catch (ApiException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.Key, string.Join(", ", error.Value));
                }

                if (ex.Errors.Count == 0)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }

                return View(request);
            }
        }
    }
}
