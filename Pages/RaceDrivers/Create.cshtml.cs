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

namespace Projeto.Pages.RaceDrivers;

// Apenas utilizadores com perfil de Administrador podem aceder.
[Authorize(Roles = "Admin")]

// Classe responsável pela lógica desta Razor Page.
public class CreateModel : PageModel
{
//Variáveis privadas para aceder à base de dados e ao SignalR.
    private readonly ApplicationDbContext _context;
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
    public async Task<IActionResult> OnGetAsync(int? raceId)
    {
        if (raceId.HasValue)
        {
            RaceDriver.RaceId = raceId.Value;
        }

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

// Procura os registos na base de dados.
        var race = await _context.Races.FindAsync(RaceDriver.RaceId);
        var driver = await _context.Drivers.FindAsync(RaceDriver.DriverId);

// Envia uma notificação em tempo real a todos os clientes ligados.
        await _hubContext.Clients.All.SendAsync(
            "ReceiveNotification",
            $"Resultado adicionado: {driver?.Name} em {race?.GrandPrixName} - posição {RaceDriver.Position}"
        );

        return RedirectToPage("Index");
    }

    // Carrega as opções para os campos de seleção (dropdowns) na página.
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