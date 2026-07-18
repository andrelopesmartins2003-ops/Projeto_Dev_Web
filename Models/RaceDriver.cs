using System.ComponentModel.DataAnnotations;

namespace Projeto.Models;

public class RaceDriver
{
    //Relação entre pilotos e corrida onde colocamos a posição

    // Chave estrangeira da corrida.
    public int RaceId { get; set; }
    public Race? Race { get; set; }

    // Chave estrangeira do piloto.
    public int DriverId { get; set; }
    public Driver? Driver { get; set; }

    //Posição final do piloto
    [Required(ErrorMessage = "A posição é obrigatória.")]
    [RegularExpression(@"^(1|[2-9]|[1-2][0-9]|DNF|DNS|DSQ|NC)$", ErrorMessage = "A posição deve ser um valor entre 1 e 22, DNF, DNS, DSQ ou NC.")]
    public string Position { get; set; } = "";
}