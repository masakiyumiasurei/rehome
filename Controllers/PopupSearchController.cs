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


        public PopupSearchController(ILogger<PopupSearchController> logger, IConfiguration configuration, 
            ISyouhinService SyouhinService )
        {
            _logger = logger;
            
            _connectionString = configuration.GetConnectionString("DefaultConnection");            
            _SyouhinService = SyouhinService;
            
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

        //[HttpGet]
        //public IActionResult Office()
        //{
        //    var viewModel = new PopupSearchOfficeModel();
        //    //return PartialView("_PopupSearchSyouhin", viewModel);
        //    return PartialView("_PopupSearchOffice", viewModel);
            
        //}

        //[HttpPost]
        //public ActionResult Office(string? PopupOfficeID, string? PopupOfficeName, string? PopupOfficerank)
        //{
        //    var viewModel = new PopupSearchOfficeModel();
        //    int count = 0;

        //    if (PopupOfficeID != null) { viewModel.SearchConditions.販売店ID = Int32.Parse(PopupOfficeID); count++; }
        //    if (PopupOfficeName != null) { viewModel.SearchConditions.販売店名 = PopupOfficeName; count++; }
        //    if (PopupOfficerank != null) { viewModel.SearchConditions.rank = PopupOfficerank; count++; }
            
        //    //if (count > 0) { viewModel.SearchOffices = _OfficeService.SearchOffices(viewModel.SearchConditions); }

        //   viewModel.SearchOffices = _OfficeService.SearchOffices(viewModel.SearchConditions);
        //    return PartialView("_PopupSearchOfficeResult", viewModel);
        //}

    }
}