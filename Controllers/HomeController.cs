using JolDos2.Data;
using JolDos2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace JolDos2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private ApplicationDbContext _context { get; set; }
        public HomeController(ILogger<HomeController> logger,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }


        [HttpPost]
        public JsonResult AutoComplete(string search)
        {
            var locations = (from location in _context.Locations
                             where location.LocationName.StartsWith(search)
                             select new
                             {
                                 label = location.LocationName,
                                 val = location.Id
                             }).ToList();
            return Json(locations);
        }

        public JsonResult AutoComplete2(string search)
        {
            var locations = (from location in _context.Locations
                             where location.LocationName.StartsWith(search)
                             select new
                             {
                                 label = location.LocationName,
                                 val = location.Id
                             }).ToList();
            return Json(locations);
        }

        [HttpGet]
        public IActionResult Index()
        {
            var result = User.IsInRole("driver");
            if (result)
            {
            return RedirectToAction("IndexD");
            }
            IEnumerable<Trip> TripsList = _context.Trips.ToList();
            return View(TripsList);
        }

        public IActionResult IndexD()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string search, string search2, DateTime tripDate, int seats)
        {
            var month = int.Parse(tripDate.Month.ToString());
            var day = int.Parse(tripDate.Day.ToString());
            var year = int.Parse(tripDate.Year.ToString());
            var tripDate2 = new DateTime(year, day, month);
            IEnumerable<Trip> TripsList = _context.Trips.Where(x => x.From == search
            && x.To == search2
            && x.DateOfTrip.Date == tripDate2.Date
            && x.Seats >= seats).ToList();
            return View(TripsList);
        }
        [HttpGet]

        public IActionResult ReadMore(int id)
        {
            IEnumerable <Trip> tripFromDb =_context.Trips.Where(x => x.Id == id).ToList();

            return View(tripFromDb);
        }

        [Authorize]
        public IActionResult Book(int id)
        {          
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}