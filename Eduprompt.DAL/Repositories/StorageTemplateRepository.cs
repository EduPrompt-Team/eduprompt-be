
using Eduprompt.Domain.Entities;
using Eduprompt.DAL.DbContexts;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Eduprompt.DAL.Repositories;

public class StorageTemplateRepository : IStorageTemplateRepository
{
    private readonly EdupromptV2Context _context;

    public StorageTemplateRepository(EdupromptV2Context context)
    {
        _context = context;
    }

    public async Task<StorageTemplate?> GetByIdAsync(int id)
    {
        return await _context.StorageTemplates
            .Include(s => s.User)
            .Include(s => s.Package)
            .FirstOrDefaultAsync(s => s.StorageId == id);
    }

    public async Task<IEnumerable<StorageTemplate>> GetByUserIdAsync(int UserId)
    {
        return await _context.StorageTemplates
            .Include(s => s.User)
            .Include(s => s.Package)
            .Where(s => s.UserId == UserId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<StorageTemplate?> GetUserStorageItemAsync(int UserId, int templateId)
    {
        return await _context.StorageTemplates
            .Include(s => s.User)
            .Include(s => s.Package)
            .FirstOrDefaultAsync(s => s.UserId == UserId && s.PackageId == templateId);
    }

    public async Task<StorageTemplate> CreateAsync(StorageTemplate storage)
    {
        storage.CreatedAt = DateTime.Now;

        await _context.StorageTemplates.AddAsync(storage);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(storage.StorageId) ?? storage;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var storage = await _context.StorageTemplates.FindAsync(id);
        if (storage == null)
            return false;

        _context.StorageTemplates.Remove(storage);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int UserId, int templateId)
    {
        return await _context.StorageTemplates
            .AnyAsync(s => s.UserId == UserId && s.PackageId == templateId);
    }

    private static readonly Dictionary<string, string[]> SubjectSlugMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "chemistry", new[] { "hoa hoc", "hoa-hoc" } },
        { "physics", new[] { "vat ly", "vat-ly" } },
        { "math", new[] { "toan", "toan hoc", "toan-hoc" } },
        { "biology", new[] { "sinh hoc", "sinh-hoc" } },
        { "history", new[] { "lich su", "lich-su" } },
        { "geography", new[] { "dia ly", "dia-ly" } },
        { "english", new[] { "tieng anh", "tieng-anh" } },
        { "literature", new[] { "ngu van", "ngu-van", "van hoc", "van-hoc" } },
    };

    public async Task<IEnumerable<StorageTemplate>> GetPublicAsync(int? packageId, string? grade, string? subject, string? chapter)
    {
        var query = _context.StorageTemplates
            .Include(s => s.User)
            .Include(s => s.Package)
            .Where(s => s.IsPublic)
            .AsQueryable();

        if (packageId.HasValue)
        {
            query = query.Where(s => s.PackageId == packageId.Value);
        }

        var list = await query
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        if (!string.IsNullOrWhiteSpace(grade))
        {
            list = list
                .Where(s => MatchesFilter(grade, s.Grade))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(subject))
        {
            list = list
                .Where(s => MatchesFilter(subject, s.Subject))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(chapter))
        {
            list = list
                .Where(s => MatchesFilter(chapter, s.Chapter))
                .ToList();
        }

        return list;
    }

        public async Task<StorageTemplate?> UpdateAsync(StorageTemplate entity)
        {
            _context.ChangeTracker.Clear();
            _context.StorageTemplates.Update(entity);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(entity.StorageId);
        }

    public async Task<bool> SetPublishAsync(int id, bool isPublic)
    {
        var affected = await _context.Database.ExecuteSqlRawAsync(
            "UPDATE [StorageTemplates] SET [IsPublic] = {0} WHERE [StorageID] = {1}",
            isPublic, id);

        return affected > 0;
    }

    private static bool MatchesFilter(string filter, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var normalizedFilter = Normalize(filter);
        var normalizedValue = Normalize(value);

        if (value.Equals(filter, StringComparison.OrdinalIgnoreCase) ||
            normalizedValue.Equals(normalizedFilter, StringComparison.OrdinalIgnoreCase) ||
            normalizedValue.Contains(normalizedFilter) ||
            normalizedFilter.Contains(normalizedValue))
        {
            return true;
        }

        if (SubjectSlugMap.TryGetValue(normalizedFilter, out var synonyms))
        {
            foreach (var synonym in synonyms)
            {
                if (normalizedValue.Equals(Normalize(synonym), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static string Normalize(string input)
    {
        // Convert to lowercase, remove accents, replace spaces with hyphen to handle slug values
        var normalized = input
            .Trim()
            .ToLowerInvariant()
            .Normalize(NormalizationForm.FormD);

        var sb = new StringBuilder();
        foreach (var ch in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (unicodeCategory == UnicodeCategory.NonSpacingMark)
                continue;

            sb.Append(ch);
        }

        var stripped = sb.ToString()
            .Normalize(NormalizationForm.FormC);

        var noSeparators = new StringBuilder();
        foreach (var ch in stripped)
        {
            if (char.IsLetterOrDigit(ch))
            {
                noSeparators.Append(ch);
            }
        }

        return noSeparators.ToString();
    }
} 
