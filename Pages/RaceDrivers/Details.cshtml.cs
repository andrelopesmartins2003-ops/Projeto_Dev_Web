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

// Classe responsável pela lógica desta Razor Page.
public class DetailsModel : PageModel
{
// Contexto da base de dados utilizado para comunicar com o Entity Framework.
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Race? Race { get; set; }

    public IList<RaceDriver> RaceDrivers { get; set; } = new List<RaceDriver>();

// Executado quando a página é aberta pelo utilizador.
    public async Task<IActionResult> OnGetAsync(int raceId)
    {
        Race = await _context.Races
// Procura o registo correspondente na base de dados.
            .FirstOrDefaultAsync(r => r.Id == raceId);

        if (Race == null)
        {
            return NotFound();
        }

        RaceDrivers = await _context.RaceDrivers
            .Include(rd => rd.Driver)
            .ThenInclude(d => d.Team)
            .Where(rd => rd.RaceId == raceId)
            .ToListAsync();

        RaceDrivers = RaceDrivers
            .OrderBy(rd =>
            {
                if (int.TryParse(rd.Position, out int pos))
                    return pos;

                return rd.Position switch
                {
                    "DNF" => 23,
                    "DNS" => 24,
                    "DSQ" => 25,
                    _ => 26
                };
            })
            .ToList();

        return Page();
    }
}