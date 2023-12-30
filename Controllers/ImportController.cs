using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using rehome.Models;
using System.Formats.Asn1;
using System.Globalization;
using System.Text;
using rehome.Models.DB;
using rehome.Services;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace rehome.Controllers
{
    public class ImportController : Controller
    {
        // Core では Server.MapPath が使えないことの対応
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IImportService _importService;
        private readonly ISyouhinService _SyouhinService;
        private readonly string _connectionString;
        public ImportController(IImportService importService, IConfiguration configuration,
            ISyouhinService SyouhinService,IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _importService = importService;
            _SyouhinService = SyouhinService;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue,ValueLengthLimit = int.MaxValue)]
        public ActionResult ImportFile(IFormFile input_file)
        {
            //    // ファイルがアップロードされていない場合はエラーを返す
            if (input_file == null || input_file.Length == 0)
            {
                return BadRequest("ファイルがアップロードされていません。");
                //TempData["import"] = "ファイルがありませんでした。";
                //return RedirectToAction("Index", "Quote");
            }
            try
            {
                            
                using (var reader = new StreamReader(input_file.OpenReadStream(), Encoding.GetEncoding("Shift-JIS")))
                {
                    List<商品> imports = new List<商品>();

                    bool isFirstLine = true; // 1行目かどうかを示すフラグ

                    while (!reader.EndOfStream)
                    {


                        var line = reader.ReadLine();
                        //var values = line.Split('\t'); //タブ文字で区切られていたので
                        var values = line.Split(','); //カンマに変更

                        if (isFirstLine)
                        {
                            isFirstLine = false;
                            continue; // 1行目はスキップして次の行へ
                        }

                        // カンマによる分割を行わず、そのまま1行を処理
                        // ダブルクオートで囲まれた部分を正しく取り扱うための処理
                        List<string> processedValues = new List<string>();
                        bool insideQuotes = false;
                        string currentValue = "";

                        foreach (var value in values)
                        {
                            if (insideQuotes)
                            {
                                currentValue += "," + value;
                                if (value.EndsWith("\""))
                                {
                                    processedValues.Add(to_str(currentValue));//Add(currentValue.Trim('"'));
                                    insideQuotes = false;
                                    currentValue = "";
                                }
                            }
                            else
                            {
                                if (value.StartsWith("\"") && !value.EndsWith("\""))
                                {
                                    insideQuotes = true;
                                    currentValue = value.Substring(1);
                                }
                                else
                                {                                    
                                    processedValues.Add(to_str(value));
                                }
                            }
                        }

                        if (insideQuotes)
                        {
                            // 最後の項目がダブルクォーテーションで終わっていない場合、次の行の値と連結する
                            line = reader.ReadLine();
                            if (line != null)
                            {
                                values = line.Split(',');
                                currentValue += "," + values[0];
                                processedValues.Add(to_str(currentValue));//Add(currentValue.Trim('"'));
                            }
                        }

                        if (values.Length >= 6)　
                        {
                            var Syouhin = new 商品
                            {
                                商品ID = int.TryParse(processedValues[0],out int val0) ? val0 : 0,                                
                                商品名 = processedValues[1],
                                単価 = int.TryParse(processedValues[2], out int val2) ? val2 : 0,
                                単位 = processedValues[3],
                                仕入額 = int.TryParse(processedValues[4], out int val4) ? val4 : 0,
                               
                                //仕入掛率 = decimal.TryParse(processedValues[10], out decimal val10) ? val10 : 0,                                
                                削除フラグ = bool.TryParse(processedValues[5], out bool val5) ? val5 : false
                            };
                            imports.Add(Syouhin);                        
                        }
                        else
                        {
                            TempData["import"] = "取込処理に失敗しました。取込ファイルが誤っていませんか?改行が入った項目がありませんか？確認してください!";
                            return RedirectToAction("Index");
                        }
                    }
                    
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        //connection.Open();
                     //   SqlTransaction transaction = connection.BeginTransaction();
                        try
                        {
                            //取込んだファイル情報をインサート
                            _importService.InsertData(imports);                                                        

                          //  transaction.Commit();
                        }
                        catch (Exception ex) {
                        
                         //   transaction.Rollback();
                            TempData["import"] = "取込処理に失敗しました => " + ex.Message;
                            ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");
                            return RedirectToAction("Index");
                        }
                    }

                    TempData["import"] = "商品マスタファイルのアップロードを完了しました";
                    return View("Index");
                }
            }
            catch (System.Exception ex)
            {
                TempData["import"] = "アップロードファイルが不正です。"+ ex.Message;
                ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");
                return RedirectToAction("Index");
            }

            //前後のダブルクォーテーションを削除し、2個連続するダブルクォーテーションを1個に置換する
            //string to_str(StringBuilder p_str)
             string to_str(string p_str)
            {
                string l_val = p_str.ToString().Replace("\"\"", "\"");
                int l_start = (l_val.StartsWith("\"")) ? 1 : 0;
                int l_end = l_val.EndsWith("\"") ? 1 : 0;
                //カラムが空の時にマイナスになるため
                int resultLength = l_val.Length - l_start - l_end;
                if (resultLength < 0)
                {
                    return "";
                }
                    return l_val.Substring(l_start, l_val.Length - l_start - l_end);
            }

        }

        [HttpGet]
        public ActionResult ExportCSV()
        {
            IList<商品> viewModel = new List<商品>();
            viewModel = _SyouhinService.ListSyouhins();

            // CSVファイルの内容を文字列として構築します
            StringBuilder csvContent = new StringBuilder();
            csvContent.AppendLine("商品ID,商品名,単価,単位,仕入額,削除フラグ"); // ヘッダ行

            foreach (var product in viewModel)
            {
                csvContent.AppendLine(product.ToCsvString());
            }

            // CSVファイルの内容を文字列として取得します
            string csvFileName = "商品マスタ.csv";
            byte[] csvData = Encoding.GetEncoding("Shift-JIS").GetBytes(csvContent.ToString());

            var response = new FileContentResult(csvData, "text/csv")
            {
                FileDownloadName = csvFileName
            };

            return response;

        }




        [HttpPost]
        public IActionResult ImportFileChunk(IFormFile fileChunk, string fileName, long totalSize)
        {
            // ファイルがアップロードされていない場合はエラーを返す
            if (fileChunk == null || fileChunk.Length == 0)
            {
                return BadRequest("ファイルがアップロードされていません。");
            }

            // 分割されたファイルのデータをサーバーに保存するか、適切な処理を行う
            // ここでは例として、ファイルを一時的なディレクトリに保存しています
            string tempDir = "TempFiles";
            string tempFilePath = Path.Combine(tempDir, fileName);

            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            using (var stream = new FileStream(tempFilePath, FileMode.Append))
            {
                fileChunk.CopyTo(stream);
            }

            // 最後のチャンクであれば、ファイルを結合して処理を行う
            if (fileChunk.Length + totalSize == new FileInfo(tempFilePath).Length)
            {
                ProcessUploadedFile(tempFilePath);
                // ここでファイルの内容を処理する必要があります
                // ProcessUploadedFileメソッドは、ファイルの内容を適切に処理するために自身で実装する必要があります
            }

            return Ok(); // 成功を示すレスポンスを返す
        }

        private void ProcessUploadedFile(string filePath)
        {
            // ファイルの内容を適切に処理する
            // 例：データベースに保存する、別のサービスに送信する、などの処理を行う
            // ここでは、ファイルを削除するだけとします
            System.IO.File.Delete(filePath);
        }
    

                //[HttpPost]
                //public async Task<IActionResult> ImportFile(IFormFile input_file)
                //{
                //    // ファイルがアップロードされていない場合はエラーを返す
                //    if (input_file == null || input_file.Length == 0)
                //    {
                //        //return BadRequest("ファイルがアップロードされていません。");
                //        //TempData["PaidVacation"] = "ファイルがありませんでした。";
                //        return RedirectToAction("Index", "Quote");
                //    }
                //    try
                //    {
                //        // CSVファイルの内容をモデルにバインドする
                //        using (var reader = new StreamReader(input_file.OpenReadStream(),
                //        Encoding.GetEncoding("shift_jis")))
                //        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                //        {

                //          //  TempData["PaidVacation"] = "ファイルがアップロードされました。";
                //           // string OperationMsg = (string)TempData["PaidVacation"];
                //            var viewModel = new PaidVacationDetailModel();
                //            csv.Context.RegisterClassMap<CSV勤怠有給残Map>();
                //            viewModel.CSVPaidVacations = csv.GetRecords<CSV勤怠有給残>().ToList();


                //            DateTime StartDay;
                //            DateTime EndDay;

                //            if (DateTime.Now.Month < 10)
                //            {
                //                StartDay = new DateTime(DateTime.Now.Year, 10, 1).AddYears(-1);
                //                EndDay = new DateTime(DateTime.Now.Year, 9, 30);
                //            }
                //            else
                //            {
                //                StartDay = new DateTime(DateTime.Now.Year, 10, 1);
                //                EndDay = new DateTime(DateTime.Now.Year, 9, 30).AddYears(1);
                //            }

                //            viewModel.期首日 = StartDay.ToString("yyyy年MM月dd日");
                //            viewModel.期末日 = EndDay.ToString("yyyy年MM月dd日");

                //            ViewBag.OperationMessage = OperationMsg;
                //            return View("Details", viewModel);
                //        }
                //    }
                //    catch (System.Exception ex)
                //    {
                //        TempData["PaidVacation"] = "アップロードファイルが不正です。";
                //        ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");
                //        return RedirectToAction("Details", "PaidVacation");
                //    }
                //}

                //[HttpPost]
                ////[ValidateAntiForgeryToken]
                //[AutoValidateAntiforgeryToken]
                //public ActionResult RegistCSV(PaidVacationDetailModel model)
                //{
                //    if (!ModelState.IsValid)
                //    {
                //        return View(model);
                //    }
                //    string CautionMsg = "";
                //    var viewModel = new PaidVacationDetailModel();
                //    try
                //    {
                //        if (model.CSVPaidVacations != null)
                //        {
                //            foreach (var row in model.CSVPaidVacations)
                //            {
                //                //viewModel.PaidVacations = _PaidVacationService.RegistCSVPaidVacations(model);
                //                //TempData["PaidVacation"] = String.Format("CSVから有給休暇残日数を更新しました");
                //                CautionMsg += _PaidVacationService.RegistCSVPaidVacations(row);
                //            }
                //            //ModelState.Clear();
                //            //return RedirectToAction("Details", "PaidVacation");
                //            ViewBag.OperationMessage = "登録しました";
                //            ViewBag.CautionMessage = CautionMsg;
                //            return View("Details", model);
                //        }
                //        else
                //        {
                //            CautionMsg = "更新内容がありません";
                //            ViewBag.CautionMessage = CautionMsg;
                //            return View("Details", model);
                //        }
                //    }
                //    catch (System.Exception ex)
                //    {
                //        ModelState.AddModelError("", $"問題が発生しました。[{ex.Message}]");

                //        ViewBag.OperationMessage = String.Format("更新できませんでした");
                //        return RedirectToAction("Details", "PaidVacation");
                //        //return View(model);
                //    }
                //}




        //[HttpPost("/fileupload")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Index(UploadModel model)
        //{
        //    //IIS でも Kestrel も最大要求本文サイズに 30,000,000 バイトの制限がある
        //    //ASP.NET Core MVC 組み込みの CSRF 防止機能は Ajax でもそのまま使えるので、Controller のアクションメソッドへの [ValidateAntiForgeryToken] を忘れずに設定する
        //    string result = "";
        //    IFormFile postedFile = model.PostedFile;
        //    if (postedFile != null && postedFile.Length > 0)
        //    {
        //        // アップロードされたファイル名を取得。ブラウザが IE 
        //        // の場合 postedFile.FileName はクライアント側でのフ
        //        // ルパスになることがあるので Path.GetFileName を使う
        //        string filename =
        //                      Path.GetFileName(postedFile.FileName);

        //        // アプリケーションルートの物理パスを取得。Core では
        //        // Server.MapPath は使えないので以下のようにする
        //        string contentRootPath =
        //                        _hostingEnvironment.ContentRootPath;
        //        string filePath = contentRootPath + "\\" +
        //                          "UploadedFiles\\" + filename;

        //        using (var stream =
        //                    new FileStream(filePath, FileMode.Create))
        //        {
        //            await postedFile.CopyToAsync(stream);
        //        }

        //        result = filename + " (" + postedFile.ContentType +
        //                 ") - " + postedFile.Length +
        //                 " bytes アップロード完了";
        //    }
        //    else
        //    {
        //        result = "ファイルアップロードに失敗しました";
        //    }

        //    // Core では Request.IsAjaxRequest() は使えない
        //    if (Request.Headers["X-Requested-With"] ==
        //                                        "XMLHttpRequest")
        //    {
        //        return Content(result);
        //    }
        //    else
        //    {
        //        ViewBag.Result = result;
        //        return View();
        //    }
        //}
    }
}