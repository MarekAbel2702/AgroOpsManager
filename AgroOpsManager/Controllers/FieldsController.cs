using AgroOpsManager.Core.Entities;
using AgroOpsManager.Infrastructure.Data;
using AgroOpsManager.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgroOpsManager.Web.Controllers
{
    public class FieldsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FieldsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchTerm, string? status)
        {
            var query = _context.Fields.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x =>
                x.Name.Contains(searchTerm) ||
                x.Location.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<Core.Enums.FieldStatus>(status, out var parsedStatus))
            {
                query = query.Where(x => x.Status == parsedStatus);
            }

            var fields = await query
                .OrderBy(x => x.Name)
                .ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.Status = status;
            
            return View(fields);
        }

        public async Task<IActionResult> Details(int id)
        {
            var field = await _context.Fields
                .Include(x => x.FieldWorks)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (field is null)
            {
                return NotFound();
            }

            return View(field);
        }

        public IActionResult Create()
        {
            var viewModel = new FieldFormViewModel
            {
                Status = Core.Enums.FieldStatus.Active
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FieldFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var field = new Field
            {
                Name = viewModel.Name,
                Location = viewModel.Location,
                AreaInHectares = viewModel.AreaInHectares,
                SoilType = viewModel.SoilType,
                CurrentCrop = viewModel.CurrentCrop,
                Status = viewModel.Status,
                Notes = viewModel.Notes
            };

            _context.Fields.Add(field);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Pole zostało dodane.";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var field = await _context.Fields.FirstOrDefaultAsync(x => x.Id == id);

            if (field is null)
            {
                return NotFound();
            }

            var viewModel = new FieldFormViewModel
            {
                Id = field.Id,
                Name = field.Name,
                Location = field.Location,
                AreaInHectares = field.AreaInHectares,
                SoilType = field.SoilType,
                CurrentCrop = field.CurrentCrop,
                Status = field.Status,
                Notes = field.Notes
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FieldFormViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var field = await _context.Fields.FirstOrDefaultAsync(x => x.Id == id);

            if (field is null)
            {
                return NotFound();
            }

            field.Name = viewModel.Name;
            field.Location = viewModel.Location;
            field.AreaInHectares = viewModel.AreaInHectares;
            field.SoilType = viewModel.SoilType;
            field.CurrentCrop = viewModel.CurrentCrop;
            field.Status = viewModel.Status;
            field.Notes = viewModel.Notes;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Pole zostało zaktualizowane.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var field = await _context.Fields.FirstOrDefaultAsync(x => x.Id == id);

            if (field is null)
            {
                return NotFound();
            }

            return View(field);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var field = await _context.Fields.FirstOrDefaultAsync(x => x.Id == id);

            if (field is null)
            {
                return NotFound();
            }

            field.IsDeleted = true;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Pole zostało usunięte.";

            return RedirectToAction(nameof(Index));
        }
    }
}
