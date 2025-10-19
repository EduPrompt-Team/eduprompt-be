using Eduprompt.Domain.DTOs.PaymentMethod;
using Eduprompt.Domain.Entities;
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

        return MapToDto(paymentMethod);
    }

    public async Task<IEnumerable<PaymentMethodDto>> GetByUserIdAsync(int userId)
    {
        // PaymentMethod is global, not user-specific in current design
        var paymentMethods = await _paymentMethodRepository.GetAllAsync();
        return paymentMethods.Select(MapToDto);
    }

    public async Task<IEnumerable<PaymentMethodDto>> GetAllAsync()
    {
        var paymentMethods = await _paymentMethodRepository.GetAllAsync();
        return paymentMethods.Select(MapToDto);
    }

    public async Task<PaymentMethodDto> CreateAsync(CreatePaymentMethodDto createDto)
    {
        var paymentMethod = new PaymentMethod
        {
            MethodName = createDto.MethodName,
            Provider = createDto.Provider,
            IsActive = createDto.IsActive,
            ProcessingFee = createDto.ProcessingFee
        };

        var createdPaymentMethod = await _paymentMethodRepository.CreateAsync(paymentMethod);
        return MapToDto(createdPaymentMethod);
    }

    public async Task<PaymentMethodDto> UpdateAsync(int paymentMethodId, CreatePaymentMethodDto updateDto)
    {
        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(paymentMethodId);
        if (paymentMethod == null)
            throw new KeyNotFoundException("Payment method not found");

        paymentMethod.MethodName = updateDto.MethodName;
        paymentMethod.Provider = updateDto.Provider;
        paymentMethod.IsActive = updateDto.IsActive;
        paymentMethod.ProcessingFee = updateDto.ProcessingFee;

        var updatedPaymentMethod = await _paymentMethodRepository.UpdateAsync(paymentMethod);
        return MapToDto(updatedPaymentMethod);
    }

    public async Task<bool> DeleteAsync(int paymentMethodId)
    {
        return await _paymentMethodRepository.DeleteAsync(paymentMethodId);
    }

    public async Task<PaymentMethodDto?> GetDefaultByUserIdAsync(int userId)
    {
        // PaymentMethod is global, not user-specific in current design
        var paymentMethods = await _paymentMethodRepository.GetAllAsync();
        var defaultMethod = paymentMethods.FirstOrDefault(pm => pm.IsActive);
        
        if (defaultMethod == null) return null;

        return MapToDto(defaultMethod);
    }

    public async Task<bool> SetAsDefaultAsync(int paymentMethodId, int userId)
    {
        // PaymentMethod is global, not user-specific in current design
        var paymentMethods = await _paymentMethodRepository.GetAllAsync();
        var targetMethod = paymentMethods.FirstOrDefault(pm => pm.PaymentMethodID == paymentMethodId);
        
        if (targetMethod == null) return false;

        // Remove default from all other methods
        foreach (var method in paymentMethods)
        {
            if (method.PaymentMethodID != paymentMethodId)
            {
                method.IsActive = false;
                await _paymentMethodRepository.UpdateAsync(method);
            }
        }

        // Set target method as default
        targetMethod.IsActive = true;
        await _paymentMethodRepository.UpdateAsync(targetMethod);
        
        return true;
    }

    private static PaymentMethodDto MapToDto(PaymentMethod paymentMethod)
    {
        return new PaymentMethodDto
        {
            PaymentMethodID = paymentMethod.PaymentMethodID,
            MethodName = paymentMethod.MethodName,
            Provider = paymentMethod.Provider,
            IsActive = paymentMethod.IsActive,
            ProcessingFee = paymentMethod.ProcessingFee
        };
    }
}