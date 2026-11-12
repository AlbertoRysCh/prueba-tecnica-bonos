
using Microsoft.EntityFrameworkCore;
using BonusTest.Application.Bonuses;
using BonusTest.Domain.Entities;
using BonusTest.Infrastructure.Persistence;

namespace BonusTest.Infrastructure.Repositories;

public class BonusRepository(AppDbContext context) : IBonusRepository
{
    // Consultar empleado por departamento y número
    public async Task<Employee?> GetEmployeeAsync(
        string departmentCode,
        int employeeNumber,
        CancellationToken cancellationToken)
    {
        return await context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.DepartmentCode == departmentCode
                  && x.EmployeeNumber == employeeNumber,
                cancellationToken);
    }

    // Consultar motivo del bono
    public async Task<BonusReason?> GetBonusReasonAsync(
        string reasonCode,
        CancellationToken cancellationToken)
    {
        return await context.BonusReasons
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.ReasonCode == reasonCode,
                cancellationToken);
    }

    // Verificar duplicados activos
    public async Task<bool> ExistsActiveBonusAsync(
        string departmentCode,
        int employeeNumber,
        string period,
        CancellationToken cancellationToken)
    {
        return await context.Bonuses
            .AsNoTracking()
            .AnyAsync(
                x => x.DepartmentCode == departmentCode
                  && x.EmployeeNumber == employeeNumber
                  && x.Period == period
                  && !x.IsDeleted
                  && x.StateCode != "0203",
                cancellationToken);
    }

    // Agregar nuevo bono
    public async Task AddAsync(
        Bonus bonus,
        CancellationToken cancellationToken)
    {
        await context.Bonuses.AddAsync(
            bonus,
            cancellationToken);
    }

    // Buscar bono por ID
    public async Task<Bonus?> GetByIdAsync(
        int bonusId,
        CancellationToken cancellationToken)
    {
        return await context.Bonuses
            .FirstOrDefaultAsync(
                x => x.BonusId == bonusId && !x.IsDeleted,
                cancellationToken);
    }

    // Guardar cambios en SQL Server
    public async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
