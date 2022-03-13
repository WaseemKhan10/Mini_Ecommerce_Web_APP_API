using Microsoft.AspNetCore.Identity;
using Mini_Ecommerce_Comsuming_APIS.Models;

namespace Mini_Ecommerce_Comsuming_APIS.ViewModels
{
    public class AccountViewModel:IAccountViewModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public AccountViewModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<int> Login(LoginViewModel user)
        {
            var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, user.RememberMe, false);
            if (result.Succeeded)
            {
                return 1;
            }
            return 0;
        }
        public async Task<int> Logincheckout(CombinedModel user)
        {
            var result = await _signInManager.PasswordSignInAsync(user.LVM.Email, user.LVM.Password, user.LVM.RememberMe, false);
            if (result.Succeeded)
            {
                return 1;
            }
            return 0;
        }
        public async Task<int> Register(RegisterViewModel model)
        {
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);

                return 1;
            }

            return 0;
        }
        public async Task<int> Logout()
        {
            await _signInManager.SignOutAsync();
            return 1;
        }
    }
}
