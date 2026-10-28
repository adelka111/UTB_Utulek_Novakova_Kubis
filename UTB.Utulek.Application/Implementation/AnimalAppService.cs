using UTB.Utulek.Domain.Entities;
using UTB.Utulek.Application.Abstraction;
using UTB.Utulek.Domain.Repositories;

namespace UTB.Utulek.Application.Implementation;

public class AnimalAppService : IAnimalAppService
{
    private readonly IAnimalRepository _animalRepository;
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IImageStorage _imageStorage;

    public AnimalAppService(IAnimalRepository animalRepository, ISpeciesRepository speciesRepository, IImageStorage imageStorage)
    {
        _animalRepository = animalRepository;
        _speciesRepository = speciesRepository;
        _imageStorage = imageStorage;
    }

    public IList<Animal> SelectAll()
    {
        return _animalRepository.GetAllWithSpecies();
    }

    public IList<Species> SelectAllSpecies()
    {
        return _speciesRepository.GetAll().OrderBy(s => s.Name).ToList();
    }

    public Animal? GetById(int id)
    {
        return _animalRepository.GetByIdWithSpecies(id);
    }

    public void Create(Animal animal)
    {
        _animalRepository.Add(animal);
    }

    public bool Update(Animal animal)
    {
        Animal? existing = _animalRepository.GetById(animal.Id);
        if (existing == null)
        {
            return false;
        }

        // Přepisují se jen pole z formuláře, ostatní (např. ImageFileName) zůstávají.
        existing.Name = animal.Name;
        existing.Breed = animal.Breed;
        existing.BirthDate = animal.BirthDate;
        existing.Gender = animal.Gender;
        existing.Description = animal.Description;
        existing.SpeciesId = animal.SpeciesId;
        existing.OtherSpeciesName = animal.OtherSpeciesName;

        _animalRepository.Edit(existing);
        return true;
    }

    public bool UpdateImage(int id, Stream content, string fileName)
    {
        Animal? existing = _animalRepository.GetById(id);
        if (existing == null)
        {
            return false;
        }

        string? oldImage = existing.ImageFileName;
        existing.ImageFileName = _imageStorage.Save(content, fileName);
        _animalRepository.Edit(existing);

        // Starou fotku smažeme až po úspěšném uložení nové.
        _imageStorage.Delete(oldImage);
        return true;
    }

    public bool Delete(int id)
    {
        Animal? animal = _animalRepository.GetById(id);
        if (animal == null)
        {
            return false;
        }

        _animalRepository.Delete(animal);
        _imageStorage.Delete(animal.ImageFileName);
        return true;
    }
}
