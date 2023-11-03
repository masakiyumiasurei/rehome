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
    public class OfficeController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<OfficeController> _logger;
        private IOfficeService _OfficeService;
        private IDropDownListService _DropDownListService;

        private const int PAGE_SIZE = 20;

        public OfficeController(ILogger<OfficeController> logger, IConfiguration configuration, IOfficeService OfficeService, IDropDownListService DropDownListService)
        {
            _logger = logger;
            // appsettings.jsonファイルから接続文字列を取得
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _OfficeService = OfficeService;
            _DropDownListService = DropDownListService;
        }


        /// <summary>
        /// 新規登録画面表示時のアクション
        /// </summary>
        [HttpGet]
        public IActionResult New()
        {
            var viewModel = new OfficeDetailModel();
            viewModel.Mode = ViewMode.New;
            viewModel.Office = new 営業所();
            if (Request.Headers["Referer"].Any())
            {
                viewModel.BackUrl = Request.Headers["Referer"].ToString();
            }
            return View("Details", viewModel);
        }
        /// <summary>
        /// 編集画面表示時のアクション
        /// </summary>
        [HttpGet]
        public IActionResult Details(int 営業所ID)
        {
            ViewBag.OperationMessage = TempData["Office"];

            var viewModel = new OfficeDetailModel();
            viewModel.Mode = ViewMode.Edit;
            viewModel.Office = _OfficeService.GetOffice(営業所ID); 
            if (Request.Headers["Referer"].Any())
            {
                viewModel.BackUrl = Request.Headers["Referer"].ToString();
            }
            return View(viewModel);
        }

        /// <summary>
        /// 登録 新規、更新ともにDetailsが呼ばれる
        /// </summary>
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [AutoValidateAntiforgeryToken]
        public ActionResult Details(OfficeDetailModel model)
        {
            var viewModel = new OfficeDetailModel();

            try
            {
                // 登録成功したら営業所画面再表示
                viewModel.Office = _OfficeService.RegistOffice(model);
                viewModel.Mode = model.Mode;
                ViewBag.OperationMessage = String.Format("営業所情報を登録しました");
                TempData["Office"] = String.Format("営業所情報を登録しました");
                ModelState.Clear();
                viewModel.BackUrl = model.BackUrl;
                return RedirectToAction("Details", "Office", new { 営業所ID = viewModel.Office.営業所ID });
            }
            catch (Exception ex)
            {
       
                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Delete(OfficeDetailModel model)
        {
            var viewModel = new OfficeDetailModel();
            try
            {
                _OfficeService.DeleteOffice(model.Office.営業所ID);

                TempData["Office_Index"] = String.Format("営業所情報を削除しました");

                return RedirectToAction("Index", "Office");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");
                return View(model);
            }


        }

        /// サンプル一覧で検索ボタンクリック時のアクション
        public IActionResult Search(OfficeIndexModel model, bool Clear)
        {

            ViewBag.OperationMessage = TempData["Office_Index"];

            var viewModel = new OfficeIndexModel();
            if (Clear == true)
            {
                viewModel.OfficeSearchConditions = new OfficeSearchConditions();
            }
            else
            {
                viewModel.OfficeSearchConditions = model.OfficeSearchConditions;
            }
            ModelState.Clear();

            HttpContext.Session.SetObject(SessionKeys.Office_SEARCH_CONDITIONS, viewModel.OfficeSearchConditions);

            viewModel.Office = _OfficeService.SearchOffices(viewModel.OfficeSearchConditions);

            return View("Index", viewModel);
        }

        [HttpGet]
        public ActionResult Clear()
        {
            var viewModel = new OfficeIndexModel();
            viewModel.OfficeSearchConditions = new OfficeSearchConditions();
            return RedirectToAction("Search", "Office");
        }

        public IActionResult Index()
        {

            ViewBag.OperationMessage = TempData["Office_Index"];

            var viewModel = new OfficeIndexModel();
            if (HttpContext.Session.GetObject<OfficeSearchConditions>(SessionKeys.Office_SEARCH_CONDITIONS) != null)
            {
                viewModel.OfficeSearchConditions = HttpContext.Session.GetObject<OfficeSearchConditions>(SessionKeys.Office_SEARCH_CONDITIONS);

            }

            HttpContext.Session.SetObject(SessionKeys.Office_SEARCH_CONDITIONS, viewModel.OfficeSearchConditions);


            viewModel.Office = _OfficeService.SearchOffices(viewModel.OfficeSearchConditions);

            return View("Index", viewModel);
        }
    }
}