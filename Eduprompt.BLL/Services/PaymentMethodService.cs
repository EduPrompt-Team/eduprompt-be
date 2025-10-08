using Eduprompt.Domain.DTOs.PaymentMethod;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class PaymentMethodService : IPaymentMethodService
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public PaymentMethodService(IPaymentMethodRepository paymentMethodRepository)
    {
        _paymentMethodRepository = paymentMethodRepository;
    }

    public async Task<PaymentMethodDto?> GetByIdAsync(int paymentMethodId)
    {
        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(paymentMethodId);
        if (paymentMethod == null) return null;

        return new PaymentMethodDto
        {
            PaymentMethodID = paymentMethod.PaymentMethodID,
            UserID = paymentMethod.UserID,
            MethodType = paymentMethod.MethodType,
            // CardNumber = null, // PaymentMethod entity doesn't have CardNumber property
            // CardHolderName = null, // PaymentMethod entity doesn't have CardHolderName property
            // ExpiryDate = null, // PaymentMethod entity doesn't have ExpiryDate property
            // CVV = null, // PaymentMethod entity doesn't have CVV property
            BankName = paymentMethod.BankName,
            AccountNumber = paymentMethod.AccountNumber,
            IsDefault = paymentMethod.IsDefault,
            CreatedDate = paymentMethod.CreatedDate,
            UpdatedDate = paymentMethod.UpdatedDate,
            Status = paymentMethod.Status,
            UserName = paymentMethod.User?.FullName
        };
    }

    public async Task<IEnumerable<PaymentMethodDto>> GetByUserIdAsync(int userId)
    {
        var paymentMethods = await _paymentMethodRepository.GetByUserIdAsync(userId);
        return paymentMethods.Select(pm => new PaymentMethodDto
        {
            PaymentMethodID = pm.PaymentMethodID,
            UserID = pm.UserID,
            MethodType = pm.MethodType,
            // CardNumber = pm.CardNumber, // PaymentMethod entity doesn't have Card properties
            // CardHolderName = pm.CardHolderName,
            // ExpiryDate = pm.ExpiryDate,
            // CVV = pm.CVV,
            BankName = pm.BankName,
            AccountNumber = pm.AccountNumber,
            IsDefault = pm.IsDefault,
            CreatedDate = pm.CreatedDate,
            UpdatedDate = pm.UpdatedDate,
            Status = pm.Status,
            UserName = pm.User?.FullName
        });
    }

    public async Task<PaymentMethodDto> CreateAsync(CreatePaymentMethodDto createDto)
    {
        var paymentMethod = new Eduprompt.Domain.Entities.PaymentMethod
        {
            UserID = createDto.UserID,
            MethodType = createDto.MethodType,
            // CardNumber = createDto.CardNumber, // PaymentMethod entity doesn't have these properties
            // CardHolderName = createDto.CardHolderName,
            // ExpiryDate = createDto.ExpiryDate,
            // CVV = createDto.CVV,
            BankName = createDto.BankName,
            AccountNumber = createDto.AccountNumber,
            IsDefault = createDto.IsDefault,
            Status = createDto.Status ?? "Active",
            CreatedDate = DateTime.UtcNow
        };

        var createdPaymentMethod = await _paymentMethodRepository.CreateAsync(paymentMethod);
        return new PaymentMethodDto
        {
            PaymentMethodID = createdPaymentMethod.PaymentMethodID,
            UserID = createdPaymentMethod.UserID,
            MethodType = createdPaymentMethod.MethodType,
            // CardNumber = createdPaymentMethod.CardNumber, // PaymentMethod entity doesn't have Card properties
            // CardHolderName = createdPaymentMethod.CardHolderName,
            // ExpiryDate = createdPaymentMethod.ExpiryDate,
            // CVV = createdPaymentMethod.CVV,
            BankName = createdPaymentMethod.BankName,
            AccountNumber = createdPaymentMethod.AccountNumber,
            IsDefault = createdPaymentMethod.IsDefault,
            CreatedDate = createdPaymentMethod.CreatedDate,
            UpdatedDate = createdPaymentMethod.UpdatedDate,
            Status = createdPaymentMethod.Status,
            UserName = createdPaymentMethod.User?.FullName
        };
    }

    public async Task<PaymentMethodDto> UpdateAsync(int paymentMethodId, CreatePaymentMethodDto updateDto)
    {
        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(paymentMethodId);
        if (paymentMethod == null)
            throw new KeyNotFoundException("Payment method not found");

        paymentMethod.MethodName = updateDto.MethodType; // Use MethodName instead of MethodType
        // paymentMethod.CardNumber = updateDto.CardNumber; // PaymentMethod entity doesn't have these properties
        // paymentMethod.CardHolderName = updateDto.CardHolderName;
        // paymentMethod.ExpiryDate = updateDto.ExpiryDate;
        // paymentMethod.CVV = updateDto.CVV;
        paymentMethod.BankName = updateDto.BankName;
        paymentMethod.AccountNumber = updateDto.AccountNumber;
        paymentMethod.IsDefault = updateDto.IsDefault;
        paymentMethod.Status = updateDto.Status ?? paymentMethod.Status;
        paymentMethod.UpdatedDate = DateTime.UtcNow;

        var updatedPaymentMethod = await _paymentMethodRepository.UpdateAsync(paymentMethod);
        return new PaymentMethodDto
        {
            PaymentMethodID = updatedPaymentMethod.PaymentMethodID,
            UserID = updatedPaymentMethod.UserID,
            MethodType = updatedPaymentMethod.MethodType,
            // CardNumber = updatedPaymentMethod.CardNumber, // PaymentMethod entity doesn't have Card properties
            // CardHolderName = updatedPaymentMethod.CardHolderName,
            // ExpiryDate = updatedPaymentMethod.ExpiryDate,
            // CVV = updatedPaymentMethod.CVV,
            BankName = updatedPaymentMethod.BankName,
            AccountNumber = updatedPaymentMethod.AccountNumber,
            IsDefault = updatedPaymentMethod.IsDefault,
            CreatedDate = updatedPaymentMethod.CreatedDate,
            UpdatedDate = updatedPaymentMethod.UpdatedDate,
            Status = updatedPaymentMethod.Status,
            UserName = updatedPaymentMethod.User?.FullName
        };
    }

    public async Task<bool> DeleteAsync(int paymentMethodId)
    {
        return await _paymentMethodRepository.DeleteAsync(paymentMethodId);
    }

    public async Task<PaymentMethodDto?> GetDefaultByUserIdAsync(int userId)
    {
        var paymentMethods = await _paymentMethodRepository.GetByUserIdAsync(userId);
        var defaultMethod = paymentMethods.FirstOrDefault(pm => pm.IsDefault);
        
        if (defaultMethod == null) return null;

        return new PaymentMethodDto
        {
            PaymentMethodID = defaultMethod.PaymentMethodID,
            UserID = defaultMethod.UserID,
            MethodType = defaultMethod.MethodType,
            // CardNumber = defaultMethod.CardNumber, // PaymentMethod entity doesn't have Card properties
            // CardHolderName = defaultMethod.CardHolderName,
            // ExpiryDate = defaultMethod.ExpiryDate,
            // CVV = defaultMethod.CVV,
            BankName = defaultMethod.BankName,
            AccountNumber = defaultMethod.AccountNumber,
            IsDefault = defaultMethod.IsDefault,
            CreatedDate = defaultMethod.CreatedDate,
            UpdatedDate = defaultMethod.UpdatedDate,
            Status = defaultMethod.Status,
            UserName = defaultMethod.User?.FullName
        };
    }

    public async Task<bool> SetAsDefaultAsync(int paymentMethodId, int userId)
    {
        var paymentMethods = await _paymentMethodRepository.GetByUserIdAsync(userId);
        var targetMethod = paymentMethods.FirstOrDefault(pm => pm.PaymentMethodID == paymentMethodId);
        
        if (targetMethod == null) return false;

        // Remove default from all other methods
        foreach (var method in paymentMethods)
        {
            if (method.PaymentMethodID != paymentMethodId)
            {
                method.IsDefault = false;
                await _paymentMethodRepository.UpdateAsync(method);
            }
        }

        // Set target method as default
        targetMethod.IsDefault = true;
        await _paymentMethodRepository.UpdateAsync(targetMethod);
        
        return true;
    }
}
