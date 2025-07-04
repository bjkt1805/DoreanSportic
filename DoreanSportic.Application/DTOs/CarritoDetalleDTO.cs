﻿using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Application.DTOs;

public record CarritoDetalleDTO
{
    public int Id { get; set; }

    public int IdCarrito { get; set; }

    public int IdProducto { get; set; }

    public int IdEmpaque { get; set; }

    public int Cantidad { get; set; }

    public bool Estado { get; set; }

    public virtual Carrito IdCarritoNavigation { get; set; } = null!;

    public virtual Empaque IdEmpaqueNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
