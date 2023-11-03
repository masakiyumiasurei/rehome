using Microsoft.AspNetCore.Mvc;
using rehome.Enums;
using rehome.Http;
using rehome.Models;
using rehome.Services;
using Microsoft.AspNetCore.Authorization;
using rehome.Models.DB;
using rehome.Public;

namespace rehome.Controllers
{
    //注意
    [Authorize]
    public class HouzinController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<HouzinController> _logger;
        private IHouzinService _HouzinService;
        private IDropDownListService _DropDownListService;
        private IQuoteService _QuoteService;

        private const int PAGE_SIZE = 20;

        public HouzinController(ILogger<HouzinController> logger, IConfiguration configuration, IHouzinService HouzinService, IDropDownListService DropDownListService,IQuoteService QuoteService)
        {
            _logger = logger;
            // appsettings.jsonファイルから接続文字列を取得
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _HouzinService = HouzinService;
            _DropDownListService = DropDownListService; 
            _QuoteService = QuoteService;
        }


      
        [HttpGet]
        public IActionResult Details()
        {
            ViewBag.OperationMessage = TempData["Houzin"];

            var viewModel = new HouzinDetailModel();
            viewModel.Mode = ViewMode.Edit;
            viewModel.Houzin = _HouzinService.GetHouzin();
            //viewModel.Setting = new 設定();
            //viewModel.Setting.期 = _QuoteService.GetPeriod();
            if (Request.Headers["Referer"].Any())
            {
                viewModel.BackUrl = Request.Headers["Referer"].ToString();
            }
            return View(viewModel);
        }

       
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public ActionResult Details(HouzinDetailModel model)
        {
            var viewModel = new HouzinDetailModel();

            try
            {
                // 登録成功したら法人画面再表示
                viewModel.Houzin = _HouzinService.RegistHouzin(model);
                //viewModel.Setting = new 設定();
                //viewModel.Setting.期 = _HouzinService.RegistSetting(model.Setting);
                viewModel.Mode = model.Mode;
                TempData["Houzin"] = String.Format("法人情報を登録しました");
                ModelState.Clear();
                viewModel.BackUrl = model.BackUrl;
                return RedirectToAction("Details", "Houzin");
            }
            catch (Exception ex)
            {
       
                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");
                return View(model);
            }
        }

    }
}