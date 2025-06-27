using System;
using System.Collections.Generic;
using DoreanSportic.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace DoreanSportic.Infrastructure.Data;

public partial class DoreanSporticContext : DbContext
{
    public DoreanSporticContext(DbContextOptions<DoreanSporticContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Carrito> Carrito { get; set; }

    public virtual DbSet<CarritoDetalle> CarritoDetalle { get; set; }

    public virtual DbSet<Categoria> Categoria { get; set; }

    public virtual DbSet<Cliente> Cliente { get; set; }

    public virtual DbSet<Empaque> Empaque { get; set; }

    public virtual DbSet<Etiqueta> Etiqueta { get; set; }

    public virtual DbSet<ImagenProducto> ImagenProducto { get; set; }

    public virtual DbSet<Marca> Marca { get; set; }

    public virtual DbSet<MetodoPago> MetodoPago { get; set; }

    public virtual DbSet<Pedido> Pedido { get; set; }

    public virtual DbSet<PedidoDetalle> PedidoDetalle { get; set; }

    public virtual DbSet<Producto> Producto { get; set; }

    public virtual DbSet<Promocion> Promocion { get; set; }

    public virtual DbSet<ResennaValoracion> ResennaValoracion { get; set; }

    public virtual DbSet<Rol> Rol { get; set; }

    public virtual DbSet<Sexo> Sexo { get; set; }

    public virtual DbSet<Tarjeta> Tarjeta { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Carrito>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Carrito__3213E83F243EC652");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.EstadoPago)
                .HasMaxLength(20)
                .HasColumnName("estadoPago");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.IdCliente).HasColumnName("idCliente");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Carrito)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK_Carrito_Cliente");
        });

        modelBuilder.Entity<CarritoDetalle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Carrito___3213E83FB2BADD66");

            entity.ToTable("Carrito_Detalle");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.IdCarrito).HasColumnName("idCarrito");
            entity.Property(e => e.IdEmpaque).HasColumnName("idEmpaque");
            entity.Property(e => e.IdProducto).HasColumnName("idProducto");

            entity.HasOne(d => d.IdCarritoNavigation).WithMany(p => p.CarritoDetalle)
                .HasForeignKey(d => d.IdCarrito)
                .HasConstraintName("FK_CarritoDetalle_Carrito");

            entity.HasOne(d => d.IdEmpaqueNavigation).WithMany(p => p.CarritoDetalle)
                .HasForeignKey(d => d.IdEmpaque)
                .HasConstraintName("FK_CarritoDetalle_Empaque");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.CarritoDetalle)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK_CarritoDetalle_Producto");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3213E83FFD2A0F22");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cliente__3213E83F650CCA99");

            entity.HasIndex(e => e.Email, "UQ__Cliente__AB6E6164BE3AAF53").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .HasColumnName("apellido");
            entity.Property(e => e.DireccionEnvio)
                .HasMaxLength(500)
                .HasColumnName("direccionEnvio");
            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .HasColumnName("email");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.FechaNacimiento)
                .HasColumnType("datetime")
                .HasColumnName("fechaNacimiento");
            entity.Property(e => e.IdSexo).HasColumnName("idSexo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");

            entity.HasOne(d => d.IdSexoNavigation).WithMany(p => p.Cliente)
                .HasForeignKey(d => d.IdSexo)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Cliente_Sexo");
        });

        modelBuilder.Entity<Empaque>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Empaque__3213E83F93B410D8");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.Foto).HasColumnName("foto");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.PrecioBase)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precioBase");
            entity.Property(e => e.TipoEmpaque)
                .HasMaxLength(50)
                .HasColumnName("tipoEmpaque");
        });

        modelBuilder.Entity<Etiqueta>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<ImagenProducto>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Descripcion)
                .HasMaxLength(20)
                .HasColumnName("descripcion");

            entity.Property(e => e.Estado).HasColumnName("estado");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.IdProducto).HasColumnName("idProducto");

            entity.Property(e => e.Imagen).HasColumnName("imagen");


            entity.HasOne(d => d.IdProductoNavigation)
                .WithMany(p => p.ImagenesProducto)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK_ImagenProducto_Producto");
        });

        modelBuilder.Entity<Marca>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Marca__3213E83F79D22278");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.Foto).HasColumnName("foto");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<MetodoPago>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MetodoPa__3213E83FAE32F481");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Pedido__3213E83FA6BF513B");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.EstadoPedido)
                .HasMaxLength(50)
                .HasDefaultValue("Pendiente")
                .HasColumnName("estadoPedido");
            entity.Property(e => e.FechaPedido)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaPedido");
            entity.Property(e => e.IdCliente).HasColumnName("idCliente");
            entity.Property(e => e.IdMetodoPago).HasColumnName("idMetodoPago");
            entity.Property(e => e.Impuesto)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("impuesto");
            entity.Property(e => e.NumFactura)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("numFactura");
            entity.Property(e => e.SubTotal)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("subTotal");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("total");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Pedido)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK_Pedido_Cliente");

            entity.HasOne(d => d.IdMetodoPagoNavigation).WithMany(p => p.Pedido)
                .HasForeignKey(d => d.IdMetodoPago)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Pedido_MetodoPago");
        });

        modelBuilder.Entity<PedidoDetalle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Pedido_D__3213E83F72614079");

            entity.ToTable("Pedido_Detalle");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.IdEmpaque).HasColumnName("idEmpaque");
            entity.Property(e => e.IdPedido).HasColumnName("idPedido");
            entity.Property(e => e.IdProducto).HasColumnName("idProducto");

            entity.HasOne(d => d.IdEmpaqueNavigation).WithMany(p => p.PedidoDetalle)
                .HasForeignKey(d => d.IdEmpaque)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PedidoDetalle_Empaque");

            entity.HasOne(d => d.IdPedidoNavigation).WithMany(p => p.PedidoDetalle)
                .HasForeignKey(d => d.IdPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PedidoDetalle_Pedido");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.PedidoDetalle)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK_PedidoDetalle_Producto");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Producto__3213E83F561BECA8");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.IdCategoria).HasColumnName("idCategoria");
            entity.Property(e => e.IdMarca).HasColumnName("idMarca");
            entity.Property(e => e.Nombre)
                .HasMaxLength(200)
                .HasColumnName("nombre");
            entity.Property(e => e.PrecioBase)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precioBase");
            entity.Property(e => e.Stock).HasColumnName("stock");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Producto)
                .HasForeignKey(d => d.IdCategoria)
                .HasConstraintName("FK_Producto_Categoria");

            entity.HasOne(d => d.IdMarcaNavigation).WithMany(p => p.Producto)
                .HasForeignKey(d => d.IdMarca)
                .HasConstraintName("FK_Producto_Marca");

            entity.HasMany(d => d.IdEtiqueta).WithMany(p => p.IdProducto)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductoEtiqueta",
                    r => r.HasOne<Etiqueta>().WithMany()
                        .HasForeignKey("IdEtiqueta")
                        .HasConstraintName("FK_Producto_Etiqueta_Etiqueta"),
                    l => l.HasOne<Producto>().WithMany()
                        .HasForeignKey("IdProducto")
                        .HasConstraintName("FK_Producto_Etiqueta_Producto"),
                    j =>
                    {
                        j.HasKey("IdProducto", "IdEtiqueta");
                        j.ToTable("Producto_Etiqueta");
                        j.IndexerProperty<int>("IdProducto").HasColumnName("idProducto");
                        j.IndexerProperty<int>("IdEtiqueta").HasColumnName("idEtiqueta");
                    });
        });

        modelBuilder.Entity<Promocion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Promocio__3213E83F219D463A");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.DescuentoFijo)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("descuentoFijo");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("fechaFin");
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("fechaInicio");
            entity.Property(e => e.Nombre)
                .HasMaxLength(200)
                .HasColumnName("nombre");
            entity.Property(e => e.PorcentajeDescuento)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("porcentajeDescuento");

            entity.HasMany(d => d.IdCategoria).WithMany(p => p.IdPromocion)
                .UsingEntity<Dictionary<string, object>>(
                    "PromocionCategoria",
                    r => r.HasOne<Categoria>().WithMany()
                        .HasForeignKey("IdCategoria")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Promocion_Categoria_Categoría"),
                    l => l.HasOne<Promocion>().WithMany()
                        .HasForeignKey("IdPromocion")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Promocion_Categoria_Promocion"),
                    j =>
                    {
                        j.HasKey("IdPromocion", "IdCategoria");
                        j.ToTable("Promocion_Categoria");
                        j.IndexerProperty<int>("IdPromocion").HasColumnName("idPromocion");
                        j.IndexerProperty<int>("IdCategoria").HasColumnName("idCategoria");
                    });

            entity.HasMany(d => d.IdProducto).WithMany(p => p.IdPromocion)
                .UsingEntity<Dictionary<string, object>>(
                    "PromocionProducto",
                    r => r.HasOne<Producto>().WithMany()
                        .HasForeignKey("IdProducto")
                        .HasConstraintName("FK_PromocionProducto_Producto"),
                    l => l.HasOne<Promocion>().WithMany()
                        .HasForeignKey("IdPromocion")
                        .HasConstraintName("FK_PromocionProducto_Promocion"),
                    j =>
                    {
                        j.HasKey("IdPromocion", "IdProducto");
                        j.ToTable("Promocion_Producto");
                        j.IndexerProperty<int>("IdPromocion").HasColumnName("idPromocion");
                        j.IndexerProperty<int>("IdProducto").HasColumnName("idProducto");
                    });
        });

        modelBuilder.Entity<ResennaValoracion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ResennaV__3213E83FE6F1BD6E");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Calificacion).HasColumnName("calificacion");
            entity.Property(e => e.Comentario).HasColumnName("comentario");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.FechaResenna)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaResenna");
            entity.Property(e => e.IdProducto).HasColumnName("idProducto");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.ResennaValoracion)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK_Resenna_Producto");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.ResennaValoracion)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Resenna_Usuario");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rol__3213E83F38F4D91C");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Sexo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sexo__3213E83F9FC191F9");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Tarjeta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tarjeta__3213E83FFDB7D75D");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FechaExpiracion).HasColumnName("fechaExpiracion");
            entity.Property(e => e.IdCliente).HasColumnName("idCliente");
            entity.Property(e => e.NombreTitular)
                .HasMaxLength(100)
                .HasColumnName("nombreTitular");
            entity.Property(e => e.NumeroEnmascarado)
                .HasMaxLength(20)
                .HasColumnName("numeroEnmascarado");
            entity.Property(e => e.TipoTarjeta)
                .HasMaxLength(50)
                .HasColumnName("tipoTarjeta");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Tarjeta)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK_Tarjeta_Cliente");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuario__3213E83FEFEA1E7C");

            entity.HasIndex(e => e.UserName, "UQ__Usuario__66DCF95C610E9A60").IsUnique();

            entity.HasIndex(e => e.IdCliente, "UQ__Usuario__885457EFC40F32E5").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EsActivo)
                .HasDefaultValue(true)
                .HasColumnName("esActivo");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.IdCliente).HasColumnName("idCliente");
            entity.Property(e => e.IdRol).HasColumnName("idRol");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(200)
                .HasColumnName("passwordHash");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .HasColumnName("userName");

            entity.HasOne(d => d.IdClienteNavigation).WithOne(p => p.Usuario)
                .HasForeignKey<Usuario>(d => d.IdCliente)
                .HasConstraintName("FK_Usuario_Cliente");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuario)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK_Usuario_Rol");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
