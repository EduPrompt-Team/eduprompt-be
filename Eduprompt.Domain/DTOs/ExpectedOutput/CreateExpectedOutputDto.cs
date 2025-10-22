using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.ExpectedOutput;

public class CreateExpectedOutputDto
{
    [Required]
    public int PromptInstanceId { get; set; }

    [Required]
    [StringLength(100)]
    public string OutputName { get; set; } = string.Empty;

    public string? Status { get; set; } = "Active";

    public List<CreateOutputDetailDto>? OutputDetails { get; set; }
}

public class CreateOutputDetailDto
{
    public string? Description { get; set; }
    public int? OutputSize { get; set; }
}


