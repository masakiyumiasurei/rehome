using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using rehome.Enums;
using rehome.Http;
using rehome.Models;
using rehome.Services;


namespace rehome.Controllers
{
    public class PopupSearchController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<PopupSearchController> _logger;
        private ISyouhinService _SyouhinService;
        private ISiireService _SiireService;

        public PopupSearchController(ILogger<PopupSearchController> logger, IConfiguration configuration, 
            ISyouhinService SyouhinService, ISiireService SiireService)
        {
            _logger = logger;
            
            _connectionString = configuration.GetConnectionString("DefaultConnection");            
            _SyouhinService = SyouhinService;
            _SiireService = SiireService;
        }       

        [HttpGet]
        public IActionResult Syouhin()
        {
            //商品検索ボックスを返す
            var viewModel = new PopupSearchSyouhinModel();
            return PartialView("_PopupSearchSyouhin", viewModel);

            //viewModel.SearchSyouhins = _SyouhinService.SearchAjaxSyouhins(viewModel.SearchConditions);
            //return PartialView("_PopupSearchSyouhinResult", viewModel);
        }

        [HttpPost]
        public ActionResult Syouhin(string? PopupSyouhinID, string? PopupSyouhinName)
        {
            //検索結果を返す
            var viewModel = new PopupSearchSyouhinModel();
            int count = 0;

            if (PopupSyouhinID != null) { viewModel.SearchConditions.商品ID = Int32.Parse(PopupSyouhinID); count++; }
            if (PopupSyouhinName != null) { viewModel.SearchConditions.商品名 = PopupSyouhinName; count++; }

            // if (count > 0) { viewModel.SearchSyouhins = _SyouhinService.SearchAjaxSyouhins(viewModel.SearchConditions); }
            viewModel.SearchSyouhins = _SyouhinService.SearchAjaxSyouhins(viewModel.SearchConditions);
            return PartialView("_PopupSearchSyouhinResult", viewModel);           
        }

        [HttpGet]
        public IActionResult Siire()
        {
            var viewModel = new PopupSearchSiireModel();

            viewModel.SearchSiires = _SiireService.SearchSiires(viewModel.SearchConditions);
          
           return PartialView("_PopupSearchSiire", viewModel);

        }

        [HttpPost]
        public ActionResult Siire( string? PopupSiireName, 業種? PopupSiireGyoushu)
        {
            var viewModel = new PopupSearchSiireModel();
            int count = 0;

            //if (PopupOfficeID != null) { viewModel.SearchConditions.販売店ID = Int32.Parse(PopupOfficeID); count++; }
            if (PopupSiireName != null) { viewModel.SearchConditions.仕入先名 = PopupSiireName; count++; }
            if (PopupSiireGyoushu != null) { viewModel.SearchConditions.業種 = PopupSiireGyoushu; count++; }

            //if (count > 0) { viewModel.SearchOffices = _OfficeService.SearchOffices(viewModel.SearchConditions); }

            viewModel.SearchSiires = _SiireService.SearchSiires(viewModel.SearchConditions);
            return PartialView("_PopupSearchSiireResult", viewModel);
        }

    }
}