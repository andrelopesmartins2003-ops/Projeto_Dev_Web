// Importa a classe base do Entity Framework Core Identity,
// permitindo que a base de dados também suporte utilizadores, roles e autenticação.
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

// Importa as funcionalidades principais do Entity Framework Core,
// como DbContext, DbSet e ModelBuilder.
using Microsoft.EntityFrameworkCore;

// Importa os modelos da aplicação, como Team, Driver, Race e RaceDriver.
using Projeto.Models;

// Define o namespace onde ficam as classes relacionadas com o acesso à base de dados.
namespace Projeto.Data;

// Classe principal de contexto da base de dados da aplicação.
// Herda de IdentityDbContext para incluir automaticamente as tabelas do ASP.NET Identity.
public class ApplicationDbContext : IdentityDbContext
{
    // Construtor que recebe as opções de configuração da base de dados,
    // como a connection string e o provider usado, por exemplo SQLite ou SQL Server.
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        // Envia essas opções para o construtor da classe base IdentityDbContext.
        : base(options)
    {
    }

    // Representa a tabela Teams na base de dados.
    // Permite consultar, inserir, editar e remover equipas.
    public DbSet<Team> Teams => Set<Team>();

    // Representa a tabela Drivers na base de dados.
    // Permite gerir os pilotos da aplicação.
    public DbSet<Driver> Drivers => Set<Driver>();

    // Representa a tabela Races na base de dados.
    // Permite gerir as corridas ou Grandes Prémios.
    public DbSet<Race> Races => Set<Race>();

    // Representa a tabela RaceDrivers na base de dados.
    // Esta tabela liga corridas a pilotos e guarda o resultado/posição de cada piloto numa corrida.
    public DbSet<RaceDriver> RaceDrivers => Set<RaceDriver>();

    // Método usado para configurar manualmente relações e regras do modelo da base de dados.
    // É executado quando o Entity Framework está a construir o modelo das tabelas.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Mantém a configuração padrão do IdentityDbContext,
        // garantindo que as tabelas de utilizadores e roles são criadas corretamente.
        base.OnModelCreating(modelBuilder);

        // Configura a entidade RaceDriver.
        // Como RaceDriver representa uma relação entre Race e Driver,
        // a chave primária é composta por RaceId e DriverId.
        modelBuilder.Entity<RaceDriver>()
            // Define que a combinação RaceId + DriverId identifica unicamente cada resultado.
            .HasKey(rd => new { rd.RaceId, rd.DriverId });

        // Configura a relação entre RaceDriver e Race.
        modelBuilder.Entity<RaceDriver>()
            // Cada RaceDriver está associado a uma única corrida.
            .HasOne(rd => rd.Race)
            // Uma corrida pode ter vários RaceDrivers, ou seja, vários pilotos/resultados.
            .WithMany(r => r.RaceDrivers)
            // Define RaceId como chave estrangeira que liga RaceDriver à tabela Race.
            .HasForeignKey(rd => rd.RaceId);

        // Configura a relação entre RaceDriver e Driver.
        modelBuilder.Entity<RaceDriver>()
            // Cada RaceDriver está associado a um único piloto.
            .HasOne(rd => rd.Driver)
            // Um piloto pode ter vários RaceDrivers, um por cada corrida em que participou.
            .WithMany(d => d.RaceDrivers)
            // Define DriverId como chave estrangeira que liga RaceDriver à tabela Driver.
            .HasForeignKey(rd => rd.DriverId);
    }
}
