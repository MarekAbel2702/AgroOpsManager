using AgroOpsManager.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroOpsManager.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<Field> Fields => Set<Field>();
        public DbSet<Machine> Machines => Set<Machine>();
        public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
        public DbSet<FieldWork> FieldWorks => Set<FieldWork>();
        public DbSet<WorkResourceUsage> WorkResourceUsages => Set<WorkResourceUsage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureField(modelBuilder);
            ConfigureMachine(modelBuilder);
            ConfigureInventoryItem(modelBuilder);
            ConfigureFieldWork(modelBuilder);
            ConfigureWorkResourceUsage(modelBuilder);
        }

        private static void ConfigureField(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Field>(entity =>
            {
                entity.ToTable("Fields");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(120);

                entity.Property(x => x.Location)
                .IsRequired()
                .HasMaxLength(200);

                entity.Property(x => x.AreaInHectares)
                .HasPrecision(10, 2);

                entity.Property(x => x.Notes)
                .HasMaxLength(1000);

                entity.HasQueryFilter(x => !x.IsDeleted);
            });
        }

        private static void ConfigureMachine(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Machine>(entity =>
            {
                entity.ToTable("Machines");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(120);

                entity.Property(x => x.SerialNumber)
                .IsRequired()
                .HasMaxLength(80);

                entity.Property(x => x.Notes)
                .HasMaxLength(1000);

                entity.HasQueryFilter(x => !x.IsDeleted);
            });
        }

         private static void ConfigureInventoryItem(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InventoryItem>(entity =>
            {
                entity.ToTable("InventoryItems");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(120);

                entity.Property(x => x.Unit)
                .IsRequired()
                .HasMaxLength(30);

                entity.Property(x => x.Quantity)
                .HasPrecision(12, 2);

                entity.Property(x => x.MinimumQuantity)
                .HasPrecision(12, 2);

                entity.Property(x => x.UnitPrice)
                .HasPrecision(12, 2);

                entity.Property(x => x.SupplierName)
                .HasMaxLength(160);

                entity.Property(x => x.Notes)
                .HasMaxLength(1000);

                entity.HasQueryFilter(x => !x.IsDeleted);
            });
        }

        private static void ConfigureFieldWork(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FieldWork>(entity =>
            {
                entity.ToTable("FieldWorks");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.LaborCost)
                .HasPrecision(12, 2);

                entity.Property(x => x.OperatorName)
                .HasMaxLength(120);

                entity.Property(x => x.Notes)
                .HasMaxLength(1000);

                entity.HasOne(x => x.Field)
                .WithMany(x => x.FieldWorks)
                .HasForeignKey(x => x.FieldId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Machine)
                .WithMany(x => x.FieldWorks)
                .HasForeignKey(x => x.MachineId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(x => !x.IsDeleted);
            });
        }

        private static void ConfigureWorkResourceUsage(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkResourceUsage>(entity =>
            {
                entity.ToTable("WorkResourceUsages");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.QuantityUsed)
                .HasPrecision(12, 2);

                entity.Property(x => x.UnitPriceAtUsage)
                .HasPrecision(12, 2);

                entity.HasOne(x => x.FieldWork)
                .WithMany(x => x.ResourceUsages)
                .HasForeignKey(x => x.FieldWorkId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.InventoryItem)
                .WithMany(x => x.ResourceUsages)
                .HasForeignKey(x => x.InventoryItemId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(x => !x.IsDeleted);
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();

            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            UpdateAuditFields();

            return base.SaveChanges();
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker
                .Entries<BaseEntity>()
                .Where(x => x.State is EntityState.Added or EntityState.Modified);

            var now = DateTime.Now;

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAtUtc = now;
                }
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAtUtc = now;
                }
            }
        }
    }
}
