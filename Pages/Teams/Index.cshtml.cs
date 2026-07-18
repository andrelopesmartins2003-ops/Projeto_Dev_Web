using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Projeto.Data;

namespace Projeto.Pages.Teams;

// PageModel da página de classificação/listagem de equipas.
public class IndexModel : PageModel
{
    // DbContext usado para consultar equipas, pilotos e resultados.
    private readonly ApplicationDbContext _context;

    // Recebe o contexto da base de dados por injeção de dependências.
    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // Lista de equipas já preparada para a tabela da página.
    // Usa TeamStanding em vez de Team para incluir valores calculados, como pontos e número de pilotos.
    public IList<TeamStanding> Teams { get; set; } = new List<TeamStanding>();

    // Executado quando a página é aberta por GET.
    public async Task OnGetAsync()
    {
        // Consulta as equipas e inclui relações necessárias para calcular os pontos.
        Teams = await _context.Teams
            // Inclui os pilotos associados a cada equipa.
            .Include(t => t.Drivers)
            // Para cada piloto, inclui as participações em corridas.
            .ThenInclude(d => d.RaceDrivers)
            // Transforma cada Team num objeto TeamStanding com apenas os dados necessários para a tabela.
            .Select(t => new TeamStanding
            {
                Id = t.Id,
                Name = t.Name,
                Country = t.Country,
                // Conta quantos pilotos pertencem à equipa.
                DriverCount = t.Drivers.Count,
                // Soma os pontos de todos os pilotos da equipa com base na posição final em cada corrida.
                Points = t.Drivers
                    .SelectMany(d => d.RaceDrivers)
                    .Sum(rd =>
                        rd.Position == "1" ? 25 :
                        rd.Position == "2" ? 18 :
                        rd.Position == "3" ? 15 :
                        rd.Position == "4" ? 12 :
                        rd.Position == "5" ? 10 :
                        rd.Position == "6" ? 8 :
                        rd.Position == "7" ? 6 :
                        rd.Position == "8" ? 4 :
                        rd.Position == "9" ? 2 :
                        rd.Position == "10" ? 1 : 0)
            })
            // Ordena primeiro pelas equipas com mais pontos.
            .OrderByDescending(t => t.Points)
            // Em caso de empate, ordena alfabeticamente pelo nome.
            .ThenBy(t => t.Name)
            // Executa a query na base de dados e converte o resultado para lista.
            .ToListAsync();
    }

    // Classe auxiliar usada apenas nesta página para representar uma linha da classificação.
    public class TeamStanding
    {
        // Id da equipa, usado nos links Details/Edit/Delete.
        public int Id { get; set; }

        // Nome da equipa apresentado na tabela.
        public string Name { get; set; } = "";

        // País da equipa apresentado na tabela.
        public string Country { get; set; } = "";

        // Número de pilotos associados à equipa.
        public int DriverCount { get; set; }

        // Total de pontos calculado a partir das posições dos pilotos nas corridas.
        public int Points { get; set; }
    }
}
