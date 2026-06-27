// Importa funcionalidades de autorização, usadas para restringir páginas a certos papéis de utilizador.
using Microsoft.AspNetCore.Authorization;
// Importa classes MVC, como IActionResult, usadas para devolver respostas das ações da página.
using Microsoft.AspNetCore.Mvc;
// Importa PageModel, a classe base usada pelas páginas Razor.
using Microsoft.AspNetCore.Mvc.RazorPages;
// Importa SelectList, usada para preencher dropdowns nos formulários.
using Microsoft.AspNetCore.Mvc.Rendering;
// Importa SignalR, usado para enviar notificações em tempo real aos clientes ligados.
using Microsoft.AspNetCore.SignalR;
// Importa métodos assíncronos e de consulta do Entity Framework, como Include, ToListAsync e AnyAsync.
using Microsoft.EntityFrameworkCore;
// Importa o contexto da base de dados da aplicação.
using Projeto.Data;
// Importa o hub de notificações usado pelo SignalR.
using Projeto.Hubs;
// Importa os modelos de dados usados nesta página, como Driver.
using Projeto.Models;

// Define o namespace onde ficam agrupadas as páginas Razor relacionadas com pilotos.
namespace Projeto.Pages.Drivers;

// Restringe o acesso a esta página apenas a utilizadores com o papel Admin.
[Authorize(Roles = "Admin")]
// Classe PageModel responsável pela lógica da página de criação de pilotos.
public class CreateModel : PageModel
{
    // Guarda uma referência ao contexto da base de dados para fazer consultas e alterações.
    private readonly ApplicationDbContext _context;
    // Guarda uma referência ao hub SignalR para enviar notificações em tempo real.
    private readonly IHubContext<NotificationHub> _hubContext;

    // Construtor da página, chamado automaticamente pelo ASP.NET Core através de injeção de dependências.
    public CreateModel(ApplicationDbContext context, IHubContext<NotificationHub> hubContext)
    {
        // Atribui o contexto recebido ao campo privado, permitindo aceder à base de dados nos métodos da página.
        _context = context;
        // Atribui o hub recebido ao campo privado, permitindo enviar notificações aos utilizadores.
        _hubContext = hubContext;
    }

    // Indica que esta propriedade será automaticamente preenchida com os dados enviados pelo formulário.
    [BindProperty]
    // Representa o piloto que está a ser criado, editado, visualizado ou eliminado.
    public Driver Driver { get; set; } = new();

    // Guarda a lista de equipas usada para preencher o dropdown do formulário.
    public SelectList TeamOptions { get; set; } = default!;

    // Método executado quando a página é aberta através de um pedido GET.
    public async Task<IActionResult> OnGetAsync()
    {
        // Carrega novamente as equipas para que o dropdown apareça preenchido na página.
        await LoadTeamsAsync();
        // Devolve a própria página Razor ao utilizador, mantendo-o no mesmo ecrã.
        return Page();
    }

    // Método executado quando o formulário é submetido através de um pedido POST.
    public async Task<IActionResult> OnPostAsync()
    {
        // Verifica de forma assíncrona se existe pelo menos uma equipa registada na base de dados.
        if (!await _context.Teams.AnyAsync())
        {
            // Adiciona uma mensagem de erro de validação que será apresentada ao utilizador.
            ModelState.AddModelError(string.Empty, "É necessário criar uma equipa antes de criar um piloto.");
        }

        // Verifica se os dados recebidos do formulário respeitam as validações definidas no modelo.
        if (!ModelState.IsValid)
        {
            // Carrega novamente as equipas para que o dropdown apareça preenchido na página.
            await LoadTeamsAsync();
            // Devolve a própria página Razor ao utilizador, mantendo-o no mesmo ecrã.
            return Page();
        }

        // Adiciona o novo piloto ao contexto, preparando-o para ser inserido na base de dados.
        _context.Drivers.Add(Driver);
        // Guarda de forma assíncrona na base de dados todas as alterações feitas no contexto.
        await _context.SaveChangesAsync();

        // Envia uma notificação em tempo real para todos os clientes ligados à aplicação.
        await _hubContext.Clients.All.SendAsync(
            // Nome do método JavaScript que os clientes SignalR irão receber para mostrar a notificação.
            "ReceiveNotification",
            // Mensagem enviada aos utilizadores para informar que foi criado um novo piloto.
            $"Novo piloto adicionado: {Driver.Name}"
        );

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
