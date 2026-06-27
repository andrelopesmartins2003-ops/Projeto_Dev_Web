using Microsoft.AspNetCore.Mvc; // Importa funcionalidades para criar controllers e devolver respostas HTTP, como Ok(), NotFound() e BadRequest().
using Microsoft.EntityFrameworkCore; // Permite usar métodos assíncronos e funcionalidades do Entity Framework Core, como Include(), ToListAsync() e EntityState.
using Projeto.Data; // Dá acesso ao ApplicationDbContext, que representa a ligação à base de dados da aplicação.
using Projeto.Models; // Dá acesso aos modelos da aplicação, neste caso principalmente o modelo Driver.

namespace Projeto.Controllers; // Define o namespace onde este controller está organizado dentro do projeto.

[Route("api/drivers")] // Define a rota base deste controller; todos os endpoints começam por /api/drivers.
[ApiController] // Indica que esta classe é um controller de API, ativando validações e comportamentos automáticos do ASP.NET Core.
public class DriversApiController : ControllerBase // Controller responsável por disponibilizar operações CRUD para pilotos através da API.
{
    private readonly ApplicationDbContext _context; // Guarda a instância do contexto da base de dados usada em todos os métodos do controller.

    public DriversApiController(ApplicationDbContext context) // Construtor que recebe o contexto por injeção de dependências.
    {
        _context = context; // Atribui o contexto recebido à variável privada para poder consultar e alterar dados.
    }

    [HttpGet] // Associa este método a pedidos HTTP GET feitos para /api/drivers.
    public async Task<IActionResult> GetDrivers() // Método assíncrono que devolve a lista de todos os pilotos.
    {
        var drivers = await _context.Drivers // Acede à tabela/conjunto de pilotos na base de dados.
            .Include(d => d.Team) // Inclui a equipa associada a cada piloto, evitando que o nome da equipa venha vazio.
            .Select(d => new // Cria um objeto anónimo com apenas os campos que devem ser devolvidos pela API.
            {
                d.Id, // Inclui o identificador único do piloto.
                d.Name, // Inclui o nome do piloto.
                d.Age, // Inclui a idade do piloto.
                d.Nationality, // Inclui a nacionalidade do piloto.
                d.TeamId, // Inclui o identificador da equipa à qual o piloto pertence.
                TeamName = d.Team != null ? d.Team.Name : null // Mostra o nome da equipa, caso exista equipa associada.
            })
            .ToListAsync(); // Executa a consulta de forma assíncrona e converte o resultado numa lista.

        return Ok(drivers); // Devolve HTTP 200 OK com a lista de pilotos em formato JSON.
    }

    [HttpGet("{id}")] // Associa este método a pedidos GET para /api/drivers/{id}, onde id identifica um piloto.
    public async Task<IActionResult> GetDriver(int id) // Método assíncrono que devolve um piloto específico.
    {
        var driver = await _context.Drivers // Acede à coleção de pilotos.
            .Include(d => d.Team) // Inclui os dados da equipa relacionada com o piloto.
            .Where(d => d.Id == id) // Filtra apenas o piloto cujo Id corresponde ao parâmetro recebido na rota.
            .Select(d => new // Define quais os dados do piloto que serão enviados na resposta.
            {
                d.Id, // Inclui o Id do piloto.
                d.Name, // Inclui o nome do piloto.
                d.Age, // Inclui a idade do piloto.
                d.Nationality, // Inclui a nacionalidade do piloto.
                d.TeamId, // Inclui o Id da equipa do piloto.
                TeamName = d.Team != null ? d.Team.Name : null // Inclui o nome da equipa se existir relação com uma equipa.
            })
            .FirstOrDefaultAsync(); // Executa a consulta e devolve o primeiro resultado, ou null se não existir.

        if (driver == null) // Verifica se não foi encontrado nenhum piloto com o Id indicado.
        {
            return NotFound(); // Devolve HTTP 404 Not Found quando o piloto não existe.
        }

        return Ok(driver); // Devolve HTTP 200 OK com os dados do piloto encontrado.
    }

    [HttpPost] // Associa este método a pedidos HTTP POST para criar um novo piloto.
    public async Task<ActionResult<Driver>> PostDriver(Driver driver) // Recebe um objeto Driver enviado no corpo do pedido.
    {
        _context.Drivers.Add(driver); // Adiciona o novo piloto ao contexto para ser inserido na base de dados.
        await _context.SaveChangesAsync(); // Guarda efetivamente a alteração na base de dados.

        return CreatedAtAction(nameof(GetDriver), new { id = driver.Id }, driver); // Devolve HTTP 201 Created e indica como consultar o piloto criado.
    }

    [HttpPut("{id}")] // Associa este método a pedidos HTTP PUT para /api/drivers/{id}, usados para atualizar um piloto.
    public async Task<IActionResult> PutDriver(int id, Driver driver) // Recebe o Id da rota e os novos dados do piloto no corpo do pedido.
    {
        if (id != driver.Id) // Confirma se o Id da rota corresponde ao Id do objeto recebido.
        {
            return BadRequest(); // Devolve HTTP 400 se houver inconsistência entre os Ids.
        }

        _context.Entry(driver).State = EntityState.Modified; // Marca o objeto como modificado para o Entity Framework atualizar os seus campos.
        await _context.SaveChangesAsync(); // Guarda a atualização na base de dados.

        return NoContent(); // Devolve HTTP 204 No Content, indicando que a atualização foi feita sem devolver dados.
    }

    [HttpDelete("{id}")] // Associa este método a pedidos HTTP DELETE para /api/drivers/{id}.
    public async Task<IActionResult> DeleteDriver(int id) // Método assíncrono que elimina um piloto pelo seu Id.
    {
        var driver = await _context.Drivers.FindAsync(id); // Procura rapidamente o piloto pela chave primária.

        if (driver == null) // Verifica se o piloto não existe.
        {
            return NotFound(); // Devolve HTTP 404 se não for encontrado.
        }

        _context.Drivers.Remove(driver); // Marca o piloto para remoção da base de dados.
        await _context.SaveChangesAsync(); // Confirma a remoção na base de dados.

        return NoContent(); // Devolve HTTP 204 para indicar que a eliminação foi concluída.
    }
}
