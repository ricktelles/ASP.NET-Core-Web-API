namespace ASP.NET_Core_Web_API.Models.DTOs
{
    public class RelatorioVendaDTO
    {
        public int IdVenda { get; set; }
        public string NomeProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }
        public DateTime DataVenda { get; set; }
        public string MetodoPagamento { get; set; }
    }
}
