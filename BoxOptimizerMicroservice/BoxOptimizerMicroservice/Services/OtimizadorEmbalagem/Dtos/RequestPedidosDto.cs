using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace BoxOptimizerMicroservice.Services.OtimizadorEmbalagem.Dtos
{
    public class RequestPedidosDto
    {
        [JsonPropertyName("pedidos")]
        [Required(ErrorMessage = "A lista de pedidos é obrigatória.")]
        [MinLength(1, ErrorMessage = "A requisição deve conter pelo menos um pedido.")]
        public List<RequestPedidoDto> Pedidos { get; set; } = new List<RequestPedidoDto>();
    }
}