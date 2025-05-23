import { useState } from 'react'
import { BrowserRouter as Router, Routes, Route, Link, Navigate } from 'react-router-dom'
import { ShoppingCartIcon, ClipboardDocumentListIcon, BellIcon } from '@heroicons/react/24/outline'
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
  const [carrinho, setCarrinho] = useState([])
  const [estaAutenticado, setEstaAutenticado] = useState(false)
  const [ehAdmin, setEhAdmin] = useState(false)
  const [vendas, setVendas] = useState([])

  // Make products available globally for ID validation
  window.produtos = produtos

  const handleAddProduct = (produto) => {
    if (produtoEditando) {
      setProdutos(produtos.map(p => p.id === produtoEditando.id ? produto : p))
    } else {
      setProdutos([...produtos, produto])
    }
    setEstaAberto(false)
    setProdutoEditando(null)
  }

  const handleEdit = (produto) => {
    setProdutoEditando(produto)
    setEstaAberto(true)
  }

  const handleDelete = (produtoId) => {
    setProdutos(produtos.filter(p => p.id !== produtoId))
  }

  const handleAddToCart = (produto) => {
    const itemExistente = carrinho.find(item => item.id === produto.id)
    if (itemExistente) {
      if (itemExistente.quantidade >= produto.amount) return
      setCarrinho(carrinho.map(item => 
        item.id === produto.id 
          ? { ...item, quantidade: item.quantidade + 1 }
          : item
      ))
    } else {
      setCarrinho([...carrinho, { ...produto, quantidade: 1 }])
    }
  }

  const handleCheckoutComplete = (detalhePagamento) => {
    // Update product stock
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

    // Create sale record
    const venda = {
      id: Date.now(),
      data: new Date(),
      itens: carrinho,
      total: carrinho.reduce((total, item) => total + (item.price * item.quantidade), 0),
      pagamento: detalhePagamento
    }
    setVendas([...vendas, venda])
    setCarrinho([])
  }

  const handleLogout = () => {
    setEstaAutenticado(false)
    setEhAdmin(false)
    setCarrinho([])
  }

  const produtosBaixoEstoque = produtos.filter(p => p.amount <= 15)

  if (!estaAutenticado) {
    return <Login onLogin={(ehAdmin) => {
      setEstaAutenticado(true)
      setEhAdmin(ehAdmin)
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
              onSubmit={handleAddProduct}
              product={produtoEditando}
            />
          )}
        </div>
      </div>
    </Router>
  )
}

export default App