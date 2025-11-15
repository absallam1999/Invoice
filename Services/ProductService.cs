using AutoMapper;
using Core.Interfaces.Services;
using invoice.Core.DTO;
using invoice.Core.DTO.Product;
using invoice.Core.Entities;
using invoice.Core.Interfaces;
using invoice.Core.Interfaces.Services;
using invoice.Helpers;
using invoice.Repo;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace invoice.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public ProductService(IRepository<Product> productRepo, IFileService fileService, IMapper mapper)
        {
            _productRepo = productRepo;
            _fileService = fileService;
            _mapper = mapper;
        }
        public async Task<GeneralResponse<ProductReadDTO>> CreateAsync(ProductCreateDTO dto, string userId)
        {

            string? mainImagePath = null;
            var galleryPaths = new List<string>();

            if (dto.MainImage != null && dto.MainImage.Length > 0)
                mainImagePath = await _fileService.UploadImageAsync(dto.MainImage, "products");

      

            var product = _mapper.Map<Product>(dto);
            product.UserId = userId;
            product.MainImage = mainImagePath;
            await _productRepo.AddAsync(product);

            var readDto = _mapper.Map<ProductReadDTO>(product);
            readDto.MainImage = _fileService.GetImageUrl(product.MainImage);
            return new GeneralResponse<ProductReadDTO>(true, "Product created successfully", readDto);
        }

        public async Task<GeneralResponse<ProductReadDTO>> UpdateAsync(string id, ProductUpdateDTO dto, string userId)
        {
            var product = await _productRepo.GetByIdAsync(id, userId, q => q
            .Include(p => p.Category)
                );
            if (product == null)
                return new GeneralResponse<ProductReadDTO>(false, "Product not found");

            
            string? mainImagePath = null;

            if (dto.MainImage != null && dto.MainImage.Length > 0)
            {
                mainImagePath = await _fileService.UpdateImageAsync(dto.MainImage, product.MainImage, "products");

            }
           
            _mapper.Map(dto, product);
            product.MainImage = mainImagePath;
            product.UpdatedAt = GetSaudiTime.Now();
            await _productRepo.UpdateAsync(product);

            var readDto = _mapper.Map<ProductReadDTO>(product);
            readDto.MainImage = _fileService.GetImageUrl(product.MainImage);
            return new GeneralResponse<ProductReadDTO>(true, "Product updated successfully", readDto);
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id, string userId)
        {
            var transaction = await _productRepo.BeginTransactionAsync();
            try
            {
                var product = await _productRepo.GetByIdAsync(id, userId);
                if (product == null)
                {
                    await _productRepo.RollbackTransactionAsync(transaction);
                    return new GeneralResponse<bool>(false, "Product not found", false);
                }

                product.DeletedAt = GetSaudiTime.Now();
                await _productRepo.UpdateAsync(product);

                var result = await _productRepo.DeleteAsync(product.Id);
                if (!result.Success)
                    throw new Exception(result.Message);

                await _productRepo.CommitTransactionAsync(transaction);

                return new GeneralResponse<bool>(true, "Product deleted successfully", true);
            }
            catch (Exception ex)
            {
                await _productRepo.RollbackTransactionAsync(transaction);
                return new GeneralResponse<bool>(false, "Error deleting product: " + ex.Message, false);
            }
        }

        public async Task<GeneralResponse<ProductWithInvoicesReadDTO>> GetByIdWithInvoicesAsync(string id, string userId)
        {
            var product = await _productRepo.GetByIdAsync(id, userId, q => q.Include(p => p.Category)
            .Include(p => p.InvoiceItems).ThenInclude(i => i.Invoice).ThenInclude(n => n.Client)
           );
            if (product == null)
                return new GeneralResponse<ProductWithInvoicesReadDTO>(false, "Product not found");

            var readDto = _mapper.Map<ProductWithInvoicesReadDTO>(product);

            readDto.MainImage = _fileService.GetImageUrl(product.MainImage);

            return new GeneralResponse<ProductWithInvoicesReadDTO>(true, "Product retrieved successfully", readDto);
        }

        public async Task<GeneralResponse<ProductReadDTO>> GetByIdAsync(string id, string userId)
        {
            var product = await _productRepo.GetByIdAsync(id, userId, q => q.Include(p => p.Category)

           );
            if (product == null)
                return new GeneralResponse<ProductReadDTO>(false, "Product not found");

            var readDto = _mapper.Map<ProductReadDTO>(product);
            readDto.MainImage = _fileService.GetImageUrl(product.MainImage);

            return new GeneralResponse<ProductReadDTO>(true, "Product retrieved successfully", readDto);
        }


        public async Task<GeneralResponse<IEnumerable<GetAllProductDTO>>> GetAllAsync(string userId)
        {
            var products = await _productRepo.GetAllAsync(userId, p => p.Category, p => p.InvoiceItems);
            var dtos = _mapper.Map<IEnumerable<GetAllProductDTO>>(products);

            foreach (var dto in dtos)
            {
                var product = products.FirstOrDefault(p => p.Id == dto.Id);
                if (product != null)
                    dto.MainImage = _fileService.GetImageUrl(product.MainImage);
            }

            return new GeneralResponse<IEnumerable<GetAllProductDTO>>(true, "Products retrieved successfully", dtos);
        }
        public async Task<GeneralResponse<IEnumerable<GetAllProductDTO>>> ProductsavailableAsync(string userId)
        {
            var products = await _productRepo.QueryAsync(p =>  p.UserId == userId && (p.Quantity == null || p.Quantity > 0), p => p.Category);
            var dtos = _mapper.Map<IEnumerable<GetAllProductDTO>>(products);
            foreach (var dto in dtos)
            {
                var product = products.FirstOrDefault(p => p.Id == dto.Id);
                if (product != null)
                    dto.MainImage = _fileService.GetImageUrl(product.MainImage);
            }

            return new GeneralResponse<IEnumerable<GetAllProductDTO>>(true, "Products retrieved successfully", dtos);

        }

        public async Task<GeneralResponse<IEnumerable<ProductReadDTO>>> QueryAsync(Expression<Func<Product, bool>> predicate, string userId)
        {
            var products = await _productRepo.QueryAsync(p => p.UserId == userId && predicate.Compile()(p), p => p.Category,/* p => p.Store,*/ p => p.InvoiceItems);
            var dtos = _mapper.Map<IEnumerable<ProductReadDTO>>(products);
            foreach (var dto in dtos)
            {
                var product = products.FirstOrDefault(p => p.Id == dto.Id);
                if (product != null)
                    dto.MainImage = _fileService.GetImageUrl(product.MainImage);
            }

            return new GeneralResponse<IEnumerable<ProductReadDTO>>(true, "Products retrieved successfully", dtos);
        }

        public async Task<GeneralResponse<IEnumerable<ProductReadDTO>>> GetByCategoryAsync(string categoryId, string userId)
        {
            var products = await _productRepo.QueryAsync(p => p.CategoryId == categoryId && p.UserId == userId, p => p.Category/*, p => p.Store*/);
            var dtos = _mapper.Map<IEnumerable<ProductReadDTO>>(products);
            return new GeneralResponse<IEnumerable<ProductReadDTO>>(true, "Products by category retrieved successfully", dtos);
        }

        public async Task<GeneralResponse<IEnumerable<ProductReadDTO>>> GetByStoreAsync(string storeId, string userId)
        {
            var products = await _productRepo.QueryAsync(p => p.UserId == userId, p => p.Category);
            var dtos = _mapper.Map<IEnumerable<ProductReadDTO>>(products);
            foreach (var dto in dtos)
            {
                var product = products.FirstOrDefault(p => p.Id == dto.Id);
                if (product != null)
                    dto.MainImage = _fileService.GetImageUrl(product.MainImage);
            }

            return new GeneralResponse<IEnumerable<ProductReadDTO>>(true, "Products by store retrieved successfully", dtos);
        }

        public async Task<GeneralResponse<IEnumerable<GetAllProductDTO>>> GetAvailableForPOSAsync(string userId)
        {
            var products = await _productRepo.QueryAsync(p => p.InPOS && p.UserId == userId && (p.Quantity == null || p.Quantity > 0), p => p.Category);
            var dtos = _mapper.Map<IEnumerable<GetAllProductDTO>>(products);
            foreach (var dto in dtos)
            {
                var product = products.FirstOrDefault(p => p.Id == dto.Id);
                if (product != null)
                    dto.MainImage = _fileService.GetImageUrl(product.MainImage);
            }

            return new GeneralResponse<IEnumerable<GetAllProductDTO>>(true, "Products available for POS retrieved successfully", dtos);
        }

        public async Task<GeneralResponse<IEnumerable<GetAllProductDTO>>> GetAvailableForStoreAsync(string userId)
        {

            var products = await _productRepo.QueryAsync(p => p.InStore && p.UserId == userId && p.User.Store.IsActivated, p => p.Category);
            var dtos = _mapper.Map<IEnumerable<GetAllProductDTO>>(products);
            foreach (var dto in dtos)
            {
                var product = products.FirstOrDefault(p => p.Id == dto.Id);
                if (product != null)
                    dto.MainImage = _fileService.GetImageUrl(product.MainImage);
            }

            return new GeneralResponse<IEnumerable<GetAllProductDTO>>(true, "Products available for store retrieved successfully", dtos);
        }
        public async Task<GeneralResponse<bool>> UpdateQuantityAsync(string id, int quantity, string userId)
        {
            var product = await _productRepo.GetByIdAsync(id, userId);
            if (product == null)
                return new GeneralResponse<bool>(false, "Product not found", false);

            product.Quantity = quantity;
            await _productRepo.UpdateAsync(product);

            return new GeneralResponse<bool>(true, "Quantity updated successfully", true);
        }

        public async Task<GeneralResponse<bool>> IncrementQuantityAsync(string id, int amount, string userId)
        {
            var product = await _productRepo.GetByIdAsync(id, userId);
            if (product == null)
                return new GeneralResponse<bool>(false, "Product not found", false);

            product.Quantity += amount;
            await _productRepo.UpdateAsync(product);

            return new GeneralResponse<bool>(true, "Quantity incremented successfully", true);
        }

        public async Task<GeneralResponse<bool>> DecrementQuantityAsync(string id, int amount, string userId)
        {
            var product = await _productRepo.GetByIdAsync(id, userId);
            if (product == null)
                return new GeneralResponse<bool>(false, "Product not found", false);

            if (product.Quantity < amount)
                return new GeneralResponse<bool>(false, "Insufficient quantity", false);

            product.Quantity -= amount;
            await _productRepo.UpdateAsync(product);

            return new GeneralResponse<bool>(true, "Quantity decremented successfully", true);
        }

        public async Task<GeneralResponse<IEnumerable<ProductReadDTO>>> AddRangeAsync(IEnumerable<ProductCreateDTO> dtos, string userId)
        {
            var products = _mapper.Map<IEnumerable<Product>>(dtos);
            foreach (var product in products)
                product.UserId = userId;

            await _productRepo.AddRangeAsync(products);
            var dtosResult = _mapper.Map<IEnumerable<ProductReadDTO>>(products);

            return new GeneralResponse<IEnumerable<ProductReadDTO>>(true, "Products added successfully", dtosResult);
        }

        public async Task<GeneralResponse<IEnumerable<ProductReadDTO>>> UpdateRangeAsync(IEnumerable<ProductUpdateDTO> dtos, string userId)
        {
            var updatedProducts = new List<Product>();

            //foreach (var dto in dtos)
            //{
            //    var product = await _productRepo.GetByIdAsync(dto.Id, userId);
            //    if (product != null)
            //    {
            //        _mapper.Map(dto, product);
            //        updatedProducts.Add(product);
            //    }
            //}

            await _productRepo.UpdateRangeAsync(updatedProducts);
            var dtosResult = _mapper.Map<IEnumerable<ProductReadDTO>>(updatedProducts);

            return new GeneralResponse<IEnumerable<ProductReadDTO>>(true, "Products updated successfully", dtosResult);
        }

        public async Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids, string userId)
        {
            var products = new List<string>();

            foreach (var id in ids)
            {
                var product = await _productRepo.GetByIdAsync(id, userId);
                if (product != null)
                    products.Add(product.Id);
            }

            if (!products.Any())
                return new GeneralResponse<bool>(false, "No products found for deletion", false);

            await _productRepo.DeleteRangeAsync(products);
            return new GeneralResponse<bool>(true, "Products deleted successfully", true);
        }

        public async Task<bool> ExistsAsync(Expression<Func<Product, bool>> predicate, string userId)
        {
            return await _productRepo.ExistsAsync(p => p.UserId == userId && predicate.Compile()(p));
        }

        public async Task<int> CountAsync(string userId)
        {
            return await _productRepo.CountAsync(userId);

        }
        public IQueryable<Product> GetQueryable()
        {
            return _productRepo.GetQueryable();
        }
    }
}