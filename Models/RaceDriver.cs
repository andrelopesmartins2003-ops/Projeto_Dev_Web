// Importa as anotações de dados utilizadas para validação dos modelos.
using System.ComponentModel.DataAnnotations;

// Namespace onde se encontram as classes que representam as entidades da base de dados.

namespace Projeto.Models;

// Classe que representa a entidade RaceDriver da aplicação.
public class RaceDriver
{
    // Chave estrangeira da corrida.
    public int RaceId { get; set; }
    // Propriedade de navegação para a corrida.
    public Race? Race { get; set; }

    // Chave estrangeira do piloto.
    public int DriverId { get; set; }
    // Propriedade de navegação para o piloto.
    public Driver? Driver { get; set; }

    // Campo obrigatório. A validação impede que fique vazio.
    [Required(ErrorMessage = "A posição é obrigatória.")]
    // Expressão regular que valida as posições permitidas.
    [RegularExpression(@"^(1|[2-9]|[1-2][0-9]|DNF|DNS|DSQ|NC)$", ErrorMessage = "A posição deve ser um valor entre 1 e 22, DNF, DNS, DSQ ou NC.")]
    // Posição final do piloto na corrida.
    public string Position { get; set; } = "";
}