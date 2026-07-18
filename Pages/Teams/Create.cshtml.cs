using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Projeto.Data;
using Projeto.Models;

namespace Projeto.Pages.Teams;

// Apenas utilizadores com role Admin podem aceder a esta página de criação.
[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    // DbContext usado para comunicar com a base de dados através do Entity Framework Core.
    private readonly ApplicationDbContext _context;

    // O DbContext é recebido por injeção de dependências, configurada no Program.cs.
    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // BindProperty permite que os dados do formulário sejam copiados automaticamente para este objeto Team no POST.
    [BindProperty]
    public Team Team { get; set; } = new();

    // Executado quando a página é aberta por GET. Apenas devolve a página com o formulário vazio.
    public IActionResult OnGet()
    {
        return Page();
    }

    // Executado quando o formulário é submetido por POST.
    public async Task<IActionResult> OnPostAsync()
    {
        // Se os dados não respeitarem as validações do modelo, a página volta a ser mostrada com erros.
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Adiciona a nova equipa ao DbSet Teams. Nesta fase ainda não foi gravada na base de dados.
        _context.Teams.Add(Team);

        // Grava efetivamente a nova equipa na base de dados.
        await _context.SaveChangesAsync();

        // Depois de criar, redireciona para a listagem de equipas.
        return RedirectToPage("Index");
    }
}
