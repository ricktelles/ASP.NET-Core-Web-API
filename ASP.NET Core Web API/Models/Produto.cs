using ASP.NET_Core_Web_API.Models;

public class Produto
{
    // Identificador único do produto
    public int IdProduto { get; set; }

    // Nome do produto (ex: maçã, tomate, etc.)
    public string NomeProduto { get; set; }

    // Quantidade do produto disponível em estoque
    public int qtdProduto { get; set; }

    // Qualidade do produto (ex: alta, média, baixa)
    public string qualidadeProduto { get; set; }

    // Categoria do produto (ex: frutas, legumes, cereais)
    public string categoriaProduto { get; set; }

    // Data da colheita do produto
    public DateTime dataColheita { get; set; }

    // Destino do produto (ex: mercado local, exportação)
    public string DestinoProduto { get; set; }

    // Identificador do fornecedor associado ao produto (chave estrangeira)
    public int idFornecedor { get; set; }

    // Nome do fornecedor, facilitando a referência direta ao nome da empresa fornecedora
    public string NomeFornecedor { get; set; } // Campo adicional para o nome do fornecedor

    // Relacionamento 1:N com Vendas
    public List<Vendas> Vendas { get; set; } = new List<Vendas>();
}
