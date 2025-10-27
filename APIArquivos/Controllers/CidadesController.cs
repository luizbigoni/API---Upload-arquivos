using APIArquivos.Models;
using ArquivosLibrary.Entidades;
using ArquivosLibrary.Service;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIArquivos.Controllers
{
    [ApiController]
    [Route("api/cidades")]
    [Produces("application/json")]
    public class CidadesController : ControllerBase
    {
        private readonly CidadesService _service;

        public CidadesController(CidadesService service)
        {
            _service = service;
        }


        [HttpPost("importar")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Importar([FromForm] CidadeImportarRequest request)
        {
            // PASSO 1 - VALIDA��O INICIAL: Verifica se o arquivo foi realmente enviado na requisi��o.
            var arquivo = request.Arquivo;
            if (arquivo == null || arquivo.Length == 0)
                return BadRequest("Arquivo n�o enviado.");

            // PASSO 2 - PREPARA��O DAS VARI�VEIS: 
            var cidades = new List<Cidade>(); //Lista para armazenar as cidades v�lidas que ser�o salvas no banco.
            var erros = new List<string>(); //Lista para armazenar as mensagens de erro de cada linha que falhar.
            int numeroLinha = 0;

            // PASSO 3 - CONFIGURA��O DO PARSER E ENCODING: 
            //Configura o CsvHelper para entender que o arquivo n�o tem uma linha de cabe�alho.
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            //Define a codifica��o de texto correta para ler arquivos com acentua��o (gerados no Windows), usado quando tem aspas, null.
            var encoding = Encoding.GetEncoding("iso-8859-1");

            using (var reader = new StreamReader(arquivo.OpenReadStream(), encoding))
            {
                //Inicia a leitura do arquivo, uma linha por vez.
                string? linha;
                while ((linha = await reader.ReadLineAsync()) != null)
                {
                    numeroLinha++;
                    if (string.IsNullOrWhiteSpace(linha)) continue;

                    try
                    {
                        // PASSO 4 - CORRE��O MANUAL DA LINHA
                        if (linha.StartsWith("\"") && linha.EndsWith("\""))
                        {
                            linha = linha.Substring(1, linha.Length - 2).Replace("\"\"", "\"");
                        }

                        // PASSO 5 - PARSING DA LINHA J� CORRIGIDA: StringReader e o CsvParser para transformar a string da linha em um array de campos.
                        using (var stringReader = new StringReader(linha))
                        using (var parser = new CsvParser(stringReader, config))
                        {
                            if (!parser.Read()) continue;
                            var record = parser.Record;
                            if (record == null) continue;

                            // PASSO 6 - VALIDA��O E CRIA��O DO OBJETO
                            int cidadeId = int.Parse(record[0]);
                            if (cidadeId == 0) continue;

                            var cidade = new Cidade
                            {
                                CidadeId = cidadeId,
                                Nome = record[1],
                                Sigla = record[2],
                                IBGEMunicipio = int.TryParse(record[3], out var ibge) ? ibge : (int?)null,
                                Latitude = double.TryParse(record[4], NumberStyles.Any, CultureInfo.InvariantCulture, out var lat) ? lat : (double?)null,
                                Longitude = double.TryParse(record[5], NumberStyles.Any, CultureInfo.InvariantCulture, out var lon) ? lon : (double?)null
                            };
                            cidades.Add(cidade); //Adiciona a cidade v�lida � lista de sucesso.
                        }
                    }
                    catch (Exception ex)
                    {
                        // PASSO 7 - TRATAMENTO DE ERRO POR LINHA
                        erros.Add($"Linha {numeroLinha}: Falha ao processar. Erro: {ex.Message}. Conte�do: {linha}");
                    }
                }
            }

            // PASSO 8 - PERSIST�NCIA NO BANCO DE DADOS: Ap�s ler todo o arquivo, se houver cidades v�lidas na lista, chama o servi�o para salv�-las todas de uma vez no banco.
            if (cidades.Any())
            {
                await _service.AdicionarLoteAsync(cidades);
            }

            // PASSO 9 - RETORNO DA RESPOSTA
            return Ok(new
            {
                Mensagem = "Importa��o finalizada.",
                CidadesImportadasComSucesso = cidades.Count,
                LinhasComErro = erros.Count,
                DetalhesDosErros = erros
            });
        }


        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Cidade>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<Cidade>> GetAll() => await _service.ObterTodasAsync();


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Cidade), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var cidade = await _service.ObterPorIdAsync(id);
            if (cidade == null)
            {
                return NotFound(new { mensagem = "Cidade n�o encontrada" });
            }
            return Ok(cidade);
        }


        [HttpGet("estados")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<string>> GetEstados() => await _service.ObterEstadosAsync();


        [HttpGet("estado/{uf}")]
        [ProducesResponseType(typeof(IEnumerable<Cidade>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByEstado(string uf)
        {
            var cidades = await _service.ObterPorEstadoAsync(uf);
            if (cidades == null || !cidades.Any())
            {
                return NotFound(new { mensagem = "Nenhuma cidade encontrada para o estado informado." });
            }
            return Ok(cidades);
        }
    }
}