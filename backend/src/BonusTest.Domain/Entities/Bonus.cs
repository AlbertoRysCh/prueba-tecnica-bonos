
namespace BonusTest.Domain.Entities;

public class Bonus
{
    public int BonusId { get; set; }

    public string DepartmentCode { get; set; } = string.Empty;
    public int EmployeeNumber { get; set; }

    public string Period { get; set; } = string.Empty;
    public int BonusTypeId { get; set; }
    public decimal BonusValue { get; set; }

    public string ReasonCode { get; set; } = string.Empty;
    public string StateCode { get; set; } = "0201";

    public decimal GrossAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetAmount { get; set; }

    public bool IsDeleted { get; set; }

    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }

    
    public Employee Employee { get; set; } = null!;
    public BonusType BonusType { get; set; } = null!;
    public BonusReason BonusReason { get; set; } = null!;
    public State State { get; set; } = null!;
}
