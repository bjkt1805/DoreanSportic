using DoreanSportic.Infrastructure.Data;
using DoreanSportic.Infrastructure.Models;
using DoreanSportic.Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Infrastructure.Repository.Implementations
{
    public class RepositoryUsuario : IRepositoryUsuario
    {
        private readonly DoreanSporticContext _context;
        public RepositoryUsuario(DoreanSporticContext context)
        {
            _context = context;
        }
        public Task<Usuario> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<ICollection<Usuario>> ListAsync()
        {
            //Select * from Usuario
            var collection = await _context.Set<Usuario>().ToListAsync();
            return collection;
        }

        public async Task<Usuario> PrimerUsuario()
        {
            //Select top 1 from Usuario
            var usuario = await _context.Set<Usuario>().FirstOrDefaultAsync();
            return usuario;
        }
    }
}
