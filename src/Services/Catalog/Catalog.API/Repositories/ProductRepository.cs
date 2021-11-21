using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Repositories
{
    public class ProductRepository : Repository<Product>,IProductRepository
    {
        private  ICatalogContext _context;
        private FilterDefinition<Product> _filter;
        public ProductRepository(ICatalogContext context,IConfiguration config):base(config)
        {
            _context = context;
            _filter = Builders<Product>.Filter.Empty;
        }

        public async Task CreateProduct(Product product)
        {
             await _context.Products.InsertOneAsync(product);
        }

        public async Task<bool> DeleteProduct(string id)
        {
            _filter = Builders<Product>.Filter.Eq(x => x.Id, id);

            var result=await _context.Products.DeleteOneAsync(_filter);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<IEnumerable<Product>> GetProductByCategory(string category)
        {
            _filter = Builders<Product>.Filter.Eq(x => x.Category,category);
            return await _context.Products.Find(_filter).ToListAsync();
        }

        public async Task<Product> GetProductById(string id)
        {
            _filter = Builders<Product>.Filter.Eq(x => x.Id,id);
            return await _context.Products.Find(_filter).FirstOrDefaultAsync();
        }

        public async Task<Product> GetProductByName(string name)
        {
            _filter = Builders<Product>.Filter.Eq(x => x.Name, name);
            return await _context.Products.Find(_filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _context.Products.FindAsync(_filter).Result.ToListAsync();
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            _filter = Builders<Product>.Filter.Eq(x => x.Id, product.Id);
            var result=await _context.Products.ReplaceOneAsync(_filter, replacement: product);
            return result.IsAcknowledged && result.IsModifiedCountAvailable;
        }
    }
}
