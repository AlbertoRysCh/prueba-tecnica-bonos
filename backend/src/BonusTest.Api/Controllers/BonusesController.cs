
using Microsoft.AspNetCore.Mvc;
using BonusTest.Application.Bonuses;
using BonusTest.Domain.Exceptions;

namespace BonusTest.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class BonusesController(BonusService bonusService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateBonusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var bonus = await bonusService.CreateAsync(
                request,
                cancellationToken);

            return StatusCode(StatusCodes.Status201Created, new
            {
                bonus.BonusId,
                bonus.DepartmentCode,
                bonus.EmployeeNumber,
                bonus.Period,
                bonus.BonusTypeId,
                bonus.BonusValue,
                bonus.ReasonCode,
                bonus.StateCode,
                bonus.GrossAmount,
                bonus.TaxAmount,
                bonus.NetAmount
            });
        }
        catch (BusinessException ex)
        {
            return Conflict(new
            {
                code = ex.Code,
                description = ex.Description
            });
        }
    }

    // Consultar bonos con filtros y paginación
    [HttpGet]
    public async Task<IActionResult> GetBonuses(
        [FromQuery] string? departmentCode,
        [FromQuery] int? employeeNumber,
        [FromQuery] string? period,
        [FromQuery] string? stateCode,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var filter = new BonusFilterRequest(
            departmentCode,
            employeeNumber,
            period,
            stateCode,
            page,
            pageSize
        );

        var result = await bonusService.GetBonusesAsync(
            filter,
            cancellationToken);

        return Ok(result);
    }

    // Anular un bono registrado
    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> CancelBonus(
        int id,
        [FromBody] CancelBonusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var bonus = await bonusService.CancelAsync(
                id,
                request,
                cancellationToken);

            return Ok(new
            {
                bonus.BonusId,
                bonus.StateCode,
                bonus.ModifiedBy,
                bonus.ModifiedAt
            });
        }
        catch (BusinessException ex)
        {
            return Conflict(new
            {
                code = ex.Code,
                description = ex.Description
            });
        }
    }


}
