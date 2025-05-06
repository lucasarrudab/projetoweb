namespace ApiPDV.Repositories
{
    public interface IUnitOfWork
    {
        IProdutoRepository ProdutoRepository { get; }
        ICarrinhoRepository CarrinhoRepository { get; }
        IProdutoCarrinhoRepository ProdutoCarrinhoRepository { get; }
        Task CommitAsync();
    }
}
