export const salvarCarrinho = (carrinho) => {
  try {
    localStorage.setItem('carrinho', JSON.stringify(carrinho))
  } catch (error) {
    console.error('Erro ao salvar carrinho no localStorage:', error)
  }
}

export const carregarCarrinho = () => {
  try {
    const carrinhoJSON = localStorage.getItem('carrinho')
    return carrinhoJSON ? JSON.parse(carrinhoJSON) : []
  } catch (error) {
    console.error('Erro ao carregar carrinho do localStorage:', error)
    return []
  }
}

export const salvarVendas = (vendas) => {
  try {
    localStorage.setItem('vendas', JSON.stringify(vendas))
  } catch (error) {
    console.error('Erro ao salvar vendas no localStorage:', error)
  }
}

export const carregarVendas = () => {
  try {
    const vendasJSON = localStorage.getItem('vendas')
    return vendasJSON ? JSON.parse(vendasJSON) : []
  } catch (error) {
    console.error('Erro ao carregar vendas do localStorage:', error)
    return []
  }
}