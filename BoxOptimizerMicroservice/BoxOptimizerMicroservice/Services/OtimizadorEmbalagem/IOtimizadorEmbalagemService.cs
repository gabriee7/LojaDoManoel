using BoxOptimizerMicroservice.Services.OtimizadorEmbalagem.Dtos;

namespace BoxOptimizerMicroservice.Services.OtimizadorEmbalagem
{
    public interface IOtimizadorEmbalagemService
    {
        Task<ResponsePedidosDto> OtimizarMultiplosPedidosAsync(RequestPedidosDto request);
    }
}
