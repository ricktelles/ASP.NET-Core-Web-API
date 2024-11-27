using ASP.NET_Core_Web_API.Models;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ASP.NET_Core_Web_API.Repository
{
    public class ProdutoRepository
    {
        private readonly string _connectionString;

        public ProdutoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Método para obter todos os produtos
        // Retorna uma lista de objetos Produto do banco de dados
        public List<Produto> GetProdutos()
        {
            var produtos = new List<Produto>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                // Consulta SQL com o join e seleção explícita de colunas
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT p.IdProduto, p.NomeProduto, p.qtdProduto, p.qualidadeProduto, p.categoriaProduto, p.dataColheita, " +
                    "p.DestinoProduto, f.IdFornecedor, f.NomeEmpresa " +
                    "FROM Produto p " +
                    "INNER JOIN Fornecedor f ON p.idFornecedor = f.idFornecedor", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            produtos.Add(new Produto
                            {
                                IdProduto = (int)reader["IdProduto"],
                                NomeProduto = reader["NomeProduto"].ToString(),
                                qtdProduto = (int)reader["qtdProduto"],
                                qualidadeProduto = reader["qualidadeProduto"].ToString(),
                                categoriaProduto = reader["categoriaProduto"].ToString(),
                                dataColheita = (DateTime)reader["dataColheita"],
                                DestinoProduto = reader["DestinoProduto"].ToString(),
                                idFornecedor = (int)reader["IdFornecedor"],
                                NomeFornecedor = reader["NomeEmpresa"].ToString() // Verificando se o problema persiste
                            });
                        }
                    }
                }
            }

            return produtos;
        }

        // Método para obter um produto específico por ID
        // Retorna o produto e o nome da empresa fornecedora
        public Produto GetProdutoById(int id)
        {
            Produto produto = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                // Consulta SQL que retorna um produto específico e seu fornecedor associado
                using (SqlCommand cmd = new SqlCommand("SELECT p.IdProduto, p.NomeProduto, p.qtdProduto, p.qualidadeProduto, p.categoriaProduto, p.dataColheita, p.DestinoProduto, f.idFornecedor, f.NomeEmpresa " +  
                    "FROM Produto p " +
                    "INNER JOIN Fornecedor f ON p.idFornecedor = f.idFornecedor " +
                     "WHERE p.IdProduto = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Cria um produto a partir dos dados retornados e preenche o nome do fornecedor
                            produto = new Produto
                            {
                                IdProduto = (int)reader["IdProduto"],
                                NomeProduto = reader["NomeProduto"].ToString(),
                                qtdProduto = (int)reader["qtdProduto"],
                                qualidadeProduto = reader["qualidadeProduto"].ToString(),
                                categoriaProduto = reader["categoriaProduto"].ToString(),
                                dataColheita = (DateTime)reader["dataColheita"],
                                DestinoProduto = reader["DestinoProduto"].ToString(),
                                idFornecedor = (int)reader["idFornecedor"],
                                NomeFornecedor = reader["NomeEmpresa"].ToString() // Preenchendo o nome do fornecedor
                            };
                        }
                    }
                }
            }

            return produto;
        }

        // Método para adicionar um novo produto ao banco de dados
        public void AddProduto(Produto produto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                // Comando SQL para inserir um novo produto 
                //Foi adicionado DestinoProduto e @DestinoProduto
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Produto (NomeProduto, qtdProduto, qualidadeProduto, categoriaProduto, dataColheita, IdFornecedor, DestinoProduto) VALUES (@NomeProduto, @qtdProduto, @qualidadeProduto, @categoriaProduto, @dataColheita, @IdFornecedor, @DestinoProduto)", conn))
                {
                    cmd.Parameters.AddWithValue("@NomeProduto", produto.NomeProduto);
                    cmd.Parameters.AddWithValue("@qtdProduto", produto.qtdProduto);
                    cmd.Parameters.AddWithValue("@qualidadeProduto", produto.qualidadeProduto);
                    cmd.Parameters.AddWithValue("@categoriaProduto", produto.categoriaProduto);
                    cmd.Parameters.AddWithValue("@dataColheita", produto.dataColheita);
                    cmd.Parameters.AddWithValue("@IdFornecedor", produto.idFornecedor);  // Chave estrangeira
                    cmd.Parameters.AddWithValue("@DestinoProduto", produto.DestinoProduto);//novo
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Método para atualizar um produto existente no banco de dados
        public void UpdateProduto(Produto produto)
        {
            // Validação da dataColheita, sem alteração da data
            if (produto.dataColheita < new DateTime(1753, 1, 1) || produto.dataColheita > new DateTime(9999, 12, 31))
            {
                throw new ArgumentOutOfRangeException("dataColheita", "A data de colheita está fora do intervalo permitido.");
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("UPDATE Produto SET NomeProduto = @NomeProduto, qtdProduto = @qtdProduto, qualidadeProduto = @qualidadeProduto, categoriaProduto = @categoriaProduto, dataColheita = @dataColheita, IdFornecedor = @IdFornecedor WHERE IdProduto = @IdProduto", conn))
                {
                    cmd.Parameters.AddWithValue("@NomeProduto", produto.NomeProduto);
                    cmd.Parameters.AddWithValue("@qtdProduto", produto.qtdProduto);
                    cmd.Parameters.AddWithValue("@qualidadeProduto", produto.qualidadeProduto);
                    cmd.Parameters.AddWithValue("@categoriaProduto", produto.categoriaProduto);
                    cmd.Parameters.AddWithValue("@dataColheita", produto.dataColheita); // Mantém a data sem alterações
                    cmd.Parameters.AddWithValue("@IdFornecedor", produto.idFornecedor);
                    cmd.Parameters.AddWithValue("@IdProduto", produto.IdProduto);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Método para deletar um produto específico pelo seu ID
        public void DeleteProduto(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                // Comando SQL para deletar um produto baseado no IdProduto
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Produto WHERE IdProduto = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
