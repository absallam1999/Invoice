using Core.Interfaces.Services;
using invoice.Core.Interfaces.Services;
using invoice.Services;
using invoice.Services.Payments;
using invoice.Services.Payments.ApplePay;
using invoice.Services.Payments.Mada;
using invoice.Services.Payments.Paypal;
using invoice.Services.Payments.Stripe;

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

          //  services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IPaymentLinkService, PaymentLinkService>();
            services.AddScoped<IPaymentMethodService, PaymentMethodService>();
            services.AddScoped<IPaymentGatewayService, PaymentGatewayService>();

            services.AddScoped<PaymentGatewayFactory>();
            services.AddScoped<StripePaymentService>();
            services.AddScoped<PayPalPaymentService>();
            services.AddScoped<MadaPaymentService>();
            services.AddScoped<ApplePayPaymentService>();

            services.AddScoped<IPOSService, POSService>();
            services.AddScoped<IPageService, PageService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IOrderService, OrderService>();
           // services.AddScoped<IContactInfoService, ContactInfoService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<ITaxService, TaxService>();
            services.AddScoped<ICurrencyService, CurrencyService>();

            return services;
        }

        public static IServiceCollection AddPaymentGatewayOptions(this IServiceCollection services, IConfiguration configuration)
        {
            // Register payment gateway options
            services.Configure<StripeOptions>(configuration.GetSection("Stripe"));
            services.Configure<PayPalOptions>(configuration.GetSection("PayPal"));
            services.Configure<MadaOptions>(configuration.GetSection("Mada"));
            services.Configure<ApplePayOptions>(configuration.GetSection("ApplePay"));

            // Configure HTTP clients
            services.AddHttpClient("PayPal", client =>
            {
                client.BaseAddress = new Uri(configuration["PayPal:BaseUrl"] ?? "https://api-m.sandbox.paypal.com");
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddHttpClient("Stripe", client =>
            {
                client.BaseAddress = new Uri("https://api.stripe.com/");
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddHttpClient("Mada", client =>
            {
                client.BaseAddress = new Uri(configuration["Mada:BaseUrl"] ?? "https://secure.mada.com.sa");
                client.Timeout = TimeSpan.FromSeconds(30);
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
