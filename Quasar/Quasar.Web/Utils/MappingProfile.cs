using AutoMapper;
using Quasar.Models;
using Quasar.Web.Models.Addresses;
using Quasar.Web.Models.Orders;
using Quasar.Web.Models.Products;
using Quasar.Web.Models.Users;
using System.Linq;

namespace Quasar.Web.Utils
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, EditProductViewModel>()
                .ForPath(e => e.NewCover, opt => opt.Ignore())
                .ForPath(e => e.NewImages, opt => opt.Ignore())
                .ForPath(
                p => p.Cover.Selected, opt => opt.MapFrom(x => true))
                .ForPath(
                p => p.Cover.Path,
                opt => opt.MapFrom(src => src.Cover))
                .ForPath(
                p => p.Cover.PublicId,
                opt => opt.MapFrom(src => src.CoverPublicId))
                .ForMember(
                p => p.Category,
                opt => opt.MapFrom(src => src.Category.ToString()))
                .ForMember(
                p => p.Type,
                opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(
                p => p.Platform,
                opt => opt.MapFrom(src => src.Platform.ToString()));

            CreateMap<Product, DetailsProductViewModel>()
                .ForMember(
                p => p.CurrentCoverImage,
                opt => opt.MapFrom(src => src.Cover))
                .ForMember(
                p => p.CurrentImages,
                opt => opt.MapFrom(src => src.Images.Select(x => x.Path)));

            CreateMap<Product, ProductViewModel>()
                 .ForPath(e => e.CoverImage, opt => opt.Ignore())
                 .ForPath(e => e.Images, opt => opt.Ignore());


            CreateMap<UserProduct, UserProductViewModel>()
                .ForMember(up => up.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(up => up.ProductPrice, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(up => up.ProductType, opt => opt.MapFrom(src => src.Product.Type.ToString()));

            CreateMap<Product, SortProductViewModel>()
                .ForMember(p => p.OrdersCount, opt => opt.MapFrom(src => src.Orders.Sum(x=>x.Quantity)));

            CreateMap<OrderProduct, OrderProductViewModel>()
            .ForMember(
                op => op.Id,
                opt => opt.MapFrom(src => src.ProductId))
            .ForMember(
                op => op.Name,
                opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(
                op => op.Price,
                opt => opt.MapFrom(src => src.Product.Price))
            .ForMember(
                op => op.Cover,
                opt => opt.MapFrom(src => src.Product.Cover));

            CreateMap<Image, ImageViewModel>()
                .ForPath(e => e.Selected, opt => opt.MapFrom(x => true));
            
            CreateMap<Product, DisplayProductViewModel>()
               .ForMember(
               p => p.Platform,
               opt => opt.MapFrom(src => src.Platform.ToString()));

            CreateMap<Order, OrderViewModel>()
                .ForPath(e => e.PaginatedProducts, opt => opt.Ignore());


        }
    }
}
