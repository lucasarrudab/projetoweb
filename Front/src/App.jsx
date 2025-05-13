import { useState } from 'react'
import { BrowserRouter as Router, Routes, Route, Link, Navigate } from 'react-router-dom'
import { ShoppingCartIcon, ClipboardDocumentListIcon } from '@heroicons/react/24/outline'
import ProductDialog from './components/ProductDialog'
import ProductGrid from './components/ProductGrid'
import Cart from './components/Cart'
import Login from './components/Login'
import SalesHistory from './components/SalesHistory'
import Checkout from './components/Checkout'

function App() {
  const [isOpen, setIsOpen] = useState(false)
  const [products, setProducts] = useState([])
  const [editingProduct, setEditingProduct] = useState(null)
  const [cart, setCart] = useState([])
  const [isAuthenticated, setIsAuthenticated] = useState(false)
  const [isAdmin, setIsAdmin] = useState(false)
  const [sales, setSales] = useState([])

  const handleAddProduct = (product) => {
    if (editingProduct) {
      setProducts(products.map(p => p.id === editingProduct.id ? product : p))
    } else {
      setProducts([...products, { ...product, id: Date.now() }])
    }
    setIsOpen(false)
    setEditingProduct(null)
  }

  const handleEdit = (product) => {
    setEditingProduct(product)
    setIsOpen(true)
  }

  const handleDelete = (productId) => {
    setProducts(products.filter(p => p.id !== productId))
  }

  const handleAddToCart = (product) => {
    const existingItem = cart.find(item => item.id === product.id)
    if (existingItem) {
      setCart(cart.map(item => 
        item.id === product.id 
          ? { ...item, quantity: item.quantity + 1 }
          : item
      ))
    } else {
      setCart([...cart, { ...product, quantity: 1 }])
    }
  }

  const handleCheckoutComplete = (paymentDetails) => {
    const sale = {
      id: Date.now(),
      date: new Date(),
      items: cart,
      total: cart.reduce((total, item) => total + (item.price * item.quantity), 0),
      payment: paymentDetails
    }
    setSales([...sales, sale])
    setCart([])
  }

  const handleLogout = () => {
    setIsAuthenticated(false)
    setIsAdmin(false)
  }

  if (!isAuthenticated) {
    return <Login onLogin={(isAdmin) => {
      setIsAuthenticated(true)
      setIsAdmin(isAdmin)
    }} />
  }

  return (
    <Router>
      <div className="min-h-screen bg-gray-100">
        <nav className="bg-white shadow-sm">
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <div className="flex justify-between h-16">
              <div className="flex items-center">
                <Link to="/" className="text-xl font-bold text-gray-900">
                  Product Management
                </Link>
              </div>
              <div className="flex items-center space-x-4">
                {isAdmin && (
                  <>
                    <button
                      onClick={() => setIsOpen(true)}
                      className="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700"
                    >
                      Add Product
                    </button>
                    <Link
                      to="/sales"
                      className="flex items-center space-x-1 text-gray-600 hover:text-gray-900"
                    >
                      <ClipboardDocumentListIcon className="h-6 w-6" />
                      <span>Sales History</span>
                    </Link>
                  </>
                )}
                <Link to="/cart" className="relative">
                  <ShoppingCartIcon className="h-6 w-6 text-gray-600" />
                  {cart.length > 0 && (
                    <span className="absolute -top-2 -right-2 bg-red-500 text-white rounded-full w-5 h-5 flex items-center justify-center text-xs">
                      {cart.reduce((total, item) => total + item.quantity, 0)}
                    </span>
                  )}
                </Link>
                <button
                  onClick={handleLogout}
                  className="text-gray-600 hover:text-gray-900"
                >
                  Logout
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
                  products={products}
                  onEdit={handleEdit}
                  onDelete={handleDelete}
                  onAddToCart={handleAddToCart}
                  isAdmin={isAdmin}
                />
              } 
            />
            <Route 
              path="/cart" 
              element={
                <Cart 
                  cart={cart} 
                  setCart={setCart}
                />
              } 
            />
            <Route
              path="/checkout"
              element={
                <Checkout
                  cart={cart}
                  onComplete={handleCheckoutComplete}
                />
              }
            />
            <Route
              path="/sales"
              element={
                isAdmin ? <SalesHistory sales={sales} /> : <Navigate to="/" />
              }
            />
          </Routes>

          {isAdmin && (
            <ProductDialog
              isOpen={isOpen}
              onClose={() => {
                setIsOpen(false)
                setEditingProduct(null)
              }}
              onSubmit={handleAddProduct}
              product={editingProduct}
            />
          )}
        </div>
      </div>
    </Router>
  )
}

export default App