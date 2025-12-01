using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure;

/// <summary>
/// DbContext da aplicação para SQL Server.
/// Configura as entidades e migrations.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Produto> Produtos { get; set; } = null!;
    public DbSet<MovimentacaoEstoque> Movimentacoes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da entidade Produto
        modelBuilder.Entity<Produto>(entity =>
        {
            // Map to the singular table name used in migrations
            entity.ToTable("Produto");

            entity.HasKey(e => e.Codigo);
            entity.Property(e => e.Codigo).ValueGeneratedNever();
            
            entity.Property(e => e.Descricao)
                .HasMaxLength(255)
                .IsRequired();
            
            entity.Property(e => e.Estoque)
                .HasDefaultValue(0);
            
            entity.Property(e => e.RowVersion)
                .IsRowVersion();

            entity.HasMany(e => e.Movimentacoes)
                .WithOne(m => m.Produto)
                .HasForeignKey(m => m.CodigoProduto)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Configuração da entidade MovimentacaoEstoque
        modelBuilder.Entity<MovimentacaoEstoque>(entity =>
        {
            // Ensure table name matches migration
            entity.ToTable("MovimentacaoEstoque");

            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();
            
            entity.Property(e => e.PublicId)
                .HasDefaultValueSql("NEWSEQUENTIALID()");
            
            entity.Property(e => e.Tipo)
                .HasConversion<string>()
                .HasMaxLength(50);
            
            entity.Property(e => e.Descricao)
                .HasMaxLength(1000);
            
            entity.Property(e => e.DataHora)
                .HasDefaultValueSql("SYSUTCDATETIME()");
            
            entity.HasIndex(e => new { e.CodigoProduto, e.DataHora })
                .HasDatabaseName("IX_Movimentacao_Codigo_DataHoraDesc")
                .IsDescending(false, true);
            
            entity.HasIndex(e => e.PublicId)
                .IsUnique();
        });

        // Seeding de dados iniciais
        SeedInitialData(modelBuilder);
    }

    private static void SeedInitialData(ModelBuilder modelBuilder)
    {
        var produtos = new[]
        {
            new Produto { Codigo = 101, Descricao = "Caneta Azul", Estoque = 150 },
            new Produto { Codigo = 102, Descricao = "Caderno Universitário", Estoque = 75 },
            new Produto { Codigo = 103, Descricao = "Borracha Branca", Estoque = 200 },
            new Produto { Codigo = 104, Descricao = "Lápis Preto HB", Estoque = 320 },
            new Produto { Codigo = 105, Descricao = "Marcador de Texto Amarelo", Estoque = 90 }
        };

        modelBuilder.Entity<Produto>().HasData(produtos);
    }
}
