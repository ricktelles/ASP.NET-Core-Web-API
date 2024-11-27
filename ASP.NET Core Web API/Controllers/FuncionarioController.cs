using ASP.NET_Core_Web_API.Models;
using ASP.NET_Core_Web_API.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ASP.NET_Core_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuncionariosController : ControllerBase
    {
        private readonly FuncionarioRepository _funcionarioRepository; // Repositório de Funcionários

        public FuncionariosController(FuncionarioRepository funcionarioRepository)
        {
            _funcionarioRepository = funcionarioRepository;
        }

        [HttpPost("registrar")]
        public IActionResult RegistrarFuncionario([FromBody] Funcionario funcionario)
        {
            if (funcionario == null || string.IsNullOrEmpty(funcionario.EmailFuncionario) || string.IsNullOrEmpty(funcionario.SenhaFuncionario))
            {
                return BadRequest("Email e senha são obrigatórios.");
            }

            _funcionarioRepository.RegistrarFuncionario(funcionario.EmailFuncionario, funcionario.SenhaFuncionario, funcionario.CargoFuncionario, funcionario.NomeFuncionario);
            return CreatedAtAction(nameof(RegistrarFuncionario), new { email = funcionario.EmailFuncionario }, funcionario);
        }


        // Método HTTP POST para validar funcionário
        [HttpPost("validar")]
        public IActionResult ValidarFuncionario([FromBody] LoginModel loginModel)
        {
            if (loginModel == null ||
                string.IsNullOrEmpty(loginModel.EmailFuncionario) ||
                string.IsNullOrEmpty(loginModel.SenhaFuncionario))
            {
                return BadRequest("Email e senha são obrigatórios.");
            }

            var resultado = _funcionarioRepository.ValidarFuncionario(loginModel.EmailFuncionario, loginModel.SenhaFuncionario);
            if (resultado.isValid)
            {
                return Ok(resultado.message);
            }
            return Unauthorized("Credenciais inválidas");
        }
    }
}
