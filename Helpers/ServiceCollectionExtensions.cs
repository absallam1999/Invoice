using Core.Interfaces.Services;
using invoice.Core.Interfaces.Services;
using invoice.Services;
using invoice.Services.Payments;
using invoice.Services.Payments.Edfa;
using invoice.Services.Payments.Paypal;
using invoice.Services.Payments.Stripe;
using invoice.Services.Payments.TabPayments;

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

            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<INotificationService, NotificationService>();

            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IInvoiceItemsService, InvoiceItemsService>();
            services.AddScoped<IPayInvoiceService, PayInvoiceService>();

            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IPaymentLinkService, PaymentLinkService>();
            services.AddScoped<IPaymentMethodService, PaymentMethodService>();
            services.AddScoped<IPaymentGatewayService, PaymentGatewayService>();

            services.AddScoped<PaymentGatewayFactory>();

          //  services.AddScoped<StripePaymentService>();
            //services.AddScoped<PayPalPaymentService>();
          ///  services.AddScoped<EdfaPaymentService>();

            services.AddScoped<TabPaymentsService>();

            services.AddScoped<IPOSService, POSService>();
            services.AddScoped<IPageService, PageService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<ITaxService, TaxService>();
            services.AddScoped<ICurrencyService, CurrencyService>();

            return services;
        }

        public static IServiceCollection AddPaymentGatewayOptions(this IServiceCollection services, IConfiguration configuration)
        {
            // Register payment gateway options
            services.Configure<EdfaOptions>(configuration.GetSection("Edfa"));

            services.Configure<PayPalOptions>(configuration.GetSection("PayPal"));
            services.Configure<StripeOptions>(configuration.GetSection("Stripe"));
            services.Configure<TabPaymentsOptions>(configuration.GetSection("TabPayments"));

            services.AddHttpClient("PayPal", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddHttpClient("Edfa", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddHttpClient("TabPayments", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddHttpClient("Stripe", client =>
            {
                client.BaseAddress = new Uri("https://api.stripe.com/");
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            return services;
        }

        public static IServiceCollection AddAllApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationServices();
            services.AddPaymentGatewayOptions(configuration);

            return services;
        }
    }
}
