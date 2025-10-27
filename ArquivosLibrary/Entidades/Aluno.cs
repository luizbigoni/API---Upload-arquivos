using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArquivosLibrary.Entidades
{
    public class Aluno
    {
        public int Id { get; set; }
        public string RA { get; set; }
        public string Nome { get; set; }
        public byte[]? Foto { get; set; }
    }
}
