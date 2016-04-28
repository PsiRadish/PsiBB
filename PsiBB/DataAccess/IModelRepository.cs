// Obsolete

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Threading.Tasks;

namespace PsiBB.DataAccess
{   // TODO?: Add Async to end of method names?
    public interface IModelRepository<TModel>
    {
        Task<IEnumerable<TModel>> GetAll();
        
        Task<TModel> GetById(string id);
        
        Task Create(TModel model);
        
        Task<bool> Remove(string id);

        /// <summary>
        /// Adds an additional item to the specified list field.<para/>
        /// </summary>
        /// <typeparam name="TItem">The type of the added item.</typeparam>
        /// <param name="id">Unique id of record containing the list field.</param>
        /// <param name="listField">LINQ/lambda expression indicating the field to be added to.</param>
        /// <param name="itemValue">List item object to add.</param>
        /// <returns>Task that returns whether operation was successful (boolean).</returns>
        Task<bool> Add<TItem>(string id, Expression<Func<TModel, IEnumerable<TItem>>> listField, TItem itemValue) where TItem : IHasTimestamps;
        Task<bool> Replace(TModel model);
    }
}
