// Importa classes MVC, como IActionResult, usadas para devolver respostas das ações da página.
using Microsoft.AspNetCore.Mvc;
// Importa PageModel, a classe base usada pelas páginas Razor.
using Microsoft.AspNetCore.Mvc.RazorPages;
// Importa métodos assíncronos e de consulta do Entity Framework, como Include, ToListAsync e AnyAsync.
using Microsoft.EntityFrameworkCore;
// Importa o contexto da base de dados da aplicação.
using Projeto.Data;
// Importa os modelos de dados usados nesta página, como Driver.
using Projeto.Models;

// Define o namespace onde ficam agrupadas as páginas Razor relacionadas com pilotos.
namespace Projeto.Pages.Drivers;

// Classe PageModel responsável pela lógica da página de detalhes de um piloto.
public class DetailsModel : PageModel
{
    // Guarda uma referência ao contexto da base de dados para fazer consultas e alterações.
    private readonly ApplicationDbContext _context;

    // Construtor da página, chamado automaticamente pelo ASP.NET Core através de injeção de dependências.
    public DetailsModel(ApplicationDbContext context)
    {
        // Atribui o contexto recebido ao campo privado, permitindo aceder à base de dados nos métodos da página.
        _context = context;
    }

    public Driver? Driver { get; set; }

    // Método executado quando a página é aberta através de um pedido GET.
    public async Task<IActionResult> OnGetAsync(int id)
    {
        Driver = await _context.Drivers
            // Inclui também a equipa associada ao piloto, evitando mostrar apenas o Id da equipa.
            .Include(d => d.Team)
            // Procura o primeiro piloto com o Id indicado; se não existir, devolve null.
            .FirstOrDefaultAsync(d => d.Id == id);

        // Confirma se o piloto existe antes de continuar, evitando erros ao aceder a dados inexistentes.
        if (Driver == null)
        {
            // Devolve erro 404 quando o piloto pedido não existe.
            return NotFound();
        }

        // Devolve a própria página Razor ao utilizador, mantendo-o no mesmo ecrã.
        return Page();
    }
}
