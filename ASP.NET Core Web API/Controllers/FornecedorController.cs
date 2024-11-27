using ASP.NET_Core_Web_API.Models;
using ASP.NET_Core_Web_API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Core_Web_API.Controllers
{
    // Define a rota base do controlador para a API, que será "api/Fornecedor"
    [Route("api/[controller]")]
    [ApiController]
    public class FornecedorController : ControllerBase
    {
        // Declara uma instância do repositório de fornecedores para acesso aos dados
        private readonly FornecedorRepository _fornecedorRepository;

        // Construtor do controlador que injeta o repositório de fornecedores
        public FornecedorController(FornecedorRepository fornecedorRepository)
        {
            _fornecedorRepository = fornecedorRepository;
        }

        // Método HTTP GET para retornar todos os fornecedores
        [HttpGet]
        public ActionResult<IEnumerable<Fornecedor>> GetFornecedores()
        {
            var fornecedores = _fornecedorRepository.GetFornecedores();
            return Ok(fornecedores);
        }

        // Método HTTP GET para retornar um fornecedor específico pelo ID
        [HttpGet("{id}")]
        public ActionResult<Fornecedor> GetFornecedor(int id)
        {
            var fornecedor = _fornecedorRepository.GetFornecedorById(id);
            // Verifica se o fornecedor existe
            if (fornecedor == null)
            {
                return NotFound();
            }
            return Ok(fornecedor);
        }

        // Método HTTP POST para adicionar um novo fornecedor
        [HttpPost]
        public IActionResult AddFornecedor(Fornecedor fornecedor)
        {
            // Adiciona o fornecedor no repositório
            _fornecedorRepository.AddFornecedor(fornecedor);
            // Retorna a resposta com o local do recurso criado
            return CreatedAtAction(nameof(GetFornecedor), new { id = fornecedor.idFornecedor }, fornecedor);
        }

        // Método HTTP PUT para atualizar um fornecedor existente
        [HttpPut("{id}")]
        public IActionResult UpdateFornecedor(int id, Fornecedor fornecedor)
        {
            // Loga os detalhes da requisição para fins de auditoria
            Console.WriteLine($"Recebendo requisição PUT para atualizar fornecedor com ID: {id}");
            Console.WriteLine($"Dados recebidos: NomeEmpresa = {fornecedor.NomeEmpresa}, Email = {fornecedor.Email}, Telefone = {fornecedor.Telefone}, Endereço = {fornecedor.Endereco}, TipoProduto = {fornecedor.tipoProduto}");

            // Verifica se o ID fornecido na URL é o mesmo do fornecedor passado no corpo da requisição
            if (id != fornecedor.idFornecedor)
            {
                return BadRequest();
            }

            // Loga a validação de ID bem-sucedida
            Console.WriteLine("Validação de ID bem-sucedida, atualizando fornecedor...");

            // Atualiza o fornecedor no repositório
            _fornecedorRepository.UpdateFornecedor(fornecedor);

            // Loga o sucesso da atualização
            Console.WriteLine($"Fornecedor com ID: {id} atualizado com sucesso!");

            return Ok(fornecedor);
        }

        // Método HTTP DELETE para excluir um fornecedor existente pelo ID
        [HttpDelete("{id}")]
        public IActionResult DeleteFornecedor(int id)
        {
            // Remove o fornecedor do repositório
            _fornecedorRepository.DeleteFornecedor(id);
            // Retorna NoContent para indicar que a exclusão foi bem-sucedida
            return NoContent();
        }
    }
}
