// Importa as bibliotecas necessárias para esta página.
using Microsoft.AspNetCore.Authorization;
// Importa as bibliotecas necessárias para esta página.
using Microsoft.AspNetCore.Mvc;
// Importa as bibliotecas necessárias para esta página.
using Microsoft.AspNetCore.Mvc.RazorPages;
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
public class EditModel : PageModel
{
// Contexto da base de dados utilizado para comunicar com o Entity Framework.
    private readonly ApplicationDbContext _context;
// Contexto do SignalR utilizado para enviar notificações em tempo real.
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

// Redireciona o utilizador para a página principal.
        return RedirectToPage("Index");
    }
}