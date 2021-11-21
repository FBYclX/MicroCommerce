using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Repositories
{
    public interface IProductRepository:IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProducts();
        Task<Product> GetProductById(string id);
        Task<Product> GetProductByName(string name);
        Task<IEnumerable<Product>> GetProductByCategory(string category);    
        Task CreateProduct(Product product);
        Task<bool> UpdateProduct(Product product);
        Task<bool> DeleteProduct(string id);
    }
}
