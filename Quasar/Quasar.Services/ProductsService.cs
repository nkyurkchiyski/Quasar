using Microsoft.AspNetCore.Http;
using Quasar.Data;
using Quasar.Models;
using Quasar.Models.Enums;
using Quasar.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.IO;
using Quasar.Services.Utils;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Quasar.Services
{
    public class ProductsService : IProductsService
    {
        private readonly QuasarDbContext context;
        private readonly Cloudinary cloudinary;
        private readonly IMapper mapper;

        public ProductsService(QuasarDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.cloudinary = this.ConfigureCloudAccount();
        }

        private Cloudinary ConfigureCloudAccount()
        {
            Account account = new Account(
                          Constants.CloudName,
                          Constants.ApiKey,
                          Constants.ApiSecret);

            Cloudinary cloudinary = new Cloudinary(account);
            return cloudinary;
        }

        private Image[] UploadImagesToCloud(bool coverPresent = false, params string[] paths)
        {
            var images = new List<Image>();
            for (int i = 0; i < paths.Length; i++)
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(paths[i]),
                    Folder = "Quasar/Images",
                    UseFilename = true
                };

                if (coverPresent && i == 0)
                {
                    uploadParams.Folder = "Quasar/Covers";
                }

                var uploadResult = this.cloudinary.Upload(uploadParams);

                var image = new Image
                {
                    PublicId = uploadResult.PublicId,
                    Path = uploadResult.Uri.AbsoluteUri
                };

                images.Add(image);
            }

            return images.ToArray();
        }

        private void DeleteImagesFromCloud(string[] publicIds)
        {
            if (publicIds.Length > 0)
            {
                this.cloudinary.DeleteResources(publicIds);
            }
        }

        private async Task<string[]> SaveImagesInTempStorage(params IFormFile[] images)
        {
            var paths = new List<string>();
            string filePath = "";

            foreach (var img in images)
            {
                if (img.Length > 0)
                {
                    filePath = string.Format(Constants.TempImageStoragePath, img.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await img.CopyToAsync(stream);
                        paths.Add(filePath);
                    }
                }
            }

            return paths.ToArray();
        }

        private void ClearTempStorage()
        {
            DirectoryInfo di = new DirectoryInfo(string.Format(Constants.TempImageStoragePath, ""));

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }

        private void DeleteImagesFromDb(string[] imagesPublicIds)
        {
            var imagesToRemove = this.context.Images
                .Where(x => imagesPublicIds.Contains(x.PublicId))
                .ToArray();

            this.context.Images.RemoveRange(imagesToRemove);
            this.context.SaveChanges();
        }
        
        public async Task<bool> Create(string name,
            string description,
            int totalQuantity,
            decimal price,
            string category,
            string type,
            string platform,
            IFormFile coverImage,
            IFormFile[] images)
        {
            var validCategory = Enum.TryParse(category, out Category categoryEnum);
            var validType = Enum.TryParse(type, out ProductType typeEnum);
            var validPlatform = Enum.TryParse(platform, out Platform platformEnum);

            if (!validPlatform || !validCategory || !validType)
            {
                return false;
            }

            var product = new Product
            {
                Name = name,
                Description = description,
                Price = price,
                TotalQuantity = totalQuantity,
                Category = categoryEnum,
                Platform = platformEnum,
                Type = typeEnum,
                DateAdded = DateTime.Now
            };

            List<IFormFile> formFiles = new List<IFormFile> { coverImage };

            if (images != null)
            {
                formFiles.AddRange(images);
            }

            string[] imagePaths = await SaveImagesInTempStorage(formFiles.ToArray());
            Image[] productImages = this.UploadImagesToCloud(true, imagePaths);

            product.CoverPublicId = productImages[0].PublicId;
            product.Cover = productImages[0].Path;
            product.Images = productImages.Skip(1).ToArray();

            this.context.Products.Add(product);
            await this.context.SaveChangesAsync();

            this.ClearTempStorage();

            return true;
        }

        public void Delete(Guid id)
        {
            var product = this.context.Products.Find(id);

            if (product == null)
            {
                return;
            }

            var imagePublicIds = new List<string> { product.CoverPublicId };

            if (product.Images.Any())
            {
                imagePublicIds.AddRange(product.Images.Select(x => x.PublicId).ToArray());

            }

            this.DeleteImagesFromCloud(imagePublicIds.ToArray());

            this.context.Products.Remove(product);
            this.context.SaveChanges();
        }

        public async Task<bool> Edit(Guid id,
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
            IFormFile[] images)
        {
            var product = await this.GetProduct(id);

            if (product == null)
            {
                return false;
            }

            var validCategory = Enum.TryParse(category, out Category categoryEnum);
            var validType = Enum.TryParse(type, out ProductType typeEnum);
            var validPlatform = Enum.TryParse(platform, out Platform platformEnum);

            if (!validPlatform || !validCategory || !validType)
            {
                return false;
            }

            product.Name = name;
            product.Description = description;
            product.Price = price;
            product.TotalQuantity = totalQuantity;
            product.Category = categoryEnum;
            product.Platform = platformEnum;
            product.Type = typeEnum;

            List<IFormFile> formFiles = new List<IFormFile>();

            bool newCoverPresent = coverImage != null;
            bool newImagesPresent = images != null;

            if (newCoverPresent)
            {
                formFiles.Add(coverImage);
            }

            if (newImagesPresent)
            {
                formFiles.AddRange(images);
            }

            string[] imagePaths = await SaveImagesInTempStorage(formFiles.ToArray());
            Image[] productImages = this.UploadImagesToCloud(newCoverPresent, imagePaths);

            if (newCoverPresent)
            {
                product.CoverPublicId = productImages[0].PublicId;
                product.Cover = productImages[0].Path;
            }

            if (newImagesPresent)
            {
                if (newCoverPresent)
                {
                    productImages = productImages.Skip(1).ToArray();
                }

                foreach (var img in productImages)
                {
                    product.Images.Add(img);
                }
            }

            await this.context.SaveChangesAsync();

            this.ClearTempStorage();

            var imagePublicIds = new List<string>();

            if (!string.IsNullOrEmpty(currentCoverImage))
            {
                imagePublicIds.Add(currentCoverImage);
            }

            if (currentImages.Any())
            {
                imagePublicIds.AddRange(currentImages);

            }

            this.DeleteImagesFromDb(imagePublicIds.ToArray());
            this.DeleteImagesFromCloud(imagePublicIds.ToArray());

            return true;
        }

        public async Task<ICollection<T>> All<T>()
        {
            var products = await this.context.Products.ToListAsync();

            var models = products
                .Select(o => this.mapper.Map<T>(o))
                .ToList();

            return models;
        }

        private async Task<Product> GetProduct(Guid id)
        {
            var product = await this.context.Products.FindAsync(id);
            return product;
        }

        public ICollection<Product> GetProductsBy(Func<Product, bool> predicate)
        {
            var products = this.context.Products
                   .Where(predicate)
                   .ToList();
            return products;
        }

        public ICollection<T> GetProducts<T>(string queryType, string queryValue)
        {
            var products = new List<Product>();

            switch (queryType)
            {
                case "Category":
                    products = GetProductsBy(x => x.Category.ToString() == queryValue).ToList();
                    break;
                case "Name":
                    products = GetProductsBy(x => x.Name.ToLower().Contains(queryValue.ToLower())).ToList();
                    break;
                case "Platform":
                    products = GetProductsBy(x => x.Platform.ToString() == queryValue).ToList();
                    break;
                case "Type":
                    products = GetProductsBy(x => x.Type.ToString() == queryValue).ToList();
                    break;
            }

            var models = products
                .Select(o => this.mapper.Map<T>(o))
                .ToList();

            return models;

        }

        public async Task<T> GetProductInfo<T>(Guid id)
        {
            var product = await this.GetProduct(id);
            var model = this.mapper.Map<T>(product);
            return model;
        }
    }
}
