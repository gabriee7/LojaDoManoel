using System.Text.Json.Serialization;

namespace BoxOptimizerMicroservice.Services.OtimizadorEmbalagem.Dtos
{
    public class RequestDimensaoDto
    {
        [JsonPropertyName("altura")]
        public int Altura { get; set; }
        [JsonPropertyName("largura")]
        public int Largura { get; set; }
        [JsonPropertyName("comprimento")]
        public int Comprimento { get; set; }

        [JsonIgnore]
        public int Volume => Altura * Largura * Comprimento;
    }
}
