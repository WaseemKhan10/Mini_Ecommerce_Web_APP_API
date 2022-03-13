using Microsoft.AspNetCore.Identity;
using Mini_Ecommerce_Comsuming_APIS.Helper;
using Mini_Ecommerce_Comsuming_APIS.Models;
using Newtonsoft.Json;

namespace Mini_Ecommerce_Comsuming_APIS.ViewModels
{
    public class CouponViewModel : ICouponViewModel
    {
        private IHttpContextAccessor _httpContextAccessor;

        public const string SessionKeyName = "_Name";
        AppDBContext _context;
        IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        HttpClient? client;
        APIRequest APIURL;
        public CouponViewModel(UserManager<IdentityUser> userManager, AppDBContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
            APIURL = new APIRequest(configuration);
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<int> Saving(CouponCodeModel ccm)
        {
            client = APIURL.initialCoupon();
            var postTask = client.PostAsJsonAsync<CouponCodeModel>("", ccm);
            postTask.Wait();
            var result = postTask.Result;
            if (result.IsSuccessStatusCode)
            {
                return 1;
            }
            return 0;
        }
        public async Task<CouponCodeModel> Details(string couponCode)
        {
            var CouponCodeModelobj = new CouponCodeModel();
            client = APIURL.initialCoupon();
            HttpResponseMessage res = await client.GetAsync("" + couponCode);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result.ToString();
                CouponCodeModelobj = JsonConvert.DeserializeObject<CouponCodeModel>(result);
            }
            return CouponCodeModelobj;
        }
    }
}
