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

    public async Task<PackageDto?> GetByIdAsync(int PackageId)
    {
        var package = await _packageRepository.GetByIdAsync(PackageId);
        return package != null ? MapToDto(package) : null;
    }

    public async Task<IEnumerable<PackageDto>> GetAllAsync()
    {
        var packages = await _packageRepository.GetAllAsync();
        return packages.Select(MapToDto);
    }

    public async Task<IEnumerable<PackageDto>> GetByCategoryIdAsync(int CategoryId)
    {
        var packages = await _packageRepository.GetByCategoryIdAsync(CategoryId);
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
            CategoryId = createPackageDto.CategoryId,
            PackageName = createPackageDto.PackageName,
            Description = createPackageDto.Description,
            Price = createPackageDto.Price,
            DurationDays = createPackageDto.DurationDays,
            IsActive = createPackageDto.IsActive,
            CreatedDate = DateTime.UtcNow
        };

        var createdPackage = await _packageRepository.CreateAsync(package);
        return MapToDto(createdPackage);
    }

    public async Task<PackageDto> UpdateAsync(int PackageId, UpdatePackageDto updateDto)
    {
        var package = await _packageRepository.GetByIdAsync(PackageId);
        if (package == null) throw new KeyNotFoundException("Package not found");

        package.CategoryId = updateDto.CategoryId ?? package.CategoryId;
        package.PackageName = updateDto.PackageName ?? package.PackageName;
        package.Description = updateDto.Description ?? package.Description;
        package.Price = updateDto.Price ?? package.Price;
        package.DurationDays = updateDto.DurationDays ?? package.DurationDays;
        package.IsActive = updateDto.IsActive ?? package.IsActive;

        var updatedPackage = await _packageRepository.UpdateAsync(package);
        return MapToDto(updatedPackage);
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

    public async Task<bool> DeleteAsync(int PackageId)
    {
        return await _packageRepository.DeleteAsync(PackageId);
    }

    private static PackageDto MapToDto(Package package)
    {
        return new PackageDto
        {
            PackageId = package.PackageId,
            CategoryId = package.CategoryId,
            PackageName = package.PackageName,
            Description = package.Description,
            Price = package.Price,
            DurationDays = package.DurationDays,
            IsActive = package.IsActive,
            CreatedDate = package.CreatedDate,
            CategoryName = package.Category?.CategoryName ?? "Unknown Category"
        };
    }
}