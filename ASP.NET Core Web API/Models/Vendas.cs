namespace ASP.NET_Core_Web_API.Models
{
    public class Vendas
    {
        public int IdVenda { get; set; }  // Identificador único da venda
        public DateTime DataVenda { get; set; }  // Data da venda
        public string MetodoPagamento { get; set; }  // Método de pagamento
        public List<VendaProduto> Produtos { get; set; } = new List<VendaProduto>();  // Lista de produtos vendidos na venda
    }
}
