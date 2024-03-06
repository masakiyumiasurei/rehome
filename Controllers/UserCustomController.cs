using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using rehome.Enums;
using rehome.Http;
using rehome.Models;
using rehome.Services;

namespace rehome.Controllers
{
    public class UserCustomController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<UserCustomController> _logger;



        public UserCustomController(ILogger<UserCustomController> logger, IConfiguration configuration)
        {
            _logger = logger;
            // appsettings.jsonファイルから接続文字列を取得
            _connectionString = configuration.GetConnectionString("DefaultConnection");

        }


        [HttpGet]
        public IActionResult ReturnPopup(string RenderURL)
        {
            return PartialView(RenderURL);
        }

    }
}