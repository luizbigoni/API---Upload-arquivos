using Microsoft.AspNetCore.Http;

namespace APIArquivos.Models
{
    public class CidadeImportarRequest
    {
        public IFormFile Arquivo { get; set; }
    }
}