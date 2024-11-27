using ASP.NET_Core_Web_API.Models;
using ASP.NET_Core_Web_API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Core_Web_API.Controllers
{
    // Define a rota base para este controlador como "api/Produto"
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        // Declara uma instância do repositório de produtos para interagir com o banco de dados
        private readonly ProdutoRepository _produtoRepository;

        // Construtor do controlador, que injeta o repositório de produtos
        public ProdutoController(ProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        // Método HTTP GET para retornar todos os produtos
        [HttpGet]
        public ActionResult<IEnumerable<Produto>> GetProdutos()
        {
            // Obtém a lista de produtos do repositório
            var produtos = _produtoRepository.GetProdutos();
            // Retorna a lista de produtos com status 200 OK
            return Ok(produtos);
        }

        // Método HTTP GET para retornar um produto específico pelo ID
        [HttpGet("{id}")]
        public ActionResult<Produto> GetProduto(int id)
        {
            // Obtém o produto pelo ID do repositório
            var produto = _produtoRepository.GetProdutoById(id);
            // Verifica se o produto existe
            if (produto == null)
            {
                // Retorna 404 NotFound se o produto não for encontrado
                return NotFound();
            }
            // Retorna o produto com status 200 OK
            return Ok(produto);
        }

        // Método HTTP POST para adicionar um novo produto
        [HttpPost]
        public IActionResult AddProduto([FromBody] Produto produto)
        {
            // Valida se o objeto produto não é nulo
            if (produto == null)
            {
                // Retorna 400 BadRequest se o produto for nulo
                return BadRequest("Produto não pode ser nulo.");
            }

            // Adiciona o produto ao repositório
            _produtoRepository.AddProduto(produto);
            // Retorna 201 Created e a URL do novo recurso
            return CreatedAtAction(nameof(GetProduto), new { id = produto.IdProduto }, produto);
        }

        // Método HTTP PUT para atualizar um produto existente
        [HttpPut("{id}")]
        public IActionResult UpdateProduto(int id, Produto produto)
        {
            // Verifica se o ID do produto na URL corresponde ao ID no corpo da requisição
            if (id != produto.IdProduto)
            {
                // Retorna 400 BadRequest se os IDs não coincidirem
                return BadRequest();
            }

            // Atualiza o produto no repositório
            _produtoRepository.UpdateProduto(produto);
            // Retorna 204 NoContent para indicar que a atualização foi bem-sucedida
            return NoContent();
        }

        // Método HTTP DELETE para excluir um produto pelo ID
        [HttpDelete("{id}")]
        public IActionResult DeleteProduto(int id)
        {
            // Remove o produto do repositório pelo ID fornecido
            _produtoRepository.DeleteProduto(id);
            // Retorna 204 NoContent para indicar que a exclusão foi bem-sucedida
            return NoContent();
        }
    }
}
