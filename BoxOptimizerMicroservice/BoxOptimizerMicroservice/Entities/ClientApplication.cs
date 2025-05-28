using BoxOptimizerMicroservice.Entities.Base;

namespace BoxOptimizerMicroservice.Entities
{
    public class ClientApplication : AuditableBaseEntity
    {
        private string _nome;
        private string _hashedKey;

        private ClientApplication() : base() { }

        public ClientApplication(string nome, string hashedApiKey) : this()
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome da aplicação cliente não pode ser vazio.", nameof(nome));
            if (string.IsNullOrWhiteSpace(hashedApiKey))
                throw new ArgumentException("O hash da API key não pode ser vazio.", nameof(hashedApiKey));

            _nome = nome;
            _hashedKey = hashedApiKey;
        }

        public string GetNome() => _nome;
        public string GetHashedKey() => _hashedKey;

        public void SetNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome da aplicação cliente não pode ser vazio.", nameof(nome));
            _nome = nome;
        }
    }
}