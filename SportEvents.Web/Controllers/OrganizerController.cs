using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportEvents.Domain.Models.Organizer;
using SportEvents.Domain.Repositories;

namespace SportEvents.Web.Controllers
{
    public class OrganizerController(
        IOrganizerRepository orgRepository,
        ITokenProvider tokenProvider,
        ILogger<OrganizerController> logger) : Controller
    {
        private readonly IOrganizerRepository _orgRepository = orgRepository;
        private readonly ITokenProvider _tokenProvider = tokenProvider;
        private readonly ILogger<OrganizerController> _logger = logger;

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            int draw,
            int start,
            int length,
            string? searchValue,
            string? orderColumn,
            string? orderDir)
        {
            try
            {
                int page = (start / length) + 1;
                var request = new OrganizersRequest
                {
                    Page = page,
                    PerPage = length,
                    SearchValue = searchValue,
                    SortColumn = orderColumn,
                    SortDirection = orderDir
                };
                var paged = await _orgRepository.GetAllOrganizers(request);
                return Json(new
                {
                    draw,
                    recordsTotal = paged.RecordsTotal,
                    recordsFiltered = paged.RecordsFiltered,
                    data = paged.Data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "Internal server error");
            }

        }

        //[HttpPost]
        //public async Task<IActionResult> GetAllOrganizers(OrganizersRequest request)
        //{
        //    try
        //    {
        //        var user = await _orgRepository.GetAllOrganizers(request);
        //        if (user == null)
        //        {
        //            //_logger.LogWarning("User {UserId} not found", request.Id);
        //            return NotFound();
        //        }
        //        //_logger.LogInformation($"User id: {request.Id} successfully fetched.");

        //        return View(user);
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.Message;
        //        ModelState.AddModelError("", message);

        //        _logger.LogError(ex, message);
        //        return View(request);
        //    }
        //}
    }
}
