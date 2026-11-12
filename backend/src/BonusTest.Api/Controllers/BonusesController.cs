
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
}
