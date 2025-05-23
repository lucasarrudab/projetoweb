using ApiPDV.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ApiPDV.Context
{
    public class AppDbContext : IdentityDbContext<AplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<ProdutoCarrinho> ProdutosCarrinho { get; set; }
        public DbSet<Carrinho> Carrinhos { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<MetodoPagamento> MetodosPagamento { get; set; }


        protected override void OnModelCreating(ModelBuilder mb)
        {

            base.OnModelCreating(mb);

           

            mb.Entity<Produto>().HasData(
                new Produto
                {
                    Id = 1,
                    Nome = "Pão Carioca",
                    Codigo = "1234567",
                    Preco = 1.0M,
                    Estoque = 10000
                },
                 new Produto
                 {
                     Id = 2,
                     Nome = "Salgado",
                     Codigo = "7654321",
                     Preco = 5.0M,
                     Estoque = 10000
                 });

            mb.Entity<MetodoPagamento>().HasData(
                new MetodoPagamento
                {
                    Id = 1,
                    Nome = "Dinheiro"
                },
                new MetodoPagamento
                {
                    Id = 2,
                    Nome = "Débito"
                },
                new MetodoPagamento
                {
                    Id = 3,
                    Nome = "Crédito"
                },
                new MetodoPagamento
                {
                    Id = 4,
                    Nome = "PIX"
                });

            

            mb.Entity<Carrinho>()
                .HasMany(c => c.Produtos)
                .WithOne(p => p.Carrinho)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<ProdutoCarrinho>()
    .HasOne(p => p.Carrinho)
    .WithMany(c => c.Produtos)
    
    .OnDelete(DeleteBehavior.Cascade);


        }

    }
}
