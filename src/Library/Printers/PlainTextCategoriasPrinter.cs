namespace Library;


public class PlainTextCategoriaPrinter : ITextPrinter<Categoria, Usuario>
{
    public string PrintAll(List<Categoria> catalog, Usuario user)
    {
        string response = $"Categorías ({catalog.Count} en total)";

        foreach (var cat in catalog)
        {
            response += $"\n»» ID: {cat.GetId()} ║ Descripción: {cat.Descripcion}\n";
        }
        return response;
    }
}