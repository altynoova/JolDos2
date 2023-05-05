using JolDos2.Data;
using JolDos2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace JolDos2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _context { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(UserManager<ApplicationUser> userManager,
            ILogger<HomeController> logger,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
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

        public IActionResult ReadMore(int id)
        {
            IEnumerable<Trip> tripFromDb = _context.Trips.Where(x => x.Id == id).ToList();

            return View(tripFromDb);
        }
        public async Task<IActionResult> IndexDAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;
            //IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            IEnumerable<Trip> tripFromDb = _context.Trips.Where(x=> x.DriverId == userId);

            return View(tripFromDb);
        }
        [HttpPost]
        public async Task<IActionResult> IndexDAsync(string from, string to, DateTime tripDate, int seats, string fare, string aboutcar)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;
            var trip = new Trip
            {
                From = from,
                To = to,
                DateOfTrip = tripDate,
                Seats = seats,
                Fare = fare,
                AutoInf = aboutcar,
                DriverId = userId
            };
            if (ModelState.IsValid)
            {
                _context.Trips.Add(trip);
                _context.SaveChanges();
                TempData["success"] = "Category updated successfully!!!";
                return RedirectToAction("IndexD");
            }
            return View(trip);
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