using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IPackageDetailRepository
{
    Task<PackageDetail?> GetByIdAsync(int detailId);
    Task<IEnumerable<PackageDetail>> GetByPackageIdAsync(int packageId);
    Task<PackageDetail> CreateAsync(PackageDetail detail);
    Task<PackageDetail> UpdateAsync(PackageDetail detail);
    Task<bool> DeleteAsync(int detailId);
    Task<bool> ExistsAsync(int detailId);
    Task<IEnumerable<PackageDetail>> GetIncludedFeaturesByPackageIdAsync(int packageId);
    Task<bool> DeleteByPackageIdAsync(int packageId);
}
