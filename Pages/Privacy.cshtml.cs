// Importa tipos do ASP.NET Core MVC.
using Microsoft.AspNetCore.Mvc;
// Importa a classe PageModel, base das páginas Razor.
using Microsoft.AspNetCore.Mvc.RazorPages;

// Define o namespace onde esta PageModel se encontra.
namespace Projeto.Pages;

// PageModel da página de privacidade.
public class PrivacyModel : PageModel
{
    // Método executado quando a página Privacy é aberta através de um pedido HTTP GET.
    public void OnGet()
    {
        // Não existem dados dinâmicos a carregar nesta página.
    }
}
