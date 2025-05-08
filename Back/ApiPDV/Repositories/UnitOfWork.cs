
using ApiPDV.Context;
using ApiPDV.Models;

namespace ApiPDV.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private IProdutoRepository _produtoRepo;
    private ICarrinhoRepository _carrinhoRepo;
    private IProdutoCarrinhoRepository _produtoCarrinhoRepo;
    private IVendaRepository _vendaRepo;
    private IMetodoPagamentoRepository _metodoPagamentoRepo;
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

    public IProdutoCarrinhoRepository ProdutoCarrinhoRepository
    {
        get
        {
            return _produtoCarrinhoRepo = _produtoCarrinhoRepo ?? new ProdutoCarrinhoRepository(_context);
        }
    }
    public IVendaRepository VendaRepository
    {
        get
        {
            return _vendaRepo = _vendaRepo ?? new VendaRepository(_context);
        }
    }
    public IMetodoPagamentoRepository MetodoPagamentoRepository
    {
        get
        {
            return _metodoPagamentoRepo = _metodoPagamentoRepo ?? new MetodoPagamentoRepository(_context);
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
