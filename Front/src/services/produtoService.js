import { rawApi } from './api';

export const produtoService = {
  getAll: async () => {
    const response = await rawApi.get('/produto');
    return response.data;
  },

  getAllPaginated: async (pageNumber = 1, pageSize = 10) => {
    const response = await api.get('/produto/pagination', {
      params: { pageNumber, pageSize },
    });
    return {
      data: response.data,
      pagination: JSON.parse(response.headers['x-pagination']),
    };
  },

  getFilteredByNome: async (nome, pageNumber = 1, pageSize = 10) => {
    const response = await api.get('/produto/filter/pagination/nome', {
      params: { nome, pageNumber, pageSize },
    });
    return {
      data: response.data,
      pagination: JSON.parse(response.headers['x-pagination']),
    };
  },

  getFilteredByPreco: async (minPreco, maxPreco, pageNumber = 1, pageSize = 10) => {
    const response = await api.get('/produto/filter/pagination/preco', {
      params: { precoMin: minPreco, precoMax: maxPreco, pageNumber, pageSize },
    });
    return {
      data: response.data,
      pagination: JSON.parse(response.headers['x-pagination']),
    };
  },

  getById: async (id) => {
    const response = await api.get(`/produto/${id}`);
    return response.data;
  },

  create: async (produtoData) => {
    const response = await api.post('/produto', produtoData);
    return response.data;
  },

  update: async (id, produtoData) => {
    const response = await api.put(`/produto/${id}`, produtoData);
    return response.data;
  },

  addEstoque: async (id, quantidade) => {
    const response = await api.put(`/produto/estoque/${id}`, quantidade, {
      headers: {
        'Content-Type': 'application/json',
      },
    });
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/produto/${id}`);
    return response.data;
  },
};