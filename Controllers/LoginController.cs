using rehome.Models;
using rehome.Services;
using rehome.Public;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace rehome.Controllers
{
    public class LoginController : Controller
    {

        private readonly ILogger<LoginController> _logger;
        private ITantouService _TantouService;

        public LoginController(ILogger<LoginController> logger, IConfiguration configuration, ITantouService TantouService)
        {
            _logger = logger;
            _TantouService = TantouService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(Login user)
        {
            if (user == null)
            {
                ViewBag.OperationMessage = String.Format("ID、パスワードを入力してください");
                return View("Login");
            }

            // ユーザー名が一致するユーザーを抽出
            var lookupUser = _TantouService.LoginTantou(user.ログインID);
            if (lookupUser == null)
            {
                ViewBag.OperationMessage = String.Format("ユーザーが見当たりません");
                return View("Login");
            }

            //パスワードの比較
            if (lookupUser?.pass != PublicClass.GetSHA256HashedString(user.Pass))
            {
                ViewBag.OperationMessage = String.Format("パスワードが違います");
                return View("Login");
            }

            // Cookies 認証スキームで新しい ClaimsIdentity を作成し、ユーザー名を追加します。
            var identity = new ClaimsIdentity("rehomeAuthenticationScheme");
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, lookupUser.担当ID.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, lookupUser.氏名));
            identity.AddClaim(new Claim(ClaimTypes.GroupSid, lookupUser.営業所ID.ToString()));
            identity.AddClaim(new Claim("assistant", lookupUser.assistant.ToString())); // カスタムのクレームを追加
            //identity.AddClaim(new Claim(ClaimTypes.Locality, lookupUser.所属名));
            identity.AddClaim(new Claim(ClaimTypes.Role, lookupUser.auth.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.System, lookupUser.admin.ToString()));

            // クッキー認証スキームと、上の数行で作成されたIDから作成された新しい ClaimsPrincipal を渡します。
            await HttpContext.SignInAsync("rehomeAuthenticationScheme", new ClaimsPrincipal(identity));


            return RedirectToAction("Clear", "Quote");

            //ViewBag.OperationMessage = String.Format("ログイン成功！");
            //return View("Login");


        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("rehomeAuthenticationScheme");

            return RedirectToAction("Login");
        }
    }
}