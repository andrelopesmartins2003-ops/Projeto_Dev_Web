// Imports
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Projeto.Data;

namespace Projeto.Pages.RaceDrivers;

// Classe responsável pela lógica desta Razor Page.
public class IndexModel : PageModel
{

    // Variável privada para aceder à base de dados.
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<RaceResult> Races { get; set; } = new List<RaceResult>();

// Executado quando a página é aberta pelo utilizador.
    public async Task OnGetAsync()
    {
        Races = await _context.Races
            .Include(r => r.RaceDrivers)
                .ThenInclude(rd => rd.Driver)
            .OrderBy(r => r.Date)
            .Select(r => new RaceResult
            {
                Id = r.Id,
                GrandPrixName = r.GrandPrixName,
                Date = r.Date,
                Circuit = r.Circuit,
                Winner = r.RaceDrivers
                    .Where(rd => rd.Position == "1")
                    .Select(rd => rd.Driver!.Name)
                    .FirstOrDefault() ?? "Sem vencedor"
            })
            .ToListAsync();
    }

// Classe responsável pela lógica desta Razor Page.
    public class RaceResult
    {
        public int Id { get; set; }

        public string GrandPrixName { get; set; } = "";

        public DateTime Date { get; set; }

        public string Circuit { get; set; } = "";

        public string Winner { get; set; } = "";
    }
}