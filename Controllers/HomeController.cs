using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasicAuthentication.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }
        public IActionResult Authenticate()
        {

            var grandmaClamis = new List<Claim>(){
                new Claim(ClaimTypes.Name,"Bob"),
                new Claim(ClaimTypes.Email,"bob@gmail.com"),
                new Claim("Grandma.says","nice boy")
            };

            var grandmaIdentity = new ClaimsIdentity(grandmaClamis, "Grandma identity");
            var userPrinciple = new ClaimsPrincipal(new[] { grandmaIdentity });
            HttpContext.SignInAsync(userPrinciple);
            return RedirectToAction(nameof(Index));
        }

    }
}