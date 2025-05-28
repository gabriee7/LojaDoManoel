using System.Text.Json.Serialization;

namespace BoxOptimizerMicroservice.Services.OtimizadorEmbalagem.Dtos
{
    public class RequestProdutoDto
    {
        [JsonPropertyName("produto_id")]
        public string ProdutoId { get; set; }
        [JsonPropertyName("dimensoes")]
        public RequestDimensaoDto Dimensoes { get; set; }
    }
}
