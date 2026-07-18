using System.ComponentModel.DataAnnotations;

namespace Projeto.Models;

public class Team
{
    //Validação do nome, país de origem e lista de pilotos da equipa. 

    // Chave primária da entidade.
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome da equipa é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome da equipa deve ter entre 2 e 100 caracteres.")]
    
    // Nome do piloto/equipa.
    public string Name { get; set; } = "";

    // País de origem da equipa.
    [Required(ErrorMessage = "O país de origem é obrigatório.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "O país de origem deve ter entre 2 e 60 caracteres.")]
    public string Country { get; set; } = "";

    // Lista de pilotos pertencentes à equipa.
    public ICollection<Driver> Drivers { get; set; } = new List<Driver>();
}