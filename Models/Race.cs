// Importa as anotações de dados utilizadas para validação dos modelos.
using System.ComponentModel.DataAnnotations;

// Namespace onde se encontram as classes que representam as entidades da base de dados.

namespace Projeto.Models;

// Classe que representa a entidade Race da aplicação.
public class Race
{
    // Chave primária da entidade.
    public int Id { get; set; }

    // Campo obrigatório. A validação impede que fique vazio.
    [Required(ErrorMessage = "O nome do Grande Prémio é obrigatório.")]
    // Define o número máximo de caracteres permitido.
    [StringLength(100)]
    // Nome do Grande Prémio.
    public string GrandPrixName { get; set; } = "";

    // Campo obrigatório. A validação impede que fique vazio.
    [Required(ErrorMessage = "A data da corrida é obrigatória.")]
    // Data em que a corrida é realizada.
    public DateTime Date { get; set; }

    // Nome do circuito.
    public string? Circuit { get; set; }

    // Relação muitos-para-muitos entre pilotos e corridas.
    public ICollection<RaceDriver> RaceDrivers { get; set; } = new List<RaceDriver>();
}