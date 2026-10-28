using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UTB.Utulek.Application.Abstraction;
using UTB.Utulek.Domain.Entities;
using UTB.Utulek.Web.Models;

namespace UTB.Utulek.Web.Controllers;

public class HomeController : Controller
{
    IAnimalAppService _animalAppService;
    public HomeController(IAnimalAppService animalAppService)
    {
        _animalAppService = animalAppService;
    }

    public IActionResult Index()
    {
        IList<Animal> animals = _animalAppService.SelectAll();
        return View(animals);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
