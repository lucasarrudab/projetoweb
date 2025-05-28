import { useState } from 'react'
import { PencilIcon, TrashIcon, ShoppingCartIcon, ExclamationCircleIcon } from '@heroicons/react/24/outline'
import DeleteConfirmationDialog from './DeleteConfirmationDialog'

export default function ProductGrid({ products, onEdit, onDelete, onAddToCart, isAdmin }) {
  const [produtoParaExcluir, setProdutoParaExcluir] = useState(null)

  const handleDeleteClick = (produto) => {
    setProdutoParaExcluir(produto)
  }

  const handleDeleteConfirm = () => {
    onDelete(produtoParaExcluir.id)
    setProdutoParaExcluir(null)
  }

  if (products.length === 0) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500">Nenhum produto adicionado ainda.</p>
      </div>
    )
  }

  return (
    <>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 2xl:grid-cols-5 gap-6">
        {products.map((produto) => (
          <div key={produto.codigo} className="bg-white rounded-lg shadow-md overflow-hidden relative">
            {produto.estoque <= 15 && (
              <div className="absolute top-2 right-2 bg-red-100 text-red-600 px-2 py-1 rounded-full flex items-center text-sm">
                <ExclamationCircleIcon className="h-4 w-4 mr-1" />
                Estoque Baixo: {produto.estoque}
              </div>
            )}
            <div className="aspect-w-16 aspect-h-9">
              <img
                src={produto.imageUrl}
                alt={produto.nome}
                className="w-full h-48 object-cover"
              />
            </div>
            <div className="p-4">
              <div className="flex justify-between items-start">
                <h3 className="text-lg font-semibold text-gray-900">{produto.nome}</h3>
                <span className="text-sm text-gray-500">ID: {produto.codigo}</span>
              </div>
              <p className="mt-1 text-sm text-gray-500">{produto.description}</p>
              <p className="mt-2 text-lg font-bold text-gray-900">R$ {Number(produto.preco).toFixed(2)}</p>
              <p className="mt-1 text-sm text-gray-600">Estoque: {produto.estoque}</p>
              <div className="mt-4 flex justify-between items-center">
                {isAdmin && (
                  <div className="space-x-2">
                    <button
                      onClick={() => onEdit(produto)}
                      className="text-pink-600 hover:text-pink-900"
                    >
                      <PencilIcon className="h-5 w-5" />
                    </button>
                    <button
                      onClick={() => handleDeleteClick(produto)}
                      className="text-red-600 hover:text-red-900"
                    >
                      <TrashIcon className="h-5 w-5" />
                    </button>
                  </div>
                )}
                <button
                  onClick={() => onAddToCart(produto)}
                  className="flex items-center space-x-1 bg-gradient-to-r from-pink-500 to-purple-600 text-white px-0.3 py-0.6 rounded-md hover:from-pink-600 hover:to-purple-700"
                  disabled={produto.estoque === 0}
                >
                  <ShoppingCartIcon className="h-5 w-5" />
                  <span>Add no Carrinho</span>
                </button>
              </div>
            </div>
          </div>
        ))}
      </div>

      <DeleteConfirmationDialog
        isOpen={!!produtoParaExcluir}
        onClose={() => setProdutoParaExcluir(null)}
        onConfirm={handleDeleteConfirm}
        productName={produtoParaExcluir?.nome}
      />
    </>
  )
}