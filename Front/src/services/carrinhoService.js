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
  const response = await rawApi.get(`/Carrinho/${id}`)
  return response.data
  },

  deletarCarrinho: async (id) => {
  await rawApi.delete(`/Carrinho/${id}`)
  },

  alterarQuantidadeProdto: async (id, quantidade) => {
    const response = await rawApi.put(`/quantiadeproduto/${id}`, quantidade)  
    return response.data
  },

  removerProduto: async (id) => {
  return await carrinhoService.alterarQuantidadeProdto(id, 0)
  } 

};
