using BoxOptimizerMicroservice.Attributes;
using BoxOptimizerMicroservice.Services.OtimizadorEmbalagem;
using BoxOptimizerMicroservice.Services.OtimizadorEmbalagem.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace BoxOptimizerMicroservice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmbalagemController : ControllerBase
    {
        private readonly IOtimizadorEmbalagemService _otimizadorEmbalagemService;

        public EmbalagemController(IOtimizadorEmbalagemService otimizadorEmbalagemService)
        {
            _otimizadorEmbalagemService = otimizadorEmbalagemService;
        }

        [HttpPost("Otimizar")]
        [AuthApiKey]
        public async Task<ActionResult<ResponsePedidosDto>> OtimizarEmbalagem(
            [FromBody] RequestPedidosDto request) 
        {
            var resultado = await _otimizadorEmbalagemService.OtimizarMultiplosPedidosAsync(request);
            return Ok(resultado);
        }
    }
}