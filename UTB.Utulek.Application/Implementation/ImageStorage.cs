using UTB.Utulek.Application.Abstraction;

namespace UTB.Utulek.Application.Implementation;

public class ImageStorage : IImageStorage
{
    private const string WebFolder = "/img/animals/";
    private const long MaxFileSize = 5 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    private readonly string _folderPath;

    public ImageStorage(string webRootPath)
    {
        _folderPath = Path.Combine(webRootPath, "img", "animals");
    }

    public string? Validate(string fileName, long length)
    {
        string extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            return "Fotka musí být ve formátu JPG, PNG, GIF nebo WEBP.";
        }
        if (length == 0)
        {
            return "Soubor s fotkou je prázdný.";
        }
        if (length > MaxFileSize)
        {
            return "Fotka může mít nejvýše 5 MB.";
        }
        return null;
    }

    public string Save(Stream content, string fileName)
    {
        // Vlastní jméno souboru (GUID), aby se fotky nepřepisovaly a nešlo podstrčit cestu.
        string storedName = Guid.NewGuid().ToString("N") + Path.GetExtension(fileName).ToLowerInvariant();

        Directory.CreateDirectory(_folderPath);
        using (FileStream file = File.Create(Path.Combine(_folderPath, storedName)))
        {
            content.CopyTo(file);
        }

        return WebFolder + storedName;
    }

    public void Delete(string? webPath)
    {
        // Mažou se jen nahrané fotky z naší složky, nikdy nic jiného.
        if (string.IsNullOrEmpty(webPath) || !webPath.StartsWith(WebFolder))
        {
            return;
        }

        string path = Path.Combine(_folderPath, Path.GetFileName(webPath));
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
