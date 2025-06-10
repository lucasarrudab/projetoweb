import { useState, useEffect } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { TrashIcon } from '@heroicons/react/24/outline'
import { carrinhoService } from '../services/carrinhoService'
import { produtoService } from '../services/produtoService'
import { carregarCarrinho } from '../services/localStorage'

const BASE_URL = 'http://localhost:5025'

export default function Cart({ cart, setCart }) {
  const navegacao = useNavigate()
  const [carregando, setCarregando] = useState(true)
  const [produtoCodigo, setProdutoCodigo] = useState('')
  
  useEffect(() => {
  const carregarCarrinho = async () => {
    let carrinhoId = localStorage.getItem('carrinhoId');

    if (carrinhoId == null) {
      try {
        const novoCarrinho = await carrinhoService.criarCarrinho();
        carrinhoId = novoCarrinho.id;
        localStorage.setItem('carrinhoId', carrinhoId);
        setCart(mapProdutosComImagem(novoCarrinho.produtos));
      } catch (err) {
        console.error('Erro ao criar carrinho:', err);
      } finally {
        setCarregando(false);
      }
      return;
    }

    try {
      const carrinho = await carrinhoService.buscarCarrinhoPorId(carrinhoId);
      setCart(mapProdutosComImagem(carrinho.produtos));
    } catch (err) {
      console.error('Erro ao buscar carrinho:', err);
    } finally {
      setCarregando(false);
    }
  };

  carregarCarrinho();
}, []);

const handleQuantityChange = async (produtoId, novaQuantidade) => {
  try {
    if (novaQuantidade < 1) {
      // Se quantidade < 1, vamos remover o produto no backend e no estado local
      await carrinhoService.removerProduto(produtoId)
      setCart(cart.filter(item => item.id !== produtoId))
    } else {
      // Atualiza a quantidade no backend
      await carrinhoService.alterarQuantidadeProdto(produtoId, novaQuantidade)
      // Atualiza o estado local
      setCart(cart.map(item =>
        item.id === produtoId ? { ...item, quantidade: novaQuantidade } : item
      ))
    }
  } catch (error) {
    console.error("Erro ao alterar quantidade:", error)
    // Aqui pode mostrar alguma mensagem pro usuário, etc.
  }
}

  const handleRemoveItem = async (produtoId) => {
    try {
      await carrinhoService.removerProduto(produtoId)
      setCart(cart.filter(item => item.id !== produtoId))
    } catch (error) {
      console.error("Erro ao remover produto:", error)
    }
  }

  const handleAddByCodigo = async () => {
  if (!produtoCodigo) return

  try {
    const carrinhoAtualizado = await carrinhoService.adicionarProdutoCarrinho(produtoCodigo);
setCart(mapProdutosComImagem(carrinhoAtualizado.produtos));
setProdutoCodigo('');
    
  } catch (error) {
    console.error("Erro ao adicionar produto pelo código:", error)
  }
}

const mapProdutosComImagem = (produtos) =>
  (produtos || []).map(item => ({
    ...item,
    imageUrl: item.urlImagem ? `${BASE_URL}${item.urlImagem}` : '/default.png'
  }));

  const subtotal = Array.isArray(cart)
    ? cart.reduce((total, item) => {
        const preco = Number(item.preco || item.valorUnitario)
        const quantidade = Number(item.quantidade)
        return total + (isNaN(preco) || isNaN(quantidade) ? 0 : preco * quantidade)
      }, 0)
    : 0

  if (carregando) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500">Carregando carrinho...</p>
      </div>
    )
  }

  if (cart.length === 0) {
  return (
    <div className="bg-white rounded-lg shadow-md p-6">
      <h2 className="text-2xl font-bold text-gray-900 mb-6">Carrinho de Compras</h2>
      
      <div className="mb-6">
        <label htmlFor="codigo-produto" className="block text-sm font-medium text-gray-700">
          Adicionar produto por código
        </label>
        <input
          id="codigo-produto"
          type="text"
          value={produtoCodigo}
          onChange={(e) => setProdutoCodigo(e.target.value)}
          onKeyDown={async (e) => {
            if (e.key === 'Enter') {
              e.preventDefault()
              await handleAddByCodigo()
            }
          }}
          placeholder="Digite o código e pressione Enter"
          className="mt-1 block w-full sm:w-64 rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
        />
      </div>

      <div className="text-center py-12">
        <p className="text-gray-500 mb-4">Seu carrinho está vazio.</p>
        <Link
          to="/"
          className="text-blue-600 hover:text-blue-800 font-medium"
        >
          Continuar Comprando
        </Link>
      </div>
    </div>
  )
}

  return (
    <div className="bg-white rounded-lg shadow-md p-6">
      <h2 className="text-2xl font-bold text-gray-900 mb-6">Carrinho de Compras</h2>
      <div className="mb-6">
      <label htmlFor="codigo-produto" className="block text-sm font-medium text-gray-700">
        Adicionar produto por código
      </label>
      <input
        id="codigo-produto"
        type="text"
        value={produtoCodigo}
        onChange={(e) => setProdutoCodigo(e.target.value)}
        onKeyDown={async (e) => {
          if (e.key === 'Enter') {
            e.preventDefault()
            await handleAddByCodigo()
          }
        }}
        placeholder="Digite o código e pressione Enter"
        className="mt-1 block w-full sm:w-64 rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
      />
    </div>
      <div className="space-y-4">
        {Array.isArray(cart) &&
          cart.map((item) => (
            
           <div key={item.id} className="flex items-center space-x-4 py-4 border-b">
            <img
              src={item.imageUrl || '/default.png'}
              alt={item.nome}
              className="w-20 h-20 object-cover rounded"
            />
            <div className="flex-1">
              <h3 className="text-lg font-medium text-gray-900">{item.nome || item.produtoNome}</h3>
              <p className="text-lg font-bold text-gray-900">
                R$ {Number(item.preco || item.valorUnitario).toFixed(2)}
              </p>
            </div>
            <div className="flex items-center space-x-2">
              <button
                onClick={() => handleQuantityChange(item.id, item.quantidade - 1)}
                className="text-gray-500 hover:text-gray-700"
              >
                -
              </button>
              <span className="px-4 py-2 border rounded">{item.quantidade}</span>
              <button
                onClick={() => handleQuantityChange(item.id, item.quantidade + 1)}
                className="text-gray-500 hover:text-gray-700"
              >
                +
              </button>
              <button
                onClick={() => handleRemoveItem(item.id)}
                className="text-red-600 hover:text-red-800 ml-4"
              >
                <TrashIcon className="h-5 w-5" />
              </button>
            </div>
          </div>
        ))}
      </div>
      <div className="mt-8 flex justify-between items-center">
        <Link
          to="/"
          className="text-blue-600 hover:text-blue-800 font-medium"
        >
          Continuar Comprando
        </Link>
        <div className="text-right">
          <p className="text-lg font-bold text-gray-900">
            Subtotal: R$ {subtotal.toFixed(2)}
          </p>
          <button
            onClick={() => navegacao('/checkout')}
            className="mt-4 bg-blue-600 text-white px-6 py-2 rounded-md hover:bg-blue-700"
          >
            Finalizar Compra
          </button>
        </div>
      </div>
    </div>
  )
}