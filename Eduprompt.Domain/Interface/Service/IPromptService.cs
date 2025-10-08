namespace Eduprompt.Domain.Interface.Service;

public interface IPromptService
{
    Task<PromptServiceDto?> GetByIdAsync(int id);
    Task<IEnumerable<PromptServiceDto>> GetAllAsync();
    Task<IEnumerable<PromptServiceDto>> GetByTemplateIdAsync(int templateId);
    Task<PromptServiceDto> CreateAsync(PromptCreateServiceDto promptDto);
    Task<PromptServiceDto> UpdateAsync(int id, PromptUpdateServiceDto promptDto);
    Task<bool> DeleteAsync(int id);
    
    // Prompt Detail operations
    Task<PromptDetailServiceDto> AddPromptDetailAsync(int promptId, PromptDetailCreateServiceDto detailDto);
    Task<bool> DeletePromptDetailAsync(int detailId);
    
    // Expected Output operations
    Task<ExpectedOutputServiceDto> AddExpectedOutputAsync(int promptId, ExpectedOutputCreateServiceDto outputDto);
    Task<ExpectedOutputServiceDto> UpdateExpectedOutputAsync(int outputId, ExpectedOutputUpdateServiceDto outputDto);
    Task<bool> DeleteExpectedOutputAsync(int outputId);
}

// Service DTOs
public class PromptServiceDto
{
    public int PromptId { get; set; }
    public int TemplateId { get; set; }
    public string PromptTitle { get; set; } = string.Empty;
    public string PromptText { get; set; } = string.Empty;
    public int? UsageCount { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? Status { get; set; }
    public string? TemplateName { get; set; }
    public List<PromptDetailServiceDto>? PromptDetails { get; set; }
    public List<ExpectedOutputServiceDto>? ExpectedOutputs { get; set; }
}

public class PromptCreateServiceDto
{
    public int TemplateId { get; set; }
    public string PromptTitle { get; set; } = string.Empty;
    public string PromptText { get; set; } = string.Empty;
    public string? Status { get; set; }
    public List<PromptDetailCreateServiceDto>? PromptDetails { get; set; }
}

public class PromptUpdateServiceDto
{
    public string? PromptTitle { get; set; }
    public string? PromptText { get; set; }
    public string? Status { get; set; }
}

public class PromptDetailServiceDto
{
    public int DetailId { get; set; }
    public int PromptId { get; set; }
    public string? DetailContent { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? Status { get; set; }
}

public class PromptDetailCreateServiceDto
{
    public string? DetailContent { get; set; }
    public string? Status { get; set; }
}

public class ExpectedOutputServiceDto
{
    public int OutputId { get; set; }
    public int? PromptId { get; set; }
    public string? OutputName { get; set; }
    public string? Status { get; set; }
    public List<OutputDetailServiceDto>? OutputDetails { get; set; }
}

public class ExpectedOutputCreateServiceDto
{
    public string? OutputName { get; set; }
    public string? Status { get; set; }
    public List<OutputDetailCreateServiceDto>? OutputDetails { get; set; }
}

public class ExpectedOutputUpdateServiceDto
{
    public string? OutputName { get; set; }
    public string? Status { get; set; }
}

public class OutputDetailServiceDto
{
    public int DetailId { get; set; }
    public int OutputId { get; set; }
    public string? Description { get; set; }
    public int? OutputSize { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

public class OutputDetailCreateServiceDto
{
    public string? Description { get; set; }
    public int? OutputSize { get; set; }
} 