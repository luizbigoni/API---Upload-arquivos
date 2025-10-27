using MySql.Data.MySqlClient;
using ArquivosLibrary.Entidades;

namespace ArquivosLibrary.Repository
{
    public class CidadesRepository
    {
        private readonly DbContext _context;

        public CidadesRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<bool> AdicionarLoteAsync(List<Cidade> cidades)
        {
            try
            {
                await using var con = await _context.GetConnectionAsync();
                await using var cmd = con.CreateCommand();
                cmd.CommandText = "insert into Cidades (CidadeId, Nome, Sigla, IBGEMunicipio, Latitude, Longitude) values (@CidadeId, @Nome, @Sigla, @IBGEMunicipio, @Latitude, @Longitude)";
                var transaction = await con.BeginTransactionAsync();

                try
                {
                    foreach (var cidade in cidades)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@CidadeId", cidade.CidadeId);
                        cmd.Parameters.AddWithValue("@Nome", cidade.Nome);
                        cmd.Parameters.AddWithValue("@Sigla", cidade.Sigla);
                        cmd.Parameters.AddWithValue("@IBGEMunicipio", cidade.IBGEMunicipio);
                        cmd.Parameters.AddWithValue("@Latitude", cidade.Latitude.HasValue ? cidade.Latitude : DBNull.Value);
                        cmd.Parameters.AddWithValue("@Longitude", cidade.Longitude.HasValue ? cidade.Longitude : DBNull.Value);

                        await cmd.ExecuteNonQueryAsync();
                    }
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }

                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Cidade>> ObterTodasAsync()
        {
            var cidades = new List<Cidade>();
            await using var con = await _context.GetConnectionAsync();
            await using var cmd = con.CreateCommand();
            cmd.CommandText = "select * from Cidades";
            await using var dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
            {
                cidades.Add(new Cidade
                {
                    CidadeId = Convert.ToInt32(dr["CidadeId"]),
                    Nome = dr["Nome"].ToString(),
                    Sigla = dr["Sigla"].ToString(),
                    IBGEMunicipio = Convert.ToInt32(dr["IBGEMunicipio"]),
                    Latitude = dr["Latitude"] == DBNull.Value ? null : Convert.ToDouble(dr["Latitude"]),
                    Longitude = dr["Longitude"] == DBNull.Value ? null : Convert.ToDouble(dr["Longitude"])
                });
            }
            return cidades;
        }

        public async Task<Cidade?> ObterPorIdAsync(int id)
        {
            await using var con = await _context.GetConnectionAsync();
            await using var cmd = con.CreateCommand();
            cmd.CommandText = "select * from Cidades where CidadeId = @id";
            cmd.Parameters.AddWithValue("@id", id);

            await using var dr = await cmd.ExecuteReaderAsync();
            if (await dr.ReadAsync())
            {
                return new Cidade
                {
                    CidadeId = Convert.ToInt32(dr["CidadeId"]),
                    Nome = dr["Nome"].ToString(),
                    Sigla = dr["Sigla"].ToString(),
                    IBGEMunicipio = Convert.ToInt32(dr["IBGEMunicipio"]),
                    Latitude = dr["Latitude"] == DBNull.Value ? null : Convert.ToDouble(dr["Latitude"]),
                    Longitude = dr["Longitude"] == DBNull.Value ? null : Convert.ToDouble(dr["Longitude"])
                };
            }
            return null;
        }

        public async Task<List<string>> ObterEstadosAsync()
        {
            var estados = new List<string>();
            await using var con = await _context.GetConnectionAsync();
            await using var cmd = con.CreateCommand();
            cmd.CommandText = "select distinct Sigla from Cidades order by Sigla";
            await using var dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
            {
                estados.Add(dr["Sigla"].ToString());
            }
            return estados;
        }

        public async Task<List<Cidade>> ObterPorEstadoAsync(string uf)
        {
            var cidades = new List<Cidade>();
            await using var con = await _context.GetConnectionAsync();
            await using var cmd = con.CreateCommand();
            cmd.CommandText = "select * from Cidades where Sigla = @uf";
            cmd.Parameters.AddWithValue("@uf", uf);

            await using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                cidades.Add(new Cidade
                {
                    CidadeId = Convert.ToInt32(dr["CidadeId"]),
                    Nome = dr["Nome"].ToString(),
                    Sigla = dr["Sigla"].ToString(),
                    IBGEMunicipio = Convert.ToInt32(dr["IBGEMunicipio"]),
                    Latitude = dr["Latitude"] == DBNull.Value ? null : Convert.ToDouble(dr["Latitude"]),
                    Longitude = dr["Longitude"] == DBNull.Value ? null : Convert.ToDouble(dr["Longitude"])
                });
            }
            return cidades;
        }

        public async Task LimparDadosAsync()
        {
            try
            {
                await using var con = await _context.GetConnectionAsync();
                await using var clearCmd = con.CreateCommand();
                clearCmd.CommandText = "DELETE FROM Cidades";
                await clearCmd.ExecuteNonQueryAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}