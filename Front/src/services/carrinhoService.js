import { rawApi } from "./api";

export const carrinhoService = {
  criarCarrinho: async () => {
    const response = await rawApi.post('/Carrinho');
    return response.data;
  },

  adicionarProdutoCarrinho: async (code) => {
    const response = await rawApi.put('/Carrinho', code);
    return response.data;
  },

  buscarCarrinho: async () => {
  const response = await rawApi.get('/Carrinho')
  return response.data
  },

  buscarCarrinhoPorId: async (id) => {
  const response = await api.get(`/Carrinho/${id}`)
  return response.data
  },

  deletarCarrinho: async (id) => {
  await api.delete(`/Carrinho/${id}`)
  }
};
