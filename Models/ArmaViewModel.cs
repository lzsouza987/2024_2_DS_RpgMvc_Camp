using System.Collections.Generic;
using RpgMvc.Models.Enuns;

namespace RpgMvc.Models
{
    public class ArmaViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Dano { get; set; }
        public PersonagemViewModel Personagem { get; set; } = new PersonagemViewModel();
        public int PersonagemId { get; set; }
    }
}

