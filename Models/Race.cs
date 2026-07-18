using System.ComponentModel.DataAnnotations;

namespace Projeto.Models;

public class Race
{
    //Validação do nome, data e país onde a corrida se situa.

    // Chave primária da entidade.
    public int Id { get; set; }

    //Local do grande prémio
    [Required(ErrorMessage = "O nome do Grande Prémio é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome do Grande Prémio deve ter entre 2 e 100 caracteres.")]
    public string GrandPrixName { get; set; } = "";

    //Data do Grande Prémio
    [Required(ErrorMessage = "A data da corrida é obrigatória.")]
    [DataType(DataType.Date, ErrorMessage = "A data deve ser válida.")]
    public DateTime Date { get; set; }

    //Nome do circuito.
    [Required(ErrorMessage = "O nome do circuito é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome do circuito deve ter entre 2 e 100 caracteres.")]
    public string? Circuit { get; set; }

    // Relação muitos-para-muitos entre pilotos e corridas.
    public ICollection<RaceDriver> RaceDrivers { get; set; } = new List<RaceDriver>();
}