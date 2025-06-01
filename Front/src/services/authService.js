import api from './api';

export const authService = {
  login: async (username, password) => {
    const response = await api.post('/auth/login', { username, password });
    const { token, refreshToken, expiration } = response.data;
    localStorage.setItem('token', token);
    localStorage.setItem('refreshToken', refreshToken);
    localStorage.setItem('tokenExpiration', expiration);
    return response.data;
  },

  register: async (username, email, password) => {
    return await api.post('/auth/register', { username, email, password });
  },

  logout: async () => {
    try {
      await api.post('/auth/logout');
    } finally {
      localStorage.removeItem('token');
      localStorage.removeItem('refreshToken');
      localStorage.removeItem('tokenExpiration');
    }
  },

  refreshToken: async () => {
    const token = localStorage.getItem('token');
    const refreshToken = localStorage.getItem('refreshToken');
    
    const response = await api.post('/auth/refresh-token', {
      accessToken: token,
      refreshToken: refreshToken
    });
    
    const { accessToken, refreshToken: newRefreshToken } = response.data;
    localStorage.setItem('token', accessToken);
    localStorage.setItem('refreshToken', newRefreshToken);
    
    return response.data;
  },

  getAllUsers: async () => {
    const response = await api.get('/auth/usuarios');
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/auth/${id}`);
    return response.data;
  },

  update: async (id, userData) => {
    const response = await api.put(`/auth/${id}`, userData);
    return response.data;
  },

  updatePassword: async (id, userData) => {
    const response = await api.put(`/auth/senha/${id}`, userData);
    return response.data;
  }

};