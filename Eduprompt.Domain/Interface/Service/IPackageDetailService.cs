using Eduprompt.Domain.DTOs.PackageDetail;

namespace Eduprompt.Domain.Interface.Service;

public interface IPackageDetailService
{
    Task<PackageDetailDto?> GetByIdAsync(int detailId);
    Task<IEnumerable<PackageDetailDto>> GetByPackageIdAsync(int packageId);
    Task<PackageDetailDto> CreateAsync(CreatePackageDetailDto createDto);
    Task<PackageDetailDto> UpdateAsync(int detailId, CreatePackageDetailDto updateDto);
    Task<bool> DeleteAsync(int detailId);
}


