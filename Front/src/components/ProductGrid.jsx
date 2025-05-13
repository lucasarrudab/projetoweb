import { PencilIcon, TrashIcon, ShoppingCartIcon } from '@heroicons/react/24/outline'

export default function ProductGrid({ products, onEdit, onDelete, onAddToCart, isAdmin }) {
  if (products.length === 0) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500">No products added yet.</p>
      </div>
    )
  }

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 2xl:grid-cols-5 gap-6">
      {products.map((product) => (
        <div key={product.id} className="bg-white rounded-lg shadow-md overflow-hidden">
          <div className="aspect-w-16 aspect-h-9">
            <img
              src={product.imageUrl}
              alt={product.name}
              className="w-full h-48 object-cover"
            />
          </div>
          <div className="p-4">
            <h3 className="text-lg font-semibold text-gray-900">{product.name}</h3>
            <p className="mt-1 text-sm text-gray-500">{product.description}</p>
            <p className="mt-2 text-lg font-bold text-gray-900">R$ {Number(product.price).toFixed(2)}</p>
            <div className="mt-4 flex justify-between items-center">
              {isAdmin && (
                <div className="space-x-2">
                  <button
                    onClick={() => onEdit(product)}
                    className="text-blue-600 hover:text-blue-900"
                  >
                    <PencilIcon className="h-5 w-5" />
                  </button>
                  <button
                    onClick={() => onDelete(product.id)}
                    className="text-red-600 hover:text-red-900"
                  >
                    <TrashIcon className="h-5 w-5" />
                  </button>
                </div>
              )}
              <button
                onClick={() => onAddToCart(product)}
                className="flex items-center space-x-1 bg-blue-600 text-white px-3 py-1 rounded-md hover:bg-blue-700"
              >
                <ShoppingCartIcon className="h-5 w-5" />
                <span>Add to Cart</span>
              </button>
            </div>
          </div>
        </div>
      ))}
    </div>
  )
}