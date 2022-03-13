namespace Mini_Ecommerce_Comsuming_APIS.Helper
{
    public class APIRequest
    {
        IConfiguration _configuration;
        string URL;
        string URLProduct;
        string URLCart;
        string URLCoupon;
        string URLDelivery;
        string URLMultipleImage;
        public APIRequest(IConfiguration configuration)
        {
            _configuration = configuration;
            URL = _configuration.GetValue<string>("HelperUrls:RequestURL");
            URLProduct = URL + "products/";
            URLCart = URL + "Cart/";
            URLCoupon = URL + "CouponCode/";
            URLDelivery = URL + "DeliveryInformation/";
            URLMultipleImage = URL + "MultipleImages/";
        }
        public HttpClient initialProducts()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(URLProduct);
            return client;
        }
        public HttpClient initialCart()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(URLCart);
            return client;
        }
        public HttpClient initialCoupon()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(URLCoupon);
            return client;
        }
        public HttpClient initialDeliveryInfo()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(URLDelivery);
            return client;
        }
        public HttpClient initialMultiple()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(URLMultipleImage);
            return client;
        }
    }
}
