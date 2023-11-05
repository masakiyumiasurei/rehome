using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using rehome.Enums;
using rehome.Http;
using rehome.Models;
using rehome.Services;
using Microsoft.AspNetCore.Authorization;
using rehome.Models.DB;
using rehome.Models.Client;
using System.Security.Claims;
using System.Globalization;
using System.Text;

namespace rehome.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<ClientController> _logger;
        //private const int PAGE_SIZE = 20;
        private IClientService _ClientService;
        private INissiService _NissiService;
        private ITantouService _TantouService;
        private IDropDownListService _DropDownListService;
        //private readonly IFileUpdate _FileUpdate;

        public ClientController(ILogger<ClientController> logger, IConfiguration configuration, IClientService ClientService, IDropDownListService dropDownListService,INissiService NissiService, ITantouService TantouService)
        {
            _logger = logger;            
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _ClientService = ClientService;
            _NissiService = NissiService;
            _TantouService = TantouService;
            _DropDownListService = dropDownListService;
        }

        public ActionResult Detail( int? 顧客ID)
        {
            ViewBag.OperationMessage = TempData["Client"];
            ViewBag.errorMessage = TempData["error"];
            
            using var connection = new SqlConnection(_connectionString);

            var model = new ClientDetailModel();
            if (Request.Headers["Referer"].Any())
            {
                model.BackUrl = Request.Headers["Referer"].ToString();
            }

            if (顧客ID == null)//new処理
            {
                model.Client = new ();
                model.Mode = ViewMode.New;
                model.担当DropDownList = _DropDownListService.Get担当DropDownLists();
                //model.顧客担当者数 = 0;
                //model.相談者数 = 0;
              //  model.Client.登録担当ID = Int32.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                //var Tantou = _TantouService.GetTantou((int)model.Client.登録担当ID);
                //model.Client.登録担当者名 = Tantou.担当名;
            }
            else
            {//edit処理
                model.Mode = ViewMode.Edit;
                model.Client = _ClientService.GetClient(顧客ID ?? -1);//null許容でGetClientする処理が適正ではないので、Create呼ばれる際は絶対に値が入って呼ばれるようにする？
                //var Tantou = _TantouService.GetTantou(model.Client.登録担当ID ?? -1);
                //if(Tantou != null)
                //{
                //    model.Client.登録担当者名 = Tantou.担当名;
                //}
                //model.Nissi = _NissiService.GetNissis(顧客ID ?? -1);
                //model.Files = _ClientService.GetFiles(顧客ID ?? -1);
                //model.担当DropDownList = _DropDownListService.Get担当DropDownLists();
                //model.相談者DropDownList = _DropDownListService.Get相談者DropDownLists(顧客ID ?? -1);
                //model.顧客担当リスト = _ClientService.GetClientTantou(顧客ID ?? -1);
                //if (model.顧客担当リスト != null)
                //{
                //    model.顧客担当者数 = model.顧客担当リスト.Count();
                //}
                //else
                //{
                //    model.顧客担当者数 = 0;
                //}
                //model.相談者リスト = _ClientService.GetSodan(顧客ID ?? -1);
                //if (model.相談者リスト != null)
                //{
                //    model.相談者数 = model.相談者リスト.Count();
                //}
                //else
                //{
                //    model.相談者数 = 0;
                //}
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Detail(ClientDetailModel model)
        {
            var viewModel = new ClientDetailModel();
            try
            {
                // 登録成功したら導入画面再表示
                viewModel.Client = _ClientService.RegistClient(model);
                model.Client.顧客ID = viewModel.Client.顧客ID;                

                ////ファイル登録がある場合
                //if (model.PostedFile != null)
                //{
                //    foreach (var file in model.PostedFile)
                //    {
                //        _FileUpdate.UpdFile(file, viewModel.Client.顧客ID);
                //    }
                //}

                // 削除処理後は顧客一覧画面に戻る
                //if (model.Mode == ViewMode.Delete) { return RedirectToAction("Details", "Client", new { id = model.Client.顧客ID }); }
                viewModel.Mode = ViewMode.Edit;
                TempData["Client"] = String.Format("顧客情報を登録しました");
                ModelState.Clear();
                viewModel.BackUrl = model.BackUrl;
                return RedirectToAction("Detail", "Client", new {顧客ID = viewModel.Client.顧客ID});
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");

                TempData["error"] = (ex.Message).ToString();
                return RedirectToAction("Detail", "Client", new { 顧客ID = viewModel.Client.顧客ID });                
            }

        }

        [HttpPost]
        public IActionResult Delete(ClientDetailModel model)
        {
            
            try
            {
                _ClientService.DeleteClient(model.Client.顧客ID);

                TempData["Client_Index"] = String.Format("顧客情報を削除しました");

                return RedirectToAction("Index", "Client");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");

                string str = model.Client.顧客名 + "には、相談者、日報などの登録があるため削除できません\n" ;
                TempData["error"] = str +(ex.Message).ToString();
                    return RedirectToAction("Detail", "Client", new { 顧客ID = model.Client.顧客ID });
                
                //return View(model);
            }

        }

        public IActionResult Index(int? page)
        {

            ViewBag.OperationMessage = TempData["Client_Index"];

            var viewModel = new ClientIndexModel();
            viewModel.ClientSearchConditions = new ClientSearchConditions();
            if (HttpContext.Session.GetObject<ClientSearchConditions>(SessionKeys.CLIENT_SEARCH_CONDITIONS) != null)
            {
                viewModel.ClientSearchConditions = HttpContext.Session.GetObject<ClientSearchConditions>(SessionKeys.CLIENT_SEARCH_CONDITIONS);
                //if (page == null) pageNumber = viewModel.ClientSearchConditions.page;
            }
            //viewModel.ClientSearchConditions.page = pageNumber;
            HttpContext.Session.SetObject(SessionKeys.CLIENT_SEARCH_CONDITIONS, viewModel.ClientSearchConditions);


            viewModel.Clients = _ClientService.SearchClients(viewModel.ClientSearchConditions);
 

            return View("Index", viewModel);
        }

        [HttpGet]
        public ActionResult Clear()
        {
            var viewModel = new ClientIndexModel();
            viewModel.ClientSearchConditions = new ClientSearchConditions();
            return RedirectToAction("Search", "Client");
        }

        /// サンプル一覧で検索ボタンクリック時のアクション
        public IActionResult Search(ClientIndexModel model)
        {
            ViewBag.OperationMessage = TempData["Client_Index"];

            var viewModel = new ClientIndexModel();
            viewModel.ClientSearchConditions = model.ClientSearchConditions;
            ModelState.Clear();

            HttpContext.Session.SetObject(SessionKeys.CLIENT_SEARCH_CONDITIONS, viewModel.ClientSearchConditions);

            viewModel.Clients = _ClientService.SearchClients(viewModel.ClientSearchConditions);

                return View("Index", viewModel);
        }

        [HttpPost]
        public ActionResult GetIryoken(string city)
        {
            var result = "";

            foreach(var item in 医療圏リスト.医療圏comb())
            {
                if(city.Contains(item.Text)){
                    result = item.Value;
                }
            }
            return Json(result);            
        }


    }
}


