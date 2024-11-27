using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ASP.NET_Core_Web_API.Models
{
    public class Fornecedor
    {
        // Identificador único do fornecedor
        public int idFornecedor { get; set; }

        // Nome da empresa fornecedora
        public string NomeEmpresa { get; set; }

        // Email de contato do fornecedor
        public string Email { get; set; }

        // Telefone de contato do fornecedor
        public string Telefone { get; set; }

        // Endereço do fornecedor
        public string Endereco { get; set; }

        // Tipo de produto fornecido (ex: frutas, legumes, etc.)
        public string tipoProduto { get; set; }

        // Lista de produtos associados ao fornecedor
        // Um fornecedor pode fornecer vários produtos
        public List<Produto> Produto { get; set; } = new List<Produto>();
    }
}