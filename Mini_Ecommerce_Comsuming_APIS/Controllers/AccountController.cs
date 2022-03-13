using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mini_Ecommerce_Comsuming_APIS.Models;
using Mini_Ecommerce_Comsuming_APIS.ViewModels;

namespace Mini_Ecommerce_Comsuming_APIS.Controllers
{
    public class AccountController : Controller
    {
        public IAccountViewModel _AccountViewModel { get; }
        public AccountController(IAccountViewModel AccountViewModel)
        {
            _AccountViewModel = AccountViewModel;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _AccountViewModel.Register(model);
                if (result == 1)
                {
                    return RedirectToAction("Login", "Account");
                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                var login = await _AccountViewModel.Login(user);
                if (login == 1)
                {
                    return RedirectToAction("Index", "Product");
                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }
            return View(user);
        }
        public async Task<IActionResult> Logout()
        {
            await _AccountViewModel.Logout();
            return RedirectToAction("privacy", "Home");
        }
    }
}
