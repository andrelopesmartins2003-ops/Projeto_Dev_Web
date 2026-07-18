// Imports
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Projeto.Data;
using Projeto.Models;

namespace Projeto.Pages.Races;

// Classe responsável pela lógica da página Details.cshtml, usada para mostrar os dados de uma corrida.
public class DetailsModel : PageModel
{
    // Variável privada para armazenar o contexto da base de dados.
    private readonly ApplicationDbContext _context;

    // Construtor que recebe o contexto da base de dados por injeção de dependências.
    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // Guarda a corrida encontrada;
    public Race? Race { get; set; }

    // Método executado quando a página de detalhes é aberta com um determinado id.
    public async Task<IActionResult> OnGetAsync(int id)
    {
        // Procura na tabela de corridas a corrida correspondente ao id recebido na rota.
        Race = await _context.Races.FindAsync(id);

        // Verifica se não foi encontrada nenhuma corrida.
        if (Race == null)
        {
            return NotFound();
        }

        return Page();
    }
}
