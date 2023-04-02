using JolDos2.Data;
using JolDos2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace JolDos2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _context { get; set; } 

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext context)
        {
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

        [HttpPost]
        //public JsonResult SearchTrip(string from, string to,DateTime date, int seats)
        //{
        //    List<Trip> triples = new List<Trip>(_context.Ttr);

        //    return Json(locations);
        //}
        [HttpPost]
        public IActionResult Index(string LocationId, string LocationName)
        {
            ViewBag.Message = "Location id: " + LocationId + "Location name" + LocationName;
            return View();
        }
        public IActionResult Index()
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