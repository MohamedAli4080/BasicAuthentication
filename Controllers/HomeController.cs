using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BasicAuthentication.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _usermanger;
        private readonly SignInManager<IdentityUser> _signInManger;

        public HomeController(UserManager<IdentityUser> usermanger, SignInManager<IdentityUser> signInManger)
        {
            this._signInManger = signInManger;
            this._usermanger = usermanger;

        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }
        // public IActionResult Authenticate()
        // {

        //     var grandmaClamis = new List<Claim>(){
        //         new Claim(ClaimTypes.Name,"Bob"),
        //         new Claim(ClaimTypes.Email,"bob@gmail.com"),
        //         new Claim("Grandma.says","nice boy")
        //     };

        //     var grandmaIdentity = new ClaimsIdentity(grandmaClamis, "Grandma identity");
        //     var userPrinciple = new ClaimsPrincipal(new[] { grandmaIdentity });
        //     HttpContext.SignInAsync(userPrinciple);
        //     return RedirectToAction(nameof(Index));
        // }


        public IActionResult login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> login(string userName, string password)
        {
            var user = await _usermanger.FindByNameAsync(userName);
            if (user != null)
            {
                var signInResult = await _signInManger.PasswordSignInAsync(user, password, false, false);
                if (signInResult.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string userName, string password)
        {
            var user = new IdentityUser()
            {
                UserName = userName,
                Email = ""
            };
            var result = await _usermanger.CreateAsync(user, password);
            if (result.Succeeded)
            {
                if (user != null)
                {
                    var signInResult = await _signInManger.PasswordSignInAsync(user, password, false, false);
                    if (signInResult.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManger.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}