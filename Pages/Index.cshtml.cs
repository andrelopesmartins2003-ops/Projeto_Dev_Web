// Importa tipos do ASP.NET Core MVC.
using Microsoft.AspNetCore.Mvc;
// Importa a classe PageModel, base das páginas Razor.
using Microsoft.AspNetCore.Mvc.RazorPages;

// Define o namespace onde esta PageModel se encontra.
namespace Projeto.Pages;

// PageModel da página inicial da aplicação.
public class IndexModel : PageModel
{
    // Método executado quando a página inicial é aberta através de um pedido HTTP GET.
    public void OnGet()
    {
        // Não é necessário carregar dados dinâmicos, porque a página inicial apenas mostra links de navegação.

    }
}
