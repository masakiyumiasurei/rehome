using Microsoft.AspNetCore.Mvc;
using rehome.Enums;
using rehome.Http;
using rehome.Models;
using rehome.Services;
using Microsoft.AspNetCore.Authorization;
using rehome.Models.DB;
using rehome.Public;
using System.Security.Claims;
using Pao.Reports;
using System.Security.Cryptography.X509Certificates;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;

namespace rehome.Controllers
{
    [Authorize]
    public class SyouhinController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<SyouhinController> _logger;
        private ISyouhinService _SyouhinService;
        private IDropDownListService _DropDownListService;
        private object _SyainService;
        private const int PAGE_SIZE = 500;

        public SyouhinController(ILogger<SyouhinController> logger, IConfiguration configuration, 
            ISyouhinService SyouhinService, IDropDownListService dropDownListService)
        {
            _logger = logger;
            // appsettings.jsonファイルから接続文字列を取得
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _SyouhinService = SyouhinService;
            _DropDownListService = dropDownListService;
        }

        /// <summary>
        /// 新規登録画面表示時のアクション
        /// </summary>
        [HttpGet]
        public IActionResult New()
        {
            var viewModel = new SyouhinDetailModel();
            viewModel.Mode = ViewMode.New;
            viewModel.Syouhin = new 商品();
            viewModel.分類DropDownList = _DropDownListService.Get分類DropDownLists();
            viewModel.auth = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value);

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
        public IActionResult Details(int 商品ID)
        {
            var viewModel = new SyouhinDetailModel();
            viewModel.Mode = ViewMode.Edit;
            viewModel.Syouhin = _SyouhinService.GetSyouhin(商品ID);
            viewModel.分類DropDownList = _DropDownListService.Get分類DropDownLists();
            viewModel.auth = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value);

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
        [AutoValidateAntiforgeryToken]
        public ActionResult Details(SyouhinDetailModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var viewModel = new SyouhinDetailModel();
            try
            {
                ModelState.Clear();
                int RegistID = _SyouhinService.RegistSyouhin(model);
                String message = String.Format("商品情報を更新しました");
                ViewBag.OperationMessage = message;

                // 登録成功したら商品画面再表示
                viewModel.Syouhin = _SyouhinService.GetSyouhin(RegistID);
                viewModel.Mode = model.Mode;
                model.auth = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value);
                viewModel.分類DropDownList = _DropDownListService.Get分類DropDownLists();
                viewModel.BackUrl = model.BackUrl;
                ModelState.Clear();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError("", "問題が発生しました。");
                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");
                model.auth = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value);
                model.分類DropDownList = _DropDownListService.Get分類DropDownLists();
                return View("Details",model);
            }
            //viewModel.Syouhin = _SyouhinService.GetSyouhin(model.Syouhin.商品ID);
            //return View(viewModel);
        }

        /// サンプル一覧で検索ボタンクリック時のアクション
        public IActionResult Search(SyouhinIndexModel model)
        {
            model.SyouhinSearchConditions.page = 1;
            HttpContext.Session.SetObject(SessionKeys.Syouhin_SEARCH_CONDITIONS, model.SyouhinSearchConditions);

            var viewModel = new SyouhinIndexModel();
            viewModel.SyouhinSearchConditions = model.SyouhinSearchConditions;
            viewModel.Syouhins = _SyouhinService.SearchSyouhins(model.SyouhinSearchConditions, PAGE_SIZE);
            return View("Index", viewModel);
        }

        public IActionResult Index(int? page)
        {
            int pageNumber = page ?? 1;
            if (pageNumber < 1) pageNumber = 1;
            var viewModel = new SyouhinIndexModel();
            if (HttpContext.Session.GetObject<SyouhinSearchConditions>(SessionKeys.Syouhin_SEARCH_CONDITIONS) != null)
            {
                viewModel.SyouhinSearchConditions = HttpContext.Session.GetObject<SyouhinSearchConditions>(SessionKeys.Syouhin_SEARCH_CONDITIONS);
                if (page == null) pageNumber = viewModel.SyouhinSearchConditions.page;
            }
            viewModel.SyouhinSearchConditions.page = pageNumber;
            HttpContext.Session.SetObject(SessionKeys.Syouhin_SEARCH_CONDITIONS, viewModel.SyouhinSearchConditions);


            viewModel.Syouhins = _SyouhinService.SearchSyouhins(viewModel.SyouhinSearchConditions, PAGE_SIZE);

            return View("Index", viewModel);
        }

        /// サンプル一覧で検索ボタンクリック時のアクション
        [HttpPost]
        public ActionResult SearchAjax(string? SyouhinID, string? SyouhinName, string? SyouhinMaker, string? SyouhinNumber)
        {
            var viewModel = new SyouhinDetailModel();

            if (SyouhinID != null) viewModel.AjaxSearchConditions.商品ID = Int32.Parse(SyouhinID);
            if (SyouhinName != null) viewModel.AjaxSearchConditions.商品名 = SyouhinName;
            if (SyouhinMaker != null) viewModel.AjaxSearchConditions.メーカー名 = SyouhinMaker;
            if (SyouhinNumber != null) viewModel.AjaxSearchConditions.品番 = SyouhinNumber;
            viewModel.ListSyouhins = _SyouhinService.SearchAjaxSyouhins(viewModel.AjaxSearchConditions);

            return PartialView("_SearchResult", viewModel);
            
        }
       
        
        public IActionResult ExportKakaku(string rank)
        {
            List<商品> items = _SyouhinService.GetKakaku(rank);

            MemoryStream stream = new MemoryStream();
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "価格表" };
                sheets.Append(sheet);

                SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                // ヘッダ行の設定
                Row headerRow = new Row();
                
                headerRow.Append(
                new Cell() { CellValue = new CellValue("品番"), DataType = CellValues.String },
                new Cell() { CellValue = new CellValue("商品名"), DataType = CellValues.String },
                new Cell() { CellValue = new CellValue("カタログ"), DataType = CellValues.String },
                new Cell() { CellValue = new CellValue("定価"), DataType = CellValues.String },
                new Cell() { CellValue = new CellValue("推奨売価"), DataType = CellValues.String },
                new Cell() { CellValue = new CellValue("仕切価格"), DataType = CellValues.String },
                new Cell() { CellValue = new CellValue("保証"), DataType = CellValues.String });

                sheetData.Append(headerRow);

                // データ行の設定
                foreach (var item in items)
                {
                    Row dataRow = new Row();                    
                        
                    dataRow.Append(                        
                        new Cell() { CellValue = new CellValue(item.商品名.ToString()), DataType = CellValues.String },                        
                        new Cell() { CellValue = new CellValue(item.単価.ToString()), DataType = CellValues.String },
                        new Cell() {
                            CellValue = new CellValue(item.openFLG ? "OPEN" : ""),
                            DataType = CellValues.String
                        },
                        new Cell() { CellValue = new CellValue(item.仕切価格.ToString()), DataType = CellValues.String },
                       new Cell() { CellValue = new CellValue(item.保証.ToString()), DataType = CellValues.String }
                        );
                    
                    sheetData.Append(dataRow);
                }

                workbookPart.Workbook.Save();
            }

            stream.Position = 0;
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "価格表.xlsx");
        }
    




    public ActionResult 価格表印刷()
        {
            //pdfフォルダ内に保存してあるファイルを全削除
            foreach (string pathFrom in Directory.EnumerateFiles(Path.Combine(Directory.GetCurrentDirectory(), @"pdf"), "*.pdf", SearchOption.AllDirectories))
            {
                System.IO.File.Delete(pathFrom);
            }

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            //インスタンス化
            IReport paoRep = ReportCreator.GetPdf();
            paoRep.LoadDefFile("Reports/価格表.prepd");
            
             List<商品> Items = _SyouhinService.GetKakaku("");

            //明細が0行でも1ページは表示するため初期値は44にしておく
            int RowCnt ;

            if (Items != null)
            {
                RowCnt = Items.Count();
            }
            else
            {
                RowCnt = 45;
            }
           
            //現在の行
            int CurRow = 0;

                //描画すべき行がある限りページを増やす
                while (RowCnt > 0)
                {
                    RowCnt -= 45;

                    paoRep.PageStart();

                    //ボディ
                    //空の明細行を43行分用意する
                    for (int i = 0; i < 45; i++)
                    {

                        paoRep.Write("品番", " ", i + 1);
                        paoRep.Write("品名", " ", i + 1);
                        paoRep.Write("カタログ", " ", i + 1);
                        paoRep.Write("定価", " ", i + 1);
                        paoRep.Write("推奨売価", " ", i + 1);
                        paoRep.Write("仕切価格", " ", i + 1);
                        paoRep.Write("保証", " ", i + 1);                        
                    }

                    if (Items != null)
                    {
                        for (var i = 0; i < 45; i++)
                        {
                            if (CurRow >= Items.Count())
                            {
                            //描画すべき行がなくなれば、ループを抜ける
                             break;
                            }

                            
                            paoRep.Write("品名", Items[CurRow].商品名 ?? " ", i + 1);
                            paoRep.Write("カタログ", string.Format("{0:#,0}", Items[CurRow].カタログ ?? 0), i + 1);
                        paoRep.Write("定価", Items[CurRow].openFLG ? "OPEN" :
                            (Items[CurRow].openFLG == false ? string.Format("{0:#,0}", Items[CurRow].単価 ?? 0)
                            : string.Format("{0:#,0}", Items[CurRow].単価 ?? 0)), i + 1);
                        paoRep.Write("推奨売価", string.Format("{0:#,0}", Items[CurRow].単価 ?? 0), i + 1);
                            paoRep.Write("仕切価格", string.Format("{0:#,0}", Items[CurRow].仕切価格 ?? 0), i + 1);
                            paoRep.Write("保証", string.Format("{0:#,0}", Items[CurRow].保証 ?? 0), i + 1);

                            CurRow++;
                          
                        }
                    }
                    paoRep.PageEnd();
                }
            
        
        //PDFを一時保存
        string pdfPath = Path.Combine(Directory.GetCurrentDirectory(), @"pdf/価格表" + DateTime.Now.ToString("yyMMddHHmm") + ".pdf");
            paoRep.SavePDF(pdfPath);

            //保存したPDFをfsに格納
            var fs = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);
            var contentType = "application/pdf";

            //ブラウザにPDFを表示
            return new FileStreamResult(fs, contentType);

            //PDFをダウンロード
            //return File(fs, contentType, "価格表.pdf");
        }

    }
}