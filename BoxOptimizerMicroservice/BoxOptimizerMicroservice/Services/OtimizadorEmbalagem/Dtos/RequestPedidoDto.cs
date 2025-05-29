using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BoxOptimizerMicroservice.Services.OtimizadorEmbalagem.Dtos
{
    public class RequestPedidoDto
    {
        [JsonPropertyName("pedido_id")]
        public int PedidoId { get; set; }

        [JsonPropertyName("produtos")]
        [Required(ErrorMessage = "A lista de produtos é obrigatória.")]
        [MinLength(1, ErrorMessage = "O pedido deve conter pelo menos um produto.")]
        public List<RequestProdutoDto> Produtos { get; set; } = new List<RequestProdutoDto>();
    }
}
