namespace UTB.Utulek.Application.Abstraction;

public interface IImageStorage
{
    // Vrací chybovou hlášku, nebo null, když je soubor v pořádku.
    string? Validate(string fileName, long length);

    // Uloží obrázek a vrátí jeho webovou cestu (např. /img/animals/abc.jpg).
    string Save(Stream content, string fileName);

    void Delete(string? webPath);
}
