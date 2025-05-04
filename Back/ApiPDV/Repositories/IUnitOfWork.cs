namespace ApiPDV.Repositories
{
    public interface IUnitOfWork
    {
        IProdutoRepository ProdutoRepository { get; }
        ICarrinhoRepository CarrinhoRepository { get; }
        Task CommitAsync();
    }
}
