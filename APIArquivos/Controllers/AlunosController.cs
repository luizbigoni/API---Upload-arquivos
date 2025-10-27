
using APIArquivos.DTOs.Alunos;
using ArquivosLibrary.Entidades;
using ArquivosLibrary.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIArquivos.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]

    public class AlunosController : ControllerBase
    {

        private readonly AlunosService _alunosService;

        public AlunosController(AlunosService alunosService)
        {
            _alunosService = alunosService;
        }


        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AdicionarAluno([FromBody] DTOs.Alunos.AlunoCriarRequest request)
        {
            try
            {
                Aluno aluno = new Aluno
                {
                    Nome = request.Nome
                };
                var result = await _alunosService.AdicionarAlunoAsync(aluno);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExcluirAluno(int id)
        {
            try
            {
                var result = await _alunosService.ExcluirAlunoAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterAlunoPorId(int id)
        {
            try
            {
                var aluno = await _alunosService.ObterAlunoPorIdAsync(id);
                if (aluno == null)
                    return NotFound("Aluno não encontrado.");

                AlunoObterResponse alunoResponse = new()
                {
                    Id = aluno.Id,
                    Nome = aluno.Nome,
                    RA = aluno.RA
                };

                return Ok(alunoResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterTodosAlunos()
        {
            try
            {
                var alunos = await _alunosService.ObterTodosAlunosAsync();


                List<AlunoObterResponse> alunosResponse = new List<AlunoObterResponse>();

                foreach (var aluno in alunos)
                {
                    alunosResponse.Add(new AlunoObterResponse
                    {
                        Id = aluno.Id,
                        Nome = aluno.Nome,
                        RA = aluno.RA
                    });
                }


                return Ok(alunosResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }


        [HttpPost("{id}/foto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AdicionarFoto(int id, IFormFile foto)
        {
            if (foto == null || foto.Length == 0)
            {
                return BadRequest("Nenhum arquivo de foto enviado.");
            }

            try
            {
                // Converte IFormFile para byte[]
                byte[] fotoBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await foto.CopyToAsync(memoryStream);
                    fotoBytes = memoryStream.ToArray();
                }

                var sucesso = await _alunosService.SalvarFotoAlunoAsync(id, fotoBytes);
                if (!sucesso)
                {
                    return NotFound("Aluno não encontrado para associar a foto.");
                }

                return Ok("Foto do aluno salva com sucesso.");
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpGet("{id}/foto")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterFoto(int id)
        {
            try
            {
                var fotoBase64 = await _alunosService.ObterFotoBase64Async(id);

                if (string.IsNullOrEmpty(fotoBase64))
                {
                    return NotFound("Aluno ou foto não encontrados.");
                }

                // Retorna a string Base64 diretamente ou dentro de um objeto JSON
                return Ok(new { fotoBase64 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }


    }
}
