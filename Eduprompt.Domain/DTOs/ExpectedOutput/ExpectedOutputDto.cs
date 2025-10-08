namespace Eduprompt.Domain.DTOs.ExpectedOutput;

public class ExpectedOutputDto
{
    public int OutputId { get; set; }
    public int InstanceID { get; set; }
    public string OutputName { get; set; } = string.Empty;
    public string? Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public List<OutputDetailDto>? OutputDetails { get; set; }
}

public class OutputDetailDto
{
    public int DetailId { get; set; }
    public int OutputId { get; set; }
    public string? Description { get; set; }
    public int? OutputSize { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}


