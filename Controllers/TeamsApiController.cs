using Microsoft.AspNetCore.Mvc; 
using Microsoft.EntityFrameworkCore; 
using Projeto.Data; 
using Projeto.Models; 
using Microsoft.AspNetCore.Authorization;
using Projeto.Data;

namespace Projeto.Controllers; // Define o namespace onde este controller está organizado.

[Route("api/teams")] // Define a rota base deste controller: /api/teams.
[ApiController] // Indica que a classe é um controller de API do ASP.NET Core.
[Authorize] // Requer autenticação para aceder aos endpoints deste controller.
public class TeamsApiController : ControllerBase // Controller responsável por gerir equipas através da API.
{
    private readonly ApplicationDbContext _context; // Contexto da base de dados usado para aceder às equipas.

    public TeamsApiController(ApplicationDbContext context) // Construtor que recebe o contexto por injeção de dependências.
    {
        _context = context; // Guarda o contexto recebido para ser usado nos métodos do controller.
    }

    [HttpGet] // Associa este método a pedidos HTTP GET para /api/teams.
    public async Task<ActionResult<IEnumerable<Team>>> GetTeams() // Devolve todas as equipas existentes.
    {
        return await _context.Teams.ToListAsync(); // Consulta a base de dados e devolve a lista de equipas.
    }

    [HttpGet("{id}")] // Associa este método a GET /api/teams/{id}.
    public async Task<ActionResult<Team>> GetTeam(int id) // Devolve uma equipa específica pelo seu Id.
    {
        var team = await _context.Teams.FindAsync(id); // Procura a equipa pela chave primária.

        if (team == null) // Verifica se a equipa não foi encontrada.
        {
            return NotFound(); // Devolve HTTP 404 se não existir equipa com esse Id.
        }

        return team; // Devolve a equipa encontrada.
    }

    [HttpPost] // Associa este método a pedidos POST para criar uma equipa.
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Team>> PostTeam(Team team) // Recebe a nova equipa no corpo do pedido.
    {
        _context.Teams.Add(team); // Adiciona a nova equipa ao contexto.
        await _context.SaveChangesAsync(); // Guarda a equipa na base de dados.

        return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, team); // Devolve HTTP 201 Created com a localização da nova equipa.
    }

    [HttpPut("{id}")] // Associa este método a pedidos PUT para atualizar uma equipa.
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PutTeam(int id, Team team) // Recebe o Id da rota e os dados atualizados da equipa.
    {
        if (id != team.Id) // Verifica se o Id da rota corresponde ao Id da equipa recebida.
        {
            return BadRequest(); // Devolve HTTP 400 se os Ids forem diferentes.
        }

        _context.Entry(team).State = EntityState.Modified; // Marca a equipa como modificada para o Entity Framework a atualizar.
        await _context.SaveChangesAsync(); // Guarda as alterações na base de dados.

        return NoContent(); // Devolve HTTP 204 para indicar atualização concluída sem conteúdo adicional.
    }

    [HttpDelete("{id}")] // Associa este método a pedidos DELETE para /api/teams/{id}.
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTeam(int id) // Elimina uma equipa pelo seu Id.
    {
        var team = await _context.Teams.FindAsync(id); // Procura a equipa pela chave primária.

        if (team == null) // Verifica se a equipa existe antes de tentar remover.
        {
            return NotFound(); // Devolve HTTP 404 se a equipa não for encontrada.
        }

        _context.Teams.Remove(team); // Marca a equipa para remoção.
        await _context.SaveChangesAsync(); // Confirma a remoção na base de dados.

        return NoContent(); // Devolve HTTP 204, indicando que a equipa foi eliminada.
    }
}
