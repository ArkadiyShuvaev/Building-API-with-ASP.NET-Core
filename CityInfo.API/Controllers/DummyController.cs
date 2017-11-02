using CityInfo.Data;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    public class DummyController : Controller
    {
	    private readonly CityInfoContext _context;

	    public DummyController(CityInfoContext context)
	    {
		    _context = context;
	    }

		[HttpGet]
		[Route("api/testdatabase")]
		public IActionResult TestDatabase()
	    {
		    return Ok();
	    }
    }
}
