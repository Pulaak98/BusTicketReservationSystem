using BusTicketReservationSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketReservationSystem.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly SearchService _search;
        public SearchController(SearchService search) => _search = search;

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string from, [FromQuery] string to, [FromQuery] DateTime date)
        {
            var normalizedDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
            var result = await _search.SearchAvailableBusesAsync(from, to, normalizedDate);
            return Ok(result);
        }
    }


}
