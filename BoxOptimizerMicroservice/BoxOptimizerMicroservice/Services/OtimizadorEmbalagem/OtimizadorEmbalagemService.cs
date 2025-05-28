using Azure.Core;
using BoxOptimizerMicroservice.Entities;
using BoxOptimizerMicroservice.EntityFramework;
using BoxOptimizerMicroservice.Services.EstrategiaOtimizacaoEmbalagem;
using BoxOptimizerMicroservice.Services.OtimizadorEmbalagem.Dtos;

namespace BoxOptimizerMicroservice.Services.OtimizadorEmbalagem
{
    public class OtimizadorEmbalagemService : IOtimizadorEmbalagemService
    {
        private readonly List<Caixa> _caixasDisponiveisDoBanco; 
        private readonly IEstrategiaDeEmpacotamento _estrategiaDeEmpacotamento;
        private readonly BoxOptimizerDbContext _dbContext;

        public OtimizadorEmbalagemService(
            IEstrategiaDeEmpacotamento estrategiaDeEmpacotamento,
            BoxOptimizerDbContext dbContext)
        {
            _estrategiaDeEmpacotamento = estrategiaDeEmpacotamento;
            _dbContext = dbContext;
            
            _caixasDisponiveisDoBanco = _dbContext.Caixa
                                             .ToList()
                                             .OrderBy(c => c.Volume)
                                             .ToList();
        }

        public async Task<ResponsePedidosDto> OtimizarMultiplosPedidosAsync(RequestPedidosDto request)
        {
            if (request == null || request.Pedidos == null || !request.Pedidos.Any())
                throw new BadHttpRequestException("A requisição não pode ser vazia e deve conter uma lista de pedidos.");

            foreach (var pedido in request.Pedidos)
            {
                if (pedido.Produtos == null)
                    throw new BadHttpRequestException($"O PedidoId {pedido.PedidoId} não contém produtos ou a lista de produtos é inválida.");
            }

            var listaResponsePedidosDto = new List<ResponsePedidoDto>();
            var novasEntidadesEmbalagemResultado = new List<EmbalagemResultado>();

            foreach (var pedidoEntrada in request.Pedidos)
            {
                List<ResponseEmbalagemDto> caixasCalculadasParaEstePedidoDto =
                    _estrategiaDeEmpacotamento.OrganizarProdutosEmCaixas(
                        pedidoEntrada,
                        _caixasDisponiveisDoBanco
                    );

                foreach (var dtoCaixaCalculada in caixasCalculadasParaEstePedidoDto)
                {
                    EmbalagemResultado entidadeResultado;
                    if (dtoCaixaCalculada.CaixaId != null)
                    {
                        Caixa? tipoCaixaUsada = _caixasDisponiveisDoBanco
                                                .FirstOrDefault(c => c.GetNomeDaCaixa() == dtoCaixaCalculada.CaixaId);

                        if (tipoCaixaUsada == null)
                        {

                            entidadeResultado = new EmbalagemResultado(
                                pedidoEntrada.PedidoId,
                                null,
                                dtoCaixaCalculada.Produtos,
                                $"Erro: Tipo de caixa '{dtoCaixaCalculada.CaixaId}' não encontrado."
                            );
                        }
                        else
                        {
                            entidadeResultado = new EmbalagemResultado(
                                pedidoEntrada.PedidoId,
                                tipoCaixaUsada,
                                dtoCaixaCalculada.Produtos,
                                dtoCaixaCalculada.Observacao
                            );
                        }
                    }
                    else 
                    {
                        entidadeResultado = new EmbalagemResultado(
                            pedidoEntrada.PedidoId,
                            null,
                            dtoCaixaCalculada.Produtos,
                            dtoCaixaCalculada.Observacao ?? "Produto não alocado em caixa padrão."
                        );
                    }
                    novasEntidadesEmbalagemResultado.Add(entidadeResultado);
                }

                listaResponsePedidosDto.Add(new ResponsePedidoDto
                {
                    PedidoId = pedidoEntrada.PedidoId,
                    Caixas = caixasCalculadasParaEstePedidoDto.OrderBy(c => c.CaixaId).ToList()
                });
            }

            if (novasEntidadesEmbalagemResultado.Any())
            {
                _dbContext.EmbalagemResultado.AddRange(novasEntidadesEmbalagemResultado);
                await _dbContext.SaveChangesAsync();
            }

            return new ResponsePedidosDto { Pedidos = listaResponsePedidosDto };
        }
    }
}