using Eduprompt.API.DependencyInjection;
using Supabase;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add custom application services
builder.Services.AddApplicationServices(builder.Configuration);

// Add JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add Google Authentication
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Google:ClientId"] ?? "";
        options.ClientSecret = builder.Configuration["Google:ClientSecret"] ?? "";
    });

// Add Supabase Client
var supabaseUrl = builder.Configuration["Supabase:Url"];
var supabaseKey = builder.Configuration["Supabase:ServiceRoleKey"];

if (!string.IsNullOrEmpty(supabaseUrl) && !string.IsNullOrEmpty(supabaseKey))
{
    builder.Services.AddSingleton(provider => new Supabase.Client(supabaseUrl, supabaseKey, new Supabase.SupabaseOptions
    {
        AutoRefreshToken = true,
        AutoConnectRealtime = true
    }));
}
else
{
    builder.Services.AddSingleton(provider => new Supabase.Client("", "", new Supabase.SupabaseOptions()));
}

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173", "http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Eduprompt API",
        Version = "v1.0"
        // Intentionally no Description/Contact/License to keep the UI minimal (endpoints only)
    });

    // Enable XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = @"
JWT Authorization header sử dụng Bearer scheme.

Nhập 'Bearer' [space] và sau đó là token của bạn.

**Ví dụ:** `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`

**Cách lấy token:**
1. Gọi `/api/auth/login` với email và password
2. Copy giá trị `token` trong response
3. Paste vào đây với prefix 'Bearer '
        "
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Group by tags
    options.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] ?? "Default" });
    options.DocInclusionPredicate((name, api) => true);
    
    // Order tags
    options.OrderActionsBy((apiDesc) => 
        $"{GetTagOrder(apiDesc.GroupName ?? apiDesc.ActionDescriptor.RouteValues["controller"] ?? "")}{apiDesc.RelativePath}");

});

// Helper function to order tags
static string GetTagOrder(string tag)
{
    // Tags already contain ordering numbers like "🔑 01. Authentication"
    // Extract the number or return tag as is for sorting
    if (string.IsNullOrEmpty(tag)) return "99_";
    
    // If tag already starts with emoji and number, use it as is
    return tag;
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Eduprompt API v1.0");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "Eduprompt API Documentation";
        
        // UI improvements
        options.DisplayRequestDuration();
        options.EnableDeepLinking();
        options.EnableFilter();
        options.ShowExtensions();
        options.EnableValidator();
        
        // Expand all by default for better UX
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        options.DefaultModelsExpandDepth(2);
        options.DefaultModelExpandDepth(2);
    });
}

// Use global exception handling middleware
app.UseMiddleware<Eduprompt.API.Middleware.ExceptionHandlingMiddleware>();

// Only use HTTPS redirection in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Configure CORS and COOP for Google OAuth
app.UseCors("AllowAll");

// Set Cross-Origin-Opener-Policy to allow Google OAuth popup
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin-allow-popups");
    context.Response.Headers.Append("Cross-Origin-Embedder-Policy", "unsafe-none");
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Redirect root to Swagger
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

// Ensure DB schema aligns with code on startup
using (var scope = app.Services.CreateScope())
{
    var schemaUpdater = scope.ServiceProvider.GetRequiredService<IDatabaseSchemaUpdater>();
    await schemaUpdater.EnsureSchemaAsync();
    var dataSeeder = scope.ServiceProvider.GetRequiredService<IDatabaseDataSeeder>();
    await dataSeeder.SeedAsync();
}

app.Run();
