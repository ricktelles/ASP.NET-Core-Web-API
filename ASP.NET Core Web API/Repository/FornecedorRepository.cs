using ASP.NET_Core_Web_API.Models;

using System.Collections.Generic;
using System.Data.SqlClient;

namespace ASP.NET_Core_Web_API.Repository
{
    public class FornecedorRepository
    {
        private readonly string _connectionString;

        // Construtor que recebe a configuração do appsettings.json para acessar a string de conexão do banco de dados
        public FornecedorRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Método para obter todos os fornecedores
        // Faz um LEFT JOIN entre a tabela Fornecedor e Produto para retornar fornecedores com ou sem produtos associados
        public List<Fornecedor> GetFornecedores()
        {
            var fornecedores = new List<Fornecedor>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
            SELECT f.IdFornecedor, f.NomeEmpresa, f.Email, f.Telefone, f.Endereco, f.TipoProduto, 
                   p.IdProduto, p.NomeProduto, p.qtdProduto, p.qualidadeProduto, p.categoriaProduto, p.dataColheita, p.DestinoProduto
            FROM Fornecedor f
            LEFT JOIN Produto p ON f.IdFornecedor = p.IdFornecedor", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var fornecedor = fornecedores.FirstOrDefault(f => f.idFornecedor == (int)reader["IdFornecedor"]);
                            if (fornecedor == null)
                            {
                                fornecedor = new Fornecedor
                                {
                                    idFornecedor = (int)reader["IdFornecedor"],
                                    NomeEmpresa = reader["NomeEmpresa"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Telefone = reader["Telefone"].ToString(),
                                    Endereco = reader["Endereco"].ToString(),
                                    tipoProduto = reader["TipoProduto"].ToString(),
                                    Produto = new List<Produto>()
                                };
                                fornecedores.Add(fornecedor);
                            }

                            if (reader["IdProduto"] != DBNull.Value)
                            {
                                fornecedor.Produto.Add(new Produto
                                {
                                    IdProduto = (int)reader["IdProduto"],
                                    NomeProduto = reader["NomeProduto"].ToString(),
                                    qtdProduto = (int)reader["qtdProduto"],
                                    qualidadeProduto = reader["qualidadeProduto"].ToString(),
                                    categoriaProduto = reader["categoriaProduto"].ToString(),
                                    dataColheita = (DateTime)reader["dataColheita"],
                                    DestinoProduto = reader["DestinoProduto"].ToString()
                                });
                            }
                        }
                    }
                }
            }

            return fornecedores;
        }


        // Método para obter um fornecedor por ID
        // Retorna o fornecedor e seus produtos associados, se houver
        public Fornecedor GetFornecedorById(int id)
        {
            Fornecedor fornecedor = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                // Consulta que junta os dados do fornecedor e os produtos associados
                using (SqlCommand cmd = new SqlCommand(@"
            SELECT f.IdFornecedor, f.NomeEmpresa, f.Email, f.Telefone, f.Endereco, f.TipoProduto, 
    p.IdProduto, p.NomeProduto, p.QtdProduto, p.QualidadeProduto, p.CategoriaProduto, p.DataColheita, p.DestinoProduto
    FROM Fornecedor f
    LEFT JOIN Produto p ON f.IdFornecedor = p.IdFornecedor", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Cria um fornecedor a partir dos dados retornados
                            fornecedor = new Fornecedor
                            {
                                idFornecedor = (int)reader["IdFornecedor"],
                                NomeEmpresa = reader["NomeEmpresa"].ToString(),
                                Email = reader["Email"].ToString(),
                                Telefone = reader["Telefone"].ToString(),
                                Endereco = reader["Endereco"].ToString(),
                                tipoProduto = reader["TipoProduto"].ToString(),

                                Produto = new List<Produto>()// Inicializa a lista de produtos
                            };

                            // Adiciona produtos associados, se existirem
                            do
                            {
                                if (reader["IdProduto"] != DBNull.Value)
                                {
                                    fornecedor.Produto.Add(new Produto
                                    {
                                        IdProduto = (int)reader["IdProduto"],
                                        NomeProduto = reader["NomeProduto"].ToString(),
                                        qtdProduto = (int)reader["QtdProduto"],
                                        qualidadeProduto = reader["QualidadeProduto"].ToString(),
                                        categoriaProduto = reader["CategoriaProduto"].ToString(),
                                        dataColheita = (DateTime)reader["DataColheita"],
                                        DestinoProduto = reader["DestinoProduto"]?.ToString() //Adiciona a propriedade DestinoProduto
                                    });
                                };

                            } while (reader.Read());
                        }
                    }
                }
            }

            return fornecedor;
        }


        // Método para adicionar um novo fornecedor ao banco de dados
        public void AddFornecedor(Fornecedor fornecedor)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                // Comando SQL para inserir os dados do fornecedor
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Fornecedor (NomeEmpresa, Email, Telefone, Endereco, TipoProduto) VALUES (@NomeEmpresa, @Email, @Telefone, @Endereco, @TipoProduto)", conn))
                {
                    cmd.Parameters.AddWithValue("@NomeEmpresa", fornecedor.NomeEmpresa);
                    cmd.Parameters.AddWithValue("@Email", fornecedor.Email);
                    cmd.Parameters.AddWithValue("@Telefone", fornecedor.Telefone);
                    cmd.Parameters.AddWithValue("@Endereco", fornecedor.Endereco);
                    cmd.Parameters.AddWithValue("@TipoProduto", fornecedor.tipoProduto);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Método para atualizar as informações de um fornecedor existente
        public void UpdateFornecedor(Fornecedor fornecedor)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                // Comando SQL para atualizar os dados do fornecedor baseado no IdFornecedor
                using (SqlCommand cmd = new SqlCommand("UPDATE Fornecedor SET NomeEmpresa = @NomeEmpresa, Telefone = @Telefone, Endereco = @Endereco, TipoProduto = @TipoProduto WHERE IdFornecedor = @IdFornecedor", conn))
                {
                    cmd.Parameters.AddWithValue("@NomeEmpresa", fornecedor.NomeEmpresa);
                    cmd.Parameters.AddWithValue("@Telefone", fornecedor.Telefone);
                    cmd.Parameters.AddWithValue("@Endereco", fornecedor.Endereco);
                    cmd.Parameters.AddWithValue("@TipoProduto", fornecedor.tipoProduto);
                    cmd.Parameters.AddWithValue("@IdFornecedor", fornecedor.idFornecedor);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Método para deletar um fornecedor com base no seu ID
        public void DeleteFornecedor(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                // Comando SQL para deletar o fornecedor baseado no IdFornecedor
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Fornecedor WHERE IdFornecedor = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
