using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BoxOptimizerMicroservice.Services.OtimizadorEmbalagem.Dtos
{
    public class RequestProdutoDto
    {
        [JsonPropertyName("produto_id")]
        [Required(ErrorMessage = "O ID do produto é obrigatório.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O ID do produto deve ter entre 1 e 100 caracteres.")]
        public string ProdutoId { get; set; }

        [JsonPropertyName("dimensoes")]
        [Required(ErrorMessage = "As dimensões do produto são obrigatórias.")]
        public RequestDimensaoDto Dimensoes { get; set; }
    }
}
