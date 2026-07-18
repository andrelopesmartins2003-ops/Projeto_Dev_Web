// Imports
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Projeto.Data;
using Projeto.Models;

namespace Projeto.Pages.Races;

// Garante que apenas utilizadores com o papel "Admin" podem editar corridas.
[Authorize(Roles = "Admin")]

// Classe responsável pela lógica da página Edit.cshtml, usada para editar uma corrida existente.
public class EditModel : PageModel
{
    // Variável privada para armazenar o contexto da base de dados.
    private readonly ApplicationDbContext _context;

    // Construtor que recebe o contexto da base de dados por injeção de dependências.
    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // Liga automaticamente os campos submetidos no formulário à propriedade Race.
    [BindProperty]
    public Race Race { get; set; } = new();

    // Método executado quando o utilizador abre a página de edição de uma corrida.
    public async Task<IActionResult> OnGetAsync(int id)
    {
        // Procura na base de dados a corrida correspondente ao id recebido na rota.
        var race = await _context.Races.FindAsync(id);

        // Verifica se não existe corrida com esse id.
        if (race == null)
        {
            return NotFound();
        }

        // Copia a corrida encontrada para a propriedade usada pela página Razor.
        Race = race;

        return Page();
    }

    // Método executado quando o formulário de edição é submetido.
    public async Task<IActionResult> OnPostAsync()
    {
        // Verifica se os dados enviados cumprem as regras de validação do modelo.
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Marca a corrida recebida do formulário como alterada no Entity Framework.
        _context.Races.Update(Race);

        // Guarda as alterações na base de dados de forma assíncrona.
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}
