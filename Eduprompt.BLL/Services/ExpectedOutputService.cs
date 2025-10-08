using Eduprompt.Domain.DTOs.ExpectedOutput;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class ExpectedOutputService : IExpectedOutputService
{
    private readonly IExpectedOutputRepository _outputRepository;
    private readonly IOutputDetailRepository _detailRepository;
    private readonly IPromptInstanceRepository _instanceRepository;

    public ExpectedOutputService(
        IExpectedOutputRepository outputRepository,
        IOutputDetailRepository detailRepository,
        IPromptInstanceRepository instanceRepository)
    {
        _outputRepository = outputRepository;
        _detailRepository = detailRepository;
        _instanceRepository = instanceRepository;
    }

    public async Task<ExpectedOutputDto?> GetByIdAsync(int outputId)
    {
        var e = await _outputRepository.GetByIdAsync(outputId);
        return e == null ? null : Map(e);
    }

    public async Task<IEnumerable<ExpectedOutputDto>> GetByInstanceIdAsync(int instanceId)
    {
        var list = await _outputRepository.GetByInstanceIdAsync(instanceId);
        return list.Select(Map);
    }

    public async Task<ExpectedOutputDto> CreateAsync(CreateExpectedOutputDto createDto)
    {
        var instance = await _instanceRepository.GetByIdAsync(createDto.InstanceID);
        if (instance == null) throw new ArgumentException("Prompt instance not found");

        var e = new Eduprompt.Domain.Entities.ExpectedOutput
        {
            InstanceID = createDto.InstanceID,
            OutputName = createDto.OutputName,
            Status = createDto.Status,
            CreatedDate = DateTime.UtcNow
        };

        var created = await _outputRepository.CreateAsync(e);

        if (createDto.OutputDetails != null && createDto.OutputDetails.Any())
        {
            foreach (var d in createDto.OutputDetails)
            {
                await _detailRepository.CreateAsync(new Eduprompt.Domain.Entities.OutputDetail
                {
                    OutputId = created.OutputId,
                    Description = d.Description,
                    OutputSize = d.OutputSize,
                    CreatedDate = DateTime.UtcNow
                });
            }
        }

        var withDetails = await _outputRepository.GetByIdAsync(created.OutputId)!;
        return Map(withDetails!);
    }

    public async Task<ExpectedOutputDto> UpdateAsync(int outputId, CreateExpectedOutputDto updateDto)
    {
        var e = await _outputRepository.GetByIdAsync(outputId);
        if (e == null) throw new KeyNotFoundException("Expected output not found");

        if (updateDto.InstanceID != 0) e.InstanceID = updateDto.InstanceID;
        if (!string.IsNullOrEmpty(updateDto.OutputName)) e.OutputName = updateDto.OutputName;
        if (updateDto.Status != null) e.Status = updateDto.Status;
        e.UpdatedDate = DateTime.UtcNow;

        var updated = await _outputRepository.UpdateAsync(e);
        return Map(updated);
    }

    public async Task<bool> DeleteAsync(int outputId)
    {
        return await _outputRepository.DeleteAsync(outputId);
    }

    private static ExpectedOutputDto Map(Eduprompt.Domain.Entities.ExpectedOutput e)
    {
        return new ExpectedOutputDto
        {
            OutputId = e.OutputId,
            InstanceID = e.InstanceID,
            OutputName = e.OutputName,
            Status = e.Status,
            CreatedDate = e.CreatedDate,
            UpdatedDate = e.UpdatedDate,
            OutputDetails = e.OutputDetails?.Select(d => new OutputDetailDto
            {
                DetailId = d.DetailId,
                OutputId = d.OutputId,
                Description = d.Description,
                OutputSize = d.OutputSize,
                CreatedDate = d.CreatedDate,
                UpdatedDate = d.UpdatedDate
            }).ToList()
        };
    }
}


