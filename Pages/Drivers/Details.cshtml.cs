// Imports
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Projeto.Data;
using Projeto.Models;

namespace Projeto.Pages.Drivers;

// Classe PageModel responsável pela lógica da página de detalhes de um piloto.
public class DetailsModel : PageModel
{
    // Variaveis para consultar e alterar o contexto e a base de dados.
    private readonly ApplicationDbContext _context;

    // Construtor da página, chamado automaticamente pelo ASP.NET Core através de injeção de dependências.
    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Driver? Driver { get; set; }

    // Método executado quando a página é aberta através de um pedido GET.
    public async Task<IActionResult> OnGetAsync(int id)
    {
        Driver = await _context.Drivers
            // Inclui também a equipa associada ao piloto, evitando mostrar apenas o Id da equipa.
            .Include(d => d.Team)
            // Procura o primeiro piloto com o Id indicado; se não existir, devolve null.
            .FirstOrDefaultAsync(d => d.Id == id);

        // Confirma se o piloto existe antes de continuar, evitando erros ao aceder a dados inexistentes.
        if (Driver == null)
        {
            // Devolve erro 404 quando o piloto pedido não existe.
            return NotFound();
        }

        // Devolve a própria página Razor ao utilizador, mantendo-o no mesmo ecrã.
        return Page();
    }
}
