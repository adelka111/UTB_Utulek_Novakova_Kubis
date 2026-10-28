using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UTB.Utulek.Domain.Entities;
using UTB.Utulek.Application.Abstraction;

namespace UTB.Utulek.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Manager")]
public class AnimalController : Controller
{
    // Hodnota volby "Jiné" v selectu druhů (žádný skutečný druh nemá záporné Id).
    private const int OtherSpeciesId = -1;

    IAnimalAppService _animalAppService;
    IImageStorage _imageStorage;
    public AnimalController(IAnimalAppService animalAppService, IImageStorage imageStorage)
    {
        _animalAppService = animalAppService;
        _imageStorage = imageStorage;
    }

    public IActionResult Index()
    {
        IList<Animal> animals = _animalAppService.SelectAll();
        return View(animals);
    }

    public IActionResult Details(int id)
    {
        Animal? animal = _animalAppService.GetById(id);
        if (animal == null)
        {
            return NotFound();
        }
        return View(animal);
    }

    [HttpGet]
    public IActionResult Create()
    {
        LoadSpecies();
        return View(new Animal { BirthDate = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Animal animal, IFormFile? imageFile)
    {
        ValidateSpecies(animal);
        ValidateImage(imageFile);

        if (ModelState.IsValid)
        {
            NormalizeSpecies(animal);
            _animalAppService.Create(animal);
            SaveImage(animal.Id, imageFile);
            return RedirectToAction(nameof(Index));
        }

        LoadSpecies();
        return View(animal);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        Animal? animal = _animalAppService.GetById(id);
        if (animal == null)
        {
            return NotFound();
        }

        // Zvíře s vlastním druhem má SpeciesId null -> v selectu se má vybrat "Jiné".
        if (animal.SpeciesId == null)
        {
            animal.SpeciesId = OtherSpeciesId;
        }

        LoadSpecies();
        return View(animal);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Animal animal, IFormFile? imageFile)
    {
        if (id != animal.Id)
        {
            return NotFound();
        }

        ValidateSpecies(animal);
        ValidateImage(imageFile);

        if (ModelState.IsValid)
        {
            NormalizeSpecies(animal);
            bool updated = _animalAppService.Update(animal);
            if (!updated)
            {
                return NotFound();
            }
            SaveImage(animal.Id, imageFile);
            return RedirectToAction(nameof(Index));
        }

        // Při chybě validace se formulář vrací bez fotky z DB -> doplníme ji kvůli náhledu.
        animal.ImageFileName = _animalAppService.GetById(id)?.ImageFileName;
        LoadSpecies();
        return View(animal);
    }

    // Nahrání nebo výměna fotky přímo z detailu zvířete.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UploadImage(int id, IFormFile? imageFile)
    {
        if (imageFile == null)
        {
            TempData["ImageError"] = "Vyberte fotku, kterou chcete nahrát.";
            return RedirectToAction(nameof(Details), new { id });
        }

        string? error = _imageStorage.Validate(imageFile.FileName, imageFile.Length);
        if (error != null)
        {
            TempData["ImageError"] = error;
            return RedirectToAction(nameof(Details), new { id });
        }

        using (Stream stream = imageFile.OpenReadStream())
        {
            if (!_animalAppService.UpdateImage(id, stream, imageFile.FileName))
            {
                return NotFound();
            }
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        bool deleted = _animalAppService.Delete(id);
        if (deleted)
        {
            return RedirectToAction(nameof(Index));
        }
        return NotFound();
    }

    private void ValidateImage(IFormFile? imageFile)
    {
        // Fotka je nepovinná, kontroluje se jen když ji uživatel vybral.
        if (imageFile == null)
        {
            return;
        }

        string? error = _imageStorage.Validate(imageFile.FileName, imageFile.Length);
        if (error != null)
        {
            ModelState.AddModelError("imageFile", error);
        }
    }

    private void SaveImage(int animalId, IFormFile? imageFile)
    {
        if (imageFile == null)
        {
            return;
        }

        using (Stream stream = imageFile.OpenReadStream())
        {
            _animalAppService.UpdateImage(animalId, stream, imageFile.FileName);
        }
    }

    private void ValidateSpecies(Animal animal)
    {
        if (animal.SpeciesId == null)
        {
            ModelState.AddModelError(nameof(Animal.SpeciesId), "Vyberte druh zvířete.");
        }
        else if (animal.SpeciesId == OtherSpeciesId && string.IsNullOrWhiteSpace(animal.OtherSpeciesName))
        {
            ModelState.AddModelError(nameof(Animal.OtherSpeciesName), "Napište, o jaký druh zvířete jde.");
        }
    }

    private void NormalizeSpecies(Animal animal)
    {
        if (animal.SpeciesId == OtherSpeciesId)
        {
            // Vlastní druh se neukládá do tabulky druhů, patří jen k tomuto zvířeti.
            animal.SpeciesId = null;
            animal.OtherSpeciesName = animal.OtherSpeciesName!.Trim();
        }
        else
        {
            animal.OtherSpeciesName = null;
        }
    }

    private void LoadSpecies()
    {
        List<SelectListItem> items = _animalAppService.SelectAllSpecies()
            .Select(s => new SelectListItem(s.Name, s.Id.ToString()))
            .ToList();
        items.Add(new SelectListItem("Jiné…", OtherSpeciesId.ToString()));

        ViewBag.Species = items;
    }
}
