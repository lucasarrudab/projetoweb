import { useState, useEffect } from 'react'
import { format, startOfDay, startOfWeek, startOfMonth, startOfYear, isWithinInterval } from 'date-fns'
import { vendasService } from '../services/vendasService'; 

export default function SalesHistory({ isAdmin }) {
  const [sales, setSales] = useState([])
  const [loading, setLoading] = useState(true)
  const [periodoFiltro, setPeriodoFiltro] = useState('all')
  const [dataInicioPersonalizada, setDataInicioPersonalizada] = useState('')
  const [dataFimPersonalizada, setDataFimPersonalizada] = useState('')

  useEffect(() => {
    async function fetchSales() {
      try {
        const dados = await vendasService.getAllVendas() 
        setSales(dados)
      } catch (erro) {
        console.error('Erro ao buscar vendas:', erro)
      } finally {
        setLoading(false)
      }
    }

    fetchSales()
  }, [])

  const getVendasFiltradas = () => {
    const agora = new Date()
    let dataInicio

    switch (periodoFiltro) {
      case 'day':
        dataInicio = startOfDay(agora)
        break
      case 'week':
        dataInicio = startOfWeek(agora)
        break
      case 'month':
        dataInicio = startOfMonth(agora)
        break
      case 'year':
        dataInicio = startOfYear(agora)
        break
      case 'custom':
        if (dataInicioPersonalizada && dataFimPersonalizada) {
          return sales.filter(venda => {
            const dataVenda = new Date(venda.data)
            return isWithinInterval(dataVenda, {
              start: new Date(dataInicioPersonalizada),
              end: new Date(dataFimPersonalizada)
            })
          })
        }
        return sales
      default:
        return sales
    }

    return sales.filter(venda => new Date(venda.data) >= dataInicio)
  }

  const vendasFiltradas = getVendasFiltradas()
  const totalVendido = vendasFiltradas.reduce((total, venda) => total + venda.carrinho.valorTotalCarrinho, 0)

  if (loading) {
    return <p className="text-center py-12 text-gray-500">Carregando vendas...</p>
  }

  if (sales.length === 0) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500">Nenhum histórico de vendas disponível.</p>
      </div>
    )
  }
  
  console.log(vendasFiltradas);

  return (
    <div className="space-y-6">
      <div className="bg-white rounded-lg shadow-md p-6">
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-2xl font-bold text-gray-900">Histórico de Vendas</h2>
          <div className="flex items-center space-x-4">
            <select
              value={periodoFiltro}
              onChange={(e) => setPeriodoFiltro(e.target.value)}
              className="input"
            >
              <option value="all">Todo o Período</option>
              <option value="day">Hoje</option>
              <option value="week">Esta Semana</option>
              <option value="month">Este Mês</option>
              <option value="year">Este Ano</option>
              <option value="custom">Período Personalizado</option>
            </select>
            
            {periodoFiltro === 'custom' && (
              <div className="flex space-x-2">
                <input
                  type="date"
                  value={dataInicioPersonalizada}
                  onChange={(e) => setDataInicioPersonalizada(e.target.value)}
                  className="input"
                />
                <input
                  type="date"
                  value={dataFimPersonalizada}
                  onChange={(e) => setDataFimPersonalizada(e.target.value)}
                  className="input"
                />
              </div>
            )}
          </div>
        </div>
        

        {isAdmin && (
          <div className="bg-gradient-to-r from-pink-500 to-purple-600 text-white rounded-lg p-6 mb-6">
            <p className="text-lg font-semibold">Total de Vendas</p>
            <p className="text-3xl font-bold">R$ {totalVendido.toFixed(2)}</p>
          </div>
        )}
      
        <div className="space-y-6">
          {vendasFiltradas.map((venda) => (
            <div key={venda.id} className="border rounded-lg p-4">
              <div className="flex justify-between items-start mb-4">
                <div>
                  <p className="text-sm text-gray-500">
                    Pedido #{venda.id}
                  </p>
                  <p className="text-sm text-gray-500">
                    {format(new Date(venda.data), 'PPpp')}
                  </p>
                </div>
                <div className="text-right">
                  <p className="text-lg font-bold text-gray-900">
                    Total: R$ {venda.carrinho.valorTotalCarrinho.toFixed(2)}
                  </p>
                  <p className="text-sm text-gray-500 capitalize">
                    Pago com {venda.metodoPagamento}
                  </p>
                </div>
              </div>
              <div className="space-y-2">
                {venda.carrinho.produtos.map((item) => (
                  <div key={item.id} className="flex justify-between text-sm">
                    <span>{item.produtoNome} x {item.quantidade}</span>
                    <span>R$ {(item.valorUnitario * item.quantidade).toFixed(2)}</span>
                  </div>
                ))}
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}