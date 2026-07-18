// Imports
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Projeto.Data;
using Projeto.Models;

namespace Projeto.Pages.Drivers;

// Restringe o acesso a esta página apenas a utilizadores com o papel Admin.
[Authorize(Roles = "Admin")]

// Classe PageModel responsável pela lógica da página de eliminação de pilotos.
public class DeleteModel : PageModel
{
    // Variaveis para consultar e alterar o contexto e a base de dados
    private readonly ApplicationDbContext _context;

    // Construtor da página, chamado automaticamente pelo ASP.NET Core através de injeção de dependências.
    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // Indica que esta propriedade será automaticamente preenchida com os dados enviados pelo formulário.
    [BindProperty]
    public Driver? Driver { get; set; }

    // Método executado quando a página é aberta através de um pedido GET.
    public async Task<IActionResult> OnGetAsync(int id)
    {
        Driver = await _context.Drivers
            // Procura o piloto e a equipa caso são exista devolve null
            .Include(d => d.Team)
            .FirstOrDefaultAsync(d => d.Id == id);

        // Confirma se o piloto existe e dá erro se não existir.
        if (Driver == null)
        {
            return NotFound();
        }

        return Page();
    }

    // Método executado quando o formulário é submetido através de um pedido POST.
    public async Task<IActionResult> OnPostAsync(int id)
    {
        // Procura na base de dados um piloto com o identificador recebido na rota.
        var driver = await _context.Drivers.FindAsync(id);

        // Confirma se o piloto existe
        if (driver == null)
        {
            return NotFound();
        }

        // Remove o piloto do contexto e base de dados
        _context.Drivers.Remove(driver);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}
