using Mini_Ecommerce_Comsuming_APIS.Models;

namespace Mini_Ecommerce_Comsuming_APIS.ViewModels
{
    public interface IProductsViewModel
    {
        Task<List<ProductModel>> Index();
        Task<int> Saving(List<IFormFile> files, ProductModel mp);
        Task<int> SavingMultipleImages(List<IFormFile> files, MultipleImagesModel mim);
        Task<ProductModel> Details(int id);
        Task<ProductModel> Detailsbuynow(int id);
        Task<int> SavingCart(CombinedModel mp);
        Task<int> Delete(int id);
        Task<List<ProductModel>> getJoinedlist(string id);
        Task<List<ProductModel>> getJoinedlistCheckout(string id);
        Task<int> RemoveFromCart(int id);
        Task<int> UpdateCart(int id, decimal linetotal, decimal price, string quantity);
        Task<List<MultipleImagesModel>> GetMultipleImages(int id);
    }
}
