using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Projeto.Data;
using Projeto.Models;

namespace Projeto.Pages.Teams;

// Apenas utilizadores administradores podem editar equipas.
[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    // Contexto de base de dados usado para ler e atualizar equipas.
    private readonly ApplicationDbContext _context;

    // Recebe o contexto através da injeção de dependências.
    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // Objeto ligado ao formulário. No GET é preenchido com dados existentes; no POST recebe os dados alterados.
    [BindProperty]
    public Team Team { get; set; } = new();

    // Executado quando a página de edição é aberta.
    public async Task<IActionResult> OnGetAsync(int id)
    {
        // Procura a equipa que corresponde ao id recebido pela rota.
        var team = await _context.Teams.FindAsync(id);

        // Se não existir, devolve 404.
        if (team == null)
        {
            return NotFound();
        }

        // Copia a equipa encontrada para a propriedade usada pela página Razor.
        Team = team;
        return Page();
    }

    // Executado quando o formulário de edição é submetido.
    public async Task<IActionResult> OnPostAsync()
    {
        // Se houver erros de validação, a página é devolvida com as mensagens correspondentes.
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Marca a entidade Team como atualizada no Entity Framework.
        _context.Teams.Update(Team);

        // Grava as alterações na base de dados.
        await _context.SaveChangesAsync();

        // Depois de guardar, volta para a listagem.
        return RedirectToPage("Index");
    }
}
