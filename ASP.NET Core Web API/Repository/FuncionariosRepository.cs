using ASP.NET_Core_Web_API.Models;
using System.Data.SqlClient;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;

namespace ASP.NET_Core_Web_API.Repository
{
    public class FuncionarioRepository
    {
        private readonly string _connectionString;

        public FuncionarioRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void RegistrarFuncionario(string email, string senha, string cargo, string nome)
        {
            string senhaHashed = BCrypt.Net.BCrypt.HashPassword(senha, 11); // hash da senha

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Funcionarios (emailFuncionario, senhaFuncionario, cargoFuncionario, nomeFuncionario) " +
                               "VALUES (@Email, @Senha, @Cargo, @Nome)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Senha", senhaHashed);
                    command.Parameters.AddWithValue("@Cargo", cargo);
                    command.Parameters.AddWithValue("@Nome", nome);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public (bool isValid, string message) ValidarFuncionario(string email, string senha)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT senhaFuncionario FROM Funcionarios WHERE emailFuncionario = @Email";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    connection.Open();
                    object resultado = command.ExecuteScalar();

                    if (resultado != null)
                    {
                        string senhaHashed = resultado.ToString();
                        if (BCrypt.Net.BCrypt.Verify(senha, senhaHashed))
                        {
                            return (true, "Login bem-sucedido");
                        }
                    }
                }
            }

            return (false, "Credenciais inválidas");
        }
    }
}
