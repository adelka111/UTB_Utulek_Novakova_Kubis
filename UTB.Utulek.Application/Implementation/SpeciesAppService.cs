using UTB.Utulek.Domain.Entities;
using UTB.Utulek.Application.Abstraction;
using UTB.Utulek.Domain.Repositories;

namespace UTB.Utulek.Application.Implementation;

public class SpeciesAppService : ISpeciesAppService
{
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IAnimalRepository _animalRepository;

    public SpeciesAppService(ISpeciesRepository speciesRepository, IAnimalRepository animalRepository)
    {
        _speciesRepository = speciesRepository;
        _animalRepository = animalRepository;
    }

    public IList<Species> SelectAll()
    {
        return _speciesRepository.GetAll().OrderBy(s => s.Name).ToList();
    }

    public Species? GetById(int id)
    {
        return _speciesRepository.GetById(id);
    }

    public bool NameExists(string name, int exceptId = 0)
    {
        string trimmed = name.Trim();
        return _speciesRepository.GetAll(s => s.Id != exceptId && s.Name == trimmed).Any();
    }

    public bool HasAnimals(int id)
    {
        return _animalRepository.GetAll(a => a.SpeciesId == id).Any();
    }

    public void Create(Species species)
    {
        species.Name = species.Name.Trim();
        _speciesRepository.Add(species);
    }

    public bool Update(Species species)
    {
        Species? existing = _speciesRepository.GetById(species.Id);
        if (existing == null)
        {
            return false;
        }

        existing.Name = species.Name.Trim();

        _speciesRepository.Edit(existing);
        return true;
    }

    public bool Delete(int id)
    {
        Species? species = _speciesRepository.GetById(id);
        if (species == null)
        {
            return false;
        }

        _speciesRepository.Delete(species);
        return true;
    }
}
