// Importa as bibliotecas necessárias para esta página.
using Microsoft.AspNetCore.Authorization;
// Importa as bibliotecas necessárias para esta página.
using Microsoft.AspNetCore.Mvc;
// Importa as bibliotecas necessárias para esta página.
using Microsoft.AspNetCore.Mvc.RazorPages;
// Importa as bibliotecas necessárias para esta página.
using Microsoft.EntityFrameworkCore;
// Importa as bibliotecas necessárias para esta página.
using Projeto.Data;
// Importa as bibliotecas necessárias para esta página.
using Projeto.Models;

// Define o namespace onde esta página está organizada.
namespace Projeto.Pages.RaceDrivers;

// Apenas utilizadores com perfil de Administrador podem aceder.
[Authorize(Roles = "Admin")]
// Classe responsável pela lógica desta Razor Page.
public class DeleteModel : PageModel
{
// Contexto da base de dados utilizado para comunicar com o Entity Framework.
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public RaceDriver? RaceDriver { get; set; }

// Executado quando a página é aberta pelo utilizador.
    public async Task<IActionResult> OnGetAsync(int raceId, int driverId)
    {
        RaceDriver = await _context.RaceDrivers
            .Include(rd => rd.Race)
            .Include(rd => rd.Driver)
                .ThenInclude(d => d.Team)
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

        var raceDriverToDelete = await _context.RaceDrivers
// Procura o registo correspondente na base de dados.
            .FirstOrDefaultAsync(rd =>
                rd.RaceId == RaceDriver.RaceId &&
                rd.DriverId == RaceDriver.DriverId);

        if (raceDriverToDelete == null)
        {
            return NotFound();
        }

// Remove o registo da base de dados.
        _context.RaceDrivers.Remove(raceDriverToDelete);
// Guarda definitivamente as alterações na base de dados.
        await _context.SaveChangesAsync();

// Redireciona o utilizador para a página principal.
        return RedirectToPage("Index");
    }
}