import { useState } from 'react'

export default function Login({ onLogin }) {
  const [ehLogin, setEhLogin] = useState(true)
  const [usuario, setUsuario] = useState('')
  const [senha, setSenha] = useState('')
  const [ehAdmin, setEhAdmin] = useState(false)

  const handleSubmit = (e) => {
    e.preventDefault()
    // In a real application, you would validate credentials here
    onLogin(ehAdmin)
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
              onClick={() => setEhLogin(true)}
              className={`w-1/2 py-2 text-sm font-medium rounded-md transition-all duration-200 ${
                ehLogin
                  ? 'bg-white text-gray-900 shadow-sm'
                  : 'text-gray-500 hover:text-gray-900'
              }`}
            >
              Entrar
            </button>
            <button
              onClick={() => setEhLogin(false)}
              className={`w-1/2 py-2 text-sm font-medium rounded-md transition-all duration-200 ${
                !ehLogin
                  ? 'bg-white text-gray-900 shadow-sm'
                  : 'text-gray-500 hover:text-gray-900'
              }`}
            >
              Registrar-se
            </button>
          </div>
        </div>
        <form className="mt-8 space-y-6" onSubmit={handleSubmit}>
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

          {ehLogin && (
            <div className="flex items-center">
              <input
                id="admin-toggle"
                name="admin-toggle"
                type="checkbox"
                className="h-4 w-4 text-pink-600 focus:ring-pink-500 border-gray-300 rounded"
                checked={ehAdmin}
                onChange={(e) => setEhAdmin(e.target.checked)}
              />
              <label htmlFor="admin-toggle" className="ml-2 block text-sm text-gray-700">
                Login como Admin
              </label>
            </div>
          )}

          <button
            type="submit"
            className="w-full flex justify-center py-3 px-4 border border-transparent rounded-lg shadow-sm text-sm font-medium text-white bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-pink-500"
          >
            {ehLogin ? 'Entrar' : 'Criar conta'}
          </button>
        </form>
      </div>
    </div>
  )
}