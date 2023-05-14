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
            IEnumerable<Trip> TripsList = _context.Trips.Where(x => x.FromLoc == search
            && x.ToLoc == search2
            && x.DateOfTrip.Date == tripDate2.Date
            && x.Seats >= seats).ToList();
            return View(TripsList);
        }

        public IActionResult ReadMore(int id)
        {
            IEnumerable<Trip> tripFromDb = _context.Trips.Where(x => x.Trip_id == id).ToList();

            return View(tripFromDb);
        }

        public async Task<IActionResult> BookAsync(int id)
        {            
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var userId = user?.Id;
                var tripId = id;
            if (user!=null)
            {
                var bookedUser = new Book
                {
                    TripId = tripId,
                    BookedPassengerId = userId
                };
                if (!(_context.Books.Any(x => (x.TripId == tripId && x.BookedPassengerId == user.Id))))
                {
                    if (ModelState.IsValid)
                    {
                        _context.Books.Add(bookedUser);
                        _context.SaveChanges();
                        TempData["success"] = "Book updated successfully!!!";
                        return RedirectToAction("ReadMore");
                    }
                    return NotFound();
                }
                else
                {
                    return View(bookedUser);
                }                                               
            }
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
        public async Task<IActionResult> IndexDAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var query = from trip in _context.Trips
                        join books in _context.Books on trip.Trip_id equals books.TripId
                        join acceptedPassengers in _context.Users on books.AcceptedPassengerId equals acceptedPassengers.Id
                        join bookedPassengers in _context.Users on books.BookedPassengerId equals bookedPassengers.Id
                        where  trip.DriverId == user.Id & trip.TripStatus == true 
                        select new
                        {
                            trip.Trip_id,
                            trip.FromLoc,
                            trip.ToLoc,
                            trip.DateOfTrip,
                            trip.Fare,
                            trip.Seats,
                            AcceptedPassengers = acceptedPassengers.UserName,
                            BookedPassengers = bookedPassengers.UserName
                        };
            List<DriverViewModel> result = query.Select(item => new DriverViewModel
            {
                Trip_id = item.Trip_id,
                FromLoc = item.FromLoc,
                ToLoc = item.ToLoc,
                DateOfTrip = item.DateOfTrip,
                Fare = item.Fare,
                Seats = item.Seats,
                AcceptedPassengers = item.AcceptedPassengers,
                BookedPassengers = item.BookedPassengers
            }).ToList();

            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> IndexDAsync(string from, string to, DateTime tripDate, int seats, string fare, string aboutcar)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;
            var trip = new Trip
            {
                FromLoc = from,
                ToLoc = to,
                DateOfTrip = tripDate,
                Seats = seats,
                Fare = fare,
                AutoInf = aboutcar,
                DriverId = userId,
                TripStatus = true
            };
            if (ModelState.IsValid)
            {
                _context.Trips.Add(trip);
                _context.SaveChanges();
                TempData["success"] = "Trip added successfully!!!";
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