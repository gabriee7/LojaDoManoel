using BoxOptimizerMicroservice.Entities;
using BoxOptimizerMicroservice.Services.OtimizadorEmbalagem.Dtos;

namespace BoxOptimizerMicroservice.Services.EstrategiaOtimizacaoEmbalagem
{
    public class CaixaEmUso
    {
        public Caixa TipoCaixaOriginal { get; }
        public List<RequestProdutoDto> ProdutosContidos { get; }
        public int VolumeUtilizado { get; private set; }

        public CaixaEmUso(Caixa tipoCaixa)
        {
            TipoCaixaOriginal = tipoCaixa;
            ProdutosContidos = new List<RequestProdutoDto>();
            VolumeUtilizado = 0;
        }

        public bool PodeAdicionarProduto(RequestProdutoDto produto, EstrategiaEmpacotamentoEco estrategia)
        {
            if (!estrategia.ProdutoCabeNaCaixa(produto.Dimensoes, TipoCaixaOriginal))
                return false;
            if (VolumeUtilizado + produto.Dimensoes.Volume > TipoCaixaOriginal.Volume)
                return false;

            return true;
        }

        public void AdicionarProduto(RequestProdutoDto produto)
        {
            ProdutosContidos.Add(produto);
            VolumeUtilizado += produto.Dimensoes.Volume;
        }

        public ResponseEmbalagemDto ToResponseDto(string observacaoFallback = null)
        {
            var dto = new ResponseEmbalagemDto
            {
                CaixaId = TipoCaixaOriginal.GetNomeDaCaixa(),
                Produtos = ProdutosContidos.Select(p => p.ProdutoId).ToList()
            };

            if (!string.IsNullOrEmpty(observacaoFallback) && ProdutosContidos.Count == 1 && observacaoFallback.Contains("fallback"))
                dto.Observacao = observacaoFallback;

            return dto;
        }
    }
}
