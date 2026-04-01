using Polaris.WMS.MasterData.Application.Contracts.Products;
using Polaris.WMS.MasterData.Application.Contracts.Products.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.MasterData.Application.Products
{
    public class ProductAppService(IRepository<Product, Guid> repository) :
        CrudAppService<Product, ProductDto, Guid, ProductSearchDto, CreateUpdateProductDto>(repository),
        IProductAppService
    {
        public override async Task<ProductDto> CreateAsync(CreateUpdateProductDto input)
        {
            var product = new Product(
                GuidGenerator.Create(),
                input.Code,
                input.Name,
                input.Unit,
                input.AuxUnit,
                input.ConversionRate,
                input.IsBatchManagementEnabled,
                input.ShelfLifeDays);

            await Repository.InsertAsync(product);

            return ObjectMapper.Map<Product, ProductDto>(product);
        }

        public override async Task<ProductDto> UpdateAsync(Guid id, CreateUpdateProductDto input)
        {
            var product = await Repository.GetAsync(id);

            product.SetCode(input.Code);
            product.SetName(input.Name);
            product.SetUnit(input.Unit);
            product.SetAuxUnit(input.AuxUnit);
            product.SetConversionRate(input.ConversionRate);
            product.SetBatchManagement(input.IsBatchManagementEnabled, input.ShelfLifeDays);

            await Repository.UpdateAsync(product);

            return ObjectMapper.Map<Product, ProductDto>(product);
        }

        public override async Task DeleteAsync(Guid id)
        {
            await Repository.DeleteAsync(id);
        }

        public override async Task<ProductDto> GetAsync(Guid id)
        {
            var product = await Repository.GetAsync(id);

            return ObjectMapper.Map<Product, ProductDto>(product);
        }

        public override async Task<PagedResultDto<ProductDto>> GetListAsync(ProductSearchDto input)
        {
            var queryable = await Repository.GetQueryableAsync();

            if (!string.IsNullOrWhiteSpace(input.ProductCode))
            {
                queryable = queryable.Where(x => x.Code == input.ProductCode);
            }

            if (!string.IsNullOrWhiteSpace(input.ProductName))
            {
                queryable = queryable.Where(x => x.Name.Contains(input.ProductName));
            }

            var totalCount = await AsyncExecuter.CountAsync(queryable);

            var items = await AsyncExecuter.ToListAsync(
                queryable
                    .OrderBy(x => x.Code)
                    .PageBy(input.SkipCount, input.MaxResultCount));

            var dtos = ObjectMapper.Map<List<Product>, List<ProductDto>>(items);

            return new PagedResultDto<ProductDto>(totalCount, dtos);
        }
    }
}