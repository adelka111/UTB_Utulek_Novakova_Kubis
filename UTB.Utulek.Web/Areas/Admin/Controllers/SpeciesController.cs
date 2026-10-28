using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UTB.Utulek.Domain.Entities;
using UTB.Utulek.Application.Abstraction;

namespace UTB.Utulek.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Manager")]
public class SpeciesController : Controller
{
    ISpeciesAppService _speciesAppService;
    public SpeciesController(ISpeciesAppService speciesAppService)
    {
        _speciesAppService = speciesAppService;
    }

    public IActionResult Index()
    {
        IList<Species> species = _speciesAppService.SelectAll();
        return View(species);
    }

    public IActionResult Details(int id)
    {
        Species? species = _speciesAppService.GetById(id);
        if (species == null)
        {
            return NotFound();
        }
        return View(species);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new Species());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Species species)
    {
        ValidateName(species);

        if (ModelState.IsValid)
        {
            _speciesAppService.Create(species);
            return RedirectToAction(nameof(Index));
        }

        return View(species);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        Species? species = _speciesAppService.GetById(id);
        if (species == null)
        {
            return NotFound();
        }
        return View(species);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Species species)
    {
        if (id != species.Id)
        {
            return NotFound();
        }

        ValidateName(species);

        if (ModelState.IsValid)
        {
            bool updated = _speciesAppService.Update(species);
            if (!updated)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        return View(species);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        // Druh se zvířaty nejde smazat (cizí klíč Animal.SpeciesId).
        if (_speciesAppService.HasAnimals(id))
        {
            TempData["Error"] = "Druh nelze smazat, protože k němu patří zvířata.";
            return RedirectToAction(nameof(Index));
        }

        bool deleted = _speciesAppService.Delete(id);
        if (deleted)
        {
            return RedirectToAction(nameof(Index));
        }
        return NotFound();
    }

    private void ValidateName(Species species)
    {
        if (!string.IsNullOrWhiteSpace(species.Name) && _speciesAppService.NameExists(species.Name, species.Id))
        {
            ModelState.AddModelError(nameof(Species.Name), "Druh s tímto názvem už existuje.");
        }
    }
}
