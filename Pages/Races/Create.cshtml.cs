// Imports
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Projeto.Data;
using Projeto.Hubs;
using Projeto.Models;

// Define que esta classe pertence ao namespace das páginas relacionadas com corridas.
namespace Projeto.Pages.Races;

// Garante que apenas utilizadores autenticados com o papel "Admin" podem aceder a esta página.
[Authorize(Roles = "Admin")]

// Classe responsável pela lógica da página Create.cshtml, usada para criar uma nova corrida.
public class CreateModel : PageModel
{
    // Variáveis privadas para armazenar o contexto da base de dados e o Hub SignalR.
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<NotificationHub> _hubContext;

    // Construtor chamado automaticamente pelo ASP.NET Core através de injeção de dependências.
    public CreateModel(ApplicationDbContext context, IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    // Liga os dados submetidos no formulário HTML à propriedade Race.
    [BindProperty]

    // Objeto que representa a corrida que será preenchida pelo formulário e guardada na base de dados.
    public Race Race { get; set; } = new();

    // Método executado quando a página é aberta através de um pedido GET.
    public IActionResult OnGet()
    {
        return Page();
    }

    // Método executado quando o formulário é submetido através de POST.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Adiciona a nova corrida ao contexto da base de dados e guarda as alterações.
        _context.Races.Add(Race);
        await _context.SaveChangesAsync();

        // Envia uma notificação em tempo real para todos os clientes conectados ao Hub SignalR.
        await _hubContext.Clients.All.SendAsync(
            "ReceiveNotification",
            $"Nova corrida criada: {Race.GrandPrixName}"
        );

        return RedirectToPage("Index");
    }
}
