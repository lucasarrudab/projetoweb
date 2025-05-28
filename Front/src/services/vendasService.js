import api from './api';
import { rawApi } from './api';

export const vendasService = {
  // Retorna todas as vendas
  getAllVendas: async () => {
    const response = await rawApi.get('/Vendas');
    return response.data;
  },

  // Retorna uma venda específica
  getVendaById: async (id) => {
    const response = await rawApi.get(`/Vendas/${id}`);
    return response.data;
  },

  // Retorna vendas com paginação
  getVendasPaginadas: async (pageNumber, pageSize) => {
    const response = await rawApi.get('/Vendas/pagination', {
      params: { pageNumber, pageSize }
    });
    return response.data;
  },

  // Filtrar vendas por data
  getVendasPorData: async (dataInicio, dataFim, pageNumber, pageSize) => {
    const response = await rawApi.get('/Vendas/filter/pagination/date', {
      params: { dataInicio, dataFim, pageNumber, pageSize }
    });
    return response.data;
  },

  // Filtrar vendas por mês
  getVendasPorMes: async (mes, ano, pageNumber, pageSize) => {
    const response = await rawApi.get('/Vendas/filter/pagination/month', {
      params: { mes, ano, pageNumber, pageSize }
    });
    return response.data;
  },

  // Filtrar vendas por tipo de pagamento
  getVendasPorPagamento: async (tipoPagamento, pageNumber, pageSize) => {
    const response = await rawApi.get('/Vendas/filter/pagination/payment', {
      params: { tipoPagamento, pageNumber, pageSize }
    });
    return response.data;
  },

  // Filtrar vendas por dias
  getVendasPorDias: async (dias, pageNumber, pageSize) => {
    const response = await rawApi.get('/Vendas/filter/pagination/days', {
      params: { dias, pageNumber, pageSize }
    });
    return response.data;
  },

  // Realizar uma venda
  realizarVenda: async (nomeMetodoPagamento) => {
    const response = await rawApi.post('/Vendas', JSON.stringify(nomeMetodoPagamento)); 
    return response.data;
  }

  
};