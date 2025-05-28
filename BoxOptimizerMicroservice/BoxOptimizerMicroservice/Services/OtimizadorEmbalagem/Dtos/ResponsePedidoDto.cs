namespace BoxOptimizerMicroservice.Services.OtimizadorEmbalagem.Dtos
{
    public class ResponsePedidoDto
    {
        public int PedidoId { get; set; }
        public List<ResponseEmbalagemDto> Caixas { get; set; } = new List<ResponseEmbalagemDto>();
    }
}
