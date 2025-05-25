import api from './api';

export const vendasService = {
  // Retorna todas as vendas
  getAllVendas: async () => {
    const response = await api.get('/vendas');
    return response.data;
  },

  // Retorna uma venda específica
  getVendaById: async (id) => {
    const response = await api.get(`/vendas/${id}`);
    return response.data;
  },

  // Retorna vendas com paginação
  getVendasPaginadas: async (pageNumber, pageSize) => {
    const response = await api.get('/vendas/pagination', {
      params: { pageNumber, pageSize }
    });
    return response.data;
  },

  // Filtrar vendas por data
  getVendasPorData: async (dataInicio, dataFim, pageNumber, pageSize) => {
    const response = await api.get('/vendas/filter/pagination/date', {
      params: { dataInicio, dataFim, pageNumber, pageSize }
    });
    return response.data;
  },

  // Filtrar vendas por mês
  getVendasPorMes: async (mes, ano, pageNumber, pageSize) => {
    const response = await api.get('/vendas/filter/pagination/month', {
      params: { mes, ano, pageNumber, pageSize }
    });
    return response.data;
  },

  // Filtrar vendas por tipo de pagamento
  getVendasPorPagamento: async (tipoPagamento, pageNumber, pageSize) => {
    const response = await api.get('/vendas/filter/pagination/payment', {
      params: { tipoPagamento, pageNumber, pageSize }
    });
    return response.data;
  },

  // Filtrar vendas por dias
  getVendasPorDias: async (dias, pageNumber, pageSize) => {
    const response = await api.get('/vendas/filter/pagination/days', {
      params: { dias, pageNumber, pageSize }
    });
    return response.data;
  },

  // Realizar uma venda
  realizarVenda: async (metodoPagamento) => {
    const response = await api.post('/vendas', JSON.stringify(metodoPagamento));
    return response.data;
  },
};