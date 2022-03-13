using Microsoft.AspNetCore.Identity;
using Mini_Ecommerce_Comsuming_APIS.Helper;
using Mini_Ecommerce_Comsuming_APIS.Models;

namespace Mini_Ecommerce_Comsuming_APIS.ViewModels
{
    public class DeliveryInformationViewModel : IDeliveryInformationViewModel
    {
        private IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;
        public const string SessionKeyName = "_Name";
        AppDBContext _context;
        IConfiguration _configuration;
        HttpClient? client;
        APIRequest APIURL;
        public DeliveryInformationViewModel(UserManager<IdentityUser> userManager, AppDBContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _configuration = configuration;
            APIURL = new APIRequest(configuration);
        }
        public async Task<int> Saving(CombinedModel cm)
        {
            if (string.IsNullOrEmpty(_httpContextAccessor.HttpContext.Session.GetString(SessionKeyName)))
            {
                _httpContextAccessor.HttpContext.Session.SetString(SessionKeyName, _userManager.GetUserId(_httpContextAccessor.HttpContext.User));
            }
            var name = _httpContextAccessor.HttpContext.Session.GetString(SessionKeyName);
            client = APIURL.initialDeliveryInfo();
            cm.DIM.UserId = name;
            var postTask =await client.PostAsJsonAsync<DeliveryInformationModel>("", cm.DIM);
            if (postTask.IsSuccessStatusCode)
            {
                return 1;
            }
            return 0;
        }
        public async Task<int> UpdateUserID(CartDetailsModel cdm)
        {
            string? name = "";
            _httpContextAccessor.HttpContext.Session.SetString(SessionKeyName, _userManager.GetUserId(_httpContextAccessor.HttpContext.User));
            name = _httpContextAccessor.HttpContext.Session.GetString(SessionKeyName);
            cdm.UserID = name;
            cdm.Quantity = _httpContextAccessor.HttpContext.Request.Cookies["RandomID"].ToString();
            client = APIURL.initialDeliveryInfo();
            var postTaskCart = await client.PutAsJsonAsync("", cdm);
            if (postTaskCart.IsSuccessStatusCode)
            {
                return 1;
            }
            return 0;
        }
    }
}
