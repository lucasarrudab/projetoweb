import { rawApi } from './api';
import { api } from './api';

export const produtoService = {
  getAll: async () => {
    const response = await rawApi.get('/Produto');
    return response.data;
  },

  getAllPaginated: async (pageNumber = 1, pageSize = 10) => {
    const response = await rawApi.get('/Produto/pagination', {
      params: { pageNumber, pageSize },
    });
    return {
      data: response.data,
      pagination: JSON.parse(response.headers['x-pagination']),
    };
  },

  getFilteredByNome: async (nome, pageNumber = 1, pageSize = 10) => {
    const response = await rawApi.get('/Produto/filter/pagination/nome', {
      params: { nome, pageNumber, pageSize },
    });
    return {
      data: response.data,
      pagination: JSON.parse(response.headers['x-pagination']),
    };
  },

  getFilteredByPreco: async (minPreco, maxPreco, pageNumber = 1, pageSize = 10) => {
    const response = await rawApi.get('/Produto/filter/pagination/preco', {
      params: { precoMin: minPreco, precoMax: maxPreco, pageNumber, pageSize },
    });
    return {
      data: response.data,
      pagination: JSON.parse(response.headers['x-pagination']),
    };
  },

  getById: async (id) => {
    const response = await rawApi.get(`/Produto/${id}`);
    return response.data;
  },

  create: async (produtoData) => {
    const response = await rawApi.post('/Produto', produtoData);
    return response.data;
  },

  update: async (id, produtoData) => {
    const response = await rawApi.put(`/Produto/${id}`, produtoData);
    return response.data;
  },

  addEstoque: async (id, quantidade) => {
    const response = await rawApi.put(`/Produto/estoque/${id}`, quantidade, {
      headers: {
        'Content-Type': 'application/json',
      },
    });
    return response.data;
  },

  delete: async (id) => {
    const response = await rawApi.delete(`/Produto/${id}`);
    return response.data;
  },
};