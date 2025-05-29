using BoxOptimizerMicroservice.Entities;
using BoxOptimizerMicroservice.Security;
using Microsoft.EntityFrameworkCore;

namespace BoxOptimizerMicroservice.EntityFramework
{
    public static class SeedData
    {
        private static readonly Guid _caixa1Id = new Guid("E2A8C9F0-7D3B-4A1E-9C6A-08D7E5B4C3F2");
        private static readonly Guid _caixa2Id = new Guid("B5D9E8C1-6A2F-4B0D-8A3E-1F9C8B7A6D5E");
        private static readonly Guid _caixa3Id = new Guid("C9F0B7A2-5E1D-4C9A-B2D8-2A0E1C9F8B7D");

        private static readonly Guid _clientAppId = new Guid("2a8badd0-2a0a-4b79-b51f-9e7a0e2b8c3d");
        public const string _clientAppApiKey = "RzW8qYxXq0MwQzV6bXy0Ek_AbnN9pS-CaYm9pDkSn3w";

        public static void Seed(ModelBuilder modelBuilder)
        {
            SeedCaixas(modelBuilder);
            SeedClientApplications(modelBuilder);
        }

        private static void SeedCaixas(ModelBuilder modelBuilder)
        {
            var seedCreationTime = new DateTime(2025, 5, 27, 12, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<Caixa>(entity =>
            {
                entity.HasData(
                    new
                    {
                        _id = _caixa1Id,
                        _creationTime = seedCreationTime,
                        _LastModifiedTime = (DateTime?)null,
                        _nomeDaCaixa = "Caixa 1",
                        _altura = 30,
                        _largura = 40,
                        _comprimento = 80
                    },
                    new 
                    {
                        _id = _caixa2Id,
                        _creationTime = seedCreationTime,
                        _LastModifiedTime = (DateTime?)null,
                        _nomeDaCaixa = "Caixa 2",
                        _altura = 80,
                        _largura = 50,
                        _comprimento = 40
                    },
                    new
                    {
                        _id = _caixa3Id,
                        _creationTime = seedCreationTime,
                        _LastModifiedTime = (DateTime?)null,
                        _nomeDaCaixa = "Caixa 3",
                        _altura = 50,
                        _largura = 80,
                        _comprimento = 60
                    }
                );
            });
        }

        private static void SeedClientApplications(ModelBuilder modelBuilder)
        {
            var seedCreationTime = new DateTime(2025, 5, 27, 12, 0, 0, DateTimeKind.Utc);

            string clientApp1_HashedKey = ApiKeyManager.HashApiKey(_clientAppApiKey);

            modelBuilder.Entity<ClientApplication>(entity =>
            {
                entity.HasData(
                    new
                    {
                        _id = _clientAppId,
                        _creationTime = seedCreationTime,
                        _LastModifiedTime = (DateTime?)null,
                        _nome = "Aplicação Cliente via Seed",
                        _hashedKey = clientApp1_HashedKey
                    }
                );
            });
        }
    }
}