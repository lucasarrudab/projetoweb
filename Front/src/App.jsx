  import { useState, useEffect } from 'react'
  import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom'
  import { jwtDecode } from 'jwt-decode' 
  import { authService } from './services/authService'
  import { produtoService } from './services/produtoService'
  import { salvarCarrinho, carregarCarrinho, salvarVendas, carregarVendas } from './services/localStorage'
  import { ShoppingCartIcon, ClipboardDocumentListIcon, BellIcon } from '@heroicons/react/24/outline'
  import { carrinhoService } from './services/carrinhoService'
  import ProductDialog from './components/ProductDialog'
  import ProductGrid from './components/ProductGrid'
  import Cart from './components/Cart'
  import Login from './components/Login'
  import SalesHistory from './components/SalesHistory'
  import Checkout from './components/Checkout'

  function App() {
    const [estaAberto, setEstaAberto] = useState(false)
    const [produtos, setProdutos] = useState([])
    const [produtoEditando, setProdutoEditando] = useState(null)
    const [carrinho, setCarrinho] = useState(() => carregarCarrinho())
    const [estaAutenticado, setEstaAutenticado] = useState(false)
    const [ehAdmin, setEhAdmin] = useState(false)
    const [vendas, setVendas] = useState(() => carregarVendas())
    const [carrinhoCriado, setCarrinhoCriado] = useState(false)

    useEffect(() => {
      restaurarSessao()
      
      const carregarProdutos = async () => {
        try {
          const produtosAPI = await produtoService.getAll()
          const newProduto = produtosAPI.map(p => ({
            ...p,
            imageUrl: p.imageUrl || '/default.png'
          }))
          .sort((a, b) => a.nome.localeCompare(b.nome))
          setProdutos(newProduto)
        } catch (error) {
          console.error('Erro ao carregar produtos:', error)
        }
      }

      carregarProdutos()
    }, [])

    const restaurarSessao = () => {
        const token = localStorage.getItem('token')
        if (token) {
          try {
            const decoded = jwtDecode(token)
            const isExpired = decoded.exp * 1000 < Date.now()
            if (!isExpired) {
              const isAdmin = decoded?.role?.toLowerCase() === 'admin'
              setEhAdmin(isAdmin)
              setEstaAutenticado(true)
            } else {
              localStorage.removeItem('token')
            }
          } catch (err) {
            console.error('Token inválido:', err)
            localStorage.removeItem('token')
          }
        }
      }

    useEffect(() => {
      salvarCarrinho(carrinho)
    }, [carrinho])

    useEffect(() => {
      salvarVendas(vendas)
    }, [vendas])
    const handleCreateProduct = async (produto) => {
      try {
        const novoProduto = await produtoService.create(produto); 
        setProdutos([...produtos, novoProduto]); 
        setEstaAberto(false); 
      } catch (error) {
        console.error('Erro ao adicionar produto:', error);
        alert('Falha ao adicionar o produto.');
      }
    };

    const handleEditProduct = async (updatedProduto) => {
      try {
        const updated = await produtoService.update(updatedProduto.id, updatedProduto)
        setProdutos((prevProdutos) =>
          prevProdutos.map((p) => (p.id === updated.id ? updated : p))
        )
        setProdutoEditando(null)
        setEstaAberto(false)
      } catch (error) {
        console.error('Erro ao atualizar produto:', error)
        alert('Falha ao atualizar o produto.')
      }
    }

    const handleEdit = (produto) => {
      setProdutoEditando(produto)
      setEstaAberto(true)
    }


    const handleDelete = async (id) => {
      try {
        await produtoService.delete(id);
        setProdutos(produtos.filter(p => p.id !== id));
      } catch (error) {
        console.error('Erro ao deletar produto:', error);
      }
    };

  const handleAddToCart = async (produto) => {
    try {
      if (!carrinhoCriado) {
        const novoCarrinho = await carrinhoService.criarCarrinho()
        localStorage.setItem('carrinhoId', novoCarrinho.id)
        setCarrinhoCriado(true)
      }

      const carrinhoAtualizado = await carrinhoService.adicionarProdutoCarrinho(produto.id)
      setCarrinho(carrinhoAtualizado.produtos || [])
    } catch (err) {
      console.error('Erro ao adicionar produto ao carrinho:', err)
      alert('Não foi possível adicionar o produto ao carrinho.')
    }
  }

    const handleCheckoutComplete = (detalhePagamento) => {
      // Atualiza estoque
      const produtosAtualizados = produtos.map(produto => {
        const itemCarrinho = carrinho.find(item => item.id === produto.id)
        if (itemCarrinho) {
          return {
            ...produto,
            amount: produto.amount - itemCarrinho.quantidade
          }
        }
        return produto
      })

      setProdutos(produtosAtualizados)

      // Cria registro da venda
      const venda = {
        id: Date.now(),
        data: new Date(),
        itens: carrinho,
        total: carrinho.reduce((total, item) => total + (item.price * item.quantidade), 0),
        pagamento: detalhePagamento
      }
      setVendas([...vendas, venda])
      setCarrinho([])
      setCarrinhoCriado(false)
    }

  const handleLogout = async () => {
    try {
      const carrinhoId = localStorage.getItem('carrinhoId')
      if (carrinhoId) {
        try {
          await carrinhoService.deletarCarrinho(carrinhoId)
        } catch (error) {
          console.warn('Erro ao tentar deletar o carrinho no logout:', error)
        }
      }

      await authService.logout()
    } catch (error) {
      console.error('Erro ao deslogar:', error)
    } finally {
      setEstaAutenticado(false)
      setEhAdmin(false)
      setCarrinho([])
      setVendas([])
      localStorage.removeItem('token')
      localStorage.removeItem('carrinho')
      localStorage.removeItem('carrinhoId')
      localStorage.removeItem('vendas')
      localStorage.removeItem('refreshToken')
      localStorage.removeItem('tokenExpiration')
    }
  }

    const produtosBaixoEstoque = produtos.filter(p => p.amount <= 15)

    if (!estaAutenticado) {
      return <Login onLogin={(ehAdmin) => {
        setEstaAutenticado(true)
        restaurarSessao()
      }} />
    }
    return (
      <Router>
        <div className="min-h-screen bg-gray-50">
          <nav className="bg-white shadow-sm">
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
              <div className="flex justify-between h-16">
                <div className="flex items-center">
                  <Link to="/" className="text-xl font-bold text-transparent bg-clip-text bg-gradient-to-r from-pink-500 to-purple-600">
                    Nossa Padaria
                  </Link>
                </div>
                <div className="flex items-center space-x-4">
                  {ehAdmin && (
                    <>
                      <button
                        onClick={() => setEstaAberto(true)}
                        className="btn-primary"
                      >
                        Adicionar Produto
                      </button>
                      <Link
                        to="/sales"
                        className="flex items-center space-x-1 text-gray-600 hover:text-gray-900"
                      >
                        <ClipboardDocumentListIcon className="h-6 w-6" />
                        <span>Histórico de Vendas</span>
                      </Link>
                      {produtosBaixoEstoque.length > 0 && (
                        <div className="relative">
                          <BellIcon className="h-6 w-6 text-red-500" />
                          <span className="absolute -top-2 -right-2 bg-red-500 text-white rounded-full w-5 h-5 flex items-center justify-center text-xs">
                            {produtosBaixoEstoque.length}
                          </span>
                        </div>
                      )}
                    </>
                  )}
                  {!ehAdmin && (
                    <Link
                      to="/sales"
                      className="flex items-center space-x-1 text-gray-600 hover:text-gray-900"
                    >
                      <ClipboardDocumentListIcon className="h-6 w-6" />
                      <span>Histórico de Vendas</span>
                    </Link>
                  )}
                  <Link to="/cart" className="relative">
                    <ShoppingCartIcon className="h-6 w-6 text-gray-600" />
                    {carrinho.length > 0 && (
                      <span className="absolute -top-2 -right-2 bg-pink-500 text-white rounded-full w-5 h-5 flex items-center justify-center text-xs">
                        {carrinho.reduce((total, item) => total + item.quantidade, 0)}
                      </span>
                    )}
                  </Link>
                  <button
                    onClick={handleLogout}
                    className="text-gray-600 hover:text-gray-900"
                  >
                    Sair
                  </button>
                </div>
              </div>
            </div>
          </nav>

          <div className="max-w-7xl mx-auto py-8 px-4 sm:px-6 lg:px-8">
            <Routes>
              <Route
                path="/"
                element={
                  <ProductGrid
                    products={produtos}
                    onEdit={handleEdit}
                    onDelete={handleDelete}
                    onAddToCart={handleAddToCart}
                    isAdmin={ehAdmin}
                  />
                }
              />
              <Route
                path="/cart"
                element={
                  <Cart
                    cart={carrinho}
                    setCart={setCarrinho}
                  />
                }
              />
              <Route
                path="/checkout"
                element={
                  <Checkout
                    cart={carrinho}
                    onComplete={handleCheckoutComplete}
                  />
                }
              />
              <Route
                path="/sales"
                element={
                  <SalesHistory sales={vendas} isAdmin={ehAdmin} />
                }
              />
            </Routes>

            {ehAdmin && (
              <ProductDialog
                isOpen={estaAberto}
                onClose={() => {
                  setEstaAberto(false)
                  setProdutoEditando(null)
                }}
                onSubmit={produtoEditando ? handleEditProduct : handleCreateProduct}
                product={produtoEditando}
                produtos={produtos}
              />
            )}
          </div>
        </div>
      </Router>
    )
  }

  export default App