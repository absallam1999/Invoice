using Core.Interfaces.Services;
using invoice.Core.Interfaces.Services;
using invoice.Services;

namespace invoice.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IClientService, ClientService>();
            
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();

            services.AddScoped<ILanguageService, LanguageService> ();
            services.AddScoped<INotificationService, NotificationService>();
            
            services.AddScoped<IInvoiceService, InvoiceService> ();
            services.AddScoped<IInvoiceItemsService, InvoiceItemsService>();
            services.AddScoped<IPayInvoiceService, PayInvoiceService>();

            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IPaymentLinkService, PaymentLinkService>();
            services.AddScoped<IPaymentMethodService, PaymentMethodService>();
            services.AddScoped<IPaymentGatewayService, PaymentGatewayService>();

            services.AddScoped<IPageService, PageService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IContactInfoService, ContactInfoService>();
            services.AddScoped<ITaxService, TexService>();

            return services;
        }
    }
}
