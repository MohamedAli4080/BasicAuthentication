using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;

namespace BasicAuthentication.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _usermanger;
        private readonly SignInManager<IdentityUser> _signInManger;
        private readonly IEmailService _emailSender;

        public HomeController(UserManager<IdentityUser> usermanger,
        SignInManager<IdentityUser> signInManger,
        IEmailService EmailSender)
        {
            this._emailSender = EmailSender;
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
                // # send email confirmation to user to verfiy his email in case we enable 
                //   required singin confiramtion email option
                var code = await _usermanger.GenerateEmailConfirmationTokenAsync(user);

                var link = Url.Action(nameof(VerfiyEmail), "Home", new { userId = user.Id, code = code },Request.Scheme,Request.Host.ToString());
                await _emailSender.SendAsync("test@testto.com","Confirm Email",$"<a href=\"{link}\">VerfiyEmail</a>",true);


                // # Inform user that he has to verfiy his email
                return RedirectToAction(nameof(EmailVerifaction));

                // if (user != null)
                // {
                //     var signInResult = await _signInManger.PasswordSignInAsync(user, password, false, false);
                //     if (signInResult.Succeeded)
                //     {
                //         return RedirectToAction(nameof(Index));
                //     }
                // }
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult EmailVerifaction() => View();

        public async Task<IActionResult> VerfiyEmail(string userId, string code)
        {

            var user=await _usermanger.FindByIdAsync(userId);
            if (user==null )return BadRequest();

           var result=await _usermanger.ConfirmEmailAsync(user,code);
           if (result.Succeeded)
           {
            return View();   
           }else{
               return BadRequest();
           }
            
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManger.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}