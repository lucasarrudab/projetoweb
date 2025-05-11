import { useState } from 'react'
import { useNavigate } from 'react-router-dom'

export default function Checkout({ cart, onComplete }) {
  const navigate = useNavigate()
  const [paymentMethod, setPaymentMethod] = useState('credit')
  const [formData, setFormData] = useState({
    cardNumber: '',
    cardName: '',
    expiryDate: '',
    cvv: '',
    pixKey: '',
  })

  const subtotal = cart.reduce((total, item) => total + (item.price * item.quantity), 0)

  const handleSubmit = (e) => {
    e.preventDefault()
    onComplete({
      method: paymentMethod,
      details: formData
    })
    navigate('/')
  }

  return (
    <div className="max-w-2xl mx-auto">
      <div className="bg-white rounded-lg shadow-md p-6">
        <h2 className="text-2xl font-bold text-gray-900 mb-6">Checkout</h2>
        
        <div className="mb-6">
          <h3 className="text-lg font-medium text-gray-900 mb-4">Order Summary</h3>
          <div className="space-y-2">
            {cart.map((item) => (
              <div key={item.id} className="flex justify-between text-sm">
                <span>{item.name} x {item.quantity}</span>
                <span>R$ {(item.price * item.quantity).toFixed(2)}</span>
              </div>
            ))}
            <div className="border-t pt-2 mt-2">
              <div className="flex justify-between font-bold">
                <span>Total</span>
                <span>R$ {subtotal.toFixed(2)}</span>
              </div>
            </div>
          </div>
        </div>

        <form onSubmit={handleSubmit}>
          <div className="mb-6">
            <h3 className="text-lg font-medium text-gray-900 mb-4">Payment Method</h3>
            <div className="space-x-4">
              <label className="inline-flex items-center">
                <input
                  type="radio"
                  value="credit"
                  checked={paymentMethod === 'credit'}
                  onChange={(e) => setPaymentMethod(e.target.value)}
                  className="form-radio"
                />
                <span className="ml-2">Credit Card</span>
              </label>
              <label className="inline-flex items-center">
                <input
                  type="radio"
                  value="debit"
                  checked={paymentMethod === 'debit'}
                  onChange={(e) => setPaymentMethod(e.target.value)}
                  className="form-radio"
                />
                <span className="ml-2">Debit Card</span>
              </label>
              <label className="inline-flex items-center">
                <input
                  type="radio"
                  value="pix"
                  checked={paymentMethod === 'pix'}
                  onChange={(e) => setPaymentMethod(e.target.value)}
                  className="form-radio"
                />
                <span className="ml-2">PIX</span>
              </label>
            </div>
          </div>

          {(paymentMethod === 'credit' || paymentMethod === 'debit') && (
            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700">Card Number</label>
                <input
                  type="text"
                  value={formData.cardNumber}
                  onChange={(e) => setFormData({ ...formData, cardNumber: e.target.value })}
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">Name on Card</label>
                <input
                  type="text"
                  value={formData.cardName}
                  onChange={(e) => setFormData({ ...formData, cardName: e.target.value })}
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                  required
                />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700">Expiry Date</label>
                  <input
                    type="text"
                    placeholder="MM/YY"
                    value={formData.expiryDate}
                    onChange={(e) => setFormData({ ...formData, expiryDate: e.target.value })}
                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700">CVV</label>
                  <input
                    type="text"
                    value={formData.cvv}
                    onChange={(e) => setFormData({ ...formData, cvv: e.target.value })}
                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                    required
                  />
                </div>
              </div>
            </div>
          )}

          {paymentMethod === 'pix' && (
            <div>
              <label className="block text-sm font-medium text-gray-700">PIX Key</label>
              <input
                type="text"
                value={formData.pixKey}
                onChange={(e) => setFormData({ ...formData, pixKey: e.target.value })}
                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                required
              />
            </div>
          )}

          <div className="mt-8 flex justify-end space-x-4">
            <button
              type="button"
              onClick={() => navigate('/cart')}
              className="px-4 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50"
            >
              Back to Cart
            </button>
            <button
              type="submit"
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700"
            >
              Complete Purchase
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}