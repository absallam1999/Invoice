using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.DTO.Client;
using invoice.Core.DTO.Invoice;
using invoice.Core.DTO.Store;
using invoice.Core.DTO.StoreSettings;
using invoice.Core.Entities;
using invoice.Core.Entities.utils;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Migrations;
using invoice.Models.Entities.utils;
using invoice.Repo;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace invoice.Services
{
    public class StoreService : IStoreService
    {
        private readonly IRepository<Store> _storeRepo;
        private readonly IRepository<ApplicationUser> _useRepo;
        private readonly IRepository<Client> _clientRepo;
        private readonly IRepository<InvoiceItem> _invoiceItemRepo;
        private readonly IRepository<Invoice> _invoiceRepo;
        private readonly IRepository<Product> _ProductRepo;
        private readonly IRepository<ApplicationUser> _ApplicationUserRepo;
        private readonly IMapper _mapper;

        public StoreService(IRepository<Store> storeRepo, IRepository<ApplicationUser> userepo,
            IRepository<Product> productRepo,
            IRepository<InvoiceItem> invoiceItemRepo,
            IRepository<Invoice> invoiceRepo,
            IRepository<Client> clientRepo, IRepository<ApplicationUser> ApplicationUserRepo, IMapper mapper)
        {
            _storeRepo = storeRepo;
            _useRepo = userepo;
            _clientRepo = clientRepo;
            _invoiceItemRepo = invoiceItemRepo;
            _invoiceRepo = invoiceRepo;
            _ProductRepo = productRepo;
            _ApplicationUserRepo = ApplicationUserRepo;
            _mapper = mapper; 

        }

        public async Task<GeneralResponse<StoreReadDTO>> CreateAsync(StoreCreateDTO dto, string userId)
        {
            var user =await _useRepo.GetByIdAsync(userId);
            if (dto == null)
                return new GeneralResponse<StoreReadDTO>(false, "Invalid payload");

            var storeDB = await _storeRepo.GetByUserIdAsync(userId);
            if (storeDB != null) return new GeneralResponse<StoreReadDTO>(false, "You have store already.");

            var entity = _mapper.Map<Store>(dto);
            entity.UserId = userId;

            var exists = await _storeRepo.GetBySlugAsync(dto.Slug);
            if (exists != null)
            {
                return new GeneralResponse<StoreReadDTO>
                {
                    Success = false,
                    Message = "Slug already exists, please choose another name."
                };
            }


            entity.StoreSettings = new StoreSettings
            {

                Color = dto.Color,
                Currency = dto.Currency,
                Country = dto.Country,

                purchaseOptions = new PurchaseCompletionOptions()
                
            };

        entity.ContactInformations = new ContactInfo
                {
                    Email=user.Email,
                    Phone = user.PhoneNumber,
               };

            entity.Shipping = new Shipping();
            entity.PaymentOptions = new PaymentOptions();


            var resp = await _storeRepo.AddAsync(entity);
            if (!resp.Success)
                return new GeneralResponse<StoreReadDTO>(false, resp.Message);

            ////pagerepo
            //entity.Pages = new List<Page>()
            //{
            //    new Page
            //    {
            //        Title="Contact Us",
            //        Content=$"For inquiries related to our products and services, or to share your suggestions," +
            //        $" feel free to reach us through:Phone:{entity.ContactInformations.Phone} Email: {entity.ContactInformations.Email}" +
            //        $" Address:{entity.ContactInformations.Location}",
            //        StoreId=entity.Id,
            //        InFooter=true,
            //        InHeader=true,
            //    },
            //    new Page
            //    {
            //        Title="Privacy Policy",
            //        Content="Our Privacy Policy is designed to keep your data secure",
            //        StoreId=entity.Id,
            //        InFooter=true,
            //        InHeader=true,
            //    },
            //    new Page
            //    {
            //        Title="About Us",
            //        Content=$"At {entity.Name} store, our mission is to deliver exceptional services that meet our customers’ needs and exceed their expectations." +
            //        $" Backed by a skilled team of professionals, we are committed to ensuring quality and customer satisfaction.",
            //        StoreId=entity.Id,
            //        InFooter=true,
            //        InHeader=true,
            //    }
            //};


            return new GeneralResponse<StoreReadDTO>(
                true,
                "Store created successfully",
                _mapper.Map<StoreReadDTO>(resp.Data)
            );
        }
        public async Task<GeneralResponse<StoreReadDTO>> GetAsync(string userId)
        {
            var entity = await _storeRepo.GetByUserIdAsync(userId);
            return new GeneralResponse<StoreReadDTO>(true, "Store retrieved successfully", _mapper.Map<StoreReadDTO>(entity));
        }

        public async Task<GeneralResponse<bool>> ActivateStoreAsync(string userId)
        {
            var entity = await _storeRepo.GetByUserIdAsync( userId);
            if (entity == null)
                return new GeneralResponse<bool>(false, "Store not found");

            entity.IsActivated = !entity.IsActivated;
            await _storeRepo.UpdateAsync(entity);

            return new GeneralResponse<bool>(true, "Store updated successfully", true);
        }

        public async Task<GeneralResponse<StoreReadDTO>> GetBySlug(string slug)
        {
            var entity = await _storeRepo.GetBySlugAsync(slug);
            if (entity == null)
                return new GeneralResponse<StoreReadDTO>(false, "Store not found");

            return new GeneralResponse<StoreReadDTO>(true, "Store retrieved successfully", _mapper.Map<StoreReadDTO>(entity));
        }

        public async Task<GeneralResponse<bool>> UpdateSettingsAsync(string storeId, StoreSettingsReadDTO settingsDto, string userId)
        {
            var entity = await _storeRepo.GetByIdAsync(storeId, userId);
            if (entity == null)
                return new GeneralResponse<bool>(false, "Store not found");

            entity.StoreSettings = _mapper.Map<StoreSettings>(settingsDto);
            await _storeRepo.UpdateAsync(entity);

            return new GeneralResponse<bool>(true, "Settings updated successfully", true);
        }


        #region order
        public async Task<GeneralResponse<string>> CreateOrderAsync(CreateOrderDTO dto, string userId)
        {
            if (dto == null || string.IsNullOrWhiteSpace(userId))
                return new GeneralResponse<string> { Success = false, Message = "order data and UserId are required." };
            var user = await _ApplicationUserRepo.GetByIdAsync(userId);

            //client

            string ClientId;
            var EmailExists = await _clientRepo.ExistsAsync(c => c.Email == dto.Client.Email && c.UserId == userId);
            if (EmailExists)
            {

                var client = (await _clientRepo.QueryAsync(c => c.UserId == userId && c.Email == dto.Client.Email && !c.IsDeleted)).First();
                _mapper.Map(dto.Client, client);

                var result = await _clientRepo.UpdateAsync(client);
                ClientId = result.Data.Id;

            }
            else { 
            var entity = _mapper.Map<Client>(dto.Client);
            entity.UserId = userId;
            var result = await _clientRepo.AddAsync(entity);

           if (!result.Success) return new GeneralResponse<string>(false, result.Message);
            var dtoResult = _mapper.Map<ClientReadDTO>(result.Data);
            ClientId= dtoResult.Id;
        }
            //invoice

            var invoice = _mapper.Map<Invoice>(dto.Invoice);
            invoice.UserId = userId;
            invoice.ClientId = ClientId;
            invoice.Code = $"INV-{DateTime.UtcNow.Ticks}";
            invoice.InvoiceStatus = InvoiceStatus.Active;
            invoice.Value = 0;
            invoice.LanguageId = "ar";
            invoice.InvoiceItems = new List<InvoiceItem>();

            if (dto.Invoice.InvoiceItems != null)
            {
                foreach (var item in dto.Invoice.InvoiceItems)
                {
                    var product = await _ProductRepo.GetByIdAsync(item.ProductId, userId);
                    if (product == null) return new GeneralResponse<string> { Success = false, Message = $"Product {item.ProductId} not found" };

                    if (product.Quantity != null)
                    {
                        if (product.Quantity < item.Quantity)
                            return new GeneralResponse<string> { Success = false, Message = $"Product Quantity not Enough for {product.Name}" };

                        product.Quantity -= item.Quantity;
                        await _ProductRepo.UpdateAsync(product);
                    }

                    invoice.InvoiceItems.Add(new InvoiceItem
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,

                    });

                    invoice.Value += product.Price * item.Quantity;
                }
            }
            invoice.FinalValue = invoice.Value;



            if (user?.Store?.PaymentOptions?.Tax == true && user?.Tax?.Value > 0)
            {
                var taxRate = user.Tax.Value / 100m;
                invoice.FinalValue += invoice.FinalValue * taxRate;
            }

            invoice.Order = _mapper.Map<Order>(dto);
            invoice.Order.InvoiceId = invoice.Id;
            invoice.Order.OrderStatus = OrderStatus.Delivered;

            //{
            //   InvoiceId = invoice.Id,
            //   OrderStatus = OrderStatus.Delivered,
            //   ShippingMethod = dto.ShippingMethod,
            //   DeliveryCost = dto.DeliveryCost,
            //   PaymentType = dto.PaymentType,

            //};

            await _invoiceRepo.AddAsync(invoice);

            return new GeneralResponse<string>
            {
                Success = true,
                Message = "Order created successfully.",
                Data = JsonConvert.SerializeObject(new
                {
                    InvoiceId = invoice.Id,
                    InvoiceCode = invoice.Code
                })

            };
        }

        #endregion














        public async Task<GeneralResponse<IEnumerable<StoreReadDTO>>> AddRangeAsync(IEnumerable<StoreCreateDTO> dtos, string userId)
        {
            if (dtos == null)
                return new GeneralResponse<IEnumerable<StoreReadDTO>>(false, "Invalid payload");

            var entities = _mapper.Map<List<Store>>(dtos);

            foreach (var e in entities)
            {
                e.UserId = userId;

                if (e.StoreSettings == null)
                {
                    e.StoreSettings = new StoreSettings
                    {
                       
                        Color = "#FFFFFF",
                        Currency = "USD",
                        purchaseOptions = new PurchaseCompletionOptions
                        {
                            Name = true,
                            Email = false,
                            phone = false,
                            TermsAndConditions = null
                        }
                    };
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(e.Slug))
                        e.Slug = $"store-{Guid.NewGuid():N}";

                    e.StoreSettings.Color ??= "#FFFFFF";
                    e.StoreSettings.Currency ??= "USD";
                    e.StoreSettings.purchaseOptions ??= new PurchaseCompletionOptions
                    {
                        Name = true,
                        Email = false,
                        phone = false,
                        TermsAndConditions = null
                    };
                }
            }

            var resp = await _storeRepo.AddRangeAsync(entities);
            if (!resp.Success) return new GeneralResponse<IEnumerable<StoreReadDTO>>(false, resp.Message);

            return new GeneralResponse<IEnumerable<StoreReadDTO>>(true, "Stores created successfully",
                _mapper.Map<IEnumerable<StoreReadDTO>>(resp.Data));
        }

        public async Task<GeneralResponse<StoreReadDTO>> UpdateAsync(string id, StoreUpdateDTO dto, string userId)
        {
            var entity = await _storeRepo.GetByIdAsync(id, userId);
            if (entity == null)
                return new GeneralResponse<StoreReadDTO>(false, "Store not found");

            _mapper.Map(dto, entity);
            await _storeRepo.UpdateAsync(entity);

            return new GeneralResponse<StoreReadDTO>(true, "Store updated successfully", _mapper.Map<StoreReadDTO>(entity));
        }

        public async Task<GeneralResponse<IEnumerable<StoreReadDTO>>> UpdateRangeAsync(IEnumerable<StoreUpdateDTO> dtos, string userId)
        {
            var updatedEntities = new List<Store>();
            foreach (var dto in dtos)
            {
               // var entity = await _storeRepo.GetByIdAsync(dto.Id, userId);
                //if (entity != null)
                //{
                //    _mapper.Map(dto, entity);
                //    updatedEntities.Add(entity);
                //}
            }

            await _storeRepo.UpdateRangeAsync(updatedEntities);
            return new GeneralResponse<IEnumerable<StoreReadDTO>>(true, "Stores updated successfully", _mapper.Map<IEnumerable<StoreReadDTO>>(updatedEntities));
        }
 
        public async Task<GeneralResponse<bool>> UpdatePaymentMethodsAsync(string storeId, PaymentType paymentType, string userId)
        {
            var entity = await _storeRepo.GetByIdAsync(storeId, userId);
            if (entity == null)
                return new GeneralResponse<bool>(false, "Store not found");

         //   entity.PaymentMethod = paymentType;
            await _storeRepo.UpdateAsync(entity);

            return new GeneralResponse<bool>(true, "Payment methods updated successfully", true);
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id, string userId)
        {
            var entity = await _storeRepo.GetByIdAsync(id, userId);
            if (entity == null)
                return new GeneralResponse<bool>(false, "Store not found");

            await _storeRepo.DeleteAsync(entity.Id);
            return new GeneralResponse<bool>(true, "Store deleted successfully", true);
        }

        public async Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids, string userId)
        {
            var entities = new List<string>();
            foreach (var id in ids)
            {
                var entity = await _storeRepo.GetByIdAsync(id, userId);
                if (entity != null) entities.Add(entity.Id);
            }

            await _storeRepo.DeleteRangeAsync(entities);
            return new GeneralResponse<bool>(true, "Stores deleted successfully", true);
        }

        public async Task<GeneralResponse<StoreReadDTO>> GetByIdAsync(string id, string userId)
        {
            var entity = await _storeRepo.GetByIdAsync(id, userId);
            if (entity == null)
                return new GeneralResponse<StoreReadDTO>(false, "Store not found");

            return new GeneralResponse<StoreReadDTO>(true, "Store retrieved successfully", _mapper.Map<StoreReadDTO>(entity));
        }


        public async Task<GeneralResponse<IEnumerable<StoreReadDTO>>> QueryAsync(Expression<Func<Store, bool>> predicate, string userId)
        {
            var entities = await _storeRepo.QueryAsync(predicate);
            entities = entities.Where(s => s.UserId == userId);
            return new GeneralResponse<IEnumerable<StoreReadDTO>>(true, "Stores retrieved successfully", _mapper.Map<IEnumerable<StoreReadDTO>>(entities));
        }

        public async Task<GeneralResponse<StoreReadDTO>> GetByUserAsync(string userId)
        {
            var entities = await _storeRepo.GetAllAsync(userId);
            return new GeneralResponse<StoreReadDTO>(true, "Stores retrieved successfully", _mapper.Map<StoreReadDTO>(entities));
        }

       

        public async Task<GeneralResponse<IEnumerable<StoreReadDTO>>> GetActiveStoresAsync(string userId)
        {
            var entities = await _storeRepo.QueryAsync(s => s.IsActivated && s.UserId == userId);
            return new GeneralResponse<IEnumerable<StoreReadDTO>>(true, "Active stores retrieved successfully", _mapper.Map<IEnumerable<StoreReadDTO>>(entities));
        }

        public async Task<GeneralResponse<IEnumerable<StoreReadDTO>>> GetInactiveStoresAsync(string userId)
        {
            var entities = await _storeRepo.QueryAsync(s => !s.IsActivated && s.UserId == userId);
            return new GeneralResponse<IEnumerable<StoreReadDTO>>(true, "Inactive stores retrieved successfully", _mapper.Map<IEnumerable<StoreReadDTO>>(entities));
        }


        public async Task<GeneralResponse<StoreSettingsReadDTO>> GetSettingsAsync(string storeId, string userId)
        {
            var entity = await _storeRepo.GetByIdAsync(storeId, userId);
            if (entity == null)
                return new GeneralResponse<StoreSettingsReadDTO>(false, "Store not found");

            return new GeneralResponse<StoreSettingsReadDTO>(true, "Settings retrieved successfully", _mapper.Map<StoreSettingsReadDTO>(entity.StoreSettings));
        }

        public async Task<bool> ExistsAsync(Expression<Func<Store, bool>> predicate, string userId)
        {
            var entities = await _storeRepo.QueryAsync(predicate);
            return entities.Any(s => s.UserId == userId);
        }

        public async Task<int> CountAsync(Expression<Func<Store, bool>> predicate, string userId)
        {
            var entities = await _storeRepo.QueryAsync(predicate);
            return entities.Count(s => s.UserId == userId);
        }

        public IQueryable<Store> GetQueryable()
        {
            return _storeRepo.GetQueryable();
        }

       
    }
}
