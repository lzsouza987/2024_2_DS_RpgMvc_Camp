using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RpgMvc.Models
{
    public class HabilidadeViewModel
    {
        public int Id { get; set; } 
        public string Nome { get; set; } = string.Empty;
        public int Dano { get; set; }         
        List<PersonagemHabilidadeViewModel> PersonagemHabilidades { get; set; } = new List<PersonagemHabilidadeViewModel>();     
    }
}