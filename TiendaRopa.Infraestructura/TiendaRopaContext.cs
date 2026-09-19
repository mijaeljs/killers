using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TiendaRopa.Domain;

namespace TiendaRopa.Infraestructura;

public class TiendaRopaContext : DbContext
{
    public TiendaRopaContext(DbContextOptions<TiendaRopaContext> options)
        : base(options)
    {
    }

    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Talla> Tallas { get; set; }
    public DbSet<Color> Colores { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<ProductoVariante> ProductoVariantes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>()
            .HasOne(p => p.Categoria)
            .WithMany(c => c.Productos)
            .HasForeignKey(p => p.IdCategoria);

        modelBuilder.Entity<ProductoVariante>()
            .HasOne(v => v.Producto)
            .WithMany(p => p.Variantes)
            .HasForeignKey(v => v.IdProducto);

        modelBuilder.Entity<ProductoVariante>()
            .HasOne(v => v.Talla)
            .WithMany(t => t.Variantes)
            .HasForeignKey(v => v.IdTalla);

        modelBuilder.Entity<ProductoVariante>()
            .HasOne(v => v.Color)
            .WithMany(c => c.Variantes)
            .HasForeignKey(v => v.IdColor);

        modelBuilder.Entity<Producto>()
            .Property(p => p.Precio)
            .HasColumnType("decimal(10,2)");
    }
}