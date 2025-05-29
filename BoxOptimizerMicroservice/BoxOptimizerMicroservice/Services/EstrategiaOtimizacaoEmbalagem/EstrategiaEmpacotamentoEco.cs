using BoxOptimizerMicroservice.Entities;
using BoxOptimizerMicroservice.Services.EstrategiaOtimizacaoEmbalagem;
using BoxOptimizerMicroservice.Services.OtimizadorEmbalagem.Dtos;

namespace BoxOptimizerMicroservice.Services
{
    public class EstrategiaEmpacotamentoEco : IEstrategiaDeEmpacotamento
    {
        public List<ResponseEmbalagemDto> OrganizarProdutosEmCaixas(
            RequestPedidoDto pedidoEntrada,
            List<Caixa> caixasDisponiveis)
        {
            var todasAsEmbalagensFinais = new List<ResponseEmbalagemDto>();

            var (produtosAptosParaEmbalar, embalagensDeItensGrandesDemais) =
                SepararProdutosPorCapacidadeIndividual(pedidoEntrada.Produtos, caixasDisponiveis);

            todasAsEmbalagensFinais.AddRange(embalagensDeItensGrandesDemais);

            if (!produtosAptosParaEmbalar.Any())
                return todasAsEmbalagensFinais;

            var caixasUtilizadas = ProcessarProdutosAptos(produtosAptosParaEmbalar, caixasDisponiveis);
            todasAsEmbalagensFinais.AddRange(caixasUtilizadas.Select(c => c.ToResponseDto()));

            return todasAsEmbalagensFinais;
        }

        private (List<RequestProdutoDto> Aptos, List<ResponseEmbalagemDto> NaoEmbalaveis) SepararProdutosPorCapacidadeIndividual(
            IEnumerable<RequestProdutoDto> todosOsProdutos,
            List<Caixa> caixasDisponiveis)
        {
            var aptos = new List<RequestProdutoDto>();
            var naoEmbalaveisDtos = new List<ResponseEmbalagemDto>();

            foreach (var produto in todosOsProdutos)
            {
                if (caixasDisponiveis.Any(caixa => ProdutoCabeNaCaixa(produto.Dimensoes, caixa)))
                    aptos.Add(produto);
                else
                    naoEmbalaveisDtos.Add(CriarEmbalagemParaProdutoNaoEmbalavel(produto));
            }
            return (aptos, naoEmbalaveisDtos);
        }

        private List<CaixaEmUso> ProcessarProdutosAptos(
            List<RequestProdutoDto> produtosAptos,
            List<Caixa> caixasDisponiveis)
        {
            var produtosOrdenados = produtosAptos.OrderByDescending(p => p.Dimensoes.Volume).ToList();
            var caixasAtivas = new List<CaixaEmUso>();
            var produtosNaoAlocadosNaPrimeiraTentativa = new List<RequestProdutoDto>();

            foreach (var produto in produtosOrdenados)
            {
                if (!TentarAlocarProdutoEmCaixasAtivas(produto, caixasAtivas))
                    if (!TentarAlocarProdutoEmNovaCaixa(produto, caixasAtivas, caixasDisponiveis))
                        produtosNaoAlocadosNaPrimeiraTentativa.Add(produto);
            }

            if (produtosNaoAlocadosNaPrimeiraTentativa.Any())
                AlocarProdutosRestantesComoFallback(produtosNaoAlocadosNaPrimeiraTentativa, caixasAtivas, caixasDisponiveis);

            return caixasAtivas;
        }

        private bool TentarAlocarProdutoEmCaixasAtivas(RequestProdutoDto produto, List<CaixaEmUso> caixasAtivas)
        {
            foreach (var caixaAberta in caixasAtivas)
            {
                if (caixaAberta.PodeAdicionarProduto(produto, this))
                {
                    caixaAberta.AdicionarProduto(produto);
                    return true;
                }
            }
            return false;
        }

        private bool TentarAlocarProdutoEmNovaCaixa(
            RequestProdutoDto produto,
            List<CaixaEmUso> caixasAtivas,
            List<Caixa> caixasDisponiveis)
        {
            Caixa? melhorTipoCaixa = caixasDisponiveis.FirstOrDefault(tipoCaixa => ProdutoCabeNaCaixa(produto.Dimensoes, tipoCaixa));

            if (melhorTipoCaixa != null)
            {
                var novaCaixa = new CaixaEmUso(melhorTipoCaixa);
                novaCaixa.AdicionarProduto(produto);
                caixasAtivas.Add(novaCaixa);
                return true;
            }
            return false;
        }

        private void AlocarProdutosRestantesComoFallback(
            List<RequestProdutoDto> produtosNaoAlocados,
            List<CaixaEmUso> caixasAtivas,
            List<Caixa> caixasDisponiveis)
        {
            foreach (var produto in produtosNaoAlocados)
            {
                Caixa? melhorCaixaIndividual = null;
                foreach (var tipoCaixa in caixasDisponiveis)
                {
                    if (ProdutoCabeNaCaixa(produto.Dimensoes, tipoCaixa))
                    {
                        melhorCaixaIndividual = tipoCaixa;
                        break;
                    }
                }

                if (melhorCaixaIndividual != null)
                {
                    var caixaFallback = new CaixaEmUso(melhorCaixaIndividual);
                    caixaFallback.AdicionarProduto(produto);
                    caixasAtivas.Add(caixaFallback);
                }
            }
        }

        private ResponseEmbalagemDto CriarEmbalagemParaProdutoNaoEmbalavel(RequestProdutoDto produto)
        {
            return new ResponseEmbalagemDto
            {
                CaixaId = null,
                Produtos = new List<string> { produto.ProdutoId },
                Observacao = "Produto não cabe em nenhuma caixa disponível."
            };
        }

        internal bool ProdutoCabeNaCaixa(RequestDimensaoDto produtoDimensoes, Caixa caixa)
        {
            int[] pDims = { produtoDimensoes.Altura, produtoDimensoes.Largura, produtoDimensoes.Comprimento };
            int[][] rotacoes = {
                new int[] { pDims[0], pDims[1], pDims[2] }, new int[] { pDims[0], pDims[2], pDims[1] },
                new int[] { pDims[1], pDims[0], pDims[2] }, new int[] { pDims[1], pDims[2], pDims[0] },
                new int[] { pDims[2], pDims[0], pDims[1] }, new int[] { pDims[2], pDims[1], pDims[0] }
            };

            foreach (var pRot in rotacoes)
            {
                if (pRot[0] <= caixa.GetAltura() && 
                    pRot[1] <= caixa.GetLargura() && 
                    pRot[2] <= caixa.GetComprimento())
                    return true;
            }
            return false;
        }
    }
}