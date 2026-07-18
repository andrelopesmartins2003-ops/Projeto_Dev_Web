// Imports
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Projeto.Data;
using Projeto.Hubs;
using Projeto.Models;

namespace Projeto.Pages.RaceDrivers;

// Apenas utilizadores com perfil de Administrador podem aceder.
[Authorize(Roles = "Admin")]

// Classe responsável pela lógica desta Razor Page.
public class EditModel : PageModel
{
// Variáveis privadas para aceder à base de dados e ao SignalR.
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<NotificationHub> _hubContext;

// Contexto do SignalR utilizado para enviar notificações em tempo real.
    public EditModel(ApplicationDbContext context, IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    [BindProperty]
    public RaceDriver? RaceDriver { get; set; }

// Executado quando a página é aberta pelo utilizador.
    public async Task<IActionResult> OnGetAsync(int raceId, int driverId)
    {
        RaceDriver = await _context.RaceDrivers
            .Include(rd => rd.Race)
            .Include(rd => rd.Driver)
// Procura o registo correspondente na base de dados.
            .FirstOrDefaultAsync(rd =>
                rd.RaceId == raceId &&
                rd.DriverId == driverId);

        if (RaceDriver == null)
        {
            return NotFound();
        }

        return Page();
    }

// Executado quando o formulário é submetido.
    public async Task<IActionResult> OnPostAsync()
    {
        if (RaceDriver == null)
        {
            return NotFound();
        }

// Verifica se todos os dados introduzidos são válidos.
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var raceDriverToUpdate = await _context.RaceDrivers
            .Include(rd => rd.Race)
            .Include(rd => rd.Driver)
// Procura o registo correspondente na base de dados.
            .FirstOrDefaultAsync(rd =>
                rd.RaceId == RaceDriver.RaceId &&
                rd.DriverId == RaceDriver.DriverId);

        if (raceDriverToUpdate == null)
        {
            return NotFound();
        }

        raceDriverToUpdate.Position = RaceDriver.Position;

// Guarda definitivamente as alterações na base de dados.
        await _context.SaveChangesAsync();

// Envia uma notificação em tempo real a todos os clientes ligados.
        await _hubContext.Clients.All.SendAsync(
            "ReceiveNotification",
            $"Resultado atualizado: {raceDriverToUpdate.Driver?.Name} - {raceDriverToUpdate.Position}"
        );

        return RedirectToPage("Index");
    }
}