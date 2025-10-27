using ArquivosLibrary.Entidades;
using ArquivosLibrary.Repository;

namespace ArquivosLibrary.Services
{
    public  class AlunosService
    {
        private readonly AlunosRepository _alunosRepository;


        public AlunosService(AlunosRepository alunosRepository)
        {
            _alunosRepository = alunosRepository;
        }


        public async Task<bool> AdicionarAlunoAsync(Aluno aluno)
        {
            // Exemplo de validação simples
            if (string.IsNullOrWhiteSpace(aluno.Nome))
                throw new ArgumentException("Nome do aluno é obrigatório.");

            aluno.RA = (DateTime.Now.Year.ToString() + (await _alunosRepository.ContarAlunosAsync() + 1)).ToString();

            return await _alunosRepository.AdicionarAsync(aluno);
        }


        public async Task<bool> ExcluirAlunoAsync(int id)
        {
            var aluno = await _alunosRepository.ObterPorIdAsync(id);
            if (aluno == null)
                throw new ArgumentException("Aluno não encontrado.");

            return await _alunosRepository.ExcluirAsync(id);
        }

        public async Task<Aluno?> ObterAlunoPorIdAsync(int alunoId)
        {
            return await _alunosRepository.ObterPorIdAsync(alunoId);
        }

        public async Task<IEnumerable<Aluno>> ObterTodosAlunosAsync()
        {
            return await _alunosRepository.ObterTodosAsync();
        }

        public async Task<int> ContarAlunosAsync()
        {
            return await _alunosRepository.ContarAlunosAsync();
        }

        public async Task<bool> AdicionarLoteAlunosAsync(List<Aluno> alunos)
        {
            // Exemplo de validação em lote
            foreach (var aluno in alunos)
            {
                if (string.IsNullOrWhiteSpace(aluno.Nome))
                    throw new ArgumentException("Todos os alunos devem ter nome preenchido.");
            }

            return await _alunosRepository.AdicionarLoteAsync(alunos);
        }


        // NOVO MÉTODO: Para salvar a foto
        public async Task<bool> SalvarFotoAlunoAsync(int alunoId, byte[] foto)
        {
            var aluno = await _alunosRepository.ObterPorIdAsync(alunoId);
            if (aluno == null)
                throw new ArgumentException("Aluno não encontrado.");

            return await _alunosRepository.SalvarFotoAsync(alunoId, foto);
        }

        // NOVO MÉTODO: Para obter a foto como Base64
        public async Task<string?> ObterFotoBase64Async(int alunoId)
        {
            var aluno = await _alunosRepository.ObterPorIdAsync(alunoId);
            if (aluno?.Foto == null)
            {
                // Retorna null se o aluno ou a foto não existirem
                return null;
            }

            return Convert.ToBase64String(aluno.Foto);
        }

    }
}
