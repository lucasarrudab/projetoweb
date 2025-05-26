import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { vendasService } from '../services/vendasService'; 

export default function Checkout({ cart, onComplete }) {
  const navegacao = useNavigate()
  const [nomeMetodoPagamento, setNomeMetodoPagamento] = useState('credit')
  const [dadosFormulario, setDadosFormulario] = useState({
    numeroCartao: '',
    nomeCartao: '',
    dataValidade: '',
    cvv: '',
    chavePix: '',
  })
  const [erros, setErros] = useState({})

  const subtotal = cart.reduce((total, item) => total + (item.preco * item.quantidade), 0)

const handleSubmit = async (e) => {
  e.preventDefault();

  const venda = nomeMetodoPagamento

  try {
    await vendasService.realizarVenda(venda)
    onComplete(venda)
    navegacao('/')
  } catch (error) {
    console.error("Erro ao salvar a venda:", error)
    alert("Ocorreu um erro ao finalizar a compra. Tente novamente.")
  }
};

  return (
    <div className="max-w-2xl mx-auto">
      <div className="bg-white rounded-lg shadow-md p-6">
        <h2 className="text-2xl font-bold text-gray-900 mb-6">Finalizar Compra</h2>
        
        <div className="mb-6">
          <h3 className="text-lg font-medium text-gray-900 mb-4">Resumo do Pedido</h3>
          <div className="space-y-2">
            {cart.map((item) => (
              <div key={item.id} className="flex justify-between text-sm">
                <span>{item.nome} x {item.quantidade}</span>
                <span>R$ {(item.preco * item.quantidade).toFixed(2)}</span>
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
            <h3 className="text-lg font-medium text-gray-900 mb-4">Método de Pagamento</h3>
            <div className="space-x-4">
              <label className="inline-flex items-center">
                <input
                  type="radio"
                  value="credit"
                  checked={nomeMetodoPagamento === 'credit'}
                  onChange={(e) => setNomeMetodoPagamento(e.target.value)}
                  className="form-radio"
                />
                <span className="ml-2">Cartão de Crédito</span>
              </label>
              <label className="inline-flex items-center">
                <input
                  type="radio"
                  value="debit"
                  checked={nomeMetodoPagamento === 'debit'}
                  onChange={(e) => setNomeMetodoPagamento(e.target.value)}
                  className="form-radio"
                />
                <span className="ml-2">Cartão de Débito</span>
              </label>
              <label className="inline-flex items-center">
                <input
                  type="radio"
                  value="pix"
                  checked={nomeMetodoPagamento === 'pix'}
                  onChange={(e) => setNomeMetodoPagamento(e.target.value)}
                  className="form-radio"
                />
                <span className="ml-2">PIX</span>
              </label>
            </div>
          </div>

          <div className="mt-8 flex justify-end space-x-4">
            <button
              type="button"
              onClick={() => navegacao('/cart')}
              className="px-4 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50"
            >
              Voltar ao Carrinho
            </button>
            <button
              type="submit"
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700"
            >
              Finalizar Compra
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}