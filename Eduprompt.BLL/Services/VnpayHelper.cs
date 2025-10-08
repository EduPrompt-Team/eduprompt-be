using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Eduprompt.BLL.Services;

/// <summary>
/// VNPAY Payment Gateway Helper
/// Handles URL generation and signature verification for VNPAY
/// </summary>
public class VnpayHelper
{
    private readonly IConfiguration _configuration;

    public VnpayHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Create VNPAY payment URL
    /// </summary>
    public string CreatePaymentUrl(VnpayRequestData requestData)
    {
        // TODO: Get these from appsettings.json when VNPAY credentials are available
        var vnp_Url = _configuration["VNPay:Url"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        var vnp_TmnCode = _configuration["VNPay:TmnCode"] ?? "YOUR_TMN_CODE_HERE"; // ← REPLACE when ready
        var vnp_HashSecret = _configuration["VNPay:HashSecret"] ?? "YOUR_HASH_SECRET_HERE"; // ← REPLACE when ready
        
        var vnpay = new VnPayLibrary();
        
        vnpay.AddRequestData("vnp_Version", "2.1.0");
        vnpay.AddRequestData("vnp_Command", "pay");
        vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
        vnpay.AddRequestData("vnp_Amount", (requestData.Amount * 100).ToString()); // VNPay uses smallest unit
        vnpay.AddRequestData("vnp_CreateDate", requestData.CreateDate.ToString("yyyyMMddHHmmss"));
        vnpay.AddRequestData("vnp_CurrCode", "VND");
        vnpay.AddRequestData("vnp_IpAddr", requestData.IpAddress);
        vnpay.AddRequestData("vnp_Locale", requestData.Locale);
        vnpay.AddRequestData("vnp_OrderInfo", requestData.OrderInfo);
        vnpay.AddRequestData("vnp_OrderType", requestData.OrderType);
        vnpay.AddRequestData("vnp_ReturnUrl", requestData.ReturnUrl);
        vnpay.AddRequestData("vnp_TxnRef", requestData.OrderId.ToString());
        
        if (!string.IsNullOrEmpty(requestData.BankCode))
        {
            vnpay.AddRequestData("vnp_BankCode", requestData.BankCode);
        }
        
        string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
        return paymentUrl;
    }

    /// <summary>
    /// Verify VNPAY callback signature
    /// </summary>
    public bool ValidateSignature(Dictionary<string, string> queryParams, string secureHash)
    {
        var vnp_HashSecret = _configuration["VNPay:HashSecret"] ?? "YOUR_HASH_SECRET_HERE"; // ← REPLACE when ready
        
        var vnpay = new VnPayLibrary();
        foreach (var param in queryParams)
        {
            if (param.Key != "vnp_SecureHash" && param.Key != "vnp_SecureHashType")
            {
                vnpay.AddResponseData(param.Key, param.Value);
            }
        }
        
        bool checkSignature = vnpay.ValidateSignature(secureHash, vnp_HashSecret);
        return checkSignature;
    }
}

public class VnpayRequestData
{
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string OrderInfo { get; set; } = string.Empty;
    public string OrderType { get; set; } = "other";
    public string Locale { get; set; } = "vn";
    public string ReturnUrl { get; set; } = string.Empty;
    public string IpAddress { get; set; } = "127.0.0.1";
    public string? BankCode { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.Now;
}

/// <summary>
/// VNPay Library for generating and validating payment URLs
/// Based on VNPAY official documentation
/// </summary>
internal class VnPayLibrary
{
    private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
    private readonly SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

    public void AddRequestData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _requestData.Add(key, value);
        }
    }

    public void AddResponseData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _responseData.Add(key, value);
        }
    }

    public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
    {
        var data = new StringBuilder();
        foreach (var kv in _requestData)
        {
            if (!string.IsNullOrEmpty(kv.Value))
            {
                data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
            }
        }
        
        string queryString = data.ToString();
        
        if (queryString.Length > 0)
        {
            queryString = queryString.Remove(queryString.Length - 1, 1);
        }

        string signData = queryString;
        string vnp_SecureHash = HmacSHA512(vnp_HashSecret, signData);
        string paymentUrl = baseUrl + "?" + queryString + "&vnp_SecureHash=" + vnp_SecureHash;

        return paymentUrl;
    }

    public bool ValidateSignature(string inputHash, string secretKey)
    {
        var data = new StringBuilder();
        foreach (var kv in _responseData)
        {
            if (!string.IsNullOrEmpty(kv.Value))
            {
                data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
            }
        }
        
        string rspRaw = data.ToString();
        
        if (rspRaw.Length > 0)
        {
            rspRaw = rspRaw.Remove(rspRaw.Length - 1, 1);
        }

        string myChecksum = HmacSHA512(secretKey, rspRaw);
        return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
    }

    private string HmacSHA512(string key, string inputData)
    {
        var hash = new StringBuilder();
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
        using (var hmac = new HMACSHA512(keyBytes))
        {
            byte[] hashValue = hmac.ComputeHash(inputBytes);
            foreach (var theByte in hashValue)
            {
                hash.Append(theByte.ToString("x2"));
            }
        }

        return hash.ToString();
    }
}

internal class VnPayCompare : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (x == y) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        
        var vnpCompare = CompareInfo.GetCompareInfo("en-US");
        return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
    }
} 