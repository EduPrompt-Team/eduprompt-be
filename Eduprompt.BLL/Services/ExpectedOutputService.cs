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

    public async Task<ExpectedOutputDto?> GetByIdAsync(int outputId)
    {
        var output = await _expectedOutputRepository.GetByIdAsync(outputId);
        return output != null ? MapToDto(output) : null;
    }

    public async Task<IEnumerable<ExpectedOutputDto>> GetByInstanceIdAsync(int instanceId)
    {
        var outputs = await _expectedOutputRepository.GetByInstanceIdAsync(instanceId);
        return outputs.Select(MapToDto);
    }

    public async Task<ExpectedOutputDto> CreateAsync(CreateExpectedOutputDto createDto)
    {
        var output = new ExpectedOutput
        {
            PromptInstanceID = createDto.InstanceID,
            OutputName = createDto.OutputName
        };

        var createdOutput = await _expectedOutputRepository.CreateAsync(output);
        return MapToDto(createdOutput);
    }

    public async Task<ExpectedOutputDto> UpdateAsync(int outputId, CreateExpectedOutputDto updateDto)
    {
        var output = await _expectedOutputRepository.GetByIdAsync(outputId);
        if (output == null) throw new KeyNotFoundException("Expected output not found");

        output.OutputName = updateDto.OutputName;

        var updatedOutput = await _expectedOutputRepository.UpdateAsync(output);
        return MapToDto(updatedOutput);
    }

    public async Task<bool> DeleteAsync(int outputId)
    {
        return await _expectedOutputRepository.DeleteAsync(outputId);
    }

    private static ExpectedOutputDto MapToDto(ExpectedOutput output)
    {
        return new ExpectedOutputDto
        {
            OutputId = output.OutputID,
            InstanceID = output.PromptInstanceID,
            OutputName = output.OutputName,
            Status = "Active",
            UpdatedDate = DateTime.UtcNow,
            OutputDetails = output.OutputDetails?.Select(od => new OutputDetailDto
            {
                DetailId = od.DetailID,
                OutputId = od.OutputID,
                Description = od.DetailValue,
                OutputSize = null,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            }).ToList() ?? new List<OutputDetailDto>()
        };
    }
}