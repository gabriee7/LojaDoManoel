using BoxOptimizerMicroservice.Entities;
using BoxOptimizerMicroservice.Services.OtimizadorEmbalagem.Dtos;

namespace BoxOptimizerMicroservice.Services.EstrategiaOtimizacaoEmbalagem
{
    public interface IEstrategiaDeEmpacotamento
    {
        List<ResponseEmbalagemDto> OrganizarProdutosEmCaixas(
            RequestPedidoDto pedidoEntrada,  
            List<Caixa> caixasDisponiveis);
    }
}
