// Importa funcionalidades de autorização, usadas para restringir páginas a certos papéis de utilizador.
using Microsoft.AspNetCore.Authorization;
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

// Restringe o acesso a esta página apenas a utilizadores com o papel Admin.
[Authorize(Roles = "Admin")]
// Classe PageModel responsável pela lógica da página de eliminação de pilotos.
public class DeleteModel : PageModel
{
    // Guarda uma referência ao contexto da base de dados para fazer consultas e alterações.
    private readonly ApplicationDbContext _context;

    // Construtor da página, chamado automaticamente pelo ASP.NET Core através de injeção de dependências.
    public DeleteModel(ApplicationDbContext context)
    {
        // Atribui o contexto recebido ao campo privado, permitindo aceder à base de dados nos métodos da página.
        _context = context;
    }

    // Indica que esta propriedade será automaticamente preenchida com os dados enviados pelo formulário.
    [BindProperty]
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

    // Método executado quando o formulário é submetido através de um pedido POST.
    public async Task<IActionResult> OnPostAsync(int id)
    {
        // Procura na base de dados um piloto com o identificador recebido na rota.
        var driver = await _context.Drivers.FindAsync(id);

        // Confirma se o piloto existe antes de continuar, evitando erros ao aceder a dados inexistentes.
        if (driver == null)
        {
            // Devolve erro 404 quando o piloto pedido não existe.
            return NotFound();
        }

        // Remove o piloto do contexto, preparando a eliminação na base de dados.
        _context.Drivers.Remove(driver);
        // Guarda de forma assíncrona na base de dados todas as alterações feitas no contexto.
        await _context.SaveChangesAsync();

        // Redireciona o utilizador para a página de listagem depois da operação terminar com sucesso.
        return RedirectToPage("Index");
    }
}
