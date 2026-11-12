
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
