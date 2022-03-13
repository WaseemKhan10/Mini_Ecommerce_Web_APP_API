using Microsoft.AspNetCore.Identity;
using Mini_Ecommerce_Comsuming_APIS.Helper;
using Mini_Ecommerce_Comsuming_APIS.Models;
using Newtonsoft.Json;

namespace Mini_Ecommerce_Comsuming_APIS.ViewModels
{
    public class ProductViewModel : IProductsViewModel
    {
        private IHttpContextAccessor _httpContextAccessor;
        public const string SessionKeyName = "_Name";
        AppDBContext _context;
        IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        HttpClient? client;
        APIRequest APIURL;
        public ProductViewModel(UserManager<IdentityUser> userManager, AppDBContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
            APIURL = new APIRequest(configuration);
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<ProductModel>> Index()
        {
            if (_userManager.GetUserId(_httpContextAccessor.HttpContext.User) == null)
            {
                string key = "RandomID";
                if (_httpContextAccessor.HttpContext.Request.Cookies["RandomID"] == null)
                {
                    string value = Guid.NewGuid().ToString();
                    CookieOptions cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(10)
                    };
                    _httpContextAccessor.HttpContext.Response.Cookies.Append(key, value, cookieOptions);
                }
            }
            List<ProductModel> ProductList = new List<ProductModel>();
            client = APIURL.initialProducts();
            HttpResponseMessage res = await client.GetAsync("");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                ProductList = JsonConvert.DeserializeObject<List<ProductModel>>(result);
            }
            return ProductList;
        }
        string productId = "";
        public async Task<int> Saving(List<IFormFile> files, ProductModel mp)
        {
            foreach (IFormFile postedfiles in files)
            {

                if (postedfiles != null)
                {
                    if (postedfiles.Length > 0)
                    {
                        var fileName = Path.GetFileName(postedfiles.FileName);
                        var fileExtension = Path.GetExtension(fileName);
                        var newFileName = String.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);
                        using (var target = new MemoryStream())
                        {
                            postedfiles.CopyTo(target);
                            mp.Image = target.ToArray();
                            client = APIURL.initialProducts();
                            var postTask = await client.PostAsJsonAsync<ProductModel>("", mp);
                            using (HttpContent content1 = postTask.Content)
                            {
                                string productId1 = await content1.ReadAsStringAsync();
                                if (productId != null)
                                {
                                    productId = productId1;
                                    goto outofLoop;
                                }
                            }

                        }
                    }
                }
            }
        outofLoop:
            return Convert.ToInt32(productId);
        }
        int result = 0;
        public async Task<int> SavingMultipleImages(List<IFormFile> files, MultipleImagesModel mim)
        {
            foreach (IFormFile postedfiles in files)
            {
                if (postedfiles != null)
                {
                    if (postedfiles.Length > 0)
                    {
                        var fileName = Path.GetFileName(postedfiles.FileName);
                        var fileExtension = Path.GetExtension(fileName);
                        var newFileName = String.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);
                        using (var target = new MemoryStream())
                        {
                            postedfiles.CopyTo(target);
                            mim.Image = target.ToArray();
                            client = APIURL.initialMultiple();
                            await client.PostAsJsonAsync<MultipleImagesModel>("", mim);

                        }
                    }
                }
            }
            return 1;
        }
        public async Task<ProductModel> Details(int id)
        {
            var ProductDetails = new CombinedModel();
            client = APIURL.initialProducts();
            HttpResponseMessage res = await client.GetAsync("" + id);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result.ToString();
                ProductDetails.PM = JsonConvert.DeserializeObject<ProductModel>(result);
            }
            return ProductDetails.PM;
        }
        public async Task<ProductModel> Detailsbuynow(int id)
        {
            CombinedModel CM = new CombinedModel();
            client = APIURL.initialProducts();
            HttpResponseMessage res = await client.GetAsync("" + id);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result.ToString();
                CM.PM = JsonConvert.DeserializeObject<ProductModel>(result);
            }
            return CM.PM;
        }
        public async Task<int> SavingCart(CombinedModel cm)
        {
            string name = "";
            CartDetailsModel cdm = new CartDetailsModel();
            if (_userManager.GetUserId(_httpContextAccessor.HttpContext.User) != null)
            {
                _httpContextAccessor.HttpContext.Session.SetString(SessionKeyName, _userManager.GetUserId(_httpContextAccessor.HttpContext.User));
            }
            name = _httpContextAccessor.HttpContext.Session.GetString(SessionKeyName);

            if (name == null)
            {
                if (_httpContextAccessor.HttpContext.Request.Cookies["RandomID"] != null)
                {
                    name = _httpContextAccessor.HttpContext.Request.Cookies["RandomID"];
                }
            }

            var detailIDs = _context.cartdetailss.Where(c => c.ProductID == cm.PM.ProductID && c.UserID == name).SingleOrDefault();
            if (detailIDs != null)
            {
                cm.PM.DetailsID = detailIDs.DetailsID;
                cm.PM.LineTotal = Convert.ToDecimal(cm.PM.Price) * Convert.ToDecimal(cm.PM.Quantity);
                cm.PM.LineTotal = Convert.ToDecimal(detailIDs.LineTotal) + Convert.ToDecimal(cm.PM.LineTotal);
                cm.PM.Quantity = (Convert.ToDecimal(detailIDs.Quantity) + Convert.ToDecimal(cm.PM.Quantity)).ToString();
                cdm.DetailsID = cm.PM.DetailsID;
                cdm.LineTotal = cm.PM.LineTotal;
                cdm.Quantity = cm.PM.Quantity;
                cdm.UserID = name;
                client = APIURL.initialCart();
                var postTaskCart = client.PutAsJsonAsync("", cdm);
                postTaskCart.Wait();
                var resultcart = postTaskCart.Result;
                if (resultcart.IsSuccessStatusCode)
                {
                    return 1;
                }
            }
            else
            {
                cm.PM.UserID = name;
                var linetot = Convert.ToDecimal(cm.PM.Price) * Convert.ToDecimal(cm.PM.Quantity);
                cm.PM.LineTotal = linetot;
                client = APIURL.initialCart();
                var postTaskproduct = client.PostAsJsonAsync<ProductModel>("", cm.PM);
                postTaskproduct.Wait();
                var resultproduct = postTaskproduct.Result;
                if (resultproduct.IsSuccessStatusCode)
                {
                    return 1;
                }
            }
            return 0;
        }
        public async Task<int> Delete(int id)
        {
            client = APIURL.initialProducts();
            HttpResponseMessage res = await client.DeleteAsync("" + id);
            if (res.IsSuccessStatusCode)
            {
                return 1;
            }
            return 0;
        }
        public async Task<List<ProductModel>> getJoinedlist(string id)
        {
            string name = "";
            var CM = new CombinedModel();
            if (_userManager.GetUserId(_httpContextAccessor.HttpContext.User) != null)
            {
                _httpContextAccessor.HttpContext.Session.SetString(SessionKeyName, _userManager.GetUserId(_httpContextAccessor.HttpContext.User));
            }
            name = _httpContextAccessor.HttpContext.Session.GetString(SessionKeyName);
            if (name == null)
            {
                name = _httpContextAccessor.HttpContext.Request.Cookies["RandomID"];
            }
            id = name;
            client = APIURL.initialCart();
            HttpResponseMessage res = await client.GetAsync("" + id);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                CM.ProductList = JsonConvert.DeserializeObject<List<ProductModel>>(result);
            }
            return CM.ProductList;
        }
        public async Task<List<MultipleImagesModel>> GetMultipleImages(int id)
        {
            var CM = new CombinedModel();

            client = APIURL.initialMultiple();
            HttpResponseMessage res = await client.GetAsync("" + id);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                CM.MIM = JsonConvert.DeserializeObject<List<MultipleImagesModel>>(result);
            }
            return CM.MIM;
        }
        public async Task<List<ProductModel>> getJoinedlistCheckout(string id)
        {

            var CM = new CombinedModel();
            if (_userManager.GetUserId(_httpContextAccessor.HttpContext.User) != null)
            {
                _httpContextAccessor.HttpContext.Session.SetString(SessionKeyName, _userManager.GetUserId(_httpContextAccessor.HttpContext.User));
            }
            else
            {
                goto PleaseLogin;
            }
            var name = _httpContextAccessor.HttpContext.Session.GetString(SessionKeyName);
            id = name;
            client = APIURL.initialCart();
            HttpResponseMessage res = await client.GetAsync("" + id);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                CM.ProductList = JsonConvert.DeserializeObject<List<ProductModel>>(result);
            }
            return CM.ProductList;
        PleaseLogin:
            return CM.ProductList;
        }
        public async Task<int> RemoveFromCart(int id)
        {
            string name = "";
            if (_userManager.GetUserId(_httpContextAccessor.HttpContext.User) != null)
            {
                _httpContextAccessor.HttpContext.Session.SetString(SessionKeyName, _userManager.GetUserId(_httpContextAccessor.HttpContext.User));
            }
            name = _httpContextAccessor.HttpContext.Session.GetString(SessionKeyName);
            if (name == null)
            {
                name = _httpContextAccessor.HttpContext.Request.Cookies["RandomID"];
            }
            var detailIDs = _context.cartdetailss.Where(c => Convert.ToInt32(c.ProductID) == id && c.UserID == name).SingleOrDefault();
            int DetailID = detailIDs.DetailsID;
            client = APIURL.initialCart();
            HttpResponseMessage res = await client.DeleteAsync("" + DetailID);
            return 1;
        }
        public async Task<int> UpdateCart(int id, decimal linetotal, decimal price, string quantity)
        {
            CartDetailsModel cd = new CartDetailsModel();
            string name = "";
            if (_userManager.GetUserId(_httpContextAccessor.HttpContext.User) != null)
            {
                _httpContextAccessor.HttpContext.Session.SetString(SessionKeyName, _userManager.GetUserId(_httpContextAccessor.HttpContext.User));
            }
            name = _httpContextAccessor.HttpContext.Session.GetString(SessionKeyName);
            if (name == null)
            {
                name = _httpContextAccessor.HttpContext.Request.Cookies["RandomID"];
            }
            var detailIDs = _context.cartdetailss.Where(c => Convert.ToInt32(c.ProductID) == id && c.UserID == name).SingleOrDefault();
            cd.DetailsID = detailIDs.DetailsID;
            cd.LineTotal = Convert.ToDecimal(price) * Convert.ToDecimal(quantity);
            cd.Quantity = quantity;
            var UpdateDATA = (from p in _context.cartdetailss where p.DetailsID == cd.DetailsID select p);
            foreach (var UpdateDATAGet in UpdateDATA)
            {
                UpdateDATAGet.Quantity = cd.Quantity;
                UpdateDATAGet.LineTotal = cd.LineTotal;
            }
            _context.SaveChanges();
            return 1;
        }
    }
}
