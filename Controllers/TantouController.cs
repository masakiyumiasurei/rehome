using Microsoft.AspNetCore.Mvc;
using rehome.Enums;
using rehome.Http;
using rehome.Models;
using rehome.Services;
using Microsoft.AspNetCore.Authorization;
using rehome.Models.DB;
using rehome.Public;
using Microsoft.Data.SqlClient;
using Dapper;
using ServiceStack;

namespace rehome.Controllers
{
    //注意
    [Authorize]
    public class TantouController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<TantouController> _logger;
        private ITantouService _TantouService;
        private IDropDownListService _DropDownListService;

        private const int PAGE_SIZE = 20;

        public TantouController(ILogger<TantouController> logger, IConfiguration configuration, ITantouService TantouService, IDropDownListService DropDownListService)
        {
            _logger = logger;
            // appsettings.jsonファイルから接続文字列を取得
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _TantouService = TantouService;
            _DropDownListService = DropDownListService;
        }


        /// <summary>
        /// 新規登録画面表示時のアクション
        /// </summary>
        [HttpGet]
        public IActionResult New()
        {
            var viewModel = new TantouDetailModel();
            viewModel.Mode = ViewMode.New;
            viewModel.Tantou = new 担当();
         //   viewModel.営業所DropDownList = _DropDownListService.Get営業所DropDownLists();
            viewModel.担当営業所数 = 0;
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
        public IActionResult Details(int 担当ID)
        {
            ViewBag.OperationMessage = TempData["Tantou"];

            var viewModel = new TantouDetailModel();
            //viewModel.営業所DropDownList = _DropDownListService.Get営業所DropDownLists();
            viewModel.Mode = ViewMode.Edit;
            viewModel.Tantou = _TantouService.GetTantou(担当ID);
            viewModel.担当営業所リスト = _TantouService.GetTantouOffice(担当ID);
            if (viewModel.担当営業所リスト != null)
            {
                viewModel.担当営業所数 = viewModel.担当営業所リスト.Count();
            }
            else
            {
                viewModel.担当営業所数 = 0;
            }
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
        public ActionResult Details(TantouDetailModel model)
        {
            var viewModel = new TantouDetailModel();

            if(model.Mode == ViewMode.New)
            {
                model.Tantou.pass = PublicClass.GetSHA256HashedString(model.Tantou.pass);
            }
            else if(model.Tantou.new_pass != null)
            {
                model.Tantou.pass = PublicClass.GetSHA256HashedString(model.Tantou.new_pass);
            }

            try
            {
                var connection = new SqlConnection(_connectionString);
                connection.Open();
                var sql = "";
                sql = "select count(*) from T_担当 where loginID=@loginID and 担当ID<>@担当ID";
                var count = connection.QuerySingleOrDefault<int>(sql, model.Tantou);

                if (count > 0)
                {
                    //ViewBag.OperationMessage = string.Format("重複しているIDがあります。IDを修正してください");
                    if (model.Mode == ViewMode.New)
                    {
                        //model.営業所DropDownList = _DropDownListService.Get営業所DropDownLists();
                        return View(model);
                    }
                    else
                    {
                        TempData["Tantou"] = String.Format("ログインID：" + model.Tantou.loginID + "は重複しています。IDを修正してください");                        
                        return RedirectToAction("Details", "Tantou", new { model.Tantou.担当ID });
                    }
                }

                // 登録成功したら担当画面再表示
                viewModel.Tantou = _TantouService.RegistTantou(model);
                model.Tantou.担当ID = viewModel.Tantou.担当ID;
                _TantouService.RegistTantouOffice(model);
                viewModel.Mode = model.Mode;
                TempData["Tantou"] = String.Format("担当情報を登録しました");
                ModelState.Clear();
                viewModel.BackUrl = model.BackUrl;
                
                return RedirectToAction("Details", "Tantou", new { 担当ID = viewModel.Tantou.担当ID });
            }
            catch (Exception ex)
            {       
                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Delete(TantouDetailModel model)
        {
            var viewModel = new TantouDetailModel();
            try
            {
                _TantouService.DeleteTantou(model.Tantou.担当ID);

                TempData["Tantou_Index"] = String.Format("担当情報を削除しました");

                return RedirectToAction("Index", "Tantou");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");
                return View(model);
            }


        }

        /// サンプル一覧で検索ボタンクリック時のアクション
        public IActionResult Search(TantouIndexModel model, bool Clear)
        {

            ViewBag.OperationMessage = TempData["Tantou_Index"];

            var viewModel = new TantouIndexModel();
            if (Clear == true)
            {
                viewModel.TantouSearchConditions = new TantouSearchConditions();
            }
            else
            {
                viewModel.TantouSearchConditions = model.TantouSearchConditions;
            }
            ModelState.Clear();

           // viewModel.営業所DropDownList = _DropDownListService.Get営業所DropDownLists();

            HttpContext.Session.SetObject(SessionKeys.Tantou_SEARCH_CONDITIONS, viewModel.TantouSearchConditions);

            viewModel.Tantous = _TantouService.SearchTantous(viewModel.TantouSearchConditions);

            return View("Index", viewModel);
        }

        [HttpGet]
        public ActionResult Clear()
        {
            var viewModel = new TantouIndexModel();
            viewModel.TantouSearchConditions = new TantouSearchConditions();
            return RedirectToAction("Search", "Tantou");
        }

        public IActionResult Index()
        {

            ViewBag.OperationMessage = TempData["Tantou_Index"];

            var viewModel = new TantouIndexModel();
            if (HttpContext.Session.GetObject<TantouSearchConditions>(SessionKeys.Tantou_SEARCH_CONDITIONS) != null)
            {
                viewModel.TantouSearchConditions = HttpContext.Session.GetObject<TantouSearchConditions>(SessionKeys.Tantou_SEARCH_CONDITIONS);

            }

            HttpContext.Session.SetObject(SessionKeys.Tantou_SEARCH_CONDITIONS, viewModel.TantouSearchConditions);

           // viewModel.営業所DropDownList = _DropDownListService.Get営業所DropDownLists();

            viewModel.Tantous = _TantouService.SearchTantous(viewModel.TantouSearchConditions);

            return View("Index", viewModel);
        }
    }
}