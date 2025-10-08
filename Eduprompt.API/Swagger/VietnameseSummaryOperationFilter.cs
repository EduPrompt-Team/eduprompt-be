using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Eduprompt.API.Swagger;

public class VietnameseSummaryOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var httpMethod = context.ApiDescription.HttpMethod?.ToUpperInvariant() ?? string.Empty;
        var controller = context.ApiDescription.ActionDescriptor.RouteValues.TryGetValue("controller", out var c)
            ? c
            : "tài nguyên";

        // Resource name in Vietnamese-friendly format (with custom mapping)
        var resource = MapResource(ToWords(controller ?? string.Empty));

        operation.Summary = httpMethod switch
        {
            "GET" when context.ApiDescription.ParameterDescriptions.Any(p => p.Name.EndsWith("id", StringComparison.OrdinalIgnoreCase))
                => $"Lấy chi tiết {resource}",
            "GET" => $"Lấy danh sách {resource}",
            "POST" => $"Thêm {resource} mới",
            "PUT" => $"Cập nhật {resource}",
            "PATCH" => $"Cập nhật một phần {resource}",
            "DELETE" => $"Xóa {resource}",
            _ => operation.Summary
        };
    }

    private static string ToWords(string input)
    {
        // Convert PascalCase controller to spaced words: PaymentMethod -> phương thức thanh toán
        var withSpaces = System.Text.RegularExpressions.Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");
        return withSpaces.ToLowerInvariant();
    }

    private static string MapResource(string resource)
    {
        // Basic Vietnamese mapping for common controllers
        return resource switch
        {
            "auth" => "xác thực",
            "users" or "user" => "người dùng",
            "roles" or "role" => "vai trò",
            "categories" or "category" => "danh mục",
            "wishlist" or "wishlists" => "danh sách yêu thích",
            "storage templates" or "storagetemplates" or "storage-template" => "template lưu trữ",
            "payment method" or "payment methods" => "phương thức thanh toán",
            "transactions" or "transaction" => "giao dịch",
            "wallet" or "wallets" => "ví",
            "package categories" or "package category" => "danh mục gói",
            "package" or "packages" => "gói",
            "messages" or "message" => "tin nhắn",
            "conversations" or "conversation" => "cuộc trò chuyện",
            "ai history" or "aihistories" or "aihistory" => "lịch sử AI",
            "prompt instance" or "prompt instances" => "phiên prompt",
            "cart" or "carts" => "giỏ hàng",
            _ => resource
        };
    }
}


