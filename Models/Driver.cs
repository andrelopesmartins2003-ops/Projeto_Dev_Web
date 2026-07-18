using System.ComponentModel.DataAnnotations;

namespace Projeto.Models;

public class Driver
{   
    //Validação do nome. idade e nacionalidade e dos pilotos inseridos 

    // Chave primária da entidade.
    public int Id { get; set; }

    //Nome do piloto.
    [Required(ErrorMessage = "O nome do piloto é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome do piloto deve ter entre 2 e 100 caracteres.")]
    public string Name { get; set; } = "";

    // Idade do piloto.
    [Range(16, 80, ErrorMessage = "A idade deve estar entre 16 e 80.")]
    public int Age { get; set; }

    // Nacionalidade do piloto.
    [Required(ErrorMessage = "A nacionalidade é obrigatória.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "A nacionalidade deve ter entre 2 e 60 caracteres.")]
    public string? Nationality { get; set; }

    // Chave estrangeira da equipa associada.
    [Range(1, int.MaxValue, ErrorMessage = "Deve selecionar uma equipa válida.")]
    public int TeamId { get; set; }
    
    // Propriedade de navegação para a equipa.
    public Team? Team { get; set; }

    // Relação muitos-para-muitos entre pilotos e corridas.
    public ICollection<RaceDriver> RaceDrivers { get; set; } = new List<RaceDriver>();
}