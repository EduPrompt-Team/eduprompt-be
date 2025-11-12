using Eduprompt.Domain.DTOs.TemplateArchitecture;
using FluentValidation;

namespace Eduprompt.API.Validators;

public class CreateTemplateArchitectureValidator : AbstractValidator<CreateTemplateArchitectureDto>
{
    public CreateTemplateArchitectureValidator()
    {
        RuleFor(x => x.StorageId).GreaterThan(0).WithMessage("StorageId is required and must be greater than 0");
        RuleFor(x => x.ArchitectureName)
            .NotEmpty().WithMessage("Architecture name is required")
            .MaximumLength(100).WithMessage("Architecture name must not exceed 100 characters");
        
        RuleFor(x => x.ArchitectureType)
            .MaximumLength(50).WithMessage("Architecture type must not exceed 50 characters");
        
        When(x => x.Description != null, () => 
            RuleFor(x => x.Description!).MaximumLength(500).WithMessage("Description must not exceed 500 characters"));
        
        When(x => x.Status != null, () => 
            RuleFor(x => x.Status!).MaximumLength(50).WithMessage("Status must not exceed 50 characters"));
        
        // Validate Configuration JSON format
        When(x => !string.IsNullOrEmpty(x.Configuration), () =>
        {
            RuleFor(x => x.Configuration!)
                .Must(BeValidJson).WithMessage("Configuration must be valid JSON")
                .Must(HaveValidFieldDefinitions).WithMessage("Configuration must contain valid field definitions");
        });
    }

    private bool BeValidJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return true;
        try
        {
            System.Text.Json.JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool HaveValidFieldDefinitions(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return true;
        try
        {
            var doc = System.Text.Json.JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("fields", out var fields))
            {
                if (fields.ValueKind != System.Text.Json.JsonValueKind.Array) return false;
                foreach (var field in fields.EnumerateArray())
                {
                    if (!field.TryGetProperty("name", out _) || !field.TryGetProperty("type", out _))
                        return false;
                }
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
}


