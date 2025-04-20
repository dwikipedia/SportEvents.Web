using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportEvents.Core.Models.Exceptions;
using SportEvents.Core.Models.User;
using SportEvents.Domain.Models.User;
using SportEvents.Domain.Repositories;
using static SportEvents.Web.Constants;

namespace SportEvents.Web.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthRepository _authRepository;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserRepository userRepository, IAuthRepository authRepository, ILogger<UsersController> logger)
        {
            _userRepository = userRepository;
            _authRepository = authRepository;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("[Users/Index] Loading menu at {Time}", DateTime.UtcNow);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(UserGetByIdRequest request)
        {
            try
            {
                var user = await _userRepository.GetUserById(request.Id);
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", request.Id);
                    return NotFound();
                }
                _logger.LogInformation($"User id: {request.Id} successfully fetched.");

                return View(user);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                ModelState.AddModelError("", message);

                _logger.LogError(ex, message);
                return View(request);
            }
        }

        [HttpPut]
        public async Task<IActionResult> ChangePassword(int id, ChangePasswordRequest request)
        {
            try
            {
                await _userRepository.ChangePassword(id, request);
                TempData["SuccessMessage"] = LogMessages.SuccessMessage;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving user with ID {id}");
                return RedirectToAction(nameof(Index));
            }
        }


        public IActionResult Create()
        {
            return RedirectToAction("Register", "Auth");
        }

        public async Task<IActionResult> Update(UserGetByIdRequest request)
        {
            try
            {
                var user = await _userRepository.GetUserById(request.Id);
                return View(user);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                ModelState.AddModelError("", message);

                _logger.LogError(ex, message);
                return View("Index", new UserGetByIdRequest());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(UserGetByIdRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(LogMessages.InvalidModelUserUpdate);
                ModelState.AddModelError(string.Empty, LogMessages.InvalidModelUserUpdate);
            }

            try
            {
                await _userRepository.UpdateUser(request);
                TempData["SuccessMessage"] = LogMessages.SuccessEditMessage;
                _logger.LogInformation($"[Users/Update]{LogMessages.SuccessEditMessage}");
            }
            catch (ApiException ex)
            {
                foreach (var error in ex.Errors)
                {
                    _logger.LogError(ex, "Failed to update user {UserId}", error.Value);
                    ModelState.AddModelError(error.Key, string.Join(", ", error.Value));
                }
                return View("Update", request);
            }
            return View("Update", request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _userRepository.DeleteUserById(id);
                TempData["SuccessMessage"] = LogMessages.SuccessDeleteMessage;
                _logger.LogInformation($"[Users/Delete]{LogMessages.SuccessDeleteMessage}");
            }
            catch (ApiException ex)
            {
                foreach (var error in ex.Errors)
                {
                    _logger.LogError(ex, "Failed to delete user {UserId}", error.Value);
                    ModelState.AddModelError(error.Key, string.Join(", ", error.Value));
                }
                return View("Index", new UserGetByIdRequest());
            }
            return View("Index", new UserGetByIdRequest());
        }
    }
}
