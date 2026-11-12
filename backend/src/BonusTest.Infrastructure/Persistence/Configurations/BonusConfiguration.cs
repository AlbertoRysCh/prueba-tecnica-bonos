
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BonusTest.Domain.Entities;

namespace BonusTest.Infrastructure.Persistence.Configurations;

public class BonusConfiguration : IEntityTypeConfiguration<Bonus>
{
    public void Configure(EntityTypeBuilder<Bonus> builder)
    {
        builder.ToTable("Bonus", "dbo");

        // Clave primaria
        builder.HasKey(x => x.BonusId);

        builder.Property(x => x.BonusId)
            .ValueGeneratedOnAdd();

        // Datos del bono
        builder.Property(x => x.DepartmentCode)
            .HasColumnType("char(5)")
            .IsRequired();

        builder.Property(x => x.Period)
            .HasColumnType("char(7)")
            .IsRequired();

        builder.Property(x => x.BonusValue)
            .HasPrecision(16, 2);

        builder.Property(x => x.ReasonCode)
            .HasColumnType("char(6)")
            .IsRequired();

        builder.Property(x => x.StateCode)
            .HasColumnType("char(4)")
            .HasDefaultValue("0201")
            .IsRequired();

        // Importes calculados
        builder.Property(x => x.GrossAmount)
            .HasPrecision(16, 2);

        builder.Property(x => x.TaxAmount)
            .HasPrecision(16, 2);

        builder.Property(x => x.NetAmount)
            .HasPrecision(16, 2);

        // Auditoría
        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(100)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.Property(x => x.ModifiedBy)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(x => x.ModifiedAt)
            .HasColumnType("datetime2");

        // Relación con Employee: clave compuesta
        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => new
            {
                x.DepartmentCode,
                x.EmployeeNumber
            })
            .OnDelete(DeleteBehavior.NoAction);

        // Relación con BonusType
        builder.HasOne(x => x.BonusType)
            .WithMany()
            .HasForeignKey(x => x.BonusTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        // Relación con BonusReason
        builder.HasOne(x => x.BonusReason)
            .WithMany()
            .HasForeignKey(x => x.ReasonCode)
            .OnDelete(DeleteBehavior.NoAction);

        // Relación con State
        builder.HasOne(x => x.State)
            .WithMany()
            .HasForeignKey(x => x.StateCode)
            .OnDelete(DeleteBehavior.NoAction);

        // Evitar duplicados activos por empleado y periodo
        builder.HasIndex(x => new
        {
            x.DepartmentCode,
            x.EmployeeNumber,
            x.Period
        })
        .IsUnique()
        .HasDatabaseName("UX_Bonus_ActivePeriod")
        .HasFilter("[IsDeleted] = 0 AND [StateCode] <> '0203'");

        // Índice para consultas de listado
        builder.HasIndex(x => new
        {
            x.Period,
            x.StateCode
        })
        .HasDatabaseName("IX_Bonus_List");
    }
}
