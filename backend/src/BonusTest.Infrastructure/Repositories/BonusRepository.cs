
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


    // Consultar bonos con filtros y paginación
    public async Task<PagedResult<BonusDto>> GetBonusesAsync(
        BonusFilterRequest filter,
        CancellationToken cancellationToken)
    {
        // Consultar solamente bonos no eliminados
        var query = context.Bonuses
            .AsNoTracking()
            .AsQueryable()
            .Where(x => !x.IsDeleted);

        // Filtrar por departamento
        if (!string.IsNullOrWhiteSpace(filter.DepartmentCode))
        {
            query = query.Where(
                x => x.DepartmentCode == filter.DepartmentCode);
        }

        // Filtrar por empleado
        if (filter.EmployeeNumber.HasValue)
        {
            query = query.Where(
                x => x.EmployeeNumber == filter.EmployeeNumber.Value);
        }

        // Filtrar por período
        if (!string.IsNullOrWhiteSpace(filter.Period))
        {
            query = query.Where(
                x => x.Period == filter.Period);
        }

        // Filtrar por estado
        if (!string.IsNullOrWhiteSpace(filter.StateCode))
        {
            query = query.Where(
                x => x.StateCode == filter.StateCode);
        }

        // Contar los registros antes de paginar
        var totalCount = await query.CountAsync(cancellationToken);

        // Normalizar la paginación
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);

        // Obtener la página solicitada
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.BonusId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new BonusDto(
                x.BonusId,
                x.DepartmentCode,
                x.EmployeeNumber,
                x.Employee.FullName,
                x.Period,
                x.BonusTypeId,
                x.BonusType.BonusTypeDescription,
                x.BonusValue,
                x.ReasonCode,
                x.BonusReason.ReasonDescription,
                x.StateCode,
                x.GrossAmount,
                x.TaxAmount,
                x.NetAmount,
                x.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return new PagedResult<BonusDto>(
            items,
            totalCount,
            page,
            pageSize
        );
    }

    // Consultar bonos de un empleado
    public async Task<IReadOnlyList<BonusDto>> GetBonusesByEmployeeAsync(
        string departmentCode,
        int employeeNumber,
        CancellationToken cancellationToken)
    {
        return await context.Bonuses
            .AsNoTracking()
            .Where(x =>
                x.DepartmentCode == departmentCode &&
                x.EmployeeNumber == employeeNumber &&
                !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.BonusId)
            .Select(x => new BonusDto(
                x.BonusId,
                x.DepartmentCode,
                x.EmployeeNumber,
                x.Employee.FullName,
                x.Period,
                x.BonusTypeId,
                x.BonusType.BonusTypeDescription,
                x.BonusValue,
                x.ReasonCode,
                x.BonusReason.ReasonDescription,
                x.StateCode,
                x.GrossAmount,
                x.TaxAmount,
                x.NetAmount,
                x.CreatedAt
            ))
            .ToListAsync(cancellationToken);
    }


}
