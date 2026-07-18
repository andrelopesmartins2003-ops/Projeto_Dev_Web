// Imports
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Projeto.Data;
using Projeto.Models;

namespace Projeto.Pages.RaceDrivers;

// Classe responsável pela lógica desta Razor Page.
public class DetailsModel : PageModel
{
    // Variável privada para aceder à base de dados.
    private readonly ApplicationDbContext _context;

    // Construtor que recebe o contexto da base de dados como parâmetro.
    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Race? Race { get; set; }

    public IList<RaceDriver> RaceDrivers { get; set; } = new List<RaceDriver>();
    
    public async Task<IActionResult> OnGetAsync(int raceId)
    {
        Race = await _context.Races
            // Procura o registo correspondente na base de dados.
            .FirstOrDefaultAsync(r => r.Id == raceId);

        if (Race == null)
        {
            return NotFound();
        }

        // Procura os registos de RaceDrivers associados à corrida na base de dados.
        RaceDrivers = await _context.RaceDrivers
            .Include(rd => rd.Driver)
            .ThenInclude(d => d.Team)
            .Where(rd => rd.RaceId == raceId)
            .ToListAsync();

        RaceDrivers = RaceDrivers
            // Ordena a lista de RaceDrivers com base na posição, tratando os casos especiais.
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