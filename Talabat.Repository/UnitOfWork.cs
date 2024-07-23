using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext storeContext;
        private Hashtable repositories;

        public UnitOfWork(StoreContext storeContext) 
        {
            this.storeContext = storeContext;
            repositories = new Hashtable();
        }
        public async Task<int> CompleteAsync()
        {
           return await storeContext.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        => await storeContext.DisposeAsync();
        

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var type= typeof(TEntity).Name;
            if (!repositories.ContainsKey(type)) 
            {
                var Repository = new GenericRepository<TEntity>(storeContext);
                repositories.Add(type, Repository); 
            }
            
            return repositories[type] as IGenericRepository<TEntity>;
        }
    }
}
