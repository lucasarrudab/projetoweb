import { useState } from 'react'
import { useNavigate } from 'react-router-dom'

export default function Checkout({ cart, onComplete }) {
  const navegacao = useNavigate()
  const [metodoPagamento, setMetodoPagamento] = useState('credit')
  const [dadosFormulario, setDadosFormulario] = useState({
    numeroCartao: '',
    nomeCartao: '',
    dataValidade: '',
    cvv: '',
    chavePix: '',
  })
  const [erros, setErros] = useState({})

  const subtotal = cart.reduce((total, item) => total + (item.price * item.quantidade), 0)

  const validarDataValidade = (valor) => {
    const regex = /^(0[1-9]|1[0-2])\/([0-9]{2})$/
    if (!regex.test(valor)) {
      return 'Por favor, insira uma data válida (MM/AA)'
    }
    
    const [mes, ano] = valor.split('/')
    const validade = new Date(2000 + parseInt(ano), parseInt(mes) - 1)
    const hoje = new Date()
    
    if (validade < hoje) {
      return 'Cartão vencido'
    }
    
    return ''
  }

  const handleDataValidadeChange = (e) => {
    let valor = e.target.value.replace(/\D/g, '')
    if (valor.length >= 2) {
      valor = valor.slice(0, 2) + '/' + valor.slice(2, 4)
    }
    setDadosFormulario({ ...dadosFormulario, dataValidade: valor })
    
    const erro = validarDataValidade(valor)
    setErros({ ...erros, dataValidade: erro })
  }

  const handleCvvChange = (e) => {
    const valor = e.target.value.replace(/\D/g, '').slice(0, 3)
    setDadosFormulario({ ...dadosFormulario, cvv: valor })
  }

  const handleSubmit = (e) => {
    e.preventDefault()
    
    if (metodoPagamento === 'credit' || metodoPagamento === 'debit') {
      const erroDataValidade = validarDataValidade(dadosFormulario.dataValidade)
      if (erroDataValidade) {
        setErros({ ...erros, dataValidade: erroDataValidade })
        return
      }
      
      if (dadosFormulario.cvv.length !== 3) {
        setErros({ ...erros, cvv: 'CVV deve ter 3 dígitos' })
        return
      }
    }
    
    onComplete({
      method: metodoPagamento,
      details: dadosFormulario
    })
    navegacao('/')
  }

  return (
    <div className="max-w-2xl mx-auto">
      <div className="bg-white rounded-lg shadow-md p-6">
        <h2 className="text-2xl font-bold text-gray-900 mb-6">Finalizar Compra</h2>
        
        <div className="mb-6">
          <h3 className="text-lg font-medium text-gray-900 mb-4">Resumo do Pedido</h3>
          <div className="space-y-2">
            {cart.map((item) => (
              <div key={item.id} className="flex justify-between text-sm">
                <span>{item.name} x {item.quantidade}</span>
                <span>R$ {(item.price * item.quantidade).toFixed(2)}</span>
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
                  checked={metodoPagamento === 'credit'}
                  onChange={(e) => setMetodoPagamento(e.target.value)}
                  className="form-radio"
                />
                <span className="ml-2">Cartão de Crédito</span>
              </label>
              <label className="inline-flex items-center">
                <input
                  type="radio"
                  value="debit"
                  checked={metodoPagamento === 'debit'}
                  onChange={(e) => setMetodoPagamento(e.target.value)}
                  className="form-radio"
                />
                <span className="ml-2">Cartão de Débito</span>
              </label>
              <label className="inline-flex items-center">
                <input
                  type="radio"
                  value="pix"
                  checked={metodoPagamento === 'pix'}
                  onChange={(e) => setMetodoPagamento(e.target.value)}
                  className="form-radio"
                />
                <span className="ml-2">PIX</span>
              </label>
            </div>
          </div>

          {(metodoPagamento === 'credit' || metodoPagamento === 'debit') && (
            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700">Número do Cartão</label>
                <input
                  type="text"
                  value={dadosFormulario.numeroCartao}
                  onChange={(e) => setDadosFormulario({ ...dadosFormulario, numeroCartao: e.target.value })}
                  className="input"
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">Nome no Cartão</label>
                <input
                  type="text"
                  value={dadosFormulario.nomeCartao}
                  onChange={(e) => setDadosFormulario({ ...dadosFormulario, nomeCartao: e.target.value })}
                  className="input"
                  required
                />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700">Data de Validade</label>
                  <input
                    type="text"
                    placeholder="MM/AA"
                    value={dadosFormulario.dataValidade}
                    onChange={handleDataValidadeChange}
                    className={`input ${erros.dataValidade ? 'border-red-500 focus:ring-red-500' : ''}`}
                    required
                  />
                  {erros.dataValidade && (
                    <p className="mt-1 text-sm text-red-600">{erros.dataValidade}</p>
                  )}
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700">CVV</label>
                  <input
                    type="text"
                    value={dadosFormulario.cvv}
                    onChange={handleCvvChange}
                    className={`input ${erros.cvv ? 'border-red-500 focus:ring-red-500' : ''}`}
                    maxLength="3"
                    required
                  />
                  {erros.cvv && (
                    <p className="mt-1 text-sm text-red-600">{erros.cvv}</p>
                  )}
                </div>
              </div>
            </div>
          )}

          {metodoPagamento === 'pix' && (
            <div>
              <label className="block text-sm font-medium text-gray-700">Chave PIX</label>
              <input
                type="text"
                value={dadosFormulario.chavePix}
                onChange={(e) => setDadosFormulario({ ...dadosFormulario, chavePix: e.target.value })}
                className="input"
                required
              />
            </div>
          )}

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