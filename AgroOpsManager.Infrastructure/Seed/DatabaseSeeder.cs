using AgroOpsManager.Core.Entities;
using AgroOpsManager.Core.Enums;
using AgroOpsManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AgroOpsManager.Infrastructure.Seed
{
    public class DatabaseSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (context.Database.IsRelational())
            {
                await context.Database.MigrateAsync();
            }
            else
            {
                await context.Database.EnsureCreatedAsync();
            }

            if (await context.Fields.AnyAsync())
            {
                return;
            }

            var fields = new List<Field>
            {
                new Field
                {
                    Name = "Pole północne",
                    Location = "Sektor A",
                    AreaInHectares = 12.50m,
                    SoilType = SoilType.Loamy,
                    CurrentCrop = CropType.Wheat,
                    Status = FieldStatus.Active,
                    Notes = "Pole o dobrej jakości gleby, regularnie nawożone"
                },
                new Field
                {
                    Name = "Pole południowe",
                    Location = "Sektor B",
                    AreaInHectares = 8.75m,
                    SoilType = SoilType.Sandy,
                    CurrentCrop = CropType.Corn,
                    Status = FieldStatus.Active,
                    Notes = "Wymaga częstszego monitorowania wilgotności."
                },
                new Field
                {
                    Name = "Łąka zachodnia",
                    Location = "Sektor C",
                    AreaInHectares = 5.30m,
                    SoilType = SoilType.Peaty,
                    CurrentCrop = CropType.Grass,
                    Status = FieldStatus.Active,
                    Notes = "Użytkowana głównie pod koszenie."
                },
                new Field
                {
                    Name = "Pole testowe",
                    Location = "Sektor D",
                    AreaInHectares = 3.20m,
                    SoilType= SoilType.Clay,
                    CurrentCrop = CropType.Rapeseed,
                    Status = FieldStatus.Fallow,
                    Notes = "Pole przeznaczone pod testy upraw."
                }
            };

            var machines = new List<Machine>
            {
                new Machine
                {
                    Name = "John Deere 6155M",
                    Type = MachineType.Tractor,
                    SerialNumber = "JD-6155M-2020-001",
                    ProductionYear = 2020,
                    CurrentWorkingHours = 1050,
                    WorkingHoursAtLastService = 800,
                    ServiceIntervalHours = 250,
                    Status = MachineStatus.Available,
                    Notes = "Główny ciągnik do prac polowych"
                },
                new Machine
                {
                    Name = "Claas Lexion 670",
                    Type = MachineType.CombineHarvester,
                    SerialNumber = "CL-LX670-2018-002",
                    ProductionYear = 2018,
                    CurrentWorkingHours = 1890,
                    WorkingHoursAtLastService = 1750,
                    ServiceIntervalHours = 300,
                    Status = MachineStatus.Available,
                    Notes = "Kombajn używany głównie przy zbiorach zbóż."
                },
                new Machine
                {
                    Name = "Amazone UX 4200",
                    Type = MachineType.Sprayer,
                    SerialNumber = "AM-UX4200-2021-003",
                    ProductionYear = 2021,
                    CurrentWorkingHours = 430,
                    WorkingHoursAtLastService = 300,
                    ServiceIntervalHours = 200,
                    Status = MachineStatus.InService,
                    Notes = "Opryskiwacz aktualnie po przeglądzoe sezonowym."
                }
            };

            var inventoryItems = new List<InventoryItem>
            {
                new InventoryItem
                {
                    Name = "Saletra amonowa",
                    Category = InventoryCategory.Fertilizer,
                    Unit = "kg",
                    Quantity = 1200,
                    MinimumQuantity = 300,
                    UnitPrice = 2.10m,
                    SupplierName = "AgroChem Polska",
                    Notes = "Podstawowy nawóz azotowy"
                },
                new InventoryItem
                {
                    Name = "Olej napędowy",
                    Category = InventoryCategory.Fuel,
                    Unit = "l",
                    Quantity = 450,
                    MinimumQuantity = 500,
                    UnitPrice = 6.40m,
                    SupplierName = "Local Fuel Partner",
                    Notes = "Paliwo do maszyn rolniczych."
                },
                new InventoryItem
                {
                    Name = "Nasiona pszenicy ozimej",
                    Category= InventoryCategory.Seeds,
                    Unit = "kg",
                    Quantity = 900,
                    MinimumQuantity = 200,
                    UnitPrice = 3.20m,
                    SupplierName = "SeedFarm",
                    Notes = "Materiał siewny na sezon"
                },
                new InventoryItem
                {
                    Name = "Gerbicyd H-PLus",
                    Category = InventoryCategory.PlantProtection,
                    Unit = "l",
                    Quantity = 75,
                    MinimumQuantity = 50,
                    UnitPrice = 48.00m,
                    SupplierName = "CropProtect",
                    Notes = "Środek ochrony roślin."
                }
            };

            await context.Fields.AddRangeAsync(fields);
            await context.Machines.AddRangeAsync(machines);
            await context.InventoryItems.AddRangeAsync(inventoryItems);
            await context.SaveChangesAsync();

            var fieldWork = new FieldWork
            {
                FieldId = fields[0].Id,
                MachineId = machines[0].Id,
                Type = FieldWorkType.Fertilizing,
                PlannedDate = DateTime.UtcNow.AddDays(3),
                Status = FieldWorkStatus.Planned,
                LaborCost = 350,
                OperatorName = "Adam Nowak",
                Notes = "Planowane nawożenie pola północnego."
            };

            await context.FieldWorks.AddAsync(fieldWork);
            await context.SaveChangesAsync();

            var resourceUsage = new WorkResourceUsage
            {
                FieldWorkId = fieldWork.Id,
                InventoryItemId = inventoryItems[0].Id,
                QuantityUsed = 400,
                UnitPriceAtUsage = inventoryItems[0].UnitPrice
            };

            await context.WorkResourceUsages.AddAsync(resourceUsage);
            await context.SaveChangesAsync();
        }
    }
}
