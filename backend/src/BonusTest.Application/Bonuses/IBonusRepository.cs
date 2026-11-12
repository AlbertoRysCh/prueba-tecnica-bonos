
using BonusTest.Domain.Entities;

namespace BonusTest.Application.Bonuses;

public interface IBonusRepository
{
    // Buscar un empleado por su clave compuesta
    Task<Employee?> GetEmployeeAsync(
        string departmentCode,
        int employeeNumber,
        CancellationToken cancellationToken);

    // Buscar un motivo de bono
    Task<BonusReason?> GetBonusReasonAsync(
        string reasonCode,
        CancellationToken cancellationToken);

    // Verificar si ya existe un bono activo en el período
    Task<bool> ExistsActiveBonusAsync(
        string departmentCode,
        int employeeNumber,
        string period,
        CancellationToken cancellationToken);

    // Registrar un nuevo bono
    Task AddAsync(
        Bonus bonus,
        CancellationToken cancellationToken);

    // Buscar un bono por su identificador
    Task<Bonus?> GetByIdAsync(
        int bonusId,
        CancellationToken cancellationToken);

    // Guardar cambios, anulaciones o eliminación lógica
    Task SaveChangesAsync(
        CancellationToken cancellationToken);

    // Consultar bonos con filtros y paginación
    Task<PagedResult<BonusDto>> GetBonusesAsync(
        BonusFilterRequest filter,
        CancellationToken cancellationToken);


    // Consultar bonos de un empleado
    Task<IReadOnlyList<BonusDto>> GetBonusesByEmployeeAsync(
        string departmentCode,
        int employeeNumber,
        CancellationToken cancellationToken);


}
