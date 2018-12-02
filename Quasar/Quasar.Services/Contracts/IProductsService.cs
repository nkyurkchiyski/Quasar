using Microsoft.AspNetCore.Http;
using Quasar.Models;
using Quasar.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Services.Contracts
{
    public interface IProductsService
    {
        Task<bool> Create(string name,
           string description,
           int totalQuantity,
           decimal price,
           string category,
           string type,
           string platform,
           IFormFile coverImage,
           IFormFile[] images);

        Task<bool> Edit(Guid id,
           string name,
           string description,
           int totalQuantity,
           decimal price,
           string category,
           string type,
           string platform,
           string currentCoverImage,
            string[] currentImages,
           IFormFile coverImage,
           IFormFile[] images);

        void Delete(Guid id);

        Task<ICollection<T>> All<T>();

        Task<T> GetProductInfo<T>(Guid id);

        ICollection<Product> GetProductsBy(Func<Product, bool> predicate);

        ICollection<T> GetProducts<T>(string queryType, string queryValue);
    }
}