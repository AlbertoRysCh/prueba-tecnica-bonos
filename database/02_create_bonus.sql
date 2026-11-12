
USE BonusTest;
GO

CREATE TABLE dbo.Bonus (
    BonusId INT IDENTITY(1,1) NOT NULL,
    DepartmentCode CHAR(5) NOT NULL,
    EmployeeNumber INT NOT NULL,
    Period CHAR(7) NOT NULL,
    BonusTypeId INT NOT NULL,
    BonusValue DECIMAL(16,2) NOT NULL,
    ReasonCode CHAR(6) NOT NULL,
    StateCode CHAR(4) NOT NULL
        CONSTRAINT DF_Bonus_State DEFAULT '0201',

    GrossAmount DECIMAL(16,2) NOT NULL,
    TaxAmount DECIMAL(16,2) NOT NULL,
    NetAmount DECIMAL(16,2) NOT NULL,

    IsDeleted BIT NOT NULL
        CONSTRAINT DF_Bonus_IsDeleted DEFAULT 0,

    CreatedBy VARCHAR(100) NOT NULL,
    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Bonus_CreatedAt DEFAULT SYSUTCDATETIME(),

    ModifiedBy VARCHAR(100) NULL,
    ModifiedAt DATETIME2 NULL,

    CONSTRAINT PK_Bonus PRIMARY KEY (BonusId),

    CONSTRAINT FK_Bonus_Employee
        FOREIGN KEY (DepartmentCode, EmployeeNumber)
        REFERENCES dbo.Employee(DepartmentCode, EmployeeNumber),

    CONSTRAINT FK_Bonus_BonusType
        FOREIGN KEY (BonusTypeId)
        REFERENCES dbo.BonusType(BonusTypeId),

    CONSTRAINT FK_Bonus_BonusReason
        FOREIGN KEY (ReasonCode)
        REFERENCES dbo.BonusReason(ReasonCode),

    CONSTRAINT FK_Bonus_State
        FOREIGN KEY (StateCode)
        REFERENCES dbo.State(StateCode),

    CONSTRAINT CK_Bonus_Period
        CHECK (
             Period LIKE '[12][0-9][0-9][0-9]-[0-1][0-9]'
        AND SUBSTRING(Period, 6, 2) BETWEEN '01' AND '12'
        ),

    CONSTRAINT CK_Bonus_Value
        CHECK (BonusValue > 0),

    CONSTRAINT CK_Bonus_Amounts
        CHECK (
            GrossAmount >= 0 AND
            TaxAmount >= 0 AND
            NetAmount >= 0
        )
);
GO

CREATE UNIQUE INDEX UX_Bonus_ActivePeriod
ON dbo.Bonus (DepartmentCode, EmployeeNumber, Period)
WHERE IsDeleted = 0 AND StateCode <> '0203';
GO

CREATE INDEX IX_Bonus_List
ON dbo.Bonus (Period, StateCode, DepartmentCode, EmployeeNumber)
INCLUDE (IsDeleted);
GO
