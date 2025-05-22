namespace ApiPDV.Pagination;

public abstract class QueryStringParameters
{
    const int maxPageSize = 50;

    /// <summary>
    /// Número atual da página
    /// </summary>
    public int PageNumber { get; set; } = 1;
    public int _pageSize = maxPageSize;

    /// <summary>
    /// Quantidade de itens por página
    /// </summary>
    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }
}