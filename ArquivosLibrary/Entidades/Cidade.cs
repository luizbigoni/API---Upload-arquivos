using System.ComponentModel.DataAnnotations;

namespace ArquivosLibrary.Entidades
{
    public class Cidade
    {
        [Key]
        public int CidadeId { get; set; }
        public string? Nome { get; set; }
        public string? Sigla { get; set; }
        public int? IBGEMunicipio { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}   