// Imports
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Projeto.Data;
using Projeto.Models;

namespace Projeto.Pages.RaceDrivers;

// Apenas utilizadores com perfil de Administrador podem aceder.
[Authorize(Roles = "Admin")]

// Classe responsável pela lógica desta Razor Page.
public class DeleteModel : PageModel
{
    // Variável privada para aceder à base de dados.
    private readonly ApplicationDbContext _context;

    // Construtor que recebe o contexto da base de dados como parâmetro.
    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // Propriedade que representa a participação a ser eliminada.    
    [BindProperty]
    public RaceDriver? RaceDriver { get; set; }

    // Executado quando a página é aberta pelo utilizador.
    public async Task<IActionResult> OnGetAsync(int raceId, int driverId)
    {
        // Procura a participação na base de dados com base nos parâmetros fornecidos.
        RaceDriver = await _context.RaceDrivers
            .Include(rd => rd.Race)
            .Include(rd => rd.Driver)
                .ThenInclude(d => d.Team)
            .FirstOrDefaultAsync(rd =>
                rd.RaceId == raceId &&
                rd.DriverId == driverId);

        // Verifica se a participação foi encontrada.
        if (RaceDriver == null)
        {
            return NotFound();
        }

        return Page();
    }

    // Executado quando o formulário de eliminação é submetido.
    public async Task<IActionResult> OnPostAsync()
    {
        if (RaceDriver == null)
        {
            return NotFound();
        }

        
        var raceDriverToDelete = await _context.RaceDrivers 
            .FirstOrDefaultAsync(rd =>
                rd.RaceId == RaceDriver.RaceId &&
                rd.DriverId == RaceDriver.DriverId);

        if (raceDriverToDelete == null)
        {
            return NotFound();
        }

        // Remove o registo da base de dados e guarda as alterações na base de dados.
        _context.RaceDrivers.Remove(raceDriverToDelete);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}