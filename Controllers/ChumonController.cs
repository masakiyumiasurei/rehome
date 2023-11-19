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
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
//using System.Management;

namespace rehome.Controllers
{
    [Authorize]
    public class ChumonController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<QuoteController> _logger;
        //private const int PAGE_SIZE = 20;
        private IChumonService _ChumonService;
        private IQuoteService _QuoteService;
        private IDropDownListService _DropDownListService;
        private IHouzinService _HouzinService;
        private IOfficeService _OfficeService;

        public ChumonController(ILogger<ChumonController>? logger, IConfiguration configuration,
            IChumonService ChumonService, IDropDownListService dropDownListService, IQuoteService QuoteService, IHouzinService HouzinService, IOfficeService OfficeService)
        {
            //_logger = logger;
            // appsettings.jsonファイルから接続文字列を取得
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _ChumonService = ChumonService;
            _DropDownListService = dropDownListService;
            _QuoteService = QuoteService;
            _HouzinService = HouzinService;
            _OfficeService = OfficeService;
        }


        [HttpGet]
        public ActionResult Create(int 見積ID, int 履歴番号 )　//新規オープン
        {
            using var connection = new SqlConnection(_connectionString);

            ViewBag.OperationMessage = (string)TempData["Chumon"];

            ChumonCreateModel model = new ChumonCreateModel();            
            model.Chumon = new 注文();

            //var viewModel = new ChumonIndexModel();
            //viewModel.Chumons = _ChumonService.SearchChumon(見積ID, 履歴番号);

            //案件に既に注文レコードがある場合は最初の注文レコードと同じ値を取得する
            //同じ注文が多いため注文レコードをコピーする（bloom仕様）
            //if ( viewModel.Chumons.Count>0 ) {
            //    int minOrderID = viewModel.Chumons.Min(a => a.注文ID);

            //    model = _ChumonService.CopyChumon(minOrderID);
            model.Chumon.枝番 = _ChumonService.GetMaxeda(見積ID, 履歴番号);
            //}
            //else
            //{
              //  model.Chumon = new 注文();
              //  model.Chumon.枝番 = 1;
            //}
            

            if (Request.Headers["Referer"].Any())
            {
                model.BackUrl = Request.Headers["Referer"].ToString();
            }
            
            model.Quote = _QuoteService.GetQuote(見積ID, 履歴番号);
            model.仕入先DropDownList = _DropDownListService.Get仕入先DropDownLists();

            IList<string> 注文担当リスト = (IList<string>)_DropDownListService.Get注文担当DropDownLists();
            string[] 注文担当itemsArray = 注文担当リスト.ToArray();
           // anylist.注文担当items = string.Join(",", 注文担当itemsArray);

            model.Mode = ViewMode.New;
                model.RowCount = 0;
            
            return View("Create",model);
        }

        public ActionResult Update(int 注文ID) //更新時オープン
        {
            using var connection = new SqlConnection(_connectionString);
            ViewBag.OperationMessage = (string)TempData["Chumon"];

                var model = new ChumonCreateModel();

                model = _ChumonService.GetChumon(注文ID);

                if (Request.Headers["Referer"].Any())
                {
                    model.BackUrl = Request.Headers["Referer"].ToString();
                }

            model.Quote = _QuoteService.GetQuote(model.Chumon.見積ID, model.Chumon.履歴番号);
            model.仕入先DropDownList = _DropDownListService.Get仕入先DropDownLists();

            IList<string> 注文担当リスト = (IList<string>)_DropDownListService.Get注文担当DropDownLists();
            
            // 氏名とtelを結合して、カンマ区切りの文字列配列を作成
            string[] 注文担当itemsArray = 注文担当リスト.ToArray();
            
            // 配列をカンマ区切りの文字列に変換して、相談手段itemsに代入
            //anylist.注文担当items = string.Join(",", 注文担当itemsArray);

            model.Mode = ViewMode.Edit;

            if (model.ChumonMeisai != null)
                {
                    model.RowCount = model.ChumonMeisai.Count();
                }
                else
                {
                    model.RowCount = 0;
                }
                return View("Create",model);
        }


        [HttpPost]
        public IActionResult Create(ChumonCreateModel model)　　//登録時
        {
            var viewModel = new ChumonCreateModel();
            try
            {
                viewModel = _ChumonService.RegistChumon(model);
                viewModel.RowCount = viewModel.ChumonMeisai.Count();
                viewModel.Mode = ViewMode.Edit;
                if (viewModel.RowCount==0)
                {
                    TempData["Chumon"] = String.Format("注文登録しました。注文明細情報を登録してください");
                }
                else
                {
                    TempData["Chumon"] = String.Format("注文情報を登録しました。");
                }

                ModelState.Clear();
                viewModel.BackUrl = model.BackUrl;
                return RedirectToAction("Update", "Chumon", new { 注文ID = viewModel.Chumon.注文ID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");
                if (model.Chumon.注文ID == 0)
                {
                    return RedirectToAction("Create", "Chumon", new { 見積ID = model.Quote.見積ID, 履歴番号 = model.Quote.履歴番号 });
                }
                else
                {
                    return RedirectToAction("Update", "Chumon", new { 注文ID = viewModel.Chumon.注文ID });
                }
            }
        }

        public IActionResult SaveTekiyo(ChumonIndexModel model)
        {
            var viewModel = new ChumonIndexModel();
            try
            {
                viewModel = _ChumonService.CalcChumon(model.Quote.見積ID, model.Quote.履歴番号);                
                viewModel.Chumons = _ChumonService.SearchChumon(model.Quote.見積ID, model.Quote.履歴番号);
                viewModel.Quote = _QuoteService.GetQuote(model.Quote.見積ID, model.Quote.履歴番号);

                _QuoteService.RegistQuoteTekiyo(model.Quote.見積ID, model.Quote.履歴番号, model.Quote.注文摘要);

                ViewBag.OperationMessage = String.Format("売上適用を登録しました");
               // TempData["Chumon_Index"] = String.Format("売上適用を登録しました");

                return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                
                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");
                return View("Index", viewModel);

            }
        }

        [HttpPost]
        public IActionResult Delete(ChumonCreateModel model)
        {
            var viewModel = new ChumonCreateModel();
            try
            {
                _ChumonService.DeleteChumon(model.Chumon.注文ID);

                TempData["Chumon_Index"] = String.Format("注文明細を削除しました");

                return RedirectToAction("Index", "Chumon", new { model.Quote.見積ID,  model.Quote.履歴番号 });
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");
                return View(model);
            }
        }


        public IActionResult Index(int 見積ID,int 履歴番号)
    {

            ViewBag.OperationMessage = (string)TempData["Chumon_Index"];

            var viewModel = new ChumonIndexModel();
            //if (HttpContext.Session.GetObject<QuoteSearchConditions>(SessionKeys.QUOTE_SEARCH_CONDITIONS) != null)
            //{
            //    viewModel.QuoteSearchConditions = HttpContext.Session.GetObject<QuoteSearchConditions>(SessionKeys.QUOTE_SEARCH_CONDITIONS);
            //    //if (page == null) pageNumber = viewModel.QuoteSearchConditions.page;
            //}
            //viewModel.QuoteSearchConditions.page = pageNumber;
            //HttpContext.Session.SetObject(SessionKeys.QUOTE_SEARCH_CONDITIONS, viewModel.QuoteSearchConditions);

            //viewModel.QuoteSearchConditions.LoginID = Int32.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
 
            viewModel = _ChumonService.CalcChumon(見積ID, 履歴番号);
            viewModel.Chumons = _ChumonService.SearchChumon( 見積ID,  履歴番号);
            viewModel.Quote= _QuoteService.GetQuote(見積ID, 履歴番号);
            viewModel.仕入先DropDownList = _DropDownListService.Get仕入先DropDownLists();

            if (Request.Headers["Referer"].Any())
            {
                viewModel.BackUrl = Request.Headers["Referer"].ToString();
            }

            return View("Index", viewModel);
        }

        [HttpGet]
        public ActionResult Clear()
        {
            var viewModel = new QuoteIndexModel();
            viewModel.QuoteSearchConditions = new QuoteSearchConditions();
            return RedirectToAction("Search", "Quote");
        }


        public FileStreamResult 注文書印刷(int 注文ID)
        {            
            ChumonCreateModel ChumonRep = new ChumonCreateModel();
            ChumonRep = _ChumonService.GetChumon(注文ID);
            ChumonRep.Quote = _QuoteService.GetQuote(ChumonRep.Chumon.見積ID,ChumonRep.Chumon.履歴番号);
            法人 Houzin = _HouzinService.GetHouzin();
            営業所 Office = _OfficeService.GetOffice(ChumonRep.Quote.営業所ID ?? -1);

            //pdfフォルダ内に保存してあるファイルを全削除
            foreach (string pathFrom in System.IO.Directory.EnumerateFiles(Path.Combine(Directory.GetCurrentDirectory(), @"pdf"), "*.pdf", System.IO.SearchOption.AllDirectories))
            {
                System.IO.File.Delete(pathFrom);
            }

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            IReport paoRep = ReportCreator.GetPdf();
            paoRep.LoadDefFile("Reports/注文書.prepd");

            //明細が0行でも1ページは表示するため初期値は25にしておく
            int RowCnt = 5;

            if (ChumonRep.ChumonMeisai != null)
            {
                RowCnt = ChumonRep.ChumonMeisai.Count();
            }

            //現在の行
            int CurRow = 0;
            
            //描画すべき行がある限りページを増やす
            while (RowCnt > 0)
            {
                RowCnt -= 5;

                paoRep.PageStart();

                //ヘッダー
                paoRep.Write("見積番号", "No." + (ChumonRep.Quote.見積番号 ?? " "));
                if (ChumonRep.Quote.作成日 != null)
                {
                    paoRep.Write("作成日", string.Format("{0:yyyy年M月d日}", ChumonRep.Quote.作成日));
                }
                else
                {
                    paoRep.Write("作成日", " ");
                }
                paoRep.Write("仕入先名", ChumonRep.Chumon.仕入先名 ?? ChumonRep.Chumon.自由仕入先名 ?? " ");
                paoRep.Write("納期", ChumonRep.Chumon.納期 ?? " ");                

                //フッダー
                int zei = (int)(ChumonRep.Chumon.金額 * 0.1);

                paoRep.Write("金額", string.Format("{0:#,0}", ChumonRep.Chumon.金額));
                paoRep.Write("消費税額", string.Format("{0:#,0}", zei));
                paoRep.Write("納入先", ChumonRep.Chumon.納入先 ?? " ");
                paoRep.Write("納期", ChumonRep.Chumon.納期 ?? " ");
                paoRep.Write("担当者名", ChumonRep.Chumon.弊社担当者 ?? " ");
                paoRep.Write("支払日", ChumonRep.Chumon.支払日 ?? " ");
                paoRep.Write("支払締日", ChumonRep.Chumon.支払締め ?? " ");

                paoRep.Write("合計", "￥" + string.Format("{0:#,0}", (ChumonRep.Chumon.金額 + zei)));

                
                paoRep.Write("社名", Houzin.社名 ?? " ");
                paoRep.Write("代表名", "代表取締役　" + (Houzin.代表名 ?? " "));
                paoRep.Write("郵便番号", "〒" + (Office.郵便番号 ?? " "));
                paoRep.Write("住所", Office.住所 ?? " ");
                paoRep.Write("TEL", "TEL " + (Office.TEL ?? " ") + "　FAX " + (Office.FAX ?? " "));
                paoRep.Write("オフィス", "オフィス " + (Houzin.オフィス ?? " "));


                //        //ボディ

                //空の明細行を3行分用意する
                for (int i = 0; i < 5; i++)
                {
                    paoRep.Write("件名", " ", i + 1);
                    paoRep.Write("摘要", " ", i + 1);
                    paoRep.Write("数量", " ", i + 1);
                    paoRep.Write("単価", " ", i + 1);
                    paoRep.Write("金額計", " ", i + 1);
                }

                for (var i = 0; i < 5; i++)
                {
                    //描画すべき行がなくなれば、ループを抜ける 
                   if (CurRow >= ChumonRep.ChumonMeisai.Count())                    
                    {
                        break;
                    }

                    paoRep.Write("件名", ChumonRep.ChumonMeisai[CurRow].件名, i + 1);
                    paoRep.Write("摘要", ChumonRep.ChumonMeisai[CurRow].摘要, i + 1);
                    paoRep.Write("数量", ChumonRep.ChumonMeisai[CurRow].数量.ToString(), i + 1);
                    paoRep.Write("単価", string.Format("{0:#,0}", (ChumonRep.ChumonMeisai[CurRow].単価 ?? 0)), i + 1);
                    paoRep.Write("金額計", string.Format("{0:#,0}", (ChumonRep.ChumonMeisai[CurRow].金額計 ?? 0)), i + 1);
                                        
                    CurRow++;
                }
                paoRep.PageEnd();
            }

            //PDFを一時保存
            string pdfPath = Path.Combine(Directory.GetCurrentDirectory(), @"pdf/注文書" + DateTime.Now.ToString("yyMMddHHmm") + ".pdf");
            paoRep.SavePDF(pdfPath);

            //保存したPDFをfsに格納
            var fs = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);
            var contentType = "application/pdf";

            //ブラウザにPDFを表示
            return new FileStreamResult(fs, contentType);

        }

        
        public FileStreamResult 売上表印刷(int 見積ID, int 履歴番号, string hyoudai)
        {
            ChumonIndexModel ChumonRep = new ChumonIndexModel();

            ChumonRep = _ChumonService.CalcChumon(見積ID, 履歴番号);
            ChumonRep.Chumons = _ChumonService.SearchChumon(見積ID, 履歴番号);
            ChumonRep.Quote = _QuoteService.GetQuote(見積ID, 履歴番号);

            int 小計 = (int)((ChumonRep.Quote.見積金額 ?? 0) - (ChumonRep.Quote.値引額 ?? 0));
            int 消費税額 = (int)(小計 * 0.1);
            int 非課税額 = (int)(ChumonRep.Quote.非課税額 ?? 0);
            int 税抜収入 = 小計 + 非課税額;
            int 税込収入 = 税抜収入 + 消費税額;
            int A合計税別 = 税抜収入;
            int A合計税込 = 税込収入;
            int B合計税別 = ChumonRep.金額計 ?? 0;
            int B合計税込 = (int)(B合計税別 * 1.1);
            int C合計税別 = A合計税別 - B合計税別;
            int C合計税込 = A合計税込 - B合計税込;
            decimal 粗利益率;
            if (A合計税別 > 0)
            {
                 粗利益率 = ((decimal)C合計税別 / (decimal)A合計税別);
            }
            else
            {
                 粗利益率 = 0;
            }
            //pdfフォルダ内に保存してあるファイルを全削除
            foreach (string pathFrom in System.IO.Directory.EnumerateFiles(Path.Combine(Directory.GetCurrentDirectory(), @"pdf"), "*.pdf", System.IO.SearchOption.AllDirectories))
            {
                System.IO.File.Delete(pathFrom);
            }

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            IReport paoRep = ReportCreator.GetPdf();
            paoRep.LoadDefFile("Reports/売上表.prepd");

            int MaxRow = 20;

            //明細が0行でも1ページは表示するため初期値は20にしておく
            int RowCnt = MaxRow;

            if (ChumonRep.Chumons != null)
            {
                RowCnt = ChumonRep.Chumons.Count();
            }

            //現在の行
            int CurRow = 0;

            //通し番号
            int num = 1;


            //描画すべき行がある限りページを増やす
            while (RowCnt > 0)
            {
                RowCnt -= MaxRow;

                paoRep.PageStart();


                //ヘッダー
                string title="";

                title = (hyoudai ?? "");



                
                if ( ChumonRep.Quote.理化学医療区分=="医療")
                {
                    title = title + "(" + (ChumonRep.Quote.理化学医療区分 ?? "") + ")";
                }
                else
                {
                    title = title + "(" + (ChumonRep.Quote.営業所名 ?? "") + ")";
                }

                paoRep.Write("表題", title);


                paoRep.Write("見積番号", "No." + (ChumonRep.Quote.見積番号 ?? " "));
                if (ChumonRep.Quote.完了予定日 != null)
                {
                    paoRep.Write("完了日", "完了日 " + string.Format("{0:yyyy年M月d日}", ChumonRep.Quote.完了予定日));
                }
                else
                {
                    paoRep.Write("完了日", "完了日未定");
                }
                paoRep.Write("件名", (ChumonRep.Quote.件名 ?? " "));
                paoRep.Write("項目", (ChumonRep.Quote.項目 ?? " "));
                paoRep.Write("受注先", (ChumonRep.Quote.顧客名 ?? " "));
                paoRep.Write("収入摘要", (ChumonRep.Quote.注文摘要 ?? ChumonRep.Quote.件名 ?? " "));
                paoRep.Write("税抜収入", string.Format("{0:#,0}", 税抜収入));
                paoRep.Write("税込収入", string.Format("{0:#,0}", 税込収入));

               string str = "";
               if (ChumonRep.Quote.入金締日 == null)
                {
                    str = " ";
                }
                else
                {
                    str = string.Format("{0:M月d日}", ChumonRep.Quote.入金締日);
                }
                paoRep.Write("収入締日", str);

                if (ChumonRep.Quote.入金日==null)
                {
                    str = str + " ";
                }
                else
                {
                    str = string.Format("{0:M月d日}", ChumonRep.Quote.入金日);
                }
                paoRep.Write("収入日", str);
                paoRep.Write("A合計税別", string.Format("{0:#,0}", A合計税別));
                paoRep.Write("A合計税込", string.Format("{0:#,0}", A合計税込));

                //フッダー
                paoRep.Write("B合計税別", string.Format("{0:#,0}", B合計税別));
                paoRep.Write("B合計税込", string.Format("{0:#,0}", B合計税込));
                paoRep.Write("C合計税別", string.Format("{0:#,0}", C合計税別));
                paoRep.Write("C合計税込", string.Format("{0:#,0}", C合計税込));
                paoRep.Write("粗利益率", string.Format("{0:0.0%}", 粗利益率));

                //ボディ

                //空の明細行を設定した行分用意する
                for (int i = 0; i < MaxRow; i++)
                {
                    paoRep.Write("枝番", num.ToString(), i+1);
                    paoRep.Write("支出先", " ", i + 1);
                    paoRep.Write("支出摘要", " ", i + 1);
                    paoRep.Write("税抜支出"," ", i + 1);
                    paoRep.Write("税込支出", " ", i + 1);
                    paoRep.Write("支出締日", " ", i + 1);
                    paoRep.Write("支払日", " ", i + 1);
                    num++;
                }
                num -= MaxRow;

                if (ChumonRep.Chumons != null)
                {

                    for (var i = 0; i < MaxRow; i++)
                    {
                        if (CurRow >= ChumonRep.Chumons.Count())
                        {
                            break;
                        }

                        paoRep.Write("枝番", num.ToString(), i + 1);
                        paoRep.Write("支出先", ChumonRep.Chumons[CurRow].自由仕入先名 ?? ChumonRep.Chumons[CurRow].仕入先名 ?? " ", i + 1);
                        paoRep.Write("支出摘要", ChumonRep.Chumons[CurRow].摘要 ?? " ", i + 1);
                        paoRep.Write("税抜支出", string.Format("{0:#,0}", (ChumonRep.Chumons[CurRow].金額 ?? 0)), i + 1);
                        paoRep.Write("税込支出", string.Format("{0:#,0}", ((ChumonRep.Chumons[CurRow].金額 ?? 0)*1.1)), i + 1);
                        paoRep.Write("支出締日", ChumonRep.Chumons[CurRow].支払締め ?? " ", i + 1);
                        paoRep.Write("支払日", ChumonRep.Chumons[CurRow].支払日 ?? " ", i + 1);

                        num++;
                        CurRow++;
                    }                   

                }
                paoRep.PageEnd();
            }

            //PDFを一時保存
            string pdfPath = Path.Combine(Directory.GetCurrentDirectory(), @"pdf/売上表" + DateTime.Now.ToString("yyMMddHHmm") + ".pdf");
            paoRep.SavePDF(pdfPath);

            //保存したPDFをfsに格納
            var fs = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);
            var contentType = "application/pdf";

            //ブラウザにPDFを表示
            return new FileStreamResult(fs, contentType);

        }

    }
}


