using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using rehome.Enums;
using rehome.Http;
using rehome.Models;
using rehome.Services;
using Microsoft.AspNetCore.Authorization;
using rehome.Models.DB;
using System.Security.Claims;
using Pao.Reports;
using rehome.Public;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
//using System.Management;

namespace rehome.Controllers
{
    [Authorize]
    public class PayController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<QuoteController> _logger;
        //private const int PAGE_SIZE = 20;
        private IQuoteService _QuoteService;
        private IDropDownListService _DropDownListService;
        private IHouzinService _HouzinService;
        private IChumonService _ChumonService;
        private IOfficeService _OfficeService;
        private ITantouService _TantouService;
        private IPayService _PayService;

        public PayController(ILogger<QuoteController> logger, IConfiguration configuration, IQuoteService QuoteService, 
            IDropDownListService dropDownListService, IHouzinService HouzinService,IChumonService ChumonService,
            IOfficeService OfficeService,ITantouService TantouService,IPayService PayService)
        {
            _logger = logger;
            // appsettings.jsonファイルから接続文字列を取得
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _QuoteService = QuoteService;
            _DropDownListService = dropDownListService;
            _HouzinService = HouzinService;
            _ChumonService = ChumonService;
            _OfficeService = OfficeService;
            _TantouService = TantouService;
            _PayService = PayService;
        }


        [HttpGet]
        public ActionResult Create(int? 仕入ID, string? BackUrl)
        {
            using var connection = new SqlConnection(_connectionString);

            ViewBag.OperationMessage = (string)TempData["Quote"];

            var model = new PayCreateModel();

            //postした時にRefererヘッダーが変わらない様に
            
            if (BackUrl != null)
            {
                model.BackUrl = BackUrl;
            }
            else if (Request.Headers["Referer"].Any())
            {
                model.BackUrl = Request.Headers["Referer"].ToString();
            }

            if (仕入ID == null)//new処理
            {
                

                model.Mode = ViewMode.New;
                //開発中コメント
                model.Pay.担当ID = Int32.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                model.auth= bool.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value);
            }
            else
            {//edit処理
                
                model.Mode = ViewMode.Edit;
                model.Pay = _PayService.GetPay(仕入ID ?? -1);//null許容でGetQuoteする処理が適正ではないので、Create呼ばれる際は絶対に値が入って呼ばれるようにする？
 
                model.auth = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value);
               
            }

            model.担当DropDownList = _DropDownListService.Get担当DropDownLists();

            return View(model);

        }

        [HttpPost]
        public IActionResult Create(PayCreateModel model)
        {
            var viewModel = new PayCreateModel();
            try
            {
                
                viewModel.Pay = _PayService.RegistPay(model);
                
                viewModel.Mode = ViewMode.Edit;
                TempData["Pay"] = String.Format("仕入帳情報を登録しました");
                ModelState.Clear();
                viewModel.BackUrl = model.BackUrl;
                return RedirectToAction("Create", "Pay", new {仕入ID = viewModel.Pay.仕入ID, viewModel.BackUrl });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");
                return View(model);
            }
        }


        [HttpPost]
        public IActionResult Delete(PayCreateModel model)
        {
            var viewModel = new PayCreateModel();
            try
            {
                _PayService.DeletePay(model.Pay.仕入ID);

                TempData["Pay_Index"] = String.Format("仕入帳情報を削除しました");

                return RedirectToAction("Index", "Pay");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");
                return View(model);
            }


        }
             

        public IActionResult Index(int? page)
    {
            ViewBag.OperationMessage = (string)TempData["Quote_Index"];

            var viewModel = new PayIndexModel();
            if (HttpContext.Session.GetObject<PaySearchConditions>(SessionKeys.QUOTE_SEARCH_CONDITIONS) != null)
            {
                viewModel.PaySearchConditions = HttpContext.Session.GetObject<PaySearchConditions>(SessionKeys.QUOTE_SEARCH_CONDITIONS);
                //if (page == null) pageNumber = viewModel.QuoteSearchConditions.page;
            }
            //viewModel.QuoteSearchConditions.page = pageNumber;
            HttpContext.Session.SetObject(SessionKeys.QUOTE_SEARCH_CONDITIONS, viewModel.PaySearchConditions);

            //開発中はコメント
            //viewModel.PaySearchConditions.LoginID = Int32.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            return View("Index", viewModel);
        }

        [HttpGet]
        public ActionResult Clear()
        {
            var viewModel = new PayIndexModel();
            viewModel.PaySearchConditions = new PaySearchConditions();
            return RedirectToAction("Index", "Pay");
        }

    }
}


