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

    // Nuevos DbSets del Módulo 2
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Carrito> Carritos { get; set; }
    public DbSet<ItemCarrito> ItemsCarrito { get; set; }

    // Módulo 3
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<DetallePedido> DetallesPedido { get; set; }

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

        // Relación Cliente 1 - 1 Carrito
        modelBuilder.Entity<Carrito>()
            .HasOne(c => c.Cliente)
            .WithOne(cl => cl.Carrito)
            .HasForeignKey<Carrito>(c => c.IdCliente);

        // Relación Carrito 1 - N ItemCarrito
        modelBuilder.Entity<ItemCarrito>()
            .HasOne(i => i.Carrito)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.IdCarrito);

        // Relación ItemCarrito N - 1 ProductoVariante
        modelBuilder.Entity<ItemCarrito>()
            .HasOne(i => i.Variante)
            .WithMany()
            .HasForeignKey(i => i.IdVariante);

        // Relación Cliente 1 - N Pedido
        modelBuilder.Entity<Pedido>() 
            .HasOne(p => p.Cliente) 
            .WithMany() 
            .HasForeignKey(p => p.IdCliente);

        // Relación Pedido 1 - N DetallePedido
        modelBuilder.Entity<DetallePedido>() 
            .HasOne(d => d.Pedido) 
            .WithMany(p => p.Detalles) 
            .HasForeignKey(d => d.IdPedido);

        // Relación DetallePedido N - 1 ProductoVariante
        modelBuilder.Entity<DetallePedido>() 
            .HasOne(d => d.Variante) 
            .WithMany() 
            .HasForeignKey(d => d.IdVariante);
    }
}