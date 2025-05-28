using BoxOptimizerMicroservice.Entities;
using BoxOptimizerMicroservice.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace BoxOptimizerMicroservice.EntityFramework
{
    public class BoxOptimizerDbContext : DbContext
    {
        public BoxOptimizerDbContext(DbContextOptions<BoxOptimizerDbContext> options) : base(options) { }

        public DbSet<Caixa> Caixa { get; set; }
        public DbSet<EmbalagemResultado> EmbalagemResultado { get; set; }
        public DbSet<ClientApplication> AplicacaoCliente { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var baseEntityTypes = modelBuilder.Model.GetEntityTypes()
                .Where(t => typeof(BaseEntity).IsAssignableFrom(t.ClrType) && t.ClrType != typeof(BaseEntity));
            foreach (var entityType in baseEntityTypes)
            {
                modelBuilder.Entity(entityType.ClrType).Property<Guid>("_id").ValueGeneratedNever();
                modelBuilder.Entity(entityType.ClrType).HasKey("_id");
            }

            var auditableEntityTypes = modelBuilder.Model.GetEntityTypes()
                .Where(t => typeof(AuditableBaseEntity).IsAssignableFrom(t.ClrType) && t.ClrType != typeof(AuditableBaseEntity));
            foreach (var entityType in auditableEntityTypes)
            {
                modelBuilder.Entity(entityType.ClrType).Property<DateTime>("_creationTime").HasColumnName("CreationTime").IsRequired();
                modelBuilder.Entity(entityType.ClrType).Property<DateTime?>("_LastModifiedTime").HasColumnName("LastModifiedTime");
            }

            modelBuilder.Entity<Caixa>(entity =>
            {
                entity.ToTable("Caixa");
                entity.Property<string>("_nomeDaCaixa").HasColumnName("NomeCaixa").IsRequired().HasMaxLength(50);
                entity.HasIndex("_nomeDaCaixa").IsUnique();
                entity.Property<int>("_altura").HasColumnName("Altura").IsRequired();
                entity.Property<int>("_largura").HasColumnName("Largura").IsRequired();
                entity.Property<int>("_comprimento").HasColumnName("Comprimento").IsRequired();
            });

            modelBuilder.Entity<EmbalagemResultado>(entity =>
            {
                entity.ToTable("EmbalagemResultado");
                entity.Property<int>("_pedidoId").HasColumnName("PedidoId").IsRequired();
                entity.Property<Guid?>("_tipoCaixaUsadaId").HasColumnName("TipoCaixaUsadaId");
                entity.HasOne<Caixa>("_tipoCaixaUsada")
                      .WithMany()
                      .HasForeignKey("_tipoCaixaUsadaId")
                      .HasPrincipalKey("_id")
                      .OnDelete(DeleteBehavior.Restrict);
                entity.Property<List<string>>("_produtosNestaCaixa")
                      .HasColumnName("ProdutosNestaCaixa")
                      .IsRequired();
                entity.Property<string>("_observacao").HasColumnName("Observacao").IsRequired(false);
            });

            modelBuilder.Entity<ClientApplication>(entity =>
            {
                entity.ToTable("AplicacaoCliente");

                entity.Property<string>("_nome").HasColumnName("Nome").IsRequired().HasMaxLength(100);
                entity.HasIndex("_nome").IsUnique();

                entity.Property<string>("_hashedKey").HasColumnName("HashedApiKey").IsRequired();
            });

            SeedData.Seed(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            OnBeforeSaving();
            return base.SaveChanges();
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is AuditableBaseEntity &&
                            (e.State == EntityState.Added || e.State == EntityState.Modified));
            foreach (var entityEntry in entries)
            {
                ((AuditableBaseEntity)entityEntry.Entity).setModifiedTime();
            }
        }
    }
}