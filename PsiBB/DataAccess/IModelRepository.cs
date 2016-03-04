using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Threading.Tasks;

namespace PsiBB.DataAccess
{   // TODO: Add Async to end of method names?
    public interface IModelRepository<TModel>
    {
        Task<IEnumerable<TModel>> GetAll();
        
        Task<TModel> GetById(string id);
        
        Task Create(TModel model);
        
        Task<bool> Remove(string id);

        /// <summary>
        /// Adds an additional item to the specified list field.<para/>
        /// <c>bool success = await repo.Add("56cd35159d2feb7ef4100e59", e => e.Prescriptions, "cowbell");</c>
        /// </summary>
        /// <typeparam name="TItem">The type of the added item.</typeparam>
        /// <param name="id">Unique id of record containing list field.</param>
        /// <param name="listField">One of those LINQ lambda expressions indicating the field to be added to.</param>
        /// <param name="itemValue">The new item to be added.</param>
        /// <returns>Whether the operation was successful.</returns>
        /// <example><code>bool success = await repo.Add("56cd35159d2feb7ef4100e59", e => e.Prescriptions, "cowbell");</code></example>
        Task<bool> Add<TItem>(string id, Expression<Func<TModel, IEnumerable<TItem>>> listField, TItem itemValue);
        Task<bool> FullUpdate(TModel model);
    }
}
