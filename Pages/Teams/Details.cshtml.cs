using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Projeto.Data;
using Projeto.Models;

namespace Projeto.Pages.Teams;

// PageModel da página de detalhes de uma equipa.
public class DetailsModel : PageModel
{
    // DbContext usado para aceder à tabela Teams.
    private readonly ApplicationDbContext _context;

    // O contexto é injetado pelo ASP.NET Core.
    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // Equipa que será apresentada na página. Pode ser null se o id não existir.
    public Team? Team { get; set; }

    // Executado quando o utilizador abre a página com um id específico.
    public async Task<IActionResult> OnGetAsync(int id)
    {
        // Procura a equipa pela chave primária.
        Team = await _context.Teams.FindAsync(id);

        // Se não encontrar, devolve 404 para indicar que o recurso não existe.
        if (Team == null)
        {
            return NotFound();
        }

        // Se encontrar, apresenta a página com os detalhes da equipa.
        return Page();
    }
}
