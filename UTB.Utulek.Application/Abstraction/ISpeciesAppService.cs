using UTB.Utulek.Domain.Entities;

namespace UTB.Utulek.Application.Abstraction;

public interface ISpeciesAppService
{
    IList<Species> SelectAll();
    Species? GetById(int id);
    bool NameExists(string name, int exceptId = 0);
    bool HasAnimals(int id);
    void Create(Species species);
    bool Update(Species species);
    bool Delete(int id);
}
