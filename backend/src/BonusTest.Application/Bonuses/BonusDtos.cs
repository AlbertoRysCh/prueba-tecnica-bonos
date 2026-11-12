
namespace BonusTest.Application.Bonuses;

// Datos que envía Angular para registrar un bono
public record CreateBonusRequest(
    string DepartmentCode,
    int EmployeeNumber,
    string Period,
    int BonusTypeId,
    decimal BonusValue,
    string ReasonCode
);

// Datos que devuelve la API
public record BonusDto(
    int BonusId,
    string DepartmentCode,
    int EmployeeNumber,
    string EmployeeName,
    string Period,
    int BonusTypeId,
    string BonusTypeDescription,
    decimal BonusValue,
    string ReasonCode,
    string ReasonDescription,
    string StateCode,
    decimal GrossAmount,
    decimal TaxAmount,
    decimal NetAmount,
    DateTime CreatedAt
);

// Datos para solicitar la anulación de un bono
public record CancelBonusRequest(
    string ModifiedBy
);


// Filtros para consultar bonos
public record BonusFilterRequest(
    string? DepartmentCode,
    int? EmployeeNumber,
    string? Period,
    string? StateCode,
    int Page = 1,
    int PageSize = 10
);

// Resultado paginado de la consulta
public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize
);
