using UTB.Utulek.Domain.Entities;

namespace UTB.Utulek.Application.Abstraction;

public interface IAnimalAppService
{
    IList<Animal> SelectAll();
    IList<Species> SelectAllSpecies();
    Animal? GetById(int id);
    void Create(Animal animal);
    bool Update(Animal animal);
    bool UpdateImage(int id, Stream content, string fileName);
    bool Delete(int id);
}
