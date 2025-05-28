using BoxOptimizerMicroservice.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoxOptimizerMicroservice.Entities
{
    public class Caixa : AuditableBaseEntity
    {
        private string _nomeDaCaixa;
        private int _altura;
        private int _largura;
        private int _comprimento;

        [NotMapped] 
        public int Volume => _altura * _largura * _comprimento;

        private Caixa() : base() { }

        public Caixa(string nomeDaCaixa, int altura, int largura, int comprimento) : this()
        {
            _nomeDaCaixa = nomeDaCaixa;
            _altura = altura;
            _largura = largura;
            _comprimento = comprimento;
        }

        public string GetNomeDaCaixa() => _nomeDaCaixa;
        public int GetAltura() => _altura;
        public int GetLargura() => _largura;
        public int GetComprimento() => _comprimento;

        public void SetNomeDaCaixa(String nomeDaCaixa) => _nomeDaCaixa = nomeDaCaixa;
        public void SetAltura(int altura) => _altura = altura;
        public void SetLargura(int largura) => _largura = largura;
        public void SetComprimento(int comprimento) => _comprimento = comprimento;
    }
}