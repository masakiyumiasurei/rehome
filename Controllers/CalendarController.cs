using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using rehome.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using rehome.Calendar;
using Microsoft.AspNetCore.Authorization;
using rehome.Services;

namespace rehome.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        private readonly ILogger<CalendarController> _logger;
        private readonly string _connectionString;
        private ICalendarService _CalendarService;
        public CalendarController(IConfiguration configuration, ICalendarService CalendarService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _CalendarService = CalendarService;
        }

       
        public IActionResult Calendar()
        {
            return View("Calendar");
        }

        public IActionResult GetAllEvents(DateTime start, DateTime end)
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                connection.Open();
                IList<Event> events ;
                events = _CalendarService.GetCalendar(start, end);                

                return new JsonResult(events);
            }
        }
    
     

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return RedirectToAction("Login", "Login");
            //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}