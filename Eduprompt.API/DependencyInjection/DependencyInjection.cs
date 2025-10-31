using Eduprompt.BLL.Mapping;
using Eduprompt.BLL.Services;
using Eduprompt.DAL.DbContexts;
using Eduprompt.DAL.Repositories;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Eduprompt.API.DependencyInjection;

namespace Eduprompt.API.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<EdupromptV2Context>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Add AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));

        // Schema updater
        services.AddScoped<IDatabaseSchemaUpdater, DatabaseSchemaUpdater>();
        services.AddScoped<IDatabaseDataSeeder, DatabaseDataSeeder>();

        // Add Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IWishlistRepository, WishlistRepository>();
        services.AddScoped<IStorageTemplateRepository, StorageTemplateRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        
        // New Repositories
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>(); // Repository disabled but interface maintained
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IPackageRepository, PackageRepository>();
        services.AddScoped<IPromptInstanceRepository, PromptInstanceRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IAihistoryRepository, AIHistoryRepository>();
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        services.AddScoped<IPackageCategoryRepository, PackageCategoryRepository>();
        services.AddScoped<IPackageDetailRepository, PackageDetailRepository>();
        services.AddScoped<IApikeyRepository, APIKeyRepository>();
        services.AddScoped<IPromptInstanceDetailRepository, PromptInstanceDetailRepository>();
        services.AddScoped<ITemplateArchitectureRepository, TemplateArchitectureRepository>();
        services.AddScoped<IExpectedOutputRepository, ExpectedOutputRepository>();
        services.AddScoped<IOutputDetailRepository, OutputDetailRepository>();

        // Add Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        // services.AddScoped<IStorageService, StorageService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IWishlistService, WishlistService>();
        services.AddScoped<IStorageTemplateService, StorageTemplateService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        
        // New Services
        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<IPackageService, PackageService>();
        services.AddScoped<IPromptInstanceService, PromptInstanceService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IConversationService, ConversationService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IPaymentMethodService, PaymentMethodService>(); // Service disabled but interface maintained
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IAihistoryService, AihistoryService>();
        services.AddScoped<IFeedbackService, FeedbackService>();
        services.AddScoped<IPackageCategoryService, PackageCategoryService>();
        services.AddScoped<IApikeyService, ApikeyService>();
        services.AddScoped<IPackageDetailService, PackageDetailService>();
        services.AddScoped<ITemplateArchitectureService, TemplateArchitectureService>();
        services.AddScoped<IPromptInstanceDetailService, PromptInstanceDetailService>();
        services.AddScoped<IExpectedOutputService, ExpectedOutputService>();
        services.AddScoped<ITemplateCommerceService, TemplateCommerceService>();
        services.AddScoped<IPaymentService, PaymentService>();

        // Add HttpClient for Google API calls
        services.AddHttpClient<IGoogleAuthService, GoogleAuthService>();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("Jwt");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
            };
        });

        // Add Authorization policies
        services.AddAuthorization(options =>
        {
            // Policy for Admin-only endpoints
            options.AddPolicy("AdminOnly", policy =>
            {
                policy.RequireRole("Admin");
            });

            // Policy for Admin or User own resource
            options.AddPolicy("AdminOrOwner", policy =>
            {
                policy.RequireAuthenticatedUser();
            });

            // Policy for authenticated users (any role)
            options.AddPolicy("AuthenticatedUser", policy =>
            {
                policy.RequireAuthenticatedUser();
            });
        });

        return services;
    }
}
