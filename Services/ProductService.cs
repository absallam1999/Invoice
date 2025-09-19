using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.DTO.Product;
using invoice.Core.Entites;
using invoice.Core.Interfaces.Services;
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

        public async Task<GeneralResponse<ProductReadDTO>> CreateAsync(ProductCreateRequest request, string userId)
        {
            string? mainImagePath = null;
            var galleryPaths = new List<string>();

            if (request.MainImage != null && request.MainImage.Length > 0)
                mainImagePath = await _fileService.UploadImageAsync(request.MainImage, "Products");

            if (request.Images != null && request.Images.Any())
            {
                foreach (var img in request.Images)
                    galleryPaths.Add(await _fileService.UploadImageAsync(img, "Products"));
            }

            var dto = new ProductCreateDTO
            {
                Name = request.Name,
                Description = request.Description,
                Code = $"PRO-{DateTime.UtcNow.Ticks}",
                MainImage = mainImagePath,
                Images = galleryPaths,
                Price = request.Price,
                Quantity = request.Quantity,
                InPOS = request.InPOS,
                InStore = request.InStore,
                CategoryId = request.CategoryId,
            };

            var product = _mapper.Map<Product>(dto);
            product.UserId = userId;

            await _productRepo.AddAsync(product);
            var readDto = _mapper.Map<ProductReadDTO>(product);

            return new GeneralResponse<ProductReadDTO>(true, "Product created successfully", readDto);
        }

        public async Task<GeneralResponse<ProductReadDTO>> UpdateAsync(string id, ProductUpdateRequest request, string userId)
        {
            var product = await _productRepo.GetByIdAsync(id, userId, q => q.Include(p => p.Category));
            if (product == null)
                return new GeneralResponse<ProductReadDTO>(false, "Product not found");

            string? mainImagePath = null;
            var galleryPaths = new List<string>();

            if (request.MainImage != null && request.MainImage.Length > 0)
                mainImagePath = await _fileService.UploadImageAsync(request.MainImage, "Products");

            if (request.Images != null && request.Images.Any())
            {
                foreach (var img in request.Images)
                    galleryPaths.Add(await _fileService.UploadImageAsync(img, "Products"));
            }

            var dto = new ProductUpdateDTO
            {
                Name = request.Name,
                Description = request.Description,
                MainImage = mainImagePath ?? product.MainImage,
                Images = galleryPaths.Any() ? galleryPaths : product.Images,
                Price = request.Price,
                Quantity = request.Quantity,
                InPOS = request.InPOS,
                InStore = request.InStore,
                CategoryId = request.CategoryId,
            };

            _mapper.Map(dto, product);
            await _productRepo.UpdateAsync(product);

            var readDto = _mapper.Map<ProductReadDTO>(product);
            return new GeneralResponse<ProductReadDTO>(true, "Product updated successfully", readDto);
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id, string userId)
        {
            var product = await _productRepo.GetByIdAsync(id, userId);
            if (product == null)
                return new GeneralResponse<bool>(false, "Product not found", false);

            await _productRepo.DeleteAsync(product.Id);
            return new GeneralResponse<bool>(true, "Product deleted successfully", true);
        }

        public async Task<GeneralResponse<ProductReadDTO>> GetByIdAsync(string id, string userId)
        {
            var product = await _productRepo.GetByIdAsync(id, userId, q => q.Include(p => p.Category)
            .Include(p => p.InvoiceItems).ThenInclude(i => i.Invoice).ThenInclude(n => n.Client)
           );
            if (product == null)
                return new GeneralResponse<ProductReadDTO>(false, "Product not found");

            var readDto = _mapper.Map<ProductReadDTO>(product);
            return new GeneralResponse<ProductReadDTO>(true, "Product retrieved successfully", readDto);
        }

        public async Task<GeneralResponse<IEnumerable<GetAllProductDTO>>> GetAllAsync(string userId)
        {
            var products = await _productRepo.GetAllAsync(userId, p => p.Category, p => p.InvoiceItems);
            var dtos = _mapper.Map<IEnumerable<GetAllProductDTO>>(products);
            return new GeneralResponse<IEnumerable<GetAllProductDTO>>(true, "Products retrieved successfully", dtos);
        }
        public async Task<GeneralResponse<IEnumerable<GetAllProductDTO>>> Productsavailable(string userId)
        {
            var products = await _productRepo.GetAllAsync(userId, p => (p.Quantity == null || p.Quantity > 0), p => p.Category, p => p.InvoiceItems);
            var dtos = _mapper.Map<IEnumerable<GetAllProductDTO>>(products);
            return new GeneralResponse<IEnumerable<GetAllProductDTO>>(true, "Products retrieved successfully", dtos);
        }


        public async Task<GeneralResponse<IEnumerable<ProductReadDTO>>> QueryAsync(Expression<Func<Product, bool>> predicate, string userId)
        {
            var products = await _productRepo.QueryAsync(p => p.UserId == userId && predicate.Compile()(p), p => p.Category,/* p => p.Store,*/ p => p.InvoiceItems);
            var dtos = _mapper.Map<IEnumerable<ProductReadDTO>>(products);
            return new GeneralResponse<IEnumerable<ProductReadDTO>>(true, "Products retrieved successfully", dtos);
        }

        public async Task<GeneralResponse<IEnumerable<ProductReadDTO>>> GetByCategoryAsync(string categoryId, string userId)
        {
            var products = await _productRepo.QueryAsync(p => p.CategoryId == categoryId && p.UserId == userId, p => p.Category/*, p => p.Store*/);
            var dtos = _mapper.Map<IEnumerable<ProductReadDTO>>(products);
            return new GeneralResponse<IEnumerable<ProductReadDTO>>(true, "Products by category retrieved successfully", dtos);
        }

        public async Task<GeneralResponse<IEnumerable<GetAllProductDTO>>> GetAvailableForPOSAsync(string userId)
        {
            var products = await _productRepo.QueryAsync(p => p.InPOS && p.UserId == userId && (p.Quantity == null || p.Quantity > 0), p => p.Category);
            var dtos = _mapper.Map<IEnumerable<GetAllProductDTO>>(products);
            return new GeneralResponse<IEnumerable<GetAllProductDTO>>(true, "Products available for POS retrieved successfully", dtos);
        }

        public async Task<GeneralResponse<IEnumerable<GetAllProductDTO>>> GetAvailableForStoreAsync(string userId)
        {
            var products = await _productRepo.QueryAsync(p => p.InStore && p.UserId == userId, p => p.Category);
            var dtos = _mapper.Map<IEnumerable<GetAllProductDTO>>(products);
            return new GeneralResponse<IEnumerable<GetAllProductDTO>>(true, "Products available for store retrieved successfully", dtos);
        }

        public async Task<GeneralResponse<IEnumerable<GetAllProductDTO>>> GetProductListAsync(string userId)
        {
            var products = await _productRepo.QueryAsync(p => p.InProductList && p.UserId == userId, p => p.Category);
            var dtos = _mapper.Map<IEnumerable<GetAllProductDTO>>(products);
            return new GeneralResponse<IEnumerable<GetAllProductDTO>>(true, "Product list retrieved successfully", dtos);
        }

        public async Task<GeneralResponse<bool>> UpdateImageAsync(string id, ProductImagesDTO request, string userId)
        {
            var product = await _productRepo.GetByIdAsync(id, userId);
            if (product == null)
                return new GeneralResponse<bool>(false, "Product not found");

            if (request.MainImage != null && request.MainImage.Length > 0)
                product.MainImage = await _fileService.UploadImageAsync(request.MainImage, "Products");

            if (request.GalleryImages != null && request.GalleryImages.Any())
            {
                product.Images = new List<string>();
                foreach (var file in request.GalleryImages)
                    product.Images.Add(await _fileService.UploadImageAsync(file, "Products"));
            }

            await _productRepo.UpdateAsync(product);
            return new GeneralResponse<bool>(true, "Product images updated successfully", true);
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

        public async Task<GeneralResponse<IEnumerable<ProductReadDTO>>> AddRangeAsync(ProductCreateRangeDTO request, string userId)
        {
            var products = new List<Product>();

            foreach (var req in request.Products)
            {
                string? mainImagePath = null;
                var galleryPaths = new List<string>();

                if (req.MainImage != null)
                    mainImagePath = await _fileService.UploadImageAsync(req.MainImage, "products");

                if (req.Images != null && req.Images.Any())
                {
                    foreach (var g in req.Images)
                        galleryPaths.Add(await _fileService.UploadImageAsync(g, "products"));
                }

                var dto = new ProductCreateDTO
                {
                    Name = req.Name,
                    Description = req.Description,
                    Code = $"PRO-{DateTime.UtcNow.Ticks}",
                    MainImage = mainImagePath,
                    Images = galleryPaths,
                    Price = req.Price,
                    Quantity = req.Quantity,
                    InPOS = req.InPOS,
                    InStore = req.InStore,
                    CategoryId = req.CategoryId
                };

                var product = _mapper.Map<Product>(dto);
                product.UserId = userId;
                products.Add(product);
            }

            await _productRepo.AddRangeAsync(products);
            var readDtos = _mapper.Map<IEnumerable<ProductReadDTO>>(products);

            return new GeneralResponse<IEnumerable<ProductReadDTO>>(true, "Products created successfully", readDtos);
        }

        public async Task<GeneralResponse<IEnumerable<ProductReadDTO>>> UpdateRangeAsync(ProductUpdateRangeDTO request, string userId)
        {
            var products = new List<Product>();

            foreach (var req in request.Products)
            {
                var product = await _productRepo.GetByIdAsync(req.Id, userId);
                if (product == null) continue;

                string? mainImagePath = null;
                var galleryPaths = new List<string>();

                if (req.MainImage != null && req.MainImage.Length > 0)
                    mainImagePath = await _fileService.UploadImageAsync(req.MainImage, "products");

                if (req.Images != null && req.Images.Any())
                {
                    foreach (var g in req.Images)
                        galleryPaths.Add(await _fileService.UploadImageAsync(g, "products"));
                }

                var dto = new ProductUpdateDTO
                {
                    Name = req.Name,
                    Description = req.Description,
                    MainImage = mainImagePath ?? product.MainImage,
                    Images = galleryPaths.Any() ? galleryPaths : product.Images,
                    Price = req.Price,
                    Quantity = req.Quantity,
                    InPOS = req.InPOS,
                    InStore = req.InStore,
                    CategoryId = req.CategoryId
                };

                _mapper.Map(dto, product);
                products.Add(product);
            }

            await _productRepo.UpdateRangeAsync(products);
            var readDtos = _mapper.Map<IEnumerable<ProductReadDTO>>(products);

            return new GeneralResponse<IEnumerable<ProductReadDTO>>(true, "Products updated successfully", readDtos);
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
            Expression<Func<Product, bool>> userFilter = p => p.UserId == userId;

            var combined = CombineExpressions(userFilter, predicate);

            return await _productRepo.ExistsAsync(combined);
        }

        public async Task<int> CountAsync(Expression<Func<Product, bool>> predicate, string userId)
        {
            Expression<Func<Product, bool>> userFilter = p => p.UserId == userId;

            var combined = CombineExpressions(userFilter, predicate);

            return await _productRepo.CountAsync(combined);
        }

        public IQueryable<Product> GetQueryable()
        {
            return _productRepo.GetQueryable();
        }


        private Expression<Func<T, bool>> CombineExpressions<T>(
            Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var body = Expression.AndAlso(
                Expression.Invoke(expr1, parameter),
                Expression.Invoke(expr2, parameter)
            );

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }
}
