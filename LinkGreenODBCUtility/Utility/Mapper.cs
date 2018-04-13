using AutoMapper;
using LinkGreen.Applications.Common.Model;

namespace LinkGreenODBCUtility.Utility
{
    static class Mapper
    {
        public static void InitMapper()
        {
            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<ProductMapper>();
            });
        }
    }

    internal class ProductMapper : Profile
    {
        public ProductMapper()
        {
            CreateMap<LinkGreen.Applications.Common.Model.InventoryQuantity, IdSkuQuantity>()
                .ForMember(c => c.SKU, opt => opt.MapFrom(s => s.Sku))
                .ForMember(c => c.Quantity, opt => opt.MapFrom(s => s.Quantity))
                .ForMember(c => c.Id, opt => opt.Ignore());
        }
    }
}
