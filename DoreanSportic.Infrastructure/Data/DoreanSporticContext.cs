using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DoreanSportic.Infrastructure.Data;

public partial class DoreanSporticContext : DbContext
{
    public DoreanSporticContext(DbContextOptions<DoreanSporticContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
