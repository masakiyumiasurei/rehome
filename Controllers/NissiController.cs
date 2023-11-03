using Microsoft.AspNetCore.Mvc;
using rehome.Enums;
using rehome.Http;
using rehome.Models;
using rehome.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using rehome.Models.DB;
using Microsoft.Data.SqlClient;
using System.Data;
using Pao.Reports;
using System.Diagnostics;
using System.Reflection;
using System;
using System.IO;
using rehome.Models.Nissi;
using System.Text;
using System.Globalization;

namespace rehome.Controllers
{
    //注意　開発中は認証機能をはずす
    [Authorize]
    public class NissiController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<NissiController> _logger;
        private INissiService _NissiService;
        private IClientService _ClientService;
        private ITantouService _TantouService;
        private IDropDownListService _DropDownListService;


        // private IFileUpdate _FileUpdate; 

        private const int PAGE_SIZE = 1000;

        public NissiController(ILogger<NissiController> logger, IConfiguration configuration,
             INissiService NissiService, IDropDownListService DropDownListService,  IClientService ClientService, ITantouService TantouService)

        {
            _logger = logger;
            // appsettings.jsonファイルから接続文字列を取得
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _NissiService = NissiService;
            _ClientService = ClientService;
            _DropDownListService = DropDownListService;
            _TantouService = TantouService;
        }

        //**************************************************** 相談日誌 ***********************************************************************

        [HttpGet]
        public IActionResult New(int? 顧客ID, int? 相談者ID)
        {
            var viewModel = new NissiDetailModel();
            viewModel.Mode = ViewMode.New;
            viewModel.Nissi = new 日誌();
            //viewModel.相談者DropDownList = _DropDownListService.Get相談者DropDownLists(顧客ID);
            //viewModel.相談内容DropDownList = _DropDownListService.Get相談内容DropDownLists();
            //viewModel.担当DropDownList = _DropDownListService.Get顧客担当DropDownLists(顧客ID);
            viewModel.Nissi.登録日 = DateTime.Today;
            //viewModel.顧客DropDownList = _DropDownListService.Get顧客DropDownLists();
            viewModel.Nissi.顧客ID = 顧客ID ?? null;
            var Client = _ClientService.GetClient(顧客ID ?? -1);
            viewModel.Nissi.顧客名 = Client.顧客名;


            if (Request.Headers["Referer"].Any())
            {
                viewModel.BackUrl = Request.Headers["Referer"].ToString();
            }
            return View("Details", viewModel);
        }

        [HttpGet]
        public IActionResult Details(int 日誌ID, string? BackUrl)
        {
            string OperationMsg = (string)TempData["Nissi_Details"];
            var viewModel = new NissiDetailModel();

            viewModel = _NissiService.GetNissi(日誌ID);
            //viewModel.相談者DropDownList = _DropDownListService.Get相談者DropDownLists(viewModel.Nissi.顧客ID);
            //viewModel.担当DropDownList = _DropDownListService.Get顧客担当DropDownLists(viewModel.Nissi.顧客ID);
            //viewModel.相談内容DropDownList = _DropDownListService.Get相談内容DropDownLists();
            viewModel.Mode = ViewMode.Edit;
            //viewModel.顧客DropDownList = _DropDownListService.Get顧客DropDownLists();

            if (BackUrl != null)
            {
                viewModel.BackUrl = BackUrl;
            }
            else if (Request.Headers["Referer"].Any())
            {
                viewModel.BackUrl = Request.Headers["Referer"].ToString();
            }

            if (OperationMsg == null)
            {
                OperationMsg = "";
            }
            ViewBag.OperationMessage = OperationMsg;
            return View(viewModel);
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        [AutoValidateAntiforgeryToken]
        public ActionResult Details(NissiDetailModel model)
        {
            var viewModel = new NissiDetailModel();
            try
            {
                if (model.Nissi.登録日 == null)
                {
                    model.Nissi.登録日 = DateTime.Today;
                }

                //model.Nissi.更新日 = DateTime.Today;
                //// 登録成功したら日誌画面再表示
                model.Nissi.日誌ID = _NissiService.RegistNissi(model);
                _NissiService.RegistNissiTantou(model);
                _NissiService.RegistNissiSodan(model);

                TempData["Nissi_Details"] = String.Format("日誌情報を登録しました");

                ModelState.Clear();
                viewModel.BackUrl = model.BackUrl;

                return RedirectToAction("Details", "Nissi", new { 日誌ID = model.Nissi.日誌ID, BackUrl = model.BackUrl });
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError("", "問題が発生しました。");
                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");

                if (model.Mode == ViewMode.New)
                {
                    //model.担当DropDownList = _DropDownListService.Get顧客担当DropDownLists(model.Nissi.顧客ID);
                    //model.相談者DropDownList = _DropDownListService.Get相談者DropDownLists(model.Nissi.顧客ID);
                    //model.相談内容DropDownList = _DropDownListService.Get相談内容DropDownLists();
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Details", "Nissi", new { 日誌ID = model.Nissi.日誌ID, BackUrl = model.BackUrl });
                }
            }
        }

        [HttpGet]
        public ActionResult DeleteNissi(int deleteID, string BackUrl, int? ClientID)
        {

            _NissiService.DeleteNissi(deleteID);

            try
            {
                // 削除成功したら一覧画面表示
                TempData["Nissi_Index"] = String.Format("日誌情報を削除しました");
                if (ClientID == null)
                {
                    return RedirectToAction("Index", "Nissi");
                }
                else
                {
                    return RedirectToAction("Detail", "Client", new { 顧客ID = ClientID });
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");
                if (ClientID == null)
                {
                    return RedirectToAction("Index", "Nissi");
                }
                else
                {
                    return RedirectToAction("Detail", "Client", new { 顧客ID = ClientID });
                }
            }
        }

        //********************************************** 個別支援日誌 **********************************************************

        [HttpGet]
        public IActionResult New_Kobetu(int? 顧客ID)
        {
            var viewModel = new NissiKobetuDetailModel();
            viewModel.Mode = ViewMode.New;
            //viewModel.Nissi = new 個別支援日誌();
            //viewModel.相談者DropDownList = _DropDownListService.Get相談者DropDownLists(顧客ID);
            //viewModel.相談内容DropDownList = _DropDownListService.Get相談内容DropDownLists();
            //viewModel.担当DropDownList = _DropDownListService.Get顧客担当DropDownLists(顧客ID);
            //viewModel.Nissi.登録日 = DateTime.Today;
            //viewModel.顧客DropDownList = _DropDownListService.Get顧客DropDownLists();
            //viewModel.Nissi.顧客ID = 顧客ID ?? null;
            var Client = _ClientService.GetClient(顧客ID ?? -1);
            //viewModel.Nissi.顧客名 = Client.顧客名;


            if (Request.Headers["Referer"].Any())
            {
                viewModel.BackUrl = Request.Headers["Referer"].ToString();
            }
            return View("Details_Kobetu", viewModel);
        }



        [HttpGet]
        public ActionResult DeleteNissiKobetu(int deleteID, string BackUrl, int? ClientID)
        {

            _NissiService.DeleteNissiKobetu(deleteID);

            try
            {
                // 削除成功したら一覧画面表示
                TempData["Nissi_Index"] = String.Format("日誌情報を削除しました");
                if (ClientID == null)
                {
                    return RedirectToAction("Index", "Nissi");
                }
                else
                {
                    return RedirectToAction("Detail", "Client", new { 顧客ID = ClientID });
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");
                if (ClientID == null)
                {
                    return RedirectToAction("Index", "Nissi");
                }
                else
                {
                    return RedirectToAction("Detail", "Client", new { 顧客ID = ClientID });
                }
            }
        }

        [HttpGet]
        public ActionResult DeleteNissiTokubetu(int deleteID, string BackUrl, int? ClientID)
        {

            _NissiService.DeleteNissiTokubetu(deleteID);

            try
            {
                // 削除成功したら一覧画面表示
                TempData["Nissi_Index"] = String.Format("日誌情報を削除しました");
                if (ClientID == null)
                {
                    return RedirectToAction("Index", "Nissi");
                }
                else
                {
                    return RedirectToAction("Detail", "Client", new { 顧客ID = ClientID });
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");
                if (ClientID == null)
                {
                    return RedirectToAction("Index", "Nissi");
                }
                else
                {
                    return RedirectToAction("Detail", "Client", new { 顧客ID = ClientID });
                }
            }
        }

        //*************************************** 一覧・検索 **************************************************

        [HttpGet]
        public ActionResult Clear()
        {
            var viewModel = new NissiIndexModel();
            viewModel.NissiSearchConditions = new NissiSearchConditions();

            return RedirectToAction("Index", "Nissi");
        }


        /// サンプル一覧で検索ボタンクリック時のアクション
        public IActionResult Search(NissiIndexModel model, bool Clear)
        {
            ViewBag.OperationMessage = TempData["Nissi_Index"];
            var viewModel = new NissiIndexModel();

            viewModel = _NissiService.SearchNissis(model.NissiSearchConditions);


            viewModel.NissiSearchConditions = model.NissiSearchConditions;

            ModelState.Clear();

            //viewModel.NissiSearchConditions.page = 1;
            HttpContext.Session.SetObject(SessionKeys.Nissi_SEARCH_CONDITIONS, viewModel.NissiSearchConditions);

            viewModel.印刷区分 = viewModel.NissiSearchConditions.支援区分;

            viewModel.担当DropDownList = _DropDownListService.Get担当DropDownLists();
            //viewModel.相談内容DropDownList = _DropDownListService.Get相談内容DropDownLists();

            return View("Index", viewModel);
        }

        public IActionResult Index()
        {
            ViewBag.OperationMessage = TempData["Nissi_Index"];
            //int pageNumber = page ?? 1;
            //if (pageNumber < 1) pageNumber = 1;
            var viewModel = new NissiIndexModel();


            //if (HttpContext.Session.GetObject<NissiSearchConditions>(SessionKeys.Nissi_SEARCH_CONDITIONS) != null)
            //{
            //    viewModel.NissiSearchConditions = HttpContext.Session.GetObject<NissiSearchConditions>(SessionKeys.Nissi_SEARCH_CONDITIONS);
            //    if (page == null) pageNumber = viewModel.NissiSearchConditions.page;
            //}
            //viewModel.NissiSearchConditions.page = pageNumber;
            //HttpContext.Session.SetObject(SessionKeys.Nissi_SEARCH_CONDITIONS, viewModel.NissiSearchConditions);



            // viewModel.Nissis = _NissiService.SearchNissis(viewModel.NissiSearchConditions);
            viewModel.担当DropDownList = _DropDownListService.Get担当DropDownLists();
            //viewModel.相談内容DropDownList = _DropDownListService.Get相談内容DropDownLists();

            return View("Index", viewModel);
        }

        }

}
