using Quasar.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Services.Contracts
{
    public interface IOrdersService
    {
        Task<ICollection<T>> All<T>(string username = null);

        Task<T> GetShoppingCart<T>(string username);

        Task<T> Details<T>(Guid id);

        Task<bool> Order(string username);

        Task<bool> Acquire(string username, Guid id);

        Task<bool> ChangeStatus(Guid id);

        Task<bool> AddProduct(string username, Guid id);

        Task<bool> RemoveProduct(string username, Guid id);

        Task<bool> UpdateQuantity(string username, Guid id, int newAmmount);
        
    }
}
