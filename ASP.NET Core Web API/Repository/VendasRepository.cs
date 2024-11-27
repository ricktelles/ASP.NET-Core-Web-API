using System.Data.SqlClient;
using System.Collections.Generic;
using ASP.NET_Core_Web_API.Models.DTOs;

namespace ASP.NET_Core_Web_API.Repository
{
    public class VendasRepository
    {
        private readonly string _connectionString;

        // Construtor com injeção de configuração para acessar a string de conexão
        public VendasRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }




        public void RegistrarVenda(VendaRequestDTO vendaRequest)
        {
            if (vendaRequest == null)
                throw new ArgumentNullException(nameof(vendaRequest), "Os dados da venda não podem ser nulos.");

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Verifica a quantidade disponível do produto em estoque
                        using (SqlCommand getProductCmd = new SqlCommand("SELECT qtdProduto FROM Produto WHERE IdProduto = @IdProduto", conn, transaction))
                        {
                            getProductCmd.Parameters.AddWithValue("@IdProduto", vendaRequest.IdProduto);
                            object result = getProductCmd.ExecuteScalar();

                            if (result == null)
                            {
                                throw new Exception("Produto não encontrado.");
                            }

                            int qtdAtual = (int)result;

                            // Verifica se há estoque suficiente
                            if (qtdAtual < vendaRequest.Quantidade)
                            {
                                throw new Exception("Estoque insuficiente para realizar a venda.");
                            }
                        }

                        // 2. Insere a nova venda na tabela de vendas
                        int vendaId;
                        using (SqlCommand insertVendaCmd = new SqlCommand(@"
                INSERT INTO Vendas (dataVenda, metodoPagamento) 
                OUTPUT INSERTED.IdVenda
                VALUES (@dataVenda, @metodoPagamento)", conn, transaction))
                        {
                            insertVendaCmd.Parameters.AddWithValue("@dataVenda", DateTime.Now);
                            insertVendaCmd.Parameters.AddWithValue("@metodoPagamento", vendaRequest.MetodoPagamento);
                            vendaId = (int)insertVendaCmd.ExecuteScalar(); // Obtém o ID da venda recém-criada
                        }

                        // 3. Insere o produto na tabela intermediária VendaProduto
                        using (SqlCommand insertVendaProdutoCmd = new SqlCommand(@"
                INSERT INTO VendaProduto (IdVenda, IdProduto, Quantidade, PrecoUnitario)
                VALUES (@IdVenda, @IdProduto, @Quantidade, @PrecoUnitario)", conn, transaction))
                        {
                            insertVendaProdutoCmd.Parameters.AddWithValue("@IdVenda", vendaId);
                            insertVendaProdutoCmd.Parameters.AddWithValue("@IdProduto", vendaRequest.IdProduto);
                            insertVendaProdutoCmd.Parameters.AddWithValue("@Quantidade", vendaRequest.Quantidade);
                            insertVendaProdutoCmd.Parameters.AddWithValue("@PrecoUnitario", vendaRequest.PrecoUnitario);

                            insertVendaProdutoCmd.ExecuteNonQuery();
                        }

                        // 4. Atualiza a quantidade do produto em estoque
                        using (SqlCommand updateProdutoCmd = new SqlCommand("UPDATE Produto SET qtdProduto = qtdProduto - @Quantidade WHERE IdProduto = @IdProduto", conn, transaction))
                        {
                            updateProdutoCmd.Parameters.AddWithValue("@Quantidade", vendaRequest.Quantidade);
                            updateProdutoCmd.Parameters.AddWithValue("@IdProduto", vendaRequest.IdProduto);

                            updateProdutoCmd.ExecuteNonQuery();
                        }

                        // Confirma a transação em caso de sucesso
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Reverte a transação em caso de erro
                        transaction.Rollback();
                        throw new Exception("Erro ao realizar a venda: " + ex.Message);
                    }
                }
            }
        }
        public List<RelatorioVendaDTO> GetRelatorioVendas()
        {
            var relatorio = new List<RelatorioVendaDTO>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
            SELECT 
                v.IdVenda,
                p.NomeProduto,
                vp.Quantidade,
                vp.PrecoUnitario,
                v.DataVenda,
                v.MetodoPagamento
            FROM Vendas v
            INNER JOIN VendaProduto vp ON v.IdVenda = vp.IdVenda
            INNER JOIN Produto p ON vp.IdProduto = p.IdProduto", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Monta o relatório com as informações específicas solicitadas
                            relatorio.Add(new RelatorioVendaDTO
                            {
                                IdVenda = (int)reader["IdVenda"],
                                NomeProduto = reader["NomeProduto"].ToString(),
                                Quantidade = (int)reader["Quantidade"],
                                Preco = (decimal)reader["PrecoUnitario"],
                                DataVenda = (DateTime)reader["DataVenda"],
                                MetodoPagamento = reader["MetodoPagamento"].ToString()
                            });
                        }
                    }
                }
            }

            return relatorio;
        }


        // Método para deletar uma venda pelo ID
        public void DeleteVenda(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Vendas WHERE IdVenda = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
