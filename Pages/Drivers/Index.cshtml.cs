// Imports
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Projeto.Data;

namespace Projeto.Pages.Drivers;

// Classe PageModel responsável pela listagem e classificação dos pilotos.
public class IndexModel : PageModel
{
    // Variaveis para consultar e alterar o contexto e a base de dados.
    private readonly ApplicationDbContext _context;

    // Construtor da página, chamado automaticamente pelo ASP.NET Core através de injeção de dependências.
    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // Guarda a lista de pilotos já transformada para apresentação na classificação.
    public IList<DriverStanding> Drivers { get; set; } = new List<DriverStanding>();

    // Método executado ao abrir a página de listagem, carregando os dados a apresentar.
    public async Task OnGetAsync()
    {
        // Inicia a consulta aos pilotos para construir a classificação apresentada no índice.
        Drivers = await _context.Drivers
            .Include(d => d.Team)
            .Include(d => d.RaceDrivers)
            .Select(d => new DriverStanding
            {
                Id = d.Id,
                Name = d.Name,
                Nationality = d.Nationality,
                TeamName = d.Team != null ? d.Team.Name : "",
                Points = d.RaceDrivers.Sum(rd =>
                    // Atribui pontos consoante a posição
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
            // Ordena os pilotos por pontos, do maior para o menor, formando a classificação.
            .OrderByDescending(d => d.Points)
            // Em caso de empate nos pontos, ordena alfabeticamente pelo nome do piloto.
            .ThenBy(d => d.Name)
            .ToListAsync();
    }

    // Classe interna usada apenas para guardar os dados que aparecem na tabela de classificação.
    public class DriverStanding
    {
        // Identificador do piloto usado para criar ligações para outras páginas.
        public int Id { get; set; }
        // Nome do piloto apresentado ao utilizador.
        public string Name { get; set; } = "";
        // Nacionalidade do piloto apresentada ao utilizador.
        public string Nationality { get; set; } = "";
        // Nome da equipa do piloto apresentado na tabela.
        public string TeamName { get; set; } = "";
        // Total de pontos calculado para o piloto.
        public int Points { get; set; }
    }
}
