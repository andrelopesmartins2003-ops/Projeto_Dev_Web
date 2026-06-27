// Importa as bibliotecas necessárias para esta página.
using Microsoft.AspNetCore.Authorization;
// Importa as bibliotecas necessárias para esta página.
using Microsoft.AspNetCore.Mvc;
// Importa as bibliotecas necessárias para esta página.
using Microsoft.AspNetCore.Mvc.RazorPages;
// Importa as bibliotecas necessárias para esta página.
using Microsoft.AspNetCore.Mvc.Rendering;
// Importa as bibliotecas necessárias para esta página.
using Microsoft.AspNetCore.SignalR;
// Importa as bibliotecas necessárias para esta página.
using Microsoft.EntityFrameworkCore;
// Importa as bibliotecas necessárias para esta página.
using Projeto.Data;
// Importa as bibliotecas necessárias para esta página.
using Projeto.Hubs;
// Importa as bibliotecas necessárias para esta página.
using Projeto.Models;

// Define o namespace onde esta página está organizada.
namespace Projeto.Pages.RaceDrivers;

// Apenas utilizadores com perfil de Administrador podem aceder.
[Authorize(Roles = "Admin")]
// Classe responsável pela lógica desta Razor Page.
public class CreateModel : PageModel
{
// Contexto da base de dados utilizado para comunicar com o Entity Framework.
    private readonly ApplicationDbContext _context;
// Contexto do SignalR utilizado para enviar notificações em tempo real.
    private readonly IHubContext<NotificationHub> _hubContext;

// Contexto do SignalR utilizado para enviar notificações em tempo real.
    public CreateModel(ApplicationDbContext context, IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    [BindProperty]
    public RaceDriver RaceDriver { get; set; } = new();

    public SelectList RaceOptions { get; set; } = default!;
    public SelectList DriverOptions { get; set; } = default!;

// Executado quando a página é aberta pelo utilizador.
    public async Task<IActionResult> OnGetAsync()
    {
        await LoadOptionsAsync();
        return Page();
    }

// Executado quando o formulário é submetido.
    public async Task<IActionResult> OnPostAsync()
    {
        var alreadyExists = await _context.RaceDrivers.AnyAsync(rd =>
            rd.RaceId == RaceDriver.RaceId &&
            rd.DriverId == RaceDriver.DriverId);

        if (alreadyExists)
        {
            ModelState.AddModelError(string.Empty, "Este piloto já está associado a esta corrida.");
        }

// Verifica se todos os dados introduzidos são válidos.
        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            return Page();
        }

// Adiciona o novo registo à base de dados.
        _context.RaceDrivers.Add(RaceDriver);
// Guarda definitivamente as alterações na base de dados.
        await _context.SaveChangesAsync();

// Procura o registo correspondente na base de dados.
        var race = await _context.Races.FindAsync(RaceDriver.RaceId);
// Procura o registo correspondente na base de dados.
        var driver = await _context.Drivers.FindAsync(RaceDriver.DriverId);

// Envia uma notificação em tempo real a todos os clientes ligados.
        await _hubContext.Clients.All.SendAsync(
            "ReceiveNotification",
            $"Resultado adicionado: {driver?.Name} em {race?.GrandPrixName} - posição {RaceDriver.Position}"
        );

// Redireciona o utilizador para a página principal.
        return RedirectToPage("Index");
    }

    private async Task LoadOptionsAsync()
    {
        var races = await _context.Races
            .OrderBy(r => r.Date)
            .ToListAsync();

        var drivers = await _context.Drivers
            .Include(d => d.Team)
            .OrderBy(d => d.Name)
            .ToListAsync();

        RaceOptions = new SelectList(races, "Id", "GrandPrixName");

        DriverOptions = new SelectList(
            drivers.Select(d => new
            {
                d.Id,
                DisplayName = d.Team == null
                    ? d.Name
                    : $"{d.Name} ({d.Team.Name})"
            }),
            "Id",
            "DisplayName"
        );
    }
}