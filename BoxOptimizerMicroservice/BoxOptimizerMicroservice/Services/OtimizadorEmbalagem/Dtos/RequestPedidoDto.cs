using System.Text.Json.Serialization;

namespace BoxOptimizerMicroservice.Services.OtimizadorEmbalagem.Dtos
{
    public class RequestPedidoDto
    {
        [JsonPropertyName("pedido_id")]
        public int PedidoId { get; set; }
        [JsonPropertyName("produtos")]
        public List<RequestProdutoDto> Produtos { get; set; } = new List<RequestProdutoDto>();
    }
}
