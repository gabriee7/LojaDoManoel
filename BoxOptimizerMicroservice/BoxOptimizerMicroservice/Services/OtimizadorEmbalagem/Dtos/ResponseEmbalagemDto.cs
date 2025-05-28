namespace BoxOptimizerMicroservice.Services.OtimizadorEmbalagem.Dtos
{
    public class ResponseEmbalagemDto
    {
        public string? CaixaId { get; set; }
        public List<string> Produtos { get; set; } = new List<string>();
        public string? Observacao { get; set; }
    }
}
