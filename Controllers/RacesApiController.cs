using Microsoft.AspNetCore.Mvc; 
using Microsoft.EntityFrameworkCore; 
using Projeto.Models; 
using Microsoft.AspNetCore.Authorization;
using Projeto.Data;

namespace Projeto.Controllers; // Define o namespace onde o controller está localizado.

[Route("api/races")] // Define a rota base deste controller: /api/races.
[ApiController] // Indica que esta classe funciona como controller de API.
[Authorize] // Requer autenticação para aceder aos endpoints deste controller.
public class RacesApiController : ControllerBase // Controller responsável por gerir corridas através da API.
{
    private readonly ApplicationDbContext _context; // Contexto da base de dados usado para consultar e alterar corridas.

    public RacesApiController(ApplicationDbContext context) // Construtor que recebe o contexto por injeção de dependências.
    {
        _context = context; // Guarda o contexto para ser usado nos métodos seguintes.
    }

    [HttpGet] // Associa este método a pedidos GET para /api/races.
    public async Task<IActionResult> GetRaces() // Método assíncrono que devolve todas as corridas.
    {
        var races = await _context.Races // Acede à tabela/conjunto de corridas.
            .Select(r => new // Cria um objeto anónimo para controlar os dados enviados pela API.
            {
                r.Id, // Inclui o identificador da corrida.
                r.GrandPrixName, // Inclui o nome do Grande Prémio.
                r.Date, // Inclui a data da corrida.
                r.Circuit, // Inclui o nome/local do circuito.
                Participants = r.RaceDrivers.Select(rd => new // Inclui a lista de pilotos associados a esta corrida.
                {
                    rd.DriverId, // Inclui o Id do piloto participante.
                    DriverName = rd.Driver != null ? rd.Driver.Name : null, // Inclui o nome do piloto se a relação estiver disponível.
                    rd.Position // Inclui a posição obtida pelo piloto.
                })
            })
            .ToListAsync(); // Executa a consulta de forma assíncrona e devolve uma lista.

        return Ok(races); // Devolve HTTP 200 OK com a lista de corridas.
    }

    [HttpGet("{id}")] // Associa este método a GET /api/races/{id}.
    public async Task<IActionResult> GetRace(int id) // Devolve uma corrida específica pelo seu Id.
    {
        var race = await _context.Races // Acede às corridas existentes.
            .Where(r => r.Id == id) // Filtra apenas a corrida cujo Id corresponde ao parâmetro recebido.
            .Select(r => new // Define a estrutura de dados que será devolvida ao cliente.
            {
                r.Id, // Inclui o Id da corrida.
                r.GrandPrixName, // Inclui o nome do Grande Prémio.
                r.Date, // Inclui a data da corrida.
                r.Circuit, // Inclui o circuito.
                Participants = r.RaceDrivers.Select(rd => new // Inclui os participantes/resultados associados à corrida.
                {
                    rd.DriverId, // Inclui o Id do piloto.
                    DriverName = rd.Driver != null ? rd.Driver.Name : null, // Inclui o nome do piloto quando disponível.
                    rd.Position // Inclui a posição do piloto nesta corrida.
                })
            })
            .FirstOrDefaultAsync(); // Devolve a primeira corrida encontrada ou null se não existir.

        if (race == null) // Verifica se nenhuma corrida foi encontrada com o Id indicado.
        {
            return NotFound(); // Devolve HTTP 404 Not Found.
        }

        return Ok(race); // Devolve HTTP 200 OK com os dados da corrida.
    }

    [HttpPost] // Associa este método a pedidos POST para criar uma nova corrida.
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Race>> PostRace(Race race) // Recebe uma corrida enviada no corpo do pedido.
    {
        _context.Races.Add(race); // Adiciona a nova corrida ao contexto.
        await _context.SaveChangesAsync(); // Guarda a corrida na base de dados.

        return CreatedAtAction(nameof(GetRace), new { id = race.Id }, race); // Devolve HTTP 201 Created e a rota para consultar a corrida criada.
    }

    [HttpPut("{id}")] // Associa este método a pedidos PUT para atualizar uma corrida existente.
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PutRace(int id, Race race) // Recebe o Id da rota e os novos dados da corrida.
    {
        if (id != race.Id) // Confirma se o Id da rota é igual ao Id do objeto recebido.
        {
            return BadRequest(); // Devolve HTTP 400 quando existe incoerência nos Ids.
        }

        _context.Entry(race).State = EntityState.Modified; // Indica ao Entity Framework que a corrida deve ser atualizada.
        await _context.SaveChangesAsync(); // Guarda as alterações na base de dados.

        return NoContent(); // Devolve HTTP 204 indicando sucesso sem devolver conteúdo.
    }

    [HttpDelete("{id}")] // Associa este método a pedidos DELETE para /api/races/{id}.
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRace(int id) // Remove uma corrida pelo seu Id.
    {
        var race = await _context.Races.FindAsync(id); // Procura a corrida pela chave primária.

        if (race == null) // Verifica se a corrida não existe.
        {
            return NotFound(); // Devolve HTTP 404 se não encontrar a corrida.
        }

        _context.Races.Remove(race); // Marca a corrida para ser eliminada da base de dados.
        await _context.SaveChangesAsync(); // Confirma a eliminação na base de dados.

        return NoContent(); // Devolve HTTP 204 para indicar que a remoção foi feita.
    }
}
