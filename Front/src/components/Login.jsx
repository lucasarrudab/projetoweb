import { useState } from 'react'
import { authService } from '../services/authService'
import { jwtDecode } from 'jwt-decode'
import { salvarLogin } from '../services/localStorage'

export default function Login({ onLogin }) {
  const [isLogin, setIsLogin] = useState(true)
  const [usuario, setUsuario] = useState('')
  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [isAdmin, setIsAdmin] = useState(false)
  const [erro, setErro] = useState('')

   const handleSubmit = async (e) => {
    e.preventDefault()
    setErro('')

    try {
      if (isLogin) {
        const { token, refreshToken, expiration } = await authService.login(usuario, senha)
        salvarLogin({ token, refreshToken, expiration })
        localStorage.setItem('token', token)
        const decoded = jwtDecode(token)
        const isAdmin = decoded?.role === 'Admin'
        onLogin(isAdmin)
      } else {
        await authService.register(usuario, email, senha)
        const { token } = await authService.login(usuario, senha)
        localStorage.setItem('token', token)
        const decoded = jwtDecode(token)
        const isAdmin = decoded?.role === 'Admin'
        onLogin(isAdmin)
        setIsLogin(true)
      }
    } catch (error) {
      console.error('Erro de login:', error)
      setErro(error.response?.data?.message || 'Ocorreu um erro. Tente novamente.')
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-pink-100 to-purple-100 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full bg-white rounded-xl shadow-2xl p-8 space-y-8">
        <div>
          <h2 className="mt-2 text-center text-3xl font-extrabold text-gray-900">
            Bem-vindo A Nossa Padaria
          </h2>
          <div className="mt-4 flex rounded-lg bg-gray-100 p-1">
            <button
              onClick={() => setIsLogin(true)}
              className={`w-1/2 py-2 text-sm font-medium rounded-md transition-all duration-200 ${
                isLogin
                  ? 'bg-white text-gray-900 shadow-sm'
                  : 'text-gray-500 hover:text-gray-900'
              }`}
            >
              Entrar
            </button>
            <button
              onClick={() => setIsLogin(false)}
              className={`w-1/2 py-2 text-sm font-medium rounded-md transition-all duration-200 ${
                !isLogin
                  ? 'bg-white text-gray-900 shadow-sm'
                  : 'text-gray-500 hover:text-gray-900'
              }`}
            >
              Registrar-se
            </button>
          </div>
        </div>
        <form className="mt-8 space-y-6" onSubmit={handleSubmit}>
          {erro && (
            <div className="bg-red-50 border border-red-200 text-red-600 px-4 py-3 rounded-lg">
              {erro}
            </div>
          )}
          
          {!isLogin && (<div>
            <label htmlFor="email" className="block text-sm font-medium text-gray-700">
              Email
              </label>  
              <input
                id="email"
                name="email"
                type="email"
                required
                className="mt-1 block w-full px-3 py-2 bg-gray-50 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-pink-500 focus:border-transparent"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
              />
            </div>)}
          <div className="space-y-4">
            <div>
              <label htmlFor="usuario" className="block text-sm font-medium text-gray-700">
                Usuário
              </label>
              <input
                id="usuario"
                name="usuario"
                type="text"
                required
                className="mt-1 block w-full px-3 py-2 bg-gray-50 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-pink-500 focus:border-transparent"
                value={usuario}
                onChange={(e) => setUsuario(e.target.value)}
              />
            </div>
            <div>
              <label htmlFor="senha" className="block text-sm font-medium text-gray-700">
                Senha
              </label>
              <input
                id="senha"
                name="senha"
                type="password"
                required
                className="mt-1 block w-full px-3 py-2 bg-gray-50 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-pink-500 focus:border-transparent"
                value={senha}
                onChange={(e) => setSenha(e.target.value)}
              />
            </div>
          </div>

          <button
            type="submit"
            className="w-full flex justify-center py-3 px-4 border border-transparent rounded-lg shadow-sm text-sm font-medium text-white bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-pink-500"
          >
            {isLogin ? 'Entrar' : 'Criar conta'}
          </button>
        </form>
      </div>
    </div>
  )
}