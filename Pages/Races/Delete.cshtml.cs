// Imports
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Projeto.Data;
using Projeto.Models;

namespace Projeto.Pages.Races;

// Restringe o acesso desta página a utilizadores com o papel "Admin".
[Authorize(Roles = "Admin")]

// Classe responsável pela lógica da página Delete.cshtml, usada para confirmar e eliminar uma corrida.
public class DeleteModel : PageModel
{
    // Variável privada para armazenar o contexto da base de dados.
    private readonly ApplicationDbContext _context;

    // Construtor que recebe o contexto da base de dados por injeção de dependências.
    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // Liga os dados da corrida a eliminar à propriedade Race;
    [BindProperty]
    public Race? Race { get; set; }

    // Método executado quando o utilizador abre a página de eliminação de uma corrida.
    public async Task<IActionResult> OnGetAsync(int id)
    {
        Race = await _context.Races.FindAsync(id);

        if (Race == null)
        {
            return NotFound();
        }

        return Page();
    }

    // Método executado quando o utilizador confirma a eliminação através do formulário POST.
    public async Task<IActionResult> OnPostAsync(int id)
    {
        var race = await _context.Races.FindAsync(id);

        if (race == null)
        {
            return NotFound();
        }

        _context.Races.Remove(race);
        await _context.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
