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
            //var month = int.Parse(tripDate.Month.ToString());
            //var day = int.Parse(tripDate.Day.ToString());
            //var year = int.Parse(tripDate.Year.ToString());
            //var tripDate2 = new DateTime(year, day, month);
            IEnumerable<Trip> TripsList = _context.Trips.Where(x => x.FromLoc == search
            && x.ToLoc == search2
            && x.DateOfTrip.Date == tripDate
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
                    TempData["AlertMessage"] = "You have already booked";
                    TempData["AlertType"] = "success";
                    return RedirectToAction("ReadMore");
                }                                               
            }
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
        public async Task<IActionResult> IndexDAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var query = from t in _context.Trips
                        join b in _context.Books on t.Trip_id equals b.TripId into bGroup
                        from b in bGroup.DefaultIfEmpty()
                        join u1 in _context.Users on b.BookedPassengerId equals u1.Id into u1Group
                        from u1 in u1Group.DefaultIfEmpty()
                        join u2 in _context.Users on b.AcceptedPassengerId equals u2.Id into u2Group
                        from u2 in u2Group.DefaultIfEmpty()
                        where t.TripStatus == true && t.DriverId == user.Id
                        group new { t, u1, u2 } by new { b.Id, t.Trip_id, t.DriverId, t.FromLoc, t.ToLoc, t.DateOfTrip, t.Fare, t.Seats, u1.UserName } into g
                        orderby g.Key.Trip_id
                        select new
                        {
                            g.Key.Id,
                            g.Key.Trip_id,
                            g.Key.DriverId,
                            g.Key.FromLoc,
                            g.Key.ToLoc,
                            g.Key.DateOfTrip,
                            g.Key.Fare,
                            g.Key.Seats,
                            g.Key.UserName,
                            AcceptedPassengers = string.Join(",", g.Where(x => x.u2 != null).Select(x => x.u2.UserName)),

                        };
            List<DriverViewModel> result = query.Select(item => new DriverViewModel
            {
                Id = item.Id,
                Trip_id = item.Trip_id,
                FromLoc = item.FromLoc,
                ToLoc = item.ToLoc,
                DateOfTrip = item.DateOfTrip,
                Fare = item.Fare,
                Seats = item.Seats,
                BookedPassengers = item.UserName,
                AcceptedPassengers = item.AcceptedPassengers,
            }).ToList();

            var query1 = from t in _context.Trips
                         join b in _context.Books on t.Trip_id equals b.TripId into bGroup
                         from b in bGroup.DefaultIfEmpty()
                         join u1 in _context.Users on b.BookedPassengerId equals u1.Id into u1Group
                         from u1 in u1Group.DefaultIfEmpty()
                         join u2 in _context.Users on b.AcceptedPassengerId equals u2.Id into u2Group
                         from u2 in u2Group.DefaultIfEmpty()
                         group new { t, u1, u2 } by new { t.Trip_id, t.DriverId, t.FromLoc, t.ToLoc, t.DateOfTrip, t.Fare, t.Seats } into g
                         orderby g.Key.Trip_id
                         select new
                         {
                             g.Key.Trip_id,
                             g.Key.DriverId,
                             g.Key.FromLoc,
                             g.Key.ToLoc,
                             g.Key.DateOfTrip,
                             g.Key.Fare,
                             g.Key.Seats,
                             AcceptedPassengers = string.Join(",", g.Where(x => x.u2 != null).Select(x => x.u2.UserName))
                         };

            List<DriverViewModel> result1 = query1.Select(item => new DriverViewModel
            {
                Trip_id = item.Trip_id,
                FromLoc = item.FromLoc,
                ToLoc = item.ToLoc,
                DateOfTrip = item.DateOfTrip,
                Fare = item.Fare,
                Seats = item.Seats,
                AcceptedPassengers = item.AcceptedPassengers,
            }).ToList();

            var model1List = new List<DriverViewModel>(result);
            var model2List = new List<DriverViewModel>(result1);

            var viewModel = new MyCombinedModel { Model1List = model1List, Model2List = model2List };

            return View(viewModel);
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

        public async Task<IActionResult> AcceptPassenger(int id)
        {
            // Retrieve the booking with the specified trip ID and booked passenger ID
            var booking = _context.Books.FirstOrDefault(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            // Update the accepted passenger ID to the value of the booked passenger ID
            booking.AcceptedPassengerId = booking.BookedPassengerId;

            // Set the booked passenger ID to null
            booking.BookedPassengerId = null;

            // Save the changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction("IndexD","Home");
        }

        public async Task<IActionResult> DeclinePassenger(int id)
        {
            // Retrieve the booking with the specified trip ID and booked passenger ID
            var booking = _context.Books.FirstOrDefault(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            // Update the accepted passenger ID to the value of the booked passenger ID
            _context.Books.Remove(booking);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction("IndexD", "Home");
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