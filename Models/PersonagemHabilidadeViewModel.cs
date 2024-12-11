using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RpgMvc.Models
{
    public class PersonagemHabilidadeViewModel
    {
        public int PersonagemId { get; set; }        
        public PersonagemViewModel Personagem { get; set; }
        public int HabilidadeId { get; set; }
        public HabilidadeViewModel Habilidade { get; set; }

    }
}