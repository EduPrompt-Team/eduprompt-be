using Eduprompt.Domain.DTOs.Apikey;

namespace Eduprompt.Domain.Interface.Service;

public interface IApikeyService
{
    Task<ApikeyDto?> GetByIdAsync(int ApikeyId);
    Task<IEnumerable<ApikeyDto>> GetByPackageIdAsync(int packageId);
    Task<IEnumerable<ApikeyDto>> GetActiveKeysByPackageIdAsync(int packageId);
    Task<ApikeyDto?> GetActiveKeyByProviderAsync(string provider);
    Task<ApikeyDto> CreateAsync(CreateApikeyDto createDto);
    Task<ApikeyDto> UpdateAsync(int ApikeyId, CreateApikeyDto updateDto);
    Task<bool> DeleteAsync(int ApikeyId);
}


