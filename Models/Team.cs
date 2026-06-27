// Importa as anotações de dados utilizadas para validação dos modelos.
using System.ComponentModel.DataAnnotations;

// Namespace onde se encontram as classes que representam as entidades da base de dados.

namespace Projeto.Models;

// Classe que representa a entidade Team da aplicação.
public class Team
{
    // Chave primária da entidade.
    public int Id { get; set; }

    // Campo obrigatório. A validação impede que fique vazio.
    [Required(ErrorMessage = "O nome da equipa é obrigatório.")]
    // Define o número máximo de caracteres permitido.
    [StringLength(100)]
    // Nome do piloto/equipa.
    public string Name { get; set; } = "";

    // País de origem da equipa.
    public string? Country { get; set; }

    // Lista de pilotos pertencentes à equipa.
    public ICollection<Driver> Drivers { get; set; } = new List<Driver>();
}