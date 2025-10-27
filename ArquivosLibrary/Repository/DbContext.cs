using MySql.Data.MySqlClient;

namespace ArquivosLibrary.Repository
{
    public class DbContext
    {
        string _strCon;

        public DbContext()
        {
            try
            {
                _strCon = Environment.GetEnvironmentVariable("STRING_CONEXAO");
            }
            catch
            {
                throw new Exception("Não encontrei a variável de ambiente STRING_CONEXAO");
            }
        }

        public async Task<MySqlConnection> GetConnectionAsync()
        {
            var con = new MySqlConnection(_strCon);
            await con.OpenAsync();
            return con;

        }
    }
}
