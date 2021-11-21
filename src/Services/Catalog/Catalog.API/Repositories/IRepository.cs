using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Catalog.API.Repositories
{
    public interface IRepository<T> where T : IEntity 
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> Get(string id);
        Task<T> CreateOrUpdate(T entity);
        Task<T> Delete(string id);
        Task<IEnumerable<T>> GetBy(Expression<Func<T,bool>> filter);
    }
    public class Repository<T> : IRepository<T> where T : IEntity
    {
        private IMongoCollection<T> collection;
        public Repository(IConfiguration config)
        {
            var client = new MongoClient(config.GetValue<string>("DatabaseSettings:ConnectionString"));
            var database = client.GetDatabase(config.GetValue<string>("DatabaseSettings:DatabaseName"));
            collection = database.GetCollection<T>(typeof(T).Name);
        }
        public async Task<T> CreateOrUpdate(T entity)
        {
            try
            {
                var ent = await collection.Find(x => x.Id == entity.Id).FirstOrDefaultAsync();
                if (ent is null)
                {
                    await collection.InsertOneAsync(entity);
                    return entity;
                }
                else
                {
                    var result=await collection.ReplaceOneAsync(x => x.Id == entity.Id, replacement: entity);
                    if (result.IsAcknowledged && result.IsModifiedCountAvailable)
                        return entity;
                    else 
                        return default;
                }

            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<T> Delete(string id)
        {
            var ent= await collection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (ent != null)
            {
                ((IEntity)ent).Delete();
                await collection.FindOneAndReplaceAsync(x => x.Id == id, replacement:ent);
            }
            return ent;
        }

        public async Task<T> Get(string id)
        {
            return await collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await collection.Find(x => true).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetBy(Expression<Func<T, bool>> filter)
        {
            var _filter = Builders<T>.Filter.Where(filter);
            return await collection.Find(filter).ToListAsync();
        }


    }
}
