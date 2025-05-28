using System.Text.Json.Serialization;

namespace BoxOptimizerMicroservice.Services.OtimizadorEmbalagem.Dtos
{
    public class ResponsePedidosDto
    {
        [JsonPropertyName("pedidos")]
        public List<ResponsePedidoDto> Pedidos { get; set; } = new List<ResponsePedidoDto>();
    }
}