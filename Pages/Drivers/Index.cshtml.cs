// Importa PageModel, a classe base usada pelas páginas Razor.
using Microsoft.AspNetCore.Mvc.RazorPages;
// Importa métodos assíncronos e de consulta do Entity Framework, como Include, ToListAsync e AnyAsync.
using Microsoft.EntityFrameworkCore;
// Importa o contexto da base de dados da aplicação.
using Projeto.Data;

// Define o namespace onde ficam agrupadas as páginas Razor relacionadas com pilotos.
namespace Projeto.Pages.Drivers;

// Classe PageModel responsável pela listagem e classificação dos pilotos.
public class IndexModel : PageModel
{
    // Guarda uma referência ao contexto da base de dados para fazer consultas e alterações.
    private readonly ApplicationDbContext _context;

    // Construtor da página, chamado automaticamente pelo ASP.NET Core através de injeção de dependências.
    public IndexModel(ApplicationDbContext context)
    {
        // Atribui o contexto recebido ao campo privado, permitindo aceder à base de dados nos métodos da página.
        _context = context;
    }

    // Guarda a lista de pilotos já transformada para apresentação na classificação.
    public IList<DriverStanding> Drivers { get; set; } = new List<DriverStanding>();

    // Método executado ao abrir a página de listagem, carregando os dados a apresentar.
    public async Task OnGetAsync()
    {
        // Inicia a consulta aos pilotos para construir a classificação apresentada no índice.
        Drivers = await _context.Drivers
            // Inclui também a equipa associada ao piloto, evitando mostrar apenas o Id da equipa.
            .Include(d => d.Team)
            // Inclui os resultados de corridas de cada piloto para permitir calcular os pontos.
            .Include(d => d.RaceDrivers)
            // Transforma cada piloto num objeto próprio para apresentação na tabela de classificação.
            .Select(d => new DriverStanding
            {
                // Guarda o Id do piloto para criar links de detalhes, edição e eliminação.
                Id = d.Id,
                // Guarda o nome do piloto para ser apresentado na tabela.
                Name = d.Name,
                // Guarda a nacionalidade do piloto para apresentação.
                Nationality = d.Nationality,
                // Obtém o nome da equipa se existir; caso contrário, usa uma string vazia.
                TeamName = d.Team != null ? d.Team.Name : "",
                // Calcula a soma dos pontos do piloto com base nas posições obtidas nas corridas.
                Points = d.RaceDrivers.Sum(rd =>
                    // Atribui 25 pontos quando o piloto terminou em 1.º lugar.
                    rd.Position == "1" ? 25 :
                    // Atribui 18 pontos quando o piloto terminou em 2.º lugar.
                    rd.Position == "2" ? 18 :
                    // Atribui 15 pontos quando o piloto terminou em 3.º lugar.
                    rd.Position == "3" ? 15 :
                    // Atribui 12 pontos quando o piloto terminou em 4.º lugar.
                    rd.Position == "4" ? 12 :
                    // Atribui 10 pontos quando o piloto terminou em 5.º lugar.
                    rd.Position == "5" ? 10 :
                    // Atribui 8 pontos quando o piloto terminou em 6.º lugar.
                    rd.Position == "6" ? 8 :
                    // Atribui 6 pontos quando o piloto terminou em 7.º lugar.
                    rd.Position == "7" ? 6 :
                    // Atribui 4 pontos quando o piloto terminou em 8.º lugar.
                    rd.Position == "8" ? 4 :
                    // Atribui 2 pontos quando o piloto terminou em 9.º lugar.
                    rd.Position == "9" ? 2 :
                    // Atribui 1 ponto quando o piloto terminou em 10.º lugar; outras posições recebem 0.
                    rd.Position == "10" ? 1 : 0)
            })
            // Ordena os pilotos por pontos, do maior para o menor, formando a classificação.
            .OrderByDescending(d => d.Points)
            // Em caso de empate nos pontos, ordena alfabeticamente pelo nome do piloto.
            .ThenBy(d => d.Name)
            // Executa a consulta de forma assíncrona e transforma o resultado numa lista.
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
