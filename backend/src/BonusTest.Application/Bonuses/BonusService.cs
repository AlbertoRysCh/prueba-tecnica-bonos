
using System.Globalization;
using BonusTest.Domain.Entities;
using BonusTest.Domain.Exceptions;

namespace BonusTest.Application.Bonuses;

public class BonusService(IBonusRepository repository)
{
    public async Task<Bonus> CreateAsync(
        CreateBonusRequest request,
        CancellationToken cancellationToken)
    {
        // 1. Validar período YYYY-MM
        if (!DateTime.TryParseExact(
                request.Period,
                "yyyy-MM",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var periodDate))
        {
            throw new BusinessException(
                "INVALID_PERIOD",
                "El período debe tener el formato YYYY-MM.");
        }

        if (periodDate > new DateTime(
                DateTime.Today.Year,
                DateTime.Today.Month,
                1))
        {
            throw new BusinessException(
                "FUTURE_PERIOD",
                "No se permiten períodos futuros.");
        }

        // 2. Consultar y validar empleado
        var employee = await repository.GetEmployeeAsync(
            request.DepartmentCode,
            request.EmployeeNumber,
            cancellationToken);

        if (employee is null)
        {
            throw new BusinessException(
                "EMPLOYEE_NOT_FOUND",
                "El empleado no existe.");
        }

        if (employee.EmployeeStateCode.Trim() != "0101")
        {
            throw new BusinessException(
                "EMPLOYEE_INACTIVE",
                "El empleado no está activo.");
        }

        // 3. Validar motivo
        var reason = await repository.GetBonusReasonAsync(
            request.ReasonCode,
            cancellationToken);

        if (reason is null || !reason.State)
        {
            throw new BusinessException(
                "BONUS_REASON_INVALID",
                "El motivo no existe o está inactivo.");
        }

        // 4. Validar duplicados
        var exists = await repository.ExistsActiveBonusAsync(
            request.DepartmentCode,
            request.EmployeeNumber,
            request.Period,
            cancellationToken);

        if (exists)
        {
            throw new BusinessException(
                "DUPLICATE_BONUS",
                "El empleado ya tiene un bono activo en ese período.");
        }

        // 5. Validar el valor y calcular el bruto
        if (request.BonusValue <= 0)
        {
            throw new BusinessException(
                "INVALID_BONUS_VALUE",
                "El valor del bono debe ser mayor que cero.");
        }

        decimal grossAmount;

        if (request.BonusTypeId == 1)
        {
            // Monto fijo
            if (request.BonusValue > employee.MonthlySalary)
            {
                throw new BusinessException(
                    "BONUS_EXCEEDS_SALARY",
                    "El bono fijo no puede superar el sueldo mensual.");
            }

            grossAmount = request.BonusValue;
        }
        else if (request.BonusTypeId == 2)
        {
            // Porcentaje del sueldo
            if (request.BonusValue > 100)
            {
                throw new BusinessException(
                    "INVALID_PERCENTAGE",
                    "El porcentaje no puede superar el 100%.");
            }

            grossAmount =
                employee.MonthlySalary * request.BonusValue / 100m;
        }
        else
        {
            throw new BusinessException(
                "INVALID_BONUS_TYPE",
                "El tipo de bono no es válido.");
        }

        // 6. Calcular importes con redondeo a 2 decimales
        grossAmount = Math.Round(
            grossAmount, 2, MidpointRounding.AwayFromZero);

        decimal taxAmount = Math.Round(
            grossAmount * employee.TaxRate / 100m,
            2,
            MidpointRounding.AwayFromZero);

        decimal netAmount = Math.Round(
            grossAmount - taxAmount,
            2,
            MidpointRounding.AwayFromZero);

        // 7. Crear bono en estado Registrado
        var bonus = new Bonus
        {
            DepartmentCode = request.DepartmentCode,
            EmployeeNumber = request.EmployeeNumber,
            Period = request.Period,
            BonusTypeId = request.BonusTypeId,
            BonusValue = request.BonusValue,
            ReasonCode = request.ReasonCode,
            StateCode = "0201",
            GrossAmount = grossAmount,
            TaxAmount = taxAmount,
            NetAmount = netAmount,
            IsDeleted = false,
            CreatedBy = "system",
            CreatedAt = DateTime.UtcNow
        };

        // 8. Guardar en la base de datos
        await repository.AddAsync(bonus, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return bonus;
    }

    // Consultar bonos con filtros y paginación
    public async Task<PagedResult<BonusDto>> GetBonusesAsync(
        BonusFilterRequest filter,
        CancellationToken cancellationToken)
    {
        return await repository.GetBonusesAsync(
            filter,
            cancellationToken);
    }

    // Anular un bono registrado
    public async Task<Bonus> CancelAsync(
        int bonusId,
        CancelBonusRequest request,
        CancellationToken cancellationToken)
    {
        // 1. Buscar el bono
        var bonus = await repository.GetByIdAsync(
            bonusId,
            cancellationToken);

        if (bonus is null)
        {
            throw new BusinessException(
                "BONUS_NOT_FOUND",
                "El bono no existe o fue eliminado.");
        }

        // 2. Verificar que esté en estado Registrado
        if (bonus.StateCode.Trim() != "0201")
        {
            throw new BusinessException(
                "BONUS_CANNOT_BE_CANCELLED",
                "Solo se pueden anular bonos en estado Registrado.");
        }

        // 3. Validar el usuario que realiza la anulación
        if (string.IsNullOrWhiteSpace(request.ModifiedBy))
        {
            throw new BusinessException(
                "INVALID_MODIFIED_BY",
                "Debe indicar el usuario que realiza la anulación.");
        }

        // 4. Cambiar el estado a Anulado
        bonus.StateCode = "0203";

        // 5. Registrar los datos de auditoría
        bonus.ModifiedBy = request.ModifiedBy.Trim();
        bonus.ModifiedAt = DateTime.UtcNow;

        // 6. Guardar los cambios en SQL Server
        await repository.SaveChangesAsync(cancellationToken);

        return bonus;
    }


}
