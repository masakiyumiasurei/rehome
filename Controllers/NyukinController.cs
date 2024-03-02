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
//using System.Management;

namespace rehome.Controllers
{
    [Authorize]
    public class NyukinController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<QuoteController> _logger;
        //private const int PAGE_SIZE = 20;
        private IQuoteService _QuoteService;
        private IDropDownListService _DropDownListService;
        private IHouzinService _HouzinService;
        private ITantouService _TantouService;
        private INyukinService _NyukinService;

        public NyukinController(ILogger<QuoteController> logger, IConfiguration configuration, 
            IQuoteService QuoteService, IDropDownListService dropDownListService,
            IHouzinService HouzinService,ITantouService TantouService, INyukinService NyukinService)
        {
            _logger = logger;
            // appsettings.jsonファイルから接続文字列を取得
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _QuoteService = QuoteService;
            _DropDownListService = dropDownListService;
            _HouzinService = HouzinService;
            _NyukinService = NyukinService;
            _TantouService = TantouService;
        }

        [HttpGet]
        public IActionResult Detail(int 見積ID, int 履歴番号)
        {
            //入金入力ボックスを返す
            var viewModel = new NyukinIndexModel();
            viewModel.Nyukins = _NyukinService.GetNyukin(見積ID, 履歴番号);
            viewModel.RowCount= _NyukinService.CountNyukin(見積ID, 履歴番号);
            viewModel.見積ID = 見積ID;
            viewModel.履歴番号 = 履歴番号;
            return PartialView("_Nyukindetail", viewModel);
        }

        [HttpPost]
        public IActionResult ajaxCreate(NyukinIndexModel model)
        {
            var viewModel = new NyukinIndexModel();
            try
            {
                //viewModel.Nyukins.見積ID = _QuoteService.RegistQuote(model);
                //viewModel.RowCount = viewModel.Quote.見積明細リスト.Count();
                //viewModel.Mode = ViewMode.Edit;
                TempData["Nyukin"] = String.Format("消込情報を登録しました");
                ModelState.Clear();
                var result = _NyukinService.RegistNyukin(model);
                if (result >= 0)
                {
                    if (result > 0)
                    {
                        TempData["Nyukin"] = "消込情報を登録しました。";
                        //ajaxで使用するため、見積IDと履歴番号を返す
                        return Ok(new { Message = "登録が成功しました。", 見積ID = model.見積ID, 履歴番号 = model.履歴番号 });
                    }
                    else
                    {
                        // 0の場合は登録する情報がない
                        TempData["Nyukin"] = "登録する情報がありません。";
                        return Ok(new { Message = "登録する情報がありません。" });
                    }
                }
                else
                {
                    // マイナス1の場合はDBエラー
                    TempData["Nyukin"] = "DBエラーが発生しました。";
                    return BadRequest(new { Message = "DBエラーが発生しました。", ErrorMessage = "データベースエラーが発生しました。" });
                }               
               
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");
                return BadRequest(new { Message = "エラーが発生しました。", ErrorMessage = "エラーが発生しました。" });
            }
        }

        //[HttpGet]
        //public ActionResult Create(int? 見積ID, int? 履歴番号,  bool single, int? 顧客ID )
        //{
        //    using var connection = new SqlConnection(_connectionString);

        //    ViewBag.OperationMessage = (string)TempData["Quote"];

        //    var model = new QuoteCreateModel();
        //    if (Request.Headers["Referer"].Any())
        //    {
        //        model.BackUrl = Request.Headers["Referer"].ToString();
        //    }

        //    if (見積ID == null)//new処理
        //    {

        //        model.Quote = new 見積();
        //        model.Quote.single = single;
        //        model.Quote.顧客ID = (int)顧客ID;
        //        model.Mode = ViewMode.New;
        //        //model.Quote.期 = _QuoteService.GetPeriod(DateTime.Now);
        //        //開発中コメント
        //        model.Quote.担当ID = Int32.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
        //       // model.Quote.営業所ID = Int32.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GroupSid).Value);
        //        model.auth= bool.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value);
        //        //model.Quote.見積番号 = _QuoteService.GetQuoteNumber((int)model.Quote.営業所ID, model.Quote.担当ID, DateTime.Now);
        //        model.RowCount = 0;
        //    }
        //    else
        //    {//edit処理

        //        model.Mode = ViewMode.Edit;
        //        model.Quote = _QuoteService.GetQuote(見積ID ?? -1, 履歴番号 ?? -1);//null許容でGetQuoteする処理が適正ではないので、Create呼ばれる際は絶対に値が入って呼ばれるようにする？

        //        model.auth = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value);


        //        if (model.Quote != null)
        //        {
        //            model.RowCount = model.Quote.見積明細リスト.Count();
        //        }
        //        else
        //        {
        //            model.RowCount = 0;
        //        }
        //        //ChumonIndexModel genka = _ChumonService.CalcChumon(見積ID ?? -1, 履歴番号 ?? -1);
        //        //if (genka.金額計 != null)
        //        //{
        //        //    model.Quote.原価 = genka.金額計;
        //        //    model.Quote.利益 = (model.Quote.見積金額 ?? 0) - (model.Quote.値引額 ?? 0) + (model.Quote.非課税額 ?? 0) - (model.Quote.原価 ?? 0);
        //        //    if ((model.Quote.見積金額 ?? 0) - (model.Quote.値引額 ?? 0) + (model.Quote.非課税額 ?? 0) != 0)
        //        //    {
        //        //        model.Quote.粗利率 = (model.Quote.利益 == 0 || model.Quote.利益 == null ? 0 : (Math.Round((decimal)((model.Quote.利益) * 100 / ((model.Quote.見積金額 ?? 0) - (model.Quote.値引額 ?? 0) + (model.Quote.非課税額 ?? 0))), 1, MidpointRounding.AwayFromZero))).ToString() + "%";
        //        //        model.Quote.見込粗利率 = (model.Quote.見込利益 == 0 || model.Quote.見込利益 == null ? 0 : (Math.Round((decimal)((model.Quote.見込利益) * 100 / ((model.Quote.見積金額 ?? 0) - (model.Quote.値引額 ?? 0) + (model.Quote.非課税額 ?? 0))), 1, MidpointRounding.AwayFromZero))).ToString() + "%";
        //        //    }
        //        //    else
        //        //    {
        //        //        model.Quote.粗利率 = "0%";
        //        //        model.Quote.見込粗利率 = "0%";
        //        //    }
        //        //}
        //    }

        //    model.自由分類DropDownList = _DropDownListService.Get自由分類DropDownLists(見積ID ?? -1, 履歴番号 ?? -1);
        //    model.分類DropDownList = _DropDownListService.Get分類DropDownLists();
        //    //model.営業所DropDownList = _DropDownListService.Get営業所DropDownLists();
        //    model.担当DropDownList = _DropDownListService.Get担当DropDownLists();

        //    return View(model);
        //}




        [HttpPost]
        public IActionResult Delete(QuoteCreateModel model)
        {
            var viewModel = new QuoteCreateModel();
            try
            {
                _QuoteService.DeleteQuote(model.Quote.見積ID,model.Quote.履歴番号);

                TempData["Quote_Index"] = String.Format("見積情報を削除しました");

                return RedirectToAction("Index", "Quote");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");
                return View(model);
            }


        }

        public IActionResult ReEstimate(QuoteCreateModel model)
        {
            var viewModel = new QuoteCreateModel();
            try
            {
                model.Mode = ViewMode.ReEstimate;
               // model.Quote.見積番号 = _QuoteService.GetQuoteNumber((int)model.Quote.営業所ID, model.Quote.担当ID, DateTime.Now);
                viewModel.Quote = _QuoteService.RegistQuote(model);
                viewModel.RowCount = viewModel.Quote.見積明細リスト.Count();
                viewModel.Mode = ViewMode.Edit;
                TempData["Quote"] = String.Format("見積情報を再見積しました");
                ModelState.Clear();
                viewModel.BackUrl = model.BackUrl;
                return RedirectToAction("Create", "Quote", new { 見積ID = viewModel.Quote.見積ID, 履歴番号 = viewModel.Quote.履歴番号 });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");
                return RedirectToAction("Create", "Quote", new { 見積ID = model.Quote.見積ID, 履歴番号 = model.Quote.履歴番号 });
            }
        }

 
        public IActionResult Index(int? page)
    {
            ViewBag.OperationMessage = (string)TempData["Nyukin_Index"];

            var viewModel = new NyukinIndexModel();
            if (HttpContext.Session.GetObject<NyukinSearchConditions>(SessionKeys.Nyukin_SEARCH_CONDITIONS) != null)
            {
                viewModel.NyukinSearchConditions = HttpContext.Session.GetObject<NyukinSearchConditions>(SessionKeys.Nyukin_SEARCH_CONDITIONS);
            }
            HttpContext.Session.SetObject(SessionKeys.QUOTE_SEARCH_CONDITIONS, viewModel.NyukinSearchConditions);
            viewModel.Quotes = _NyukinService.SearchNyukins(viewModel.NyukinSearchConditions);
            viewModel.担当DropDownList = _DropDownListService.Get担当DropDownLists();
            return View("Index", viewModel);
        }

        [HttpGet]
        public ActionResult Clear()
        {
            var viewModel = new QuoteIndexModel();
            viewModel.QuoteSearchConditions = new QuoteSearchConditions();
            return RedirectToAction("Index", "Quote");
        }


        /// サンプル一覧で検索ボタンクリック時のアクション
        public IActionResult Search(NyukinIndexModel model)
    {
        var viewModel = new NyukinIndexModel();
        viewModel.NyukinSearchConditions = model.NyukinSearchConditions;
  
        ModelState.Clear();

        HttpContext.Session.SetObject(SessionKeys.Nyukin_SEARCH_CONDITIONS, viewModel.NyukinSearchConditions);

           // viewModel.QuoteSearchConditions.LoginID = Int32.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            viewModel.Quotes = _NyukinService.SearchNyukins(viewModel.NyukinSearchConditions);
           // viewModel.営業所DropDownList = _DropDownListService.Get営業所DropDownLists();
            viewModel.担当DropDownList = _DropDownListService.Get担当DropDownLists();
            return View("Index", viewModel);
    }
        [HttpGet]
        public IActionResult SalesStatus(int? period,DateTime? start_date, DateTime? end_date)      
        {

            var viewModel = new QuoteSalesStatusModel();

            var 期 = _QuoteService.GetPeriod(DateTime.Now);
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).Date;
            DateTime endDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).Date;

            viewModel = _QuoteService.SearchSalesStatus(period ?? 期, start_date ?? firstDayOfMonth, end_date ?? endDayOfMonth);

            viewModel.start_date = start_date ?? firstDayOfMonth;            
            viewModel.end_date = end_date ?? endDayOfMonth;
            viewModel.期 = period ?? 期;

            return View("SalesStatus", viewModel);
        }

        public ActionResult GetQuoteStatus(int 見積ID, int 履歴番号)
        {            
            string strsql = "select 見積ステータス from T_見積 where 見積ID="+ 見積ID + " and 履歴番号="+ 履歴番号;
            FunctionClass fn = new FunctionClass(_connectionString);
            var quoteStatus = fn.GetValue<string>(strsql);
            return Json(quoteStatus);
        }

        [HttpPost]
        public ActionResult GetQuoteNumber(int 営業所ID, int 担当ID)
        {
            var QuoteNumber = _QuoteService.GetQuoteNumber(営業所ID, 担当ID, DateTime.Now);
            return Json(QuoteNumber);
        }


        [HttpGet]
        public ActionResult 見積書_1枚印刷(int 見積ID,int 履歴番号)
        {

            //pdfフォルダ内に保存してあるファイルを全削除
            foreach (string pathFrom in System.IO.Directory.EnumerateFiles(Path.Combine(Directory.GetCurrentDirectory(), @"pdf"), "*.pdf", System.IO.SearchOption.AllDirectories))
            {
                System.IO.File.Delete(pathFrom);
            }

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);


            //インスタンス化
            IReport paoRep = ReportCreator.GetPdf();

            paoRep.LoadDefFile("Reports/見積書.prepd");


            見積書セット_1枚印刷(paoRep, 見積ID,履歴番号);

            //PDFを一時保存
            string pdfPath = Path.Combine(Directory.GetCurrentDirectory(), @"pdf/見積書" + DateTime.Now.ToString("yyMMddHHmm") + ".pdf");
            paoRep.SavePDF(pdfPath);

            //保存したPDFをfsに格納
            var fs = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);
            var contentType = "application/pdf";

            //ブラウザにPDFを表示
            return new FileStreamResult(fs, contentType);

            //PDFをダウンロード
            //return File(fs, contentType, "見積書.pdf");
        }


        public void 見積書セット_1枚印刷(IReport paoRep, int 見積ID,int 履歴番号)
        {
            var Quote = new 見積();
            Quote = _QuoteService.GetQuote(見積ID, 履歴番号);

            法人 Houzin = _HouzinService.GetHouzin();
           // 営業所 Office = _OfficeService.GetOffice(Quote.営業所ID ?? -1);

            int 小計 = (int)((Quote.見積金額 ?? 0) - (Quote.値引額 ?? 0));
            int 消費税額 = (int)(小計 * 0.1);
            int 非課税額 = (int)(Quote.非課税額 ?? 0);

            //明細が0行でも1ページは表示するため初期値は25にしておく
            int RowCnt = 25;

            if(Quote.見積明細リスト != null)
            {
                RowCnt = Quote.見積明細リスト.Count();            
            }

            //現在の行
            int CurRow = 0;

            //値引行を追加するかどうか
            bool Nebiki = false;
            if(Quote.値引額 != null)
            {
                Nebiki = true;
            }

            //通し番号
            int num = 1;


            //描画すべき行がある限りページを増やす
            while (RowCnt > 0)
            {
                RowCnt -= 25;

                paoRep.PageStart();

                //ヘッダー
                paoRep.Write("見積番号", "No." + (Quote.見積番号 ?? " "));
                if (Quote.作成日 != null)
                {
                    paoRep.Write("作成日", string.Format("{0:yyyy年M月d日}", Quote.作成日));
                }
                else
                {
                    paoRep.Write("作成日", " ");
                }
                paoRep.Write("顧客名", Quote.顧客名 ?? " ");
                paoRep.Write("敬称", Quote.敬称 ?? " ");
                paoRep.Write("件名", Quote.件名 ?? " ");
                paoRep.Write("納期", Quote.納期 ?? " ");
                paoRep.Write("受渡場所", Quote.受渡場所 ?? " ");
                paoRep.Write("支払条件", Quote.支払条件 ?? " ");
                paoRep.Write("有効期限", Quote.有効期限 ?? " ");
                paoRep.Write("見積金額", "￥" + string.Format("{0:#,0}",(小計 + 消費税額 + 非課税額)));

                paoRep.Write("社名", Houzin.社名 ?? " ");
                paoRep.Write("代表名", "代表取締役　" + (Houzin.代表名 ?? " "));
                paoRep.Write("郵便番号", "〒" + (Houzin.郵便番号 ?? " "));
                paoRep.Write("住所", Houzin.住所 ?? " ");
                paoRep.Write("TEL", "TEL " + (Houzin.TEL ?? " ") + "　FAX " + (Houzin.FAX ?? " "));
                paoRep.Write("オフィス", "オフィス " + (Houzin.オフィス ?? " "));

                //フッダー
                paoRep.Write("小計", string.Format("{0:#,0}", 小計));
                paoRep.Write("消費税額", string.Format("{0:#,0}", 消費税額));
                paoRep.Write("非課税名称", Quote.非課税名称 ?? " ");
                paoRep.Write("非課税額", string.Format("{0:#,0}", 非課税額));
                paoRep.Write("合計", string.Format("{0:#,0}", (小計 + 消費税額 + 非課税額)));
                paoRep.Write("備考", Quote.備考 ?? " ");

                //空の明細行を25行分用意する
                for (int i = 0; i < 25; i++)
                {
                    paoRep.Write("番号", " ", i + 1);
                    paoRep.Write("品名", " ", i + 1);
                    paoRep.Write("内訳", " ", i + 1);
                    paoRep.Write("数量", " ", i + 1);
                    paoRep.Write("単位", " ", i + 1);
                    paoRep.Write("単価", " ", i + 1);
                    paoRep.Write("金額", " ", i + 1);
                }
                               

                if (Quote.見積明細リスト != null)
                {

                    for (var i = 0; i < 25; i++)
                    {
                        //描画すべき行がなくなれば、値引行等を追加してループを抜ける
                        if (CurRow >= Quote.見積明細リスト.Count())
                        {

                            //値引行を追加
                            if (Nebiki)
                            {
                                paoRep.Write("番号", num.ToString(), i + 1);
                                paoRep.Write("品名", Quote.値引名称 ?? " ", i + 1);
                                paoRep.Write("数量", "1", i + 1);
                                paoRep.Write("単位", "式", i + 1);
                                paoRep.Write("単価", string.Format("{0:#,0}", -(Quote.値引額 ?? 0)), i + 1);
                                paoRep.Write("金額", string.Format("{0:#,0}", -(Quote.値引額 ?? 0)), i + 1);
                                Nebiki = false;
                            }
                           
                            break;
                        }

                        string utiwake = string.Empty;
                        var reco = Quote.見積明細リスト[CurRow];

                        if ((reco.内訳数1 ?? 0 ) !=0 && !string.IsNullOrEmpty(reco.内訳単位1) &&
                            (reco.内訳数2 ?? 0) != 0 && !string.IsNullOrEmpty(reco.内訳単位2)) 
                        
                        {
                             utiwake = reco.内訳数1 + reco.内訳単位1 + "ｘ" + reco.内訳数2 + reco.内訳単位2;
                        }


                        paoRep.Write("番号", num.ToString(), i + 1);
                        paoRep.Write("品名", Quote.見積明細リスト[CurRow].商品名 ?? " ", i + 1);
                       // paoRep.Write("内訳", utiwake, i + 1);
                        paoRep.Write("数量", string.Format("{0:#,0}", (Quote.見積明細リスト[CurRow].数量 ?? 0)), i + 1);
                        paoRep.Write("単位", Quote.見積明細リスト[CurRow].単位 ?? " ", i + 1);
                        paoRep.Write("単価", string.Format("{0:#,0}", (Quote.見積明細リスト[CurRow].単価 ?? 0)), i + 1);
                        paoRep.Write("金額", !string.IsNullOrEmpty(Quote.見積明細リスト[CurRow].備考)
                        ? Quote.見積明細リスト[CurRow].備考
                        : string.Format("{0:#,0}", Quote.見積明細リスト[CurRow].金額 ?? 0), i + 1);

                        CurRow++;
                        num++;

                    }

                    //値引行がギリギリ入らなかった場合は次のページを用意する
                    if (CurRow >= Quote.見積明細リスト.Count() && Nebiki)
                    {
                        RowCnt = 1;
                    }
                }
                paoRep.PageEnd();
            }
        }



        [HttpGet]
        public ActionResult 見積書_鑑印刷(int 見積ID, int 履歴番号)
        {

            //pdfフォルダ内に保存してあるファイルを全削除
            foreach (string pathFrom in System.IO.Directory.EnumerateFiles(Path.Combine(Directory.GetCurrentDirectory(), @"pdf"), "*.pdf", System.IO.SearchOption.AllDirectories))
            {
                System.IO.File.Delete(pathFrom);
            }

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            //インスタンス化
            IReport paoRep = ReportCreator.GetPdf();

            paoRep.LoadDefFile("Reports/見積書.prepd");

            見積書セット_鑑印刷(paoRep, 見積ID, 履歴番号);

            //PDFを一時保存
            string pdfPath = Path.Combine(Directory.GetCurrentDirectory(), @"pdf/見積書" + DateTime.Now.ToString("yyMMddHHmm") + ".pdf");
            paoRep.SavePDF(pdfPath);

            //保存したPDFをfsに格納
            var fs = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);
            var contentType = "application/pdf";

            //ブラウザにPDFを表示
            return new FileStreamResult(fs, contentType);

            //PDFをダウンロード
            //return File(fs, contentType, "見積書.pdf");
        }


        public void 見積書セット_鑑印刷(IReport paoRep, int 見積ID, int 履歴番号)
        {
            var Quote = new 見積();
            Quote = _QuoteService.GetQuote(見積ID, 履歴番号);
            var QuoteBunrui = _QuoteService.GetQuoteBunrui(見積ID, 履歴番号);

            法人 Houzin = _HouzinService.GetHouzin();
           // 営業所 Office = _OfficeService.GetOffice(Quote.営業所ID ?? -1);

            int 小計 = (int)((Quote.見積金額 ?? 0) - (Quote.値引額 ?? 0));
            int 消費税額 = (int)(小計 * 0.1);
            int 非課税額 = (int)(Quote.非課税額 ?? 0);

            //明細が0行でも1ページは表示するため初期値は25にしておく
            int RowCnt = 25;

            if (QuoteBunrui != null)
            {
                RowCnt = QuoteBunrui.Count();
            }

            //現在の行
            int CurRow = 0;

            //値引行を追加するかどうか
            bool Nebiki = false;
            if (Quote.値引額 != null)
            {
                Nebiki = true;
            }

            //通し番号
            int num = 1;


            //描画すべき行がある限りページを増やす
            while (RowCnt > 0)
            {
                RowCnt -= 25;

                paoRep.PageStart();

                //ヘッダー
                paoRep.Write("見積番号", "No." + (Quote.見積番号 ?? " "));
                if (Quote.作成日 != null)
                {
                    paoRep.Write("作成日", string.Format("{0:yyyy年M月d日}", Quote.作成日));
                }
                else
                {
                    paoRep.Write("作成日", " ");
                }
                paoRep.Write("顧客名", Quote.顧客名 ?? " ");
                paoRep.Write("敬称", Quote.敬称 ?? " ");
                paoRep.Write("件名", Quote.件名 ?? " ");
                paoRep.Write("納期", Quote.納期 ?? " ");
                paoRep.Write("受渡場所", Quote.受渡場所 ?? " ");
                paoRep.Write("支払条件", Quote.支払条件 ?? " ");
                paoRep.Write("有効期限", Quote.有効期限 ?? " ");
                paoRep.Write("見積金額", "￥" + string.Format("{0:#,0}", (小計 + 消費税額 + 非課税額)));

                paoRep.Write("社名", Houzin.社名 ?? " ");
                paoRep.Write("代表名", "代表取締役　" + (Houzin.代表名 ?? " "));
                paoRep.Write("郵便番号", "〒" + (Houzin.郵便番号 ?? " "));
                paoRep.Write("住所", Houzin.住所 ?? " ");
                paoRep.Write("TEL", "TEL " + (Houzin.TEL ?? " ") + "　FAX " + (Houzin.FAX ?? " "));
                paoRep.Write("オフィス", "オフィス " + (Houzin.オフィス ?? " "));

                //フッダー
                paoRep.Write("小計", string.Format("{0:#,0}", 小計));
                paoRep.Write("消費税額", string.Format("{0:#,0}", 消費税額));
                paoRep.Write("非課税名称", Quote.非課税名称 ?? " ");
                paoRep.Write("非課税額", string.Format("{0:#,0}", 非課税額));
                paoRep.Write("合計", string.Format("{0:#,0}", (小計 + 消費税額 + 非課税額)));
                paoRep.Write("備考", Quote.備考 ?? " ");


                //ボディ


                //空の明細行を25行分用意する
                for (int i = 0; i < 25; i++)
                {
                    paoRep.Write("番号", " ", i + 1);
                    paoRep.Write("品名", " ", i + 1);
                  //  paoRep.Write("内訳", " ", i + 1);
                    paoRep.Write("数量", " ", i + 1);
                    paoRep.Write("単位", " ", i + 1);
                    paoRep.Write("単価", " ", i + 1);
                    paoRep.Write("金額", " ", i + 1);
                }


                if (QuoteBunrui != null)
                {

                    for (var i = 0; i < 25; i++)
                    {
                        //描画すべき行がなくなれば、値引行等を追加してループを抜ける
                        if (CurRow >= QuoteBunrui.Count())
                        {

                            //値引行を追加
                            if (Nebiki)
                            {
                                paoRep.Write("番号", num.ToString(), i + 1);
                                paoRep.Write("品名", Quote.値引名称 ?? " ", i + 1);
                               // paoRep.Write("内訳", "", i + 1);
                                paoRep.Write("数量", "1", i + 1);
                                paoRep.Write("単位", "式", i + 1);
                                paoRep.Write("単価", string.Format("{0:#,0}", -(Quote.値引額 ?? 0)), i + 1);
                                paoRep.Write("金額", string.Format("{0:#,0}", -(Quote.値引額 ?? 0)), i + 1);
                                Nebiki = false;
                            }

                            break;
                        }


                        paoRep.Write("番号", num.ToString(), i + 1);
                        paoRep.Write("品名", QuoteBunrui[CurRow].分類名 ?? " ", i + 1);
                     //   paoRep.Write("内訳", "", i + 1);
                        paoRep.Write("数量", "1", i + 1);
                        paoRep.Write("単位","式", i + 1);
                        paoRep.Write("単価", string.Format("{0:#,0}", (QuoteBunrui[CurRow].単価 ?? 0)), i + 1);
                        paoRep.Write("金額", string.Format("{0:#,0}", (QuoteBunrui[CurRow].単価 ?? 0)), i + 1);

                        CurRow++;
                        num++;

                    }

                    //値引行がギリギリ入らなかった場合は次のページを用意する
                    if (CurRow >= QuoteBunrui.Count() && Nebiki)
                    {
                        RowCnt = 1;
                    }

                }
                paoRep.PageEnd();
            }
        }

        public void 請求書セット_鑑印刷(IReport paoRep, int 見積ID, int 履歴番号)
        {
            var Quote = new 見積();
            Quote = _QuoteService.GetQuote(見積ID, 履歴番号);
            var QuoteBunrui = _QuoteService.GetQuoteBunrui(見積ID, 履歴番号);

            法人 Houzin = _HouzinService.GetHouzin();
         //   営業所 Office = _OfficeService.GetOffice(Quote.営業所ID ?? -1);

            int 小計 = (int)((Quote.見積金額 ?? 0) - (Quote.値引額 ?? 0));
            int 消費税額 = (int)(小計 * 0.1);
            int 非課税額 = (int)(Quote.非課税額 ?? 0);

            //明細が0行でも1ページは表示するため初期値は25にしておく
            int RowCnt = 25;

            if (QuoteBunrui != null)
            {
                RowCnt = QuoteBunrui.Count();
            }

            //現在の行
            int CurRow = 0;

            //値引行を追加するかどうか
            bool Nebiki = false;
            if (Quote.値引額 != null)
            {
                Nebiki = true;
            }

            //通し番号
            int num = 1;


            //描画すべき行がある限りページを増やす
            while (RowCnt > 0)
            {
                RowCnt -= 25;

                paoRep.PageStart();

                //ヘッダー
                paoRep.Write("見積番号", "No." + (Quote.見積番号 ?? " "));
                if (Quote.作成日 != null)
                {
                    paoRep.Write("作成日", string.Format("{0:yyyy年M月d日}", Quote.作成日));
                }
                else
                {
                    paoRep.Write("作成日", " ");
                }
                paoRep.Write("顧客名", Quote.顧客名 ?? " ");
                paoRep.Write("敬称", Quote.敬称 ?? " ");
                paoRep.Write("件名", Quote.件名 ?? " ");
                paoRep.Write("見積金額", "￥" + string.Format("{0:#,0}", (小計 + 消費税額 + 非課税額)));

                paoRep.Write("社名", Houzin.社名 ?? " ");
                //項目がNULLか物販以外なら作業完了日、物販なら納品日
                paoRep.Write("取引年月日", Quote.項目 == null || Quote.項目 != "物販" ? 
                    "作業完了日：" + string.Format("{0:yyyy年M月d日}", Quote.取引年月日) 
                    : "納品日：" + string.Format("{0:yyyy年M月d日}", Quote.取引年月日));

                paoRep.Write("代表名", "代表取締役　" + (Houzin.代表名 ?? " "));
                paoRep.Write("郵便番号", "〒" + (Houzin.郵便番号 ?? " "));
                paoRep.Write("住所", Houzin.住所 ?? " ");
                paoRep.Write("TEL", "TEL " + (Houzin.TEL ?? " ") + "　FAX " + (Houzin.FAX ?? " "));
                paoRep.Write("オフィス", "オフィス " + (Houzin.オフィス ?? " "));

                //フッダー
                paoRep.Write("小計", string.Format("{0:#,0}", 小計));
                paoRep.Write("消費税額", string.Format("{0:#,0}", 消費税額));
                paoRep.Write("非課税名称", Quote.非課税名称 ?? " ");
                paoRep.Write("非課税額", string.Format("{0:#,0}", 非課税額));
                paoRep.Write("合計", string.Format("{0:#,0}", (小計 + 消費税額 + 非課税額)));
                paoRep.Write("備考", Quote.備考 ?? " ");

                //ボディ

                //空の明細行を25行分用意する
                for (int i = 0; i < 25; i++)
                {
                    paoRep.Write("番号", " ", i + 1);
                    paoRep.Write("品名", " ", i + 1);
                 //   paoRep.Write("内訳", " ", i + 1);
                    paoRep.Write("数量", " ", i + 1);
                    paoRep.Write("単位", " ", i + 1);
                    paoRep.Write("単価", " ", i + 1);
                    paoRep.Write("金額", " ", i + 1);
                }


                if (QuoteBunrui != null)
                {

                    for (var i = 0; i < 25; i++)
                    {
                        //描画すべき行がなくなれば、値引行等を追加してループを抜ける
                        if (CurRow >= QuoteBunrui.Count())
                        {

                            //値引行を追加
                            if (Nebiki)
                            {
                                paoRep.Write("番号", num.ToString(), i + 1);
                                paoRep.Write("品名", Quote.値引名称 ?? " ", i + 1);
                              //  paoRep.Write("内訳", "", i + 1);
                                paoRep.Write("数量", "1", i + 1);
                                paoRep.Write("単位", "式", i + 1);
                                paoRep.Write("単価", string.Format("{0:#,0}", -(Quote.値引額 ?? 0)), i + 1);
                                paoRep.Write("金額", string.Format("{0:#,0}", -(Quote.値引額 ?? 0)), i + 1);
                                Nebiki = false;
                            }

                            break;
                        }

                        paoRep.Write("番号", num.ToString(), i + 1);
                        paoRep.Write("品名", QuoteBunrui[CurRow].分類名 ?? " ", i + 1);
                     //   paoRep.Write("内訳", "", i + 1);
                        paoRep.Write("数量", "1", i + 1);
                        paoRep.Write("単位", "式", i + 1);
                        paoRep.Write("単価", string.Format("{0:#,0}", (QuoteBunrui[CurRow].単価 ?? 0)), i + 1);
                        paoRep.Write("金額", string.Format("{0:#,0}", (QuoteBunrui[CurRow].単価 ?? 0)), i + 1);

                        CurRow++;
                        num++;

                    }
                    //値引行がギリギリ入らなかった場合は次のページを用意する
                    if (CurRow >= QuoteBunrui.Count() && Nebiki)
                    {
                        RowCnt = 1;
                    }

                }
                paoRep.PageEnd();
            }
        }


        [HttpGet]
        public ActionResult 請求書印刷(int 見積ID, int 履歴番号)
        {

            //pdfフォルダ内に保存してあるファイルを全削除
            foreach (string pathFrom in System.IO.Directory.EnumerateFiles(Path.Combine(Directory.GetCurrentDirectory(), @"pdf"), "*.pdf", System.IO.SearchOption.AllDirectories))
            {
                System.IO.File.Delete(pathFrom);
            }

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);


            //インスタンス化
            IReport paoRep = ReportCreator.GetPdf();


            paoRep.LoadDefFile("Reports/請求書.prepd");


            請求書セット_鑑印刷(paoRep, 見積ID, 履歴番号);


            paoRep.LoadDefFile("Reports/見積明細書.prepd");


            見積書セット_明細印刷(paoRep, 見積ID, 履歴番号);


            //PDFを一時保存
            string pdfPath = Path.Combine(Directory.GetCurrentDirectory(), @"pdf/請求書" + DateTime.Now.ToString("yyMMddHHmm") + ".pdf");
            paoRep.SavePDF(pdfPath);

            //保存したPDFをfsに格納
            var fs = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);
            var contentType = "application/pdf";

            //ブラウザにPDFを表示
            return new FileStreamResult(fs, contentType);

            //PDFをダウンロード
            //return File(fs, contentType, "見積書.pdf");
        }

        [HttpGet]
        public ActionResult 請求書_1枚印刷(int 見積ID, int 履歴番号)
        {

            //pdfフォルダ内に保存してあるファイルを全削除
            foreach (string pathFrom in System.IO.Directory.EnumerateFiles(Path.Combine(Directory.GetCurrentDirectory(), @"pdf"), "*.pdf", System.IO.SearchOption.AllDirectories))
            {
                System.IO.File.Delete(pathFrom);
            }

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            //インスタンス化
            IReport paoRep = ReportCreator.GetPdf();


            paoRep.LoadDefFile("Reports/請求書.prepd");

            請求書セット_1枚印刷(paoRep, 見積ID, 履歴番号);            


            //PDFを一時保存
            string pdfPath = Path.Combine(Directory.GetCurrentDirectory(), @"pdf/請求書" + DateTime.Now.ToString("yyMMddHHmm") + ".pdf");
            paoRep.SavePDF(pdfPath);

            //保存したPDFをfsに格納
            var fs = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);
            var contentType = "application/pdf";

            //ブラウザにPDFを表示
            return new FileStreamResult(fs, contentType);

            //PDFをダウンロード
            //return File(fs, contentType, "見積書.pdf");
        }


        public void 請求書セット_1枚印刷(IReport paoRep, int 見積ID, int 履歴番号)
        {
            var Quote = new 見積();
            Quote = _QuoteService.GetQuote(見積ID, 履歴番号);

            法人 Houzin = _HouzinService.GetHouzin();
         //   営業所 Office = _OfficeService.GetOffice(Quote.営業所ID ?? -1);

            int 小計 = (int)((Quote.見積金額 ?? 0) - (Quote.値引額 ?? 0));
            int 消費税額 = (int)(小計 * 0.1);
            int 非課税額 = (int)(Quote.非課税額 ?? 0);

            //明細が0行でも1ページは表示するため初期値は25にしておく
            int RowCnt = 25;

            if (Quote.見積明細リスト != null)
            {
                RowCnt = Quote.見積明細リスト.Count();
            }

            //現在の行
            int CurRow = 0;

            //値引行を追加するかどうか
            bool Nebiki = false;
            if (Quote.値引額 != null)
            {
                Nebiki = true;
            }

            //通し番号
            int num = 1;


            //描画すべき行がある限りページを増やす
            while (RowCnt > 0)
            {
                RowCnt -= 25;

                paoRep.PageStart();

                //ヘッダー
                paoRep.Write("見積番号", "No." + (Quote.見積番号 ?? " "));
                if (Quote.作成日 != null)
                {
                    paoRep.Write("作成日", string.Format("{0:yyyy年M月d日}", Quote.作成日));
                }
                else
                {
                    paoRep.Write("作成日", " ");
                }
                paoRep.Write("顧客名", Quote.顧客名 ?? " ");
                paoRep.Write("敬称", Quote.敬称 ?? " ");
                paoRep.Write("件名", Quote.件名 ?? " ");
               
                paoRep.Write("見積金額", "￥" + string.Format("{0:#,0}", (小計 + 消費税額 + 非課税額)));
                paoRep.Write("社名", Houzin.社名 ?? " ");

                //項目がNULLか物販以外なら作業完了日、物販なら納品日
                paoRep.Write("取引年月日", Quote.項目 == null || Quote.項目 != "物販" ?
                    "作業完了日：" + string.Format("{0:yyyy年M月d日}", Quote.取引年月日)
                    : "納品日：" + string.Format("{0:yyyy年M月d日}", Quote.取引年月日));

                paoRep.Write("代表名", "代表取締役　" + (Houzin.代表名 ?? " "));
                paoRep.Write("郵便番号", "〒" + (Houzin.郵便番号 ?? " "));
                paoRep.Write("住所", Houzin.住所 ?? " ");
                paoRep.Write("TEL", "TEL " + (Houzin.TEL ?? " ") + "　FAX " + (Houzin.FAX ?? " "));
                paoRep.Write("オフィス", "オフィス " + (Houzin.オフィス ?? " "));

                //フッダー
                paoRep.Write("小計", string.Format("{0:#,0}", 小計));
                paoRep.Write("消費税額", string.Format("{0:#,0}", 消費税額));
                paoRep.Write("非課税名称", Quote.非課税名称 ?? " ");
                paoRep.Write("非課税額", string.Format("{0:#,0}", 非課税額));
                paoRep.Write("合計", string.Format("{0:#,0}", (小計 + 消費税額 + 非課税額)));
                paoRep.Write("備考", Quote.備考 ?? " ");


                //ボディ

                //空の明細行を25行分用意する
                for (int i = 0; i < 25; i++)
                {
                    paoRep.Write("番号", " ", i + 1);
                    paoRep.Write("品名", " ", i + 1);
                  //  paoRep.Write("内訳", " ", i + 1);
                    paoRep.Write("数量", " ", i + 1);
                    paoRep.Write("単位", " ", i + 1);
                    paoRep.Write("単価", " ", i + 1);
                    paoRep.Write("金額", " ", i + 1);
                }


                if (Quote.見積明細リスト != null)
                {

                    for (var i = 0; i < 25; i++)
                    {
                        //描画すべき行がなくなれば、値引行等を追加してループを抜ける
                        if (CurRow >= Quote.見積明細リスト.Count())
                        {

                            //値引行を追加
                            if (Nebiki)
                            {
                                paoRep.Write("番号", num.ToString(), i + 1);
                                paoRep.Write("品名", Quote.値引名称 ?? " ", i + 1);
                                paoRep.Write("数量", "1", i + 1);
                                paoRep.Write("単位", "式", i + 1);
                                paoRep.Write("単価", string.Format("{0:#,0}", -(Quote.値引額 ?? 0)), i + 1);
                                paoRep.Write("金額", string.Format("{0:#,0}", -(Quote.値引額 ?? 0)), i + 1);
                                Nebiki = false;
                            }

                            break;
                        }

                        string utiwake = string.Empty;
                        var reco = Quote.見積明細リスト[CurRow];

                        if ((reco.内訳数1 ?? 0) != 0 && !string.IsNullOrEmpty(reco.内訳単位1) &&
                            (reco.内訳数2 ?? 0) != 0 && !string.IsNullOrEmpty(reco.内訳単位2))

                        {
                            utiwake = reco.内訳数1 + reco.内訳単位1 + "ｘ" + reco.内訳数2 + reco.内訳単位2;
                        }


                        paoRep.Write("番号", num.ToString(), i + 1);
                        paoRep.Write("品名", Quote.見積明細リスト[CurRow].商品名 ?? " ", i + 1);
                     //   paoRep.Write("内訳", utiwake, i + 1);
                        paoRep.Write("数量", string.Format("{0:#,0}", (Quote.見積明細リスト[CurRow].数量 ?? 0)), i + 1);
                        paoRep.Write("単位", Quote.見積明細リスト[CurRow].単位 ?? " ", i + 1);
                        paoRep.Write("単価", string.Format("{0:#,0}", (Quote.見積明細リスト[CurRow].単価 ?? 0)), i + 1);
                        paoRep.Write("金額", !string.IsNullOrEmpty(Quote.見積明細リスト[CurRow].備考)
                        ? Quote.見積明細リスト[CurRow].備考
                        : string.Format("{0:#,0}", Quote.見積明細リスト[CurRow].金額 ?? 0), i + 1);

                        CurRow++;
                        num++;

                    }

                    //値引行がギリギリ入らなかった場合は次のページを用意する
                    if (CurRow >= Quote.見積明細リスト.Count() && Nebiki)
                    {
                        RowCnt = 1;
                    }
                }
                paoRep.PageEnd();
            }
        }




        [HttpGet]
        public ActionResult 見積書_複数印刷(int 見積ID, int 履歴番号)
        {

            //pdfフォルダ内に保存してあるファイルを全削除
            foreach (string pathFrom in System.IO.Directory.EnumerateFiles(Path.Combine(Directory.GetCurrentDirectory(), @"pdf"), "*.pdf", System.IO.SearchOption.AllDirectories))
            {
                System.IO.File.Delete(pathFrom);
            }

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);


            //インスタンス化
            IReport paoRep = ReportCreator.GetPdf();


            paoRep.LoadDefFile("Reports/見積書.prepd");


            見積書セット_鑑印刷(paoRep, 見積ID, 履歴番号);


            paoRep.LoadDefFile("Reports/見積明細書.prepd");


            見積書セット_明細印刷(paoRep, 見積ID, 履歴番号);


            //PDFを一時保存
            string pdfPath = Path.Combine(Directory.GetCurrentDirectory(), @"pdf/見積書" + DateTime.Now.ToString("yyMMddHHmm") + ".pdf");
            paoRep.SavePDF(pdfPath);

            //保存したPDFをfsに格納
            var fs = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);
            var contentType = "application/pdf";

            //ブラウザにPDFを表示
            return new FileStreamResult(fs, contentType);

            //PDFをダウンロード
            //return File(fs, contentType, "見積書.pdf");
        }


        public void 見積書セット_明細印刷(IReport paoRep, int 見積ID, int 履歴番号)
        {
            var Quote = new 見積();
            Quote = _QuoteService.GetQuote(見積ID, 履歴番号);
            var QuoteBunrui = _QuoteService.GetQuoteBunrui(見積ID, 履歴番号);

            var BunruiCnt = 0;

            foreach(var Bunrui in QuoteBunrui)
            {
                BunruiCnt++;

                var QuoteMeisai = _QuoteService.GetQuoteBunruiMeisai(見積ID, 履歴番号, Bunrui.分類ID);

                int 小計 = (int)(Bunrui.単価 ?? 0);

                //明細が0行でも1ページは表示するため初期値は44にしておく
                int RowCnt = 44;

                if (QuoteMeisai != null)
                {
                    RowCnt = QuoteMeisai.Count();
                }

                //現在の行
                int CurRow = 0;


                //通し番号
                int num = 1;


                //描画すべき行がある限りページを増やす
                while (RowCnt > 0)
                {
                    RowCnt -= 44;

                    paoRep.PageStart();

                    //ヘッダー
                    paoRep.Write("分類名", BunruiCnt + "." + (Bunrui.分類名 ?? " "));

                    //フッダー
                    paoRep.Write("小計", string.Format("{0:#,0}", 小計));

                    //ボディ

                    //空の明細行を44行分用意する
                    for (int i = 0; i < 44; i++)
                    {
                        paoRep.Write("番号", " ", i + 1);
                        paoRep.Write("品名", " ", i + 1);
                     //   paoRep.Write("内訳", " ", i + 1);
                        paoRep.Write("数量", " ", i + 1);
                        paoRep.Write("単位", " ", i + 1);
                        paoRep.Write("単価", " ", i + 1);
                        paoRep.Write("金額", " ", i + 1);
                    }


                    if (QuoteMeisai != null)
                    {

                        for (var i = 0; i < 44; i++)
                        {
                            //描画すべき行がなくなれば、値引行等を追加してループを抜ける
                            if (CurRow >= QuoteMeisai.Count())
                            {
                                break;
                            }

                            string utiwake = string.Empty;

                            var reco = QuoteMeisai[CurRow];

                            if ((reco.内訳数1 ?? 0) != 0 && !string.IsNullOrEmpty(reco.内訳単位1) &&
                                (reco.内訳数2 ?? 0) != 0 && !string.IsNullOrEmpty(reco.内訳単位2))

                            {
                                utiwake = reco.内訳数1 + reco.内訳単位1 + "ｘ" + reco.内訳数2 + reco.内訳単位2;
                            }

                            paoRep.Write("番号", num.ToString(), i + 1);
                            paoRep.Write("品名", QuoteMeisai[CurRow].商品名 ?? " ", i + 1);
                         //   paoRep.Write("内訳", utiwake, i + 1);
                            paoRep.Write("数量", string.Format("{0:#,0}", (QuoteMeisai[CurRow].数量 ?? 0)), i + 1);
                            paoRep.Write("単位", QuoteMeisai[CurRow].単位 ?? " ", i + 1);
                            paoRep.Write("単価", string.Format("{0:#,0}", (QuoteMeisai[CurRow].単価 ?? 0)), i + 1);
                            paoRep.Write("金額", !string.IsNullOrEmpty(QuoteMeisai[CurRow].備考)
                            ? QuoteMeisai[CurRow].備考
                            : string.Format("{0:#,0}", QuoteMeisai[CurRow].金額 ?? 0), i + 1);

                            CurRow++;
                            num++;

                        }
                    }
                    paoRep.PageEnd();
                }
            }
        }


        [HttpGet]
        public ActionResult 作業完了報告書印刷(int 見積ID, int 履歴番号)
        {

            //pdfフォルダ内に保存してあるファイルを全削除
            foreach (string pathFrom in System.IO.Directory.EnumerateFiles(Path.Combine(Directory.GetCurrentDirectory(), @"pdf"), "*.pdf", System.IO.SearchOption.AllDirectories))
            {
                System.IO.File.Delete(pathFrom);
            }

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);


            //インスタンス化
            IReport paoRep = ReportCreator.GetPdf();




            paoRep.LoadDefFile("Reports/完了報告書.prepd");


            paoRep.PageStart();

            見積 Quote = _QuoteService.GetQuote(見積ID, 履歴番号);

            法人 Houzin = _HouzinService.GetHouzin();
          //  営業所 Office = _OfficeService.GetOffice(Quote.営業所ID ?? -1);

            //ヘッダー
            paoRep.Write("見積番号", "No." + (Quote.見積番号 ?? " "));
            paoRep.Write("作成日", string.Format("{0:yyyy年M月d日}", DateTime.Now));
            paoRep.Write("顧客名", Quote.顧客名 ?? " ");
            paoRep.Write("敬称", Quote.敬称 ?? " ");



            //ボディ
            paoRep.Write("件名", (Quote.顧客名 ?? " ") + " " + (Quote.件名 ?? " "));
            paoRep.Write("作業期間", " ");


            //フッター
            paoRep.Write("社名", Houzin.社名 ?? " ");
            paoRep.Write("代表名", "代表取締役　" + (Houzin.代表名 ?? " "));
            paoRep.Write("郵便番号", "〒" + (Houzin.郵便番号 ?? " "));
            paoRep.Write("住所", Houzin.住所 ?? " ");
            paoRep.Write("TEL", "TEL " + (Houzin.TEL ?? " ") + "　FAX " + (Houzin.FAX ?? " "));
            paoRep.Write("オフィス", "オフィス " + (Houzin.オフィス ?? " "));


            paoRep.PageEnd();

            //PDFを一時保存
            string pdfPath = Path.Combine(Directory.GetCurrentDirectory(), @"pdf/作業完了報告書" + DateTime.Now.ToString("yyMMddHHmm") + ".pdf");
            paoRep.SavePDF(pdfPath);

            //保存したPDFをfsに格納
            var fs = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);
            var contentType = "application/pdf";

            //ブラウザにPDFを表示
            return new FileStreamResult(fs, contentType);

        }


    }
}


