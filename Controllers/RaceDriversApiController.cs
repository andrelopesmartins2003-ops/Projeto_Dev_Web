using Microsoft.AspNetCore.Mvc; // Importa classes para criar controllers e respostas HTTP.
using Microsoft.EntityFrameworkCore; // Permite usar Entity Framework Core e métodos assíncronos de consulta/alteração.
using Projeto.Data; // Permite usar o ApplicationDbContext para aceder à base de dados.
using Projeto.Models; // Permite usar o modelo RaceDriver.

namespace Projeto.Controllers; // Define o namespace dos controllers da aplicação.

[Route("api/racedrivers")] // Define a rota base da API para resultados/associações entre corridas e pilotos.
[ApiController] // Ativa funcionalidades próprias de controllers de API, incluindo validação automática de modelos.
public class RaceDriversApiController : ControllerBase // Controller responsável pela relação entre corridas e pilotos.
{
    private readonly ApplicationDbContext _context; // Contexto da base de dados usado para consultar e alterar resultados.

    public RaceDriversApiController(ApplicationDbContext context) // Construtor que recebe o contexto por injeção de dependências.
    {
        _context = context; // Guarda o contexto recebido para ser usado pelos métodos da API.
    }

    [HttpGet] // Responde a pedidos GET para /api/racedrivers.
    public async Task<IActionResult> GetRaceDrivers() // Devolve todos os resultados de pilotos em corridas.
    {
        var raceDrivers = await _context.RaceDrivers // Acede à tabela de associação entre corridas e pilotos.
            .Select(rd => new // Projeta os dados para um formato mais simples e adequado à resposta JSON.
            {
                rd.RaceId, // Inclui o Id da corrida.
                RaceName = rd.Race != null ? rd.Race.GrandPrixName : null, // Inclui o nome da corrida, se a relação existir.
                rd.DriverId, // Inclui o Id do piloto.
                DriverName = rd.Driver != null ? rd.Driver.Name : null, // Inclui o nome do piloto, se a relação existir.
                TeamName = rd.Driver != null && rd.Driver.Team != null ? rd.Driver.Team.Name : null, // Inclui o nome da equipa do piloto, se existir.
                rd.Position // Inclui a posição/resultado do piloto nessa corrida.
            })
            .ToListAsync(); // Executa a consulta de forma assíncrona e transforma o resultado numa lista.

        return Ok(raceDrivers); // Devolve HTTP 200 OK com todos os resultados.
    }

    [HttpGet("{raceId}/{driverId}")] // Define um GET com chave composta: Id da corrida e Id do piloto.
    public async Task<IActionResult> GetRaceDriver(int raceId, int driverId) // Devolve um resultado específico de um piloto numa corrida.
    {
        var raceDriver = await _context.RaceDrivers // Acede aos registos RaceDriver.
            .Where(rd => rd.RaceId == raceId && rd.DriverId == driverId) // Filtra pelo par corrida-piloto, que identifica unicamente o registo.
            .Select(rd => new // Seleciona apenas os campos necessários para a resposta da API.
            {
                rd.RaceId, // Inclui o Id da corrida.
                RaceName = rd.Race != null ? rd.Race.GrandPrixName : null, // Inclui o nome da corrida se estiver disponível.
                rd.DriverId, // Inclui o Id do piloto.
                DriverName = rd.Driver != null ? rd.Driver.Name : null, // Inclui o nome do piloto se estiver disponível.
                TeamName = rd.Driver != null && rd.Driver.Team != null ? rd.Driver.Team.Name : null, // Inclui a equipa do piloto se existir.
                rd.Position // Inclui a posição obtida pelo piloto.
            })
            .FirstOrDefaultAsync(); // Devolve o primeiro resultado encontrado ou null se não existir.

        if (raceDriver == null) // Verifica se não existe associação para aquela corrida e aquele piloto.
        {
            return NotFound(); // Devolve HTTP 404 caso o registo não seja encontrado.
        }

        return Ok(raceDriver); // Devolve HTTP 200 OK com o resultado encontrado.
    }

    [HttpPost] // Associa este método a pedidos POST para criar um novo resultado.
    public async Task<ActionResult<RaceDriver>> PostRaceDriver(RaceDriver raceDriver) // Recebe os dados da associação corrida-piloto.
    {
        var exists = await _context.RaceDrivers.AnyAsync(rd => // Verifica se já existe um registo com a mesma corrida e o mesmo piloto.
            rd.RaceId == raceDriver.RaceId && // Compara o Id da corrida recebida.
            rd.DriverId == raceDriver.DriverId); // Compara o Id do piloto recebido.

        if (exists) // Se já existir, impede a criação duplicada.
        {
            return BadRequest("Este piloto já está associado a esta corrida."); // Devolve erro 400 com mensagem explicativa.
        }

        _context.RaceDrivers.Add(raceDriver); // Adiciona a nova associação ao contexto.
        await _context.SaveChangesAsync(); // Guarda a nova associação na base de dados.

        return CreatedAtAction( // Devolve HTTP 201 Created, indicando que o recurso foi criado.
            nameof(GetRaceDriver), // Indica o método usado para consultar o recurso criado.
            new { raceId = raceDriver.RaceId, driverId = raceDriver.DriverId }, // Envia os parâmetros necessários para localizar o novo registo.
            raceDriver // Inclui o objeto criado na resposta.
        );
    }

    [HttpPut("{raceId}/{driverId}")] // Define endpoint PUT com chave composta para atualizar um resultado.
    public async Task<IActionResult> PutRaceDriver(int raceId, int driverId, RaceDriver raceDriver) // Recebe os Ids da rota e os novos dados no corpo do pedido.
    {
        if (raceId != raceDriver.RaceId || driverId != raceDriver.DriverId) // Confirma se os Ids da rota coincidem com os Ids do objeto recebido.
        {
            return BadRequest(); // Devolve HTTP 400 quando os Ids não correspondem.
        }

        _context.Entry(raceDriver).State = EntityState.Modified; // Marca o registo como modificado para o Entity Framework o atualizar.
        await _context.SaveChangesAsync(); // Guarda a alteração na base de dados.

        return NoContent(); // Devolve HTTP 204, indicando sucesso sem corpo de resposta.
    }

    [HttpDelete("{raceId}/{driverId}")] // Define endpoint DELETE com Id da corrida e Id do piloto.
    public async Task<IActionResult> DeleteRaceDriver(int raceId, int driverId) // Elimina uma associação específica entre corrida e piloto.
    {
        var raceDriver = await _context.RaceDrivers.FindAsync(raceId, driverId); // Procura o registo pela chave composta.

        if (raceDriver == null) // Verifica se o registo não foi encontrado.
        {
            return NotFound(); // Devolve HTTP 404 se não existir.
        }

        _context.RaceDrivers.Remove(raceDriver); // Marca o registo para ser removido.
        await _context.SaveChangesAsync(); // Confirma a eliminação na base de dados.

        return NoContent(); // Devolve HTTP 204 para indicar que a eliminação foi concluída.
    }
}
