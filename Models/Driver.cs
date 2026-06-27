// Importa as anotações de dados utilizadas para validação dos modelos.
using System.ComponentModel.DataAnnotations;

// Namespace onde se encontram as classes que representam as entidades da base de dados.

namespace Projeto.Models;

// Classe que representa a entidade Driver da aplicação.
public class Driver
{
    // Chave primária da entidade.
    public int Id { get; set; }

    // Campo obrigatório. A validação impede que fique vazio.
    [Required(ErrorMessage = "O nome do piloto é obrigatório.")]
    // Define o número máximo de caracteres permitido.
    [StringLength(100)]
    // Nome do piloto/equipa.
    public string Name { get; set; } = "";

    // Limita os valores permitidos para esta propriedade.
    [Range(18, 80, ErrorMessage = "A idade deve estar entre 18 e 80.")]
    // Idade do piloto.
    public int Age { get; set; }

    // Nacionalidade do piloto.
    public string? Nationality { get; set; }

    // Chave estrangeira da equipa associada.
    public int TeamId { get; set; }
    // Propriedade de navegação para a equipa.
    public Team? Team { get; set; }

    // Relação muitos-para-muitos entre pilotos e corridas.
    public ICollection<RaceDriver> RaceDrivers { get; set; } = new List<RaceDriver>();
}