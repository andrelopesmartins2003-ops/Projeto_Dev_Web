using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Projeto.Data;
using Projeto.Models;

namespace Projeto.Pages.Teams;

// Apenas administradores podem apagar equipas.
[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    // Contexto da base de dados, usado para procurar e remover equipas.
    private readonly ApplicationDbContext _context;

    // Recebe o DbContext por injeção de dependências.
    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // Guarda a equipa carregada para ser mostrada na página de confirmação.
    // É nullable porque pode não existir nenhuma equipa com o id recebido.
    [BindProperty]
    public Team? Team { get; set; }

    // Executado ao abrir a página de eliminação. Recebe o id pela rota: /Teams/Delete/1, por exemplo.
    public async Task<IActionResult> OnGetAsync(int id)
    {
        // Procura a equipa na base de dados pela chave primária.
        Team = await _context.Teams.FindAsync(id);

        // Se não existir, devolve erro 404.
        if (Team == null)
        {
            return NotFound();
        }

        // Se existir, mostra a página com os dados da equipa para confirmação.
        return Page();
    }

    // Executado quando o utilizador confirma a eliminação no formulário.
    public async Task<IActionResult> OnPostAsync(int id)
    {
        // Volta a procurar a equipa para garantir que ainda existe antes de apagar.
        var team = await _context.Teams.FindAsync(id);

        // Se a equipa já não existir, devolve 404.
        if (team == null)
        {
            return NotFound();
        }

        // Marca a equipa para remoção.
        _context.Teams.Remove(team);

        // Grava a remoção na base de dados.
        await _context.SaveChangesAsync();

        // Depois de eliminar, volta à listagem de equipas.
        return RedirectToPage("Index");
    }
}
