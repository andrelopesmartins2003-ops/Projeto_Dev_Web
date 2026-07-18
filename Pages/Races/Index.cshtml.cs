// Imports
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Projeto.Data;
using Projeto.Models;

namespace Projeto.Pages.Races;

// Classe responsável pela lógica da página Index.cshtml, que lista todas as corridas.
public class IndexModel : PageModel
{
    // Variável privada para armazenar o contexto da base de dados.
    private readonly ApplicationDbContext _context;

    // Construtor que recebe o contexto da base de dados por injeção de dependências.
    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // Propriedade pública que armazena a lista de corridas a ser exibida na página.
    public IList<Race> Races { get; set; } = new List<Race>();

    // Método executado quando o utilizador abre a página de listagem das corridas.
    public async Task OnGetAsync()
    {
        // Carrega todas as corridas da base de dados, ordenadas pela data, e armazena na propriedade Races.
        Races = await _context.Races
            .OrderBy(r => r.Date)
            .ToListAsync();
    }
}
