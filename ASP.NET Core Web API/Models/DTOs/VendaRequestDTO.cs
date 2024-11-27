namespace ASP.NET_Core_Web_API.Models.DTOs
{
    public class VendaRequestDTO
    {
        public int IdProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public string MetodoPagamento { get; set; }
    }
}
