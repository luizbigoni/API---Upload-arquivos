using MySql.Data.MySqlClient;
using ArquivosLibrary.Entidades;

namespace ArquivosLibrary.Repository
{
    public class AlunosRepository
    {
        private readonly DbContext _context;

        public AlunosRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<Aluno?> ObterPorIdAsync(int alunoId)
        {
            try
            {
                Aluno? aluno = null;
                await using var con = await _context.GetConnectionAsync();
                await using var cmd = con.CreateCommand();
                cmd.CommandText = "select * from Aluno where AlunoId = @id";
                cmd.Parameters.AddWithValue("@id", alunoId);

                await using var dr = await cmd.ExecuteReaderAsync();

                if (await dr.ReadAsync())
                {
                    aluno = new Aluno
                    {
                        Id = (int)dr["AlunoId"],
                        Nome = dr["Nome"].ToString(),
                        RA = dr["RA"].ToString(),
                    };


                    // Lendo a coluna de foto de forma explícita e segura
                    int fotoOrdinal = dr.GetOrdinal("Foto");
                    if (!dr.IsDBNull(fotoOrdinal))
                    {
                        aluno.Foto = dr.GetFieldValue<byte[]>(fotoOrdinal);
                    }
                }
                return aluno;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Aluno>> ObterTodosAsync()
        {
            try
            {
                var alunos = new List<Aluno>();
                await using var con = await _context.GetConnectionAsync();
                await using var cmd = con.CreateCommand();
                cmd.CommandText = "select * from Aluno";

                await using var dr = await cmd.ExecuteReaderAsync();

                // Obtendo o índice da coluna Foto uma vez fora do loop para otimização
                int fotoOrdinal = dr.GetOrdinal("Foto");

                while (await dr.ReadAsync())
                {
                    var aluno = new Aluno
                    {
                        Id = (int)dr["AlunoId"],
                        Nome = dr["Nome"].ToString(),
                        RA = dr["RA"].ToString()
                    };


                    // Lendo a coluna de foto de forma explícita e segura
                    if (!dr.IsDBNull(fotoOrdinal))
                    {
                        aluno.Foto = dr.GetFieldValue<byte[]>(fotoOrdinal);
                    }

                    alunos.Add(aluno);
                }
                return alunos;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<bool> AdicionarAsync(Aluno aluno)
        {
            try
            {
                await using var con = await _context.GetConnectionAsync();
                await using var cmd = con.CreateCommand();
                cmd.CommandText = "insert into Aluno (Nome, RA) values (@Nome, @RA)";
                cmd.Parameters.AddWithValue("@Nome", aluno.Nome);
                cmd.Parameters.AddWithValue("@RA", aluno.RA);
                await cmd.ExecuteNonQueryAsync();
                aluno.Id = (int)cmd.LastInsertedId;
                return true;
            }
            catch (Exception ex) { throw; }
        }

        public async Task<bool> ExcluirAsync(int id)
        {
            try
            {
                await using var con = await _context.GetConnectionAsync();
                await using var cmd = con.CreateCommand();
                cmd.CommandText = "delete from Aluno where AlunoId = @id";
                cmd.Parameters.AddWithValue("@id", id);
                int qtdeLinhas = await cmd.ExecuteNonQueryAsync();
                return qtdeLinhas > 0;
            }
            catch (Exception ex) { throw; }
        }

        public async Task<bool> SalvarFotoAsync(int alunoId, byte[] foto)
        {
            try
            {
                await using var con = await _context.GetConnectionAsync();
                await using var cmd = con.CreateCommand();
                cmd.CommandText = "UPDATE Aluno SET Foto = @foto WHERE AlunoId = @id";
                cmd.Parameters.AddWithValue("@foto", foto);
                cmd.Parameters.AddWithValue("@id", alunoId);
                int qtdeLinhas = await cmd.ExecuteNonQueryAsync();
                return qtdeLinhas > 0;
            }
            catch (Exception) { throw; }
        }

        public async Task<int> ContarAlunosAsync()
        {
            try
            {
                await using var con = await _context.GetConnectionAsync();
                await using var cmd = con.CreateCommand();
                cmd.CommandText = "select count(*) from Aluno";
                var aux = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(aux);
            }
            catch (Exception ex) { throw; }
        }

        public async Task<bool> AdicionarLoteAsync(List<Aluno> alunos)
        {
            try
            {
                await using var con = await _context.GetConnectionAsync();
                await using var cmd = con.CreateCommand();
                cmd.CommandText = "insert into Aluno (Nome, RA) values (@Nome, @RA)";
                var transaction = await con.BeginTransactionAsync();
                try
                {
                    foreach (var aluno in alunos)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@Nome", aluno.Nome);
                        cmd.Parameters.AddWithValue("@RA", aluno.RA);
                        await cmd.ExecuteNonQueryAsync();
                        aluno.Id = (int)cmd.LastInsertedId;
                    }
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
                return true;
            }
            catch (Exception ex) { throw; }
        }
    }
}