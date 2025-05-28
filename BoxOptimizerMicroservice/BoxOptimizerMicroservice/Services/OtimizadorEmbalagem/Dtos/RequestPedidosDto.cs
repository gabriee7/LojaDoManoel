using System.Text.Json.Serialization;
namespace BoxOptimizerMicroservice.Services.OtimizadorEmbalagem.Dtos
{
    public class RequestPedidosDto
    {
        [JsonPropertyName("pedidos")]
        public List<RequestPedidoDto> Pedidos { get; set; } = new List<RequestPedidoDto>();
    }
}