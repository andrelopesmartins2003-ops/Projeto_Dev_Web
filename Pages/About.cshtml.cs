// Importa a classe PageModel, base das páginas Razor.
using Microsoft.AspNetCore.Mvc.RazorPages;

// Define o namespace onde esta PageModel se encontra.
namespace Projeto.Pages;

// PageModel da página About, responsável por tratar pedidos da página About.cshtml.
public class AboutModel : PageModel
{
    // Método executado quando a página é aberta através de um pedido HTTP GET.
    public void OnGet()
    {
        // Não é necessário carregar dados dinâmicos, porque a página About contém apenas informação estática.
    }
}
