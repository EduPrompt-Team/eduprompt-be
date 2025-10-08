using Eduprompt.Domain.DTOs.PackageDetail;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class PackageDetailService : IPackageDetailService
{
    private readonly IPackageDetailRepository _detailRepository;
    private readonly IPackageRepository _packageRepository;

    public PackageDetailService(IPackageDetailRepository detailRepository, IPackageRepository packageRepository)
    {
        _detailRepository = detailRepository;
        _packageRepository = packageRepository;
    }

    public async Task<PackageDetailDto?> GetByIdAsync(int detailId)
    {
        var e = await _detailRepository.GetByIdAsync(detailId);
        return e == null ? null : Map(e);
    }

    public async Task<IEnumerable<PackageDetailDto>> GetByPackageIdAsync(int packageId)
    {
        var list = await _detailRepository.GetByPackageIdAsync(packageId);
        return list.Select(Map);
    }

    public async Task<PackageDetailDto> CreateAsync(CreatePackageDetailDto createDto)
    {
        var pkg = await _packageRepository.GetByIdAsync(createDto.PackageID);
        if (pkg == null) throw new ArgumentException("Package not found");

        var e = new Eduprompt.Domain.Entities.PackageDetail
        {
            PackageID = createDto.PackageID,
            DetailType = createDto.Unit ?? "Feature",
            DetailContent = createDto.FeatureDescription ?? createDto.FeatureName,
            OrderIndex = createDto.Limit ?? 0,
            Status = "Active",
            CreatedDate = DateTime.UtcNow
        };
        var created = await _detailRepository.CreateAsync(e);
        return Map(created);
    }

    public async Task<PackageDetailDto> UpdateAsync(int detailId, CreatePackageDetailDto updateDto)
    {
        var e = await _detailRepository.GetByIdAsync(detailId);
        if (e == null) throw new KeyNotFoundException("Package detail not found");

        if (updateDto.PackageID != 0) e.PackageID = updateDto.PackageID;
        if (updateDto.FeatureDescription != null) e.DetailContent = updateDto.FeatureDescription;
        if (!string.IsNullOrEmpty(updateDto.FeatureName)) e.DetailContent = updateDto.FeatureName;
        if (updateDto.Unit != null) e.DetailType = updateDto.Unit;
        if (updateDto.Limit.HasValue) e.OrderIndex = updateDto.Limit.Value;

        var updated = await _detailRepository.UpdateAsync(e);
        return Map(updated);
    }

    public async Task<bool> DeleteAsync(int detailId)
    {
        return await _detailRepository.DeleteAsync(detailId);
    }

    private static PackageDetailDto Map(Eduprompt.Domain.Entities.PackageDetail e)
    {
        return new PackageDetailDto
        {
            DetailID = e.DetailID,
            PackageID = e.PackageID,
            FeatureName = e.DetailContent,
            FeatureDescription = e.DetailContent,
            IsIncluded = e.Status == "Active",
            Limit = e.OrderIndex,
            Unit = e.DetailType,
            PackageName = e.Package?.PackageName
        };
    }
}


