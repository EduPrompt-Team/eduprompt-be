using Eduprompt.Domain.DTOs.PackageDetail;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class PackageDetailService : IPackageDetailService
{
    private readonly IPackageDetailRepository _packageDetailRepository;

    public PackageDetailService(IPackageDetailRepository packageDetailRepository)
    {
        _packageDetailRepository = packageDetailRepository;
    }

    public async Task<PackageDetailDto?> GetByIdAsync(int DetailId)
    {
        var detail = await _packageDetailRepository.GetByIdAsync(DetailId);
        return detail != null ? MapToDto(detail) : null;
    }

    public async Task<IEnumerable<PackageDetailDto>> GetByPackageIdAsync(int PackageId)
    {
        var details = await _packageDetailRepository.GetByPackageIdAsync(PackageId);
        return details.Select(MapToDto);
    }

    public async Task<PackageDetailDto> CreateAsync(CreatePackageDetailDto createDto)
    {
        var detail = new PackageDetail
        {
            PackageId = createDto.PackageId,
            FeatureName = createDto.FeatureName,
            FeatureValue = createDto.FeatureDescription ?? (createDto.IsIncluded ? "Included" : "Excluded"),
            FeatureType = createDto.Unit ?? "Text"
        };

        var createdDetail = await _packageDetailRepository.CreateAsync(detail);
        return MapToDto(createdDetail);
    }

    public async Task<PackageDetailDto> UpdateAsync(int DetailId, CreatePackageDetailDto updateDto)
    {
        var detail = await _packageDetailRepository.GetByIdAsync(DetailId);
        if (detail == null) throw new KeyNotFoundException("Package detail not found");

        detail.FeatureName = updateDto.FeatureName;
        detail.FeatureValue = updateDto.FeatureDescription ?? (updateDto.IsIncluded ? "Included" : "Excluded");
        detail.FeatureType = updateDto.Unit ?? "Text";

        var updatedDetail = await _packageDetailRepository.UpdateAsync(detail);
        return MapToDto(updatedDetail);
    }

    public async Task<bool> DeleteAsync(int DetailId)
    {
        return await _packageDetailRepository.DeleteAsync(DetailId);
    }

    private static PackageDetailDto MapToDto(PackageDetail detail)
    {
        return new PackageDetailDto
        {
            DetailId = detail.DetailId,
            PackageId = detail.PackageId,
            FeatureName = detail.FeatureName,
            FeatureDescription = detail.FeatureValue,
            IsIncluded = detail.FeatureValue?.Equals("Included", StringComparison.OrdinalIgnoreCase) == true,
            Unit = detail.FeatureType,
            PackageName = detail.Package?.PackageName ?? "Unknown Package"
        };
    }
}