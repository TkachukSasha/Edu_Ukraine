using Microsoft.AspNetCore.Mvc;

using Web.App.Services;

namespace Web.App.Controllers
{
    public class UniversityController(SPARQLQueryService queryService) : Controller
    {
        private readonly SPARQLQueryService _queryService = queryService;

        public async Task<IActionResult> Index(string searchQuery = "")
        {
            var universities = await _queryService.GetUniversitiesListAsync(searchQuery);
            return View(universities);
        }

        public async Task<IActionResult> Details(string name)
        {
            var university = await _queryService.GetUniversityDetailsAsync(name);

            if (university == null)
            {
                return NotFound();
            }

            return View(university);
        }
    }
}
