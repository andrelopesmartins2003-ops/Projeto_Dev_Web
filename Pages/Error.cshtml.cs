// Imports
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Projeto.Pages;

// Impede que a página de erro seja guardada em cache pelo browser ou por proxies.
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

// Ignora a validação antiforgery nesta página, porque ela pode ser chamada durante erros da aplicação.
[IgnoreAntiforgeryToken]

// PageModel responsável por preparar os dados apresentados na página Error.cshtml.
public class ErrorModel : PageModel
{
    // Guarda o identificador do pedido que causou o erro, podendo ser nulo.
    public string? RequestId { get; set; }

    // Indica se o RequestId deve ser mostrado na página.
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    // Método executado quando a página de erro é aberta através de um pedido HTTP GET.
    public void OnGet()
    {
        // Usa o Id da atividade atual, se existir; caso contrário, usa o TraceIdentifier do HttpContext.
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
}
