using ArquivosLibrary.Entidades;
using ArquivosLibrary.Repository;

namespace ArquivosLibrary.Service
{
    public class CidadesService
    {
        private readonly CidadesRepository _repo;

        public CidadesService(CidadesRepository repo)
        {
            _repo = repo;
        }

        public Task<bool> AdicionarLoteAsync(List<Cidade> cidades) => _repo.AdicionarLoteAsync(cidades);
        public Task<List<Cidade>> ObterTodasAsync() => _repo.ObterTodasAsync();
        public Task<Cidade?> ObterPorIdAsync(int id) => _repo.ObterPorIdAsync(id);
        public Task<List<string>> ObterEstadosAsync() => _repo.ObterEstadosAsync();
        public Task<List<Cidade>> ObterPorEstadoAsync(string uf) => _repo.ObterPorEstadoAsync(uf);
    }
}