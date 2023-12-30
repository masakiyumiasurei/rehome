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
    public class SiireController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<SiireController> _logger;
        private ISiireService _SiireService;
        private IDropDownListService _DropDownListService;

        private const int PAGE_SIZE = 20;

        public SiireController(ILogger<SiireController> logger, IConfiguration configuration, ISiireService SiireService, IDropDownListService DropDownListService)
        {
            _logger = logger;
            // appsettings.jsonファイルから接続文字列を取得
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _SiireService = SiireService;
            _DropDownListService = DropDownListService;
        }


        /// <summary>
        /// 新規登録画面表示時のアクション
        /// </summary>
        [HttpGet]
        public IActionResult New()
        {
            var viewModel = new SiireDetailModel();
            viewModel.Mode = ViewMode.New;
            viewModel.Siire = new 仕入先();
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
        public IActionResult Details(int 仕入先ID, string? BackUrl)
        {
            ViewBag.OperationMessage = TempData["Siire"];

            var viewModel = new SiireDetailModel();
            viewModel.Mode = ViewMode.Edit;
            viewModel.Siire = _SiireService.GetSiire(仕入先ID);

            if (BackUrl != null)
            {
                viewModel.BackUrl = BackUrl;
            }
            else if (Request.Headers["Referer"].Any())
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
        public ActionResult Details(SiireDetailModel model)
        {
            var viewModel = new SiireDetailModel();

            try
            {
                // 登録成功したら仕入先画面再表示
                viewModel.Siire = _SiireService.RegistSiire(model);
                viewModel.Mode = model.Mode;
                ViewBag.OperationMessage = String.Format("仕入先情報を登録しました");
                TempData["Siire"] = String.Format("仕入先情報を登録しました");
                ModelState.Clear();
                viewModel.BackUrl = model.BackUrl;
                return RedirectToAction("Details", "Siire", new { 仕入先ID = viewModel.Siire.仕入先ID,BackUrl= viewModel.BackUrl });
            }
            catch (Exception ex)
            {
       
                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Delete(SiireDetailModel model)
        {
            var viewModel = new SiireDetailModel();
            try
            {
                _SiireService.DeleteSiire(model.Siire.仕入先ID);

                TempData["Siire_Index"] = String.Format("仕入先情報を削除しました");

                return RedirectToAction("Index", "Siire");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");
                return View(model);
            }


        }

        /// サンプル一覧で検索ボタンクリック時のアクション
        public IActionResult Search(SiireIndexModel model, bool Clear)
        {

            ViewBag.OperationMessage = TempData["Siire_Index"];

            var viewModel = new SiireIndexModel();
            if (Clear == true)
            {
                viewModel.SiireSearchConditions = new SiireSearchConditions();
            }
            else
            {
                viewModel.SiireSearchConditions = model.SiireSearchConditions;
            }
            ModelState.Clear();

            HttpContext.Session.SetObject(SessionKeys.Siire_SEARCH_CONDITIONS, viewModel.SiireSearchConditions);

            viewModel.Siire = _SiireService.SearchSiires(viewModel.SiireSearchConditions);

            return View("Index", viewModel);
        }

        [HttpGet]
        public ActionResult Clear()
        {
            var viewModel = new SiireIndexModel();
            viewModel.SiireSearchConditions = new SiireSearchConditions();
            return RedirectToAction("Search", "Siire");
        }

        public IActionResult Index()
        {
            if (TempData["Siire_Index"]!=null)
            {
                ViewBag.OperationMessage = TempData["Siire_Index"];
            }

            var viewModel = new SiireIndexModel();
            if (HttpContext.Session.GetObject<SiireSearchConditions>(SessionKeys.Siire_SEARCH_CONDITIONS) != null)
            {
                viewModel.SiireSearchConditions = HttpContext.Session.GetObject<SiireSearchConditions>(SessionKeys.Siire_SEARCH_CONDITIONS);

            }

            HttpContext.Session.SetObject(SessionKeys.Siire_SEARCH_CONDITIONS, viewModel.SiireSearchConditions);


            viewModel.Siire = _SiireService.SearchSiires(viewModel.SiireSearchConditions);

            return View("Index", viewModel);
        }
    }
}