using Microsoft.EntityFrameworkCore;
using BonusTest.Application.Catalogs;
using BonusTest.Infrastructure.Persistence;

namespace BonusTest.Infrastructure.Repositories;

public class CatalogRepository(AppDbContext context) : ICatalogRepository
{
    public async Task<IReadOnlyList<DepartmentDto>> GetDepartmentsAsync(CancellationToken cancellationToken) =>
        await context.Departments.AsNoTracking()
            .OrderBy(x => x.DepartmentName)
            .Select(x => new DepartmentDto(x.DepartmentCode, x.DepartmentName))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<StateDto>> GetStatesAsync(string masterStateCode, CancellationToken cancellationToken) =>
        await context.States.AsNoTracking()
            .Where(x => x.MasterStateCode == masterStateCode)
            .OrderBy(x => x.StateCode)
            .Select(x => new StateDto(x.StateCode, x.StateDescription))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<BonusTypeDto>> GetBonusTypesAsync(CancellationToken cancellationToken) =>
        await context.BonusTypes.AsNoTracking()
            .OrderBy(x => x.BonusTypeId)
            .Select(x => new BonusTypeDto(x.BonusTypeId, x.BonusTypeDescription))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<BonusReasonDto>> GetActiveBonusReasonsAsync(CancellationToken cancellationToken) =>
        await context.BonusReasons.AsNoTracking()
            .Where(x => x.State)
            .OrderBy(x => x.ReasonDescription)
            .Select(x => new BonusReasonDto(x.ReasonCode, x.ReasonDescription))
            .ToListAsync(cancellationToken);


    public async Task<IReadOnlyList<EmployeeDto>> GetActiveEmployeesAsync(
        string departmentCode,
        CancellationToken cancellationToken) =>
        await context.Employees.AsNoTracking()
            .Where(x =>
                x.DepartmentCode == departmentCode &&
                x.EmployeeStateCode == "0101")
            .OrderBy(x => x.FullName)
            .Select(x => new EmployeeDto(
                x.DepartmentCode,
                x.EmployeeNumber,
                x.FullName,
                x.HireDate,
                x.MonthlySalary,
                x.TaxRate,
                x.EmployeeStateCode
            ))
            .ToListAsync(cancellationToken);

}
