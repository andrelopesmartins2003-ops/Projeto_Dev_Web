// Importa funcionalidades de autorização, usadas para restringir páginas a certos papéis de utilizador.
using Microsoft.AspNetCore.Authorization;
// Importa classes MVC, como IActionResult, usadas para devolver respostas das ações da página.
using Microsoft.AspNetCore.Mvc;
// Importa PageModel, a classe base usada pelas páginas Razor.
using Microsoft.AspNetCore.Mvc.RazorPages;
// Importa SelectList, usada para preencher dropdowns nos formulários.
using Microsoft.AspNetCore.Mvc.Rendering;
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
// Classe PageModel responsável pela lógica da página de edição de pilotos.
public class EditModel : PageModel
{
    // Guarda uma referência ao contexto da base de dados para fazer consultas e alterações.
    private readonly ApplicationDbContext _context;

    // Construtor da página, chamado automaticamente pelo ASP.NET Core através de injeção de dependências.
    public EditModel(ApplicationDbContext context)
    {
        // Atribui o contexto recebido ao campo privado, permitindo aceder à base de dados nos métodos da página.
        _context = context;
    }

    // Indica que esta propriedade será automaticamente preenchida com os dados enviados pelo formulário.
    [BindProperty]
    // Representa o piloto que está a ser criado, editado, visualizado ou eliminado.
    public Driver Driver { get; set; } = new();

    // Guarda a lista de equipas usada para preencher o dropdown do formulário.
    public SelectList TeamOptions { get; set; } = default!;

    // Método executado quando a página é aberta através de um pedido GET.
    public async Task<IActionResult> OnGetAsync(int id)
    {
        // Procura na base de dados um piloto com o identificador recebido na rota.
        var driver = await _context.Drivers.FindAsync(id);

        // Confirma se o piloto existe antes de continuar, evitando erros ao aceder a dados inexistentes.
        if (driver == null)
        {
            // Devolve erro 404 quando o piloto pedido não existe.
            return NotFound();
        }

        // Copia os dados encontrados na base de dados para a propriedade ligada à página.
        Driver = driver;
        // Carrega novamente as equipas para que o dropdown apareça preenchido na página.
        await LoadTeamsAsync();

        // Devolve a própria página Razor ao utilizador, mantendo-o no mesmo ecrã.
        return Page();
    }

    // Método executado quando o formulário é submetido através de um pedido POST.
    public async Task<IActionResult> OnPostAsync()
    {
        // Verifica se os dados recebidos do formulário respeitam as validações definidas no modelo.
        if (!ModelState.IsValid)
        {
            // Carrega novamente as equipas para que o dropdown apareça preenchido na página.
            await LoadTeamsAsync();
            // Devolve a própria página Razor ao utilizador, mantendo-o no mesmo ecrã.
            return Page();
        }

        // Marca o piloto como alterado para que as modificações sejam guardadas na base de dados.
        _context.Drivers.Update(Driver);
        // Guarda de forma assíncrona na base de dados todas as alterações feitas no contexto.
        await _context.SaveChangesAsync();

        // Redireciona o utilizador para a página de listagem depois da operação terminar com sucesso.
        return RedirectToPage("Index");
    }

    // Método auxiliar que carrega as equipas da base de dados para o dropdown.
    private async Task LoadTeamsAsync()
    {
        // Inicia uma consulta à tabela de equipas para obter os dados do dropdown.
        var teams = await _context.Teams
            // Ordena as equipas alfabeticamente pelo nome para facilitar a escolha no formulário.
            .OrderBy(t => t.Name)
            // Executa a consulta de forma assíncrona e transforma o resultado numa lista.
            .ToListAsync();

        // Cria uma lista compatível com o componente select do Razor.
        TeamOptions = new SelectList(teams, "Id", "Name");
    }
}
