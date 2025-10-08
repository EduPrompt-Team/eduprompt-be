using Eduprompt.Domain.DTOs.Package;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class PackageService : IPackageService
{
    private readonly IPackageRepository _packageRepository;

    public PackageService(IPackageRepository packageRepository)
    {
        _packageRepository = packageRepository;
    }

    public async Task<PackageDto?> GetByIdAsync(int packageId)
    {
        var package = await _packageRepository.GetByIdAsync(packageId);
        return package != null ? MapToDto(package) : null;
    }

    public async Task<IEnumerable<PackageDto>> GetAllAsync()
    {
        var packages = await _packageRepository.GetAllAsync();
        return packages.Select(MapToDto);
    }

    public async Task<IEnumerable<PackageDto>> GetByCategoryIdAsync(int categoryId)
    {
        var packages = await _packageRepository.GetByCategoryIdAsync(categoryId);
        return packages.Select(MapToDto);
    }

    public async Task<IEnumerable<PackageDto>> GetActivePackagesAsync()
    {
        var packages = await _packageRepository.GetActivePackagesAsync();
        return packages.Select(MapToDto);
    }

    public async Task<PackageDto> CreateAsync(CreatePackageDto createPackageDto)
    {
        var package = new Package
        {
            CategoryID = createPackageDto.CategoryID,
            PackageName = createPackageDto.PackageName,
            Description = createPackageDto.Description,
            Price = createPackageDto.Price,
            Duration = createPackageDto.Duration,
            MaxUsage = createPackageDto.MaxUsage,
            Features = createPackageDto.Features,
            Status = createPackageDto.Status,
            CreatedDate = DateTime.UtcNow
        };

        var createdPackage = await _packageRepository.CreateAsync(package);
        return MapToDto(createdPackage);
    }

    public async Task<PackageDto> UpdateAsync(int packageId, UpdatePackageDto updatePackageDto)
    {
        var package = await _packageRepository.GetByIdAsync(packageId);
        if (package == null)
            throw new ArgumentException("Package not found");

        if (updatePackageDto.CategoryID.HasValue)
            package.CategoryID = updatePackageDto.CategoryID.Value;

        if (!string.IsNullOrEmpty(updatePackageDto.PackageName))
            package.PackageName = updatePackageDto.PackageName;

        if (updatePackageDto.Description != null)
            package.Description = updatePackageDto.Description;

        if (updatePackageDto.Price.HasValue)
            package.Price = updatePackageDto.Price.Value;

        if (updatePackageDto.Duration.HasValue)
            package.Duration = updatePackageDto.Duration.Value;

        if (updatePackageDto.MaxUsage.HasValue)
            package.MaxUsage = updatePackageDto.MaxUsage.Value;

        if (updatePackageDto.Features != null)
            package.Features = updatePackageDto.Features;

        if (!string.IsNullOrEmpty(updatePackageDto.Status))
            package.Status = updatePackageDto.Status;

        package.UpdatedDate = DateTime.UtcNow;

        var updatedPackage = await _packageRepository.UpdateAsync(package);
        return MapToDto(updatedPackage);
    }

    public async Task<bool> DeleteAsync(int packageId)
    {
        return await _packageRepository.DeleteAsync(packageId);
    }

    public async Task<IEnumerable<PackageDto>> SearchAsync(string searchTerm)
    {
        var packages = await _packageRepository.SearchAsync(searchTerm);
        return packages.Select(MapToDto);
    }

    public async Task<IEnumerable<PackageDto>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        var packages = await _packageRepository.GetByPriceRangeAsync(minPrice, maxPrice);
        return packages.Select(MapToDto);
    }

    private static PackageDto MapToDto(Package package)
    {
        return new PackageDto
        {
            PackageID = package.PackageID,
            CategoryID = package.CategoryID,
            PackageName = package.PackageName,
            Description = package.Description,
            Price = package.Price,
            Duration = package.Duration,
            MaxUsage = package.MaxUsage,
            Features = package.Features,
            CreatedDate = package.CreatedDate,
            UpdatedDate = package.UpdatedDate,
            Status = package.Status,
            CategoryName = package.PackageCategory?.CategoryName
        };
    }
}
