using Mini_Ecom_APIS.Models;

namespace Mini_Ecom_APIS.Repository
{
    public interface IProductRepository
    {
        Task<List<ProductModel>> GetAsync();
        Task<ProductModel> GetByIDAsync(int id);
        Task<int> Saving(ProductModel pm);
        Task Delete(int id);
        Task<int> SavingMultipleImages(MultipleImagesModel mim);
        Task<List<MultipleImagesModel>> GetAsyncMultipleImages(int Id);

    }
}
