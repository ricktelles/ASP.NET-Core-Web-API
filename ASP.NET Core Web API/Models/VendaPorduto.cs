namespace ASP.NET_Core_Web_API.Models
{
    public class VendaProduto
    {
        public int IdVendaProduto { get; set; }  // Identificador único da venda-produto
        public int IdVenda { get; set; }  // Identificador da venda (chave estrangeira)
        public int IdProduto { get; set; }  // Identificador do produto (chave estrangeira)
        public int Quantidade { get; set; }  // Quantidade do produto vendido
        public decimal PrecoUnitario { get; set; }  // Preço unitário do produto na venda
    }
}
