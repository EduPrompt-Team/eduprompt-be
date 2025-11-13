using Eduprompt.Domain.DTOs.ExpectedOutput;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class ExpectedOutputService : IExpectedOutputService
{
    private readonly IExpectedOutputRepository _expectedOutputRepository;

    public ExpectedOutputService(IExpectedOutputRepository expectedOutputRepository)
    {
        _expectedOutputRepository = expectedOutputRepository;
    }

    public async Task<ExpectedOutputDto?> GetByIdAsync(int OutputId)
    {
        var output = await _expectedOutputRepository.GetByIdAsync(OutputId);
        return output != null ? MapToDto(output) : null;
    }

    public async Task<IEnumerable<ExpectedOutputDto>> GetByInstanceIdAsync(int InstanceId)
    {
        var outputs = await _expectedOutputRepository.GetByInstanceIdAsync(InstanceId);
        return outputs.Select(MapToDto);
    }

    public async Task<ExpectedOutputDto> CreateAsync(CreateExpectedOutputDto createDto)
    {
        var output = new ExpectedOutput
        {
            PromptInstanceId = createDto.PromptInstanceId,
            OutputName = createDto.OutputName,
            ExampleOutput = createDto.OutputDetails?.FirstOrDefault()?.Description // Store first detail as ExampleOutput
        };

        var createdOutput = await _expectedOutputRepository.CreateAsync(output);
        
        // Note: OutputDetails creation would need OutputDetailRepository
        // For now, we store the main content in ExampleOutput field
        
        return MapToDto(createdOutput);
    }

    public async Task<ExpectedOutputDto> UpdateAsync(int OutputId, CreateExpectedOutputDto updateDto)
    {
        var output = await _expectedOutputRepository.GetByIdAsync(OutputId);
        if (output == null) throw new KeyNotFoundException("Expected output not found");

        output.OutputName = updateDto.OutputName;

        var updatedOutput = await _expectedOutputRepository.UpdateAsync(output);
        return MapToDto(updatedOutput);
    }

    public async Task<bool> DeleteAsync(int OutputId)
    {
        return await _expectedOutputRepository.DeleteAsync(OutputId);
    }

    private static ExpectedOutputDto MapToDto(ExpectedOutput output)
    {
        return new ExpectedOutputDto
        {
            OutputId = output.OutputId,
            PromptInstanceId = output.PromptInstanceId,
            OutputName = output.OutputName,
            Status = "Active",
            UpdatedDate = DateTime.UtcNow,
            OutputDetails = output.OutputDetails?.Select(od => new OutputDetailDto
            {
                DetailId = od.DetailId,
                OutputId = od.OutputId,
                Description = od.DetailValue,
                OutputSize = null,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            }).ToList() ?? new List<OutputDetailDto>()
        };
    }
}