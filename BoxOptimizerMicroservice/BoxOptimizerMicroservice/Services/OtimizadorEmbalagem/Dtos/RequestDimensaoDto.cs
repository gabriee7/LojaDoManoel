using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BoxOptimizerMicroservice.Services.OtimizadorEmbalagem.Dtos
{
    public class RequestDimensaoDto
    {
        [JsonPropertyName("altura")]
        [Range(1, int.MaxValue, ErrorMessage = "A altura deve ser um valor positivo.")]
        public int Altura { get; set; }

        [JsonPropertyName("largura")]
        [Range(1, int.MaxValue, ErrorMessage = "A largura deve ser um valor positivo.")]
        public int Largura { get; set; }

        [JsonPropertyName("comprimento")]
        [Range(1, int.MaxValue, ErrorMessage = "O comprimento deve ser um valor positivo.")]
        public int Comprimento { get; set; }

        [JsonIgnore]
        public int Volume => Altura * Largura * Comprimento;
    }
}
