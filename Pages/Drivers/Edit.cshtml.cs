// Imports
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projeto.Data;
using Projeto.Models;

namespace Projeto.Pages.Drivers;

// Restringe o acesso a esta página apenas a utilizadores com o papel Admin.
[Authorize(Roles = "Admin")]

// Classe PageModel responsável pela lógica da página de edição de pilotos.
public class EditModel : PageModel
{
    // Variaveis para consultar e alterar o contexto e a base de dados.
    private readonly ApplicationDbContext _context;

    // Construtor da página, chamado automaticamente pelo ASP.NET Core através de injeção de dependências.
    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // Indica que esta propriedade será automaticamente preenchida com os dados enviados pelo formulário.
    [BindProperty]
    public Driver Driver { get; set; } = new();

    // Guarda a lista de equipas usada para preencher o dropdown do formulário.
    public SelectList TeamOptions { get; set; } = default!;

    // Método executado quando a página é aberta através de um pedido GET.
    public async Task<IActionResult> OnGetAsync(int id)
    {
        // Procura na base de dados um piloto com o identificador recebido na rota.
        var driver = await _context.Drivers.FindAsync(id);

        // Confirma se o piloto existe antes de continuar, evitando erros ao aceder a dados inexistentes.
        if (driver == null)
        {
            return NotFound();
        }

        Driver = driver;
        await LoadTeamsAsync();

        return Page();
    }

    // Método executado quando o formulário é submetido através de um pedido POST.
    public async Task<IActionResult> OnPostAsync()
    {
        // Verifica se os dados recebidos do formulário respeitam as validações definidas no modelo.
        if (!ModelState.IsValid)
        {
            await LoadTeamsAsync();
            return Page();
        }

        // Marca o piloto como alterado para que as modificações e guarda na base de dados.
        _context.Drivers.Update(Driver);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    // Método auxiliar que carrega as equipas da base de dados para o dropdown.
    private async Task LoadTeamsAsync()
    {
        // Inicia uma consulta à tabela de equipas para obter os dados do dropdown.
        var teams = await _context.Teams
            .OrderBy(t => t.Name)
            .ToListAsync();

        // Cria uma lista compatível com o componente select do Razor.
        TeamOptions = new SelectList(teams, "Id", "Name");
    }
}
