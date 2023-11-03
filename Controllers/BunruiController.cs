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
    public class BunruiController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<BunruiController> _logger;
        private IBunruiService _BunruiService;
        private IDropDownListService _DropDownListService;

        private const int PAGE_SIZE = 20;

        public BunruiController(ILogger<BunruiController> logger, IConfiguration configuration, IBunruiService BunruiService, IDropDownListService DropDownListService)
        {
            _logger = logger;
            // appsettings.jsonファイルから接続文字列を取得
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _BunruiService = BunruiService;
            _DropDownListService = DropDownListService;
        }


        /// <summary>
        /// 新規登録画面表示時のアクション
        /// </summary>
        [HttpGet]
        public IActionResult New()
        {
            var viewModel = new BunruiDetailModel();
            viewModel.Mode = ViewMode.New;
            viewModel.Bunrui = new 分類();
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
        public IActionResult Details(int 分類ID)
        {
            ViewBag.OperationMessage = TempData["Bunrui"];

            var viewModel = new BunruiDetailModel();
            viewModel.Mode = ViewMode.Edit;
            viewModel.Bunrui = _BunruiService.GetBunrui(分類ID); 
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
        public ActionResult Details(BunruiDetailModel model)
        {
            var viewModel = new BunruiDetailModel();

            try
            {
                // 登録成功したら分類画面再表示
                viewModel.Bunrui = _BunruiService.RegistBunrui(model);
                viewModel.Mode = model.Mode;
                ViewBag.OperationMessage = String.Format("分類情報を登録しました");
                TempData["Bunrui_Index"] = String.Format("分類情報を登録しました");
                ModelState.Clear();
                viewModel.BackUrl = model.BackUrl;
                return RedirectToAction("Index", "Bunrui");
            }
            catch (Exception ex)
            {
       
                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Delete(BunruiDetailModel model)
        {
            var viewModel = new BunruiDetailModel();
            try
            {
                _BunruiService.DeleteBunrui(model.Bunrui.分類ID);

                TempData["Bunrui_Index"] = String.Format("分類情報を削除しました");

                return RedirectToAction("Index", "Bunrui");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");
                return View(model);
            }


        }

        /// サンプル一覧で検索ボタンクリック時のアクション
        public IActionResult Search(BunruiIndexModel model, bool Clear)
        {

            ViewBag.OperationMessage = TempData["Bunrui_Index"];

            var viewModel = new BunruiIndexModel();
            if (Clear == true)
            {
                viewModel.BunruiSearchConditions = new BunruiSearchConditions();
            }
            else
            {
                viewModel.BunruiSearchConditions = model.BunruiSearchConditions;
            }
            ModelState.Clear();

            HttpContext.Session.SetObject(SessionKeys.Bunrui_SEARCH_CONDITIONS, viewModel.BunruiSearchConditions);

            viewModel.Bunrui = _BunruiService.SearchBunruis(viewModel.BunruiSearchConditions);

            return View("Index", viewModel);
        }

        [HttpGet]
        public ActionResult Clear()
        {
            var viewModel = new BunruiIndexModel();
            viewModel.BunruiSearchConditions = new BunruiSearchConditions();
            return RedirectToAction("Search", "Bunrui");
        }

        public IActionResult Index()
        {

            ViewBag.OperationMessage = TempData["Bunrui_Index"];

            var viewModel = new BunruiIndexModel();
            if (HttpContext.Session.GetObject<BunruiSearchConditions>(SessionKeys.Bunrui_SEARCH_CONDITIONS) != null)
            {
                viewModel.BunruiSearchConditions = HttpContext.Session.GetObject<BunruiSearchConditions>(SessionKeys.Bunrui_SEARCH_CONDITIONS);

            }

            HttpContext.Session.SetObject(SessionKeys.Bunrui_SEARCH_CONDITIONS, viewModel.BunruiSearchConditions);


            viewModel.Bunrui = _BunruiService.SearchBunruis(viewModel.BunruiSearchConditions);

            return View("Index", viewModel);
        }
    }
}