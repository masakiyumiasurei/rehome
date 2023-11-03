using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using rehome.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace MvcCoreApp.Controllers
{
    public class UploadController : Controller
    {
        // Core では Server.MapPath が使えないことの対応
        private readonly IWebHostEnvironment _hostingEnvironment;

        public UploadController(
                        IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet("/fileupload")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult test()
        {
            return View();
        }

        [HttpPost("/fileupload")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(UploadModel model)
        {
            //IIS でも Kestrel も最大要求本文サイズに 30,000,000 バイトの制限がある
            //ASP.NET Core MVC 組み込みの CSRF 防止機能は Ajax でもそのまま使えるので、Controller のアクションメソッドへの [ValidateAntiForgeryToken] を忘れずに設定する
            string result = "";
            IFormFile postedFile = model.PostedFile;
            if (postedFile != null && postedFile.Length > 0)
            {
                // アップロードされたファイル名を取得。ブラウザが IE 
                // の場合 postedFile.FileName はクライアント側でのフ
                // ルパスになることがあるので Path.GetFileName を使う
                string filename =
                              Path.GetFileName(postedFile.FileName);

                // アプリケーションルートの物理パスを取得。Core では
                // Server.MapPath は使えないので以下のようにする
                string contentRootPath =
                                _hostingEnvironment.ContentRootPath;
                string filePath = contentRootPath + "\\" +
                                  "UploadedFiles\\" + filename;

                using (var stream =
                            new FileStream(filePath, FileMode.Create))
                {
                    await postedFile.CopyToAsync(stream);
                }

                result = filename + " (" + postedFile.ContentType +
                         ") - " + postedFile.Length +
                         " bytes アップロード完了";
            }
            else
            {
                result = "ファイルアップロードに失敗しました";
            }

            // Core では Request.IsAjaxRequest() は使えない
            if (Request.Headers["X-Requested-With"] ==
                                                "XMLHttpRequest")
            {
                return Content(result);
            }
            else
            {
                ViewBag.Result = result;
                return View();
            }
        }
    }
}