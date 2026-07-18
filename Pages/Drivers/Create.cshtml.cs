// Imports
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Projeto.Data;
using Projeto.Hubs;
using Projeto.Models;

namespace Projeto.Pages.Drivers;

// Restringe o acesso a esta página apenas a utilizadores com o papel Admin.
[Authorize(Roles = "Admin")]

// Classe PageModel responsável pela lógica da página de criação de pilotos.
public class CreateModel : PageModel
{
    // Variaveis para consultar e alterar o contexto e a base de dados
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<NotificationHub> _hubContext;

    // Construtor da página, chamado automaticamente pelo ASP.NET Core através de injeção de dependências.
    public CreateModel(ApplicationDbContext context, IHubContext<NotificationHub> hubContext)
    {
        _context = context;
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
        // Mostra os dados presentes sobre os pilotos.
        await LoadTeamsAsync();
        return Page();
    }

    // Método executado quando o formulário é submetido através de um pedido POST.
    public async Task<IActionResult> OnPostAsync()
    {
        // Verifica se existem equipas e manda uma mensagem de erro caso não existam
        if (!await _context.Teams.AnyAsync())
        {
            ModelState.AddModelError(string.Empty, "É necessário criar uma equipa antes de criar um piloto.");
        }

        // Verifica se os dados recebidos do formulário respeitam as validações definidas no modelo.
        if (!ModelState.IsValid)
        {
            await LoadTeamsAsync();
            return Page();
        }

        // Adiciona o novo piloto ao contexto e à base de dados.
        _context.Drivers.Add(Driver);
        await _context.SaveChangesAsync();

        // Envia uma notificação em tempo real para todos os clientes ligados à aplicação ao adicinar um piloto
        await _hubContext.Clients.All.SendAsync(
            "ReceiveNotification",
            $"Novo piloto adicionado: {Driver.Name}"
        );

        return RedirectToPage("Index");
    }

    // Consulta a tabela de equipas para obter os dados do dropdown e transformar numa lista
    private async Task LoadTeamsAsync()
    {
        var teams = await _context.Teams
            .OrderBy(t => t.Name)
            .ToListAsync();
        TeamOptions = new SelectList(teams, "Id", "Name");
    }
}
