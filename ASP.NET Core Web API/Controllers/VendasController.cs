using ASP.NET_Core_Web_API.Models.DTOs;
using ASP.NET_Core_Web_API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Core_Web_API.Controllers
{
    // Define a rota base para o controlador como "api/Vendas"
    [Route("api/[controller]")]
    [ApiController]
    public class VendasController : ControllerBase
    {
        // Declara uma instância do repositório de vendas para acesso ao banco de dados
        private readonly VendasRepository _vendasRepository;

        // Construtor do controlador com injeção de dependência do repositório de vendas
        public VendasController(VendasRepository vendasRepository)
        {
            _vendasRepository = vendasRepository;
        }
        
        [HttpGet("relatorio")]
        public IActionResult GetRelatorioVendas()
        {
            var relatorio = _vendasRepository.GetRelatorioVendas();
            return Ok(relatorio);
        }

        [HttpPost("vender")]
        public IActionResult VenderProduto([FromBody] VendaRequestDTO vendaRequest)
        {
            try
            {
                if (vendaRequest == null)
                {
                    return BadRequest(new { message = "Os dados da venda são inválidos ou estão ausentes." });
                }

                _vendasRepository.RegistrarVenda(vendaRequest);
                return Ok(new { message = "Venda realizada com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao processar a venda: {ex.Message}" });
            }
        }

    }
}
