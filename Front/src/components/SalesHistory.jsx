import { format } from 'date-fns'

export default function SalesHistory({ sales }) {
  if (sales.length === 0) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500">No sales history available.</p>
      </div>
    )
  }

  return (
    <div className="bg-white rounded-lg shadow-md p-6">
      <h2 className="text-2xl font-bold text-gray-900 mb-6">Sales History</h2>
      <div className="space-y-6">
        {sales.map((sale) => (
          <div key={sale.id} className="border rounded-lg p-4">
            <div className="flex justify-between items-start mb-4">
              <div>
                <p className="text-sm text-gray-500">
                  Order #{sale.id}
                </p>
                <p className="text-sm text-gray-500">
                  {format(new Date(sale.date), 'PPpp')}
                </p>
              </div>
              <div className="text-right">
                <p className="text-lg font-bold text-gray-900">
                  Total: R$ {sale.total.toFixed(2)}
                </p>
                <p className="text-sm text-gray-500 capitalize">
                  Paid with {sale.payment.method}
                </p>
              </div>
            </div>
            <div className="space-y-2">
              {sale.items.map((item) => (
                <div key={item.id} className="flex justify-between text-sm">
                  <span>{item.name} x {item.quantity}</span>
                  <span>R$ {(item.price * item.quantity).toFixed(2)}</span>
                </div>
              ))}
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}