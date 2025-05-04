
using ApiPDV.Context;

namespace ApiPDV.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private IProdutoRepository _produtoRepo;
    private ICarrinhoRepository _carrinhoRepo;
    public AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IProdutoRepository ProdutoRepository
    {
        get
        {
            return _produtoRepo = _produtoRepo ?? new ProdutoRepository(_context);
        }
    }

    public ICarrinhoRepository CarrinhoRepository
    {
        get
        {
            return _carrinhoRepo = _carrinhoRepo ?? new CarrinhoRepository(_context);
        }
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
