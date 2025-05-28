using BoxOptimizerMicroservice.Entities.Base;

namespace BoxOptimizerMicroservice.Entities
{
    public class EmbalagemResultado : AuditableBaseEntity
    {
        private int _pedidoId;

        private Guid? _tipoCaixaUsadaId;
        private Caixa _tipoCaixaUsada;

        private List<string> _produtosNestaCaixa;
        private string _observacao;

        public EmbalagemResultado() : base()
        {
            _produtosNestaCaixa = new List<string>();
        }

        public EmbalagemResultado(int pedidoId, Caixa tipoCaixaAssociada, List<string> produtos, string observacao = null)
            : this() 
        {
            _pedidoId = pedidoId;
            _produtosNestaCaixa = produtos ?? new List<string>(); 
            _observacao = observacao;

            if (tipoCaixaAssociada != null)
            {
                _tipoCaixaUsada = tipoCaixaAssociada;
                _tipoCaixaUsadaId = tipoCaixaAssociada.GetGuid();
            } else {
                if (string.IsNullOrEmpty(observacao))
                    throw new ArgumentException("A observação é obrigatória quando nenhum tipo de caixa é especificado.", nameof(observacao));
                if (!_produtosNestaCaixa.Any())
                    throw new ArgumentException("A lista de produtos não pode ser vazia quando nenhum tipo de caixa é especificado.", nameof(produtos));
            }
        }

        public int GetPedidoId() => _pedidoId;
        public Guid? GetTipoCaixaUsadaId() => _tipoCaixaUsadaId;
        public Caixa GetTipoCaixaUsada() => _tipoCaixaUsada;
        public List<string> GetProdutosNestaCaixa() => new List<string>(_produtosNestaCaixa);
        public string GetObservacao() => _observacao;

        public void SetObservacao(string obs) => _observacao = obs;
    }
}