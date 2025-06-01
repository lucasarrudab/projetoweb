import { useState, useEffect } from 'react'
import { PencilIcon, TrashIcon } from '@heroicons/react/24/outline'
import { Dialog, Transition, Tab } from '@headlessui/react'
import { Fragment } from 'react'
import { authService } from '../services/authService'

export default function UserList() {
  const [usuarios, setUsuarios] = useState([])
  const [usuarioEditando, setUsuarioEditando] = useState(null)
  const [estaAberto, setEstaAberto] = useState(false)
  const [usuarioParaExcluir, setUsuarioParaExcluir] = useState(null)
  const [estaAbertoConfirmacao, setEstaAbertoConfirmacao] = useState(false)
  const [senhas, setSenhas] = useState({
    OldPassword: '',
    NewPassword: ''
  })

  useEffect(() => {
    carregarUsuarios()
  }, [])

  const carregarUsuarios = async () => {
    try {
      const dados = await authService.getAllUsers()
      setUsuarios(dados)
    } catch (erro) {
      console.error('Erro ao carregar usuários:', erro)
    }
  }

  const handleEdit = (usuario) => {
    setUsuarioEditando(usuario)
    setSenhas({ OldPassword: '', NewPassword: '' })
    setEstaAberto(true)
  }

  const handleDelete = (usuario) => {
    setUsuarioParaExcluir(usuario)
    setEstaAbertoConfirmacao(true)
  }

  const confirmarExclusao = async () => {
    try {
      await authService.delete(usuarioParaExcluir.id)
      setUsuarios(usuarios.filter(u => u.id !== usuarioParaExcluir.id))
      setEstaAbertoConfirmacao(false)
      setUsuarioParaExcluir(null)
    } catch (erro) {
      console.error('Erro ao excluir usuário:', erro)
    }
  }

  const handleSubmitInfo = async (e) => {
    e.preventDefault()
    try {
      await authService.update(usuarioEditando.id, {
        userName: usuarioEditando.userName,
        email: usuarioEditando.email,
      })
      setUsuarios(usuarios.map(u => 
        u.id === usuarioEditando.id ? usuarioEditando : u
      ))
      setEstaAberto(false)
      setUsuarioEditando(null)
    } catch (erro) {
      console.error('Erro ao atualizar usuário:', erro)
    }
  }

  const handleSubmitPassword = async (e) => {
    e.preventDefault()
    try {
      await authService.updatePassword(usuarioEditando.id, {
        OldPassword: senhas.OldPassword,
        NewPassword: senhas.NewPassword
      })
      setEstaAberto(false)
      setUsuarioEditando(null)
      setSenhas({ OldPassword: '', NewPassword: '' })
    } catch (erro) {
      console.error('Erro ao atualizar senha:', erro)
    }
  }

  return (
    <div className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
      <div className="px-4 py-6 sm:px-0">
        <h1 className="text-2xl font-semibold text-gray-900 mb-6">Gerenciamento de Usuários</h1>
        
        <div className="bg-white shadow overflow-hidden sm:rounded-lg">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  ID
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Nome
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Email
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Tipo
                </th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Ações
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {usuarios.map((usuario) => (
                <tr key={usuario.id}>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {usuario.id}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {usuario.userName}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {usuario.email}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {usuario.roles.includes("admin") ? 'Administrador' : usuario.roles.includes("Gerente") ? 'Gerente' : 'Usuário'}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                    <button
                      onClick={() => handleEdit(usuario)}
                      className="text-indigo-600 hover:text-indigo-900 mr-4"
                    >
                      <PencilIcon className="h-5 w-5" />
                    </button>
                    <button
                      onClick={() => handleDelete(usuario)}
                      className="text-red-600 hover:text-red-900"
                    >
                      <TrashIcon className="h-5 w-5" />
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        <Transition appear show={estaAberto} as={Fragment}>
          <Dialog
            as="div"
            className="fixed inset-0 z-10 overflow-y-auto"
            onClose={() => setEstaAberto(false)}
          >
            <div className="min-h-screen px-4 text-center">
              <Dialog.Overlay className="fixed inset-0 bg-black opacity-30" />
              <span
                className="inline-block h-screen align-middle"
                aria-hidden="true"
              >
                &#8203;
              </span>
              <div className="inline-block w-full max-w-md p-6 my-8 overflow-hidden text-left align-middle transition-all transform bg-white shadow-xl rounded-2xl">
                <Dialog.Title
                  as="h3"
                  className="text-lg font-medium leading-6 text-gray-900 mb-4"
                >
                  Editar Usuário
                </Dialog.Title>

                <Tab.Group>
                  <Tab.List className="flex space-x-1 rounded-xl bg-blue-900/20 p-1 mb-4">
                    <Tab
                      className={({ selected }) =>
                        `w-full rounded-lg py-2.5 text-sm font-medium leading-5 text-blue-700
                        ${selected
                          ? 'bg-white shadow'
                          : 'text-blue-100 hover:bg-white/[0.12] hover:text-white'
                        }`
                      }
                    >
                      Informações
                    </Tab>
                    <Tab
                      className={({ selected }) =>
                        `w-full rounded-lg py-2.5 text-sm font-medium leading-5 text-blue-700
                        ${selected
                          ? 'bg-white shadow'
                          : 'text-blue-100 hover:bg-white/[0.12] hover:text-white'
                        }`
                      }
                    >
                      Senha
                    </Tab>
                  </Tab.List>
                  <Tab.Panels>
                    <Tab.Panel>
                      <form onSubmit={handleSubmitInfo} className="space-y-4">
                        <div>
                          <label className="block text-sm font-medium text-gray-700">
                            Nome
                          </label>
                          <input
                            type="text"
                            value={usuarioEditando?.userName || ''}
                            onChange={(e) => setUsuarioEditando({
                              ...usuarioEditando,
                              userName: e.target.value
                            })}
                            className="input"
                          />
                        </div>
                        <div>
                          <label className="block text-sm font-medium text-gray-700">
                            Email
                          </label>
                          <input
                            type="email"
                            value={usuarioEditando?.email || ''}
                            onChange={(e) => setUsuarioEditando({
                              ...usuarioEditando,
                              email: e.target.value
                            })}
                            className="input"
                          />
                        </div>
                        <div className="flex justify-end space-x-3">
                          <button
                            type="button"
                            onClick={() => setEstaAberto(false)}
                            className="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 hover:bg-gray-200 rounded-md"
                          >
                            Cancelar
                          </button>
                          <button
                            type="submit"
                            className="px-4 py-2 text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 rounded-md"
                          >
                            Salvar
                          </button>
                        </div>
                      </form>
                    </Tab.Panel>
                    <Tab.Panel>
                      <form onSubmit={handleSubmitPassword} className="space-y-4">
                        <div>
                          <label className="block text-sm font-medium text-gray-700">
                            Senha Atual
                          </label>
                          <input
                            type="password"
                            value={senhas.OldPassword}
                            onChange={(e) => setSenhas({
                              ...senhas,
                              OldPassword: e.target.value
                            })}
                            className="input"
                            required
                          />
                        </div>
                        <div>
                          <label className="block text-sm font-medium text-gray-700">
                            Nova Senha
                          </label>
                          <input
                            type="password"
                            value={senhas.NewPassword}
                            onChange={(e) => setSenhas({
                              ...senhas,
                              NewPassword: e.target.value
                            })}
                            className="input"
                            required
                          />
                        </div>
                        <div className="flex justify-end space-x-3">
                          <button
                            type="button"
                            onClick={() => setEstaAberto(false)}
                            className="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 hover:bg-gray-200 rounded-md"
                          >
                            Cancelar
                          </button>
                          <button
                            type="submit"
                            className="px-4 py-2 text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 rounded-md"
                          >
                            Alterar Senha
                          </button>
                        </div>
                      </form>
                    </Tab.Panel>
                  </Tab.Panels>
                </Tab.Group>
              </div>
            </div>
          </Dialog>
        </Transition>

        <Transition appear show={estaAbertoConfirmacao} as={Fragment}>
          <Dialog
            as="div"
            className="fixed inset-0 z-10 overflow-y-auto"
            onClose={() => setEstaAbertoConfirmacao(false)}
          >
            <div className="min-h-screen px-4 text-center">
              <Dialog.Overlay className="fixed inset-0 bg-black opacity-30" />
              <span
                className="inline-block h-screen align-middle"
                aria-hidden="true"
              >
                &#8203;
              </span>
              <div className="inline-block w-full max-w-md p-6 my-8 overflow-hidden text-left align-middle transition-all transform bg-white shadow-xl rounded-2xl">
                <Dialog.Title
                  as="h3"
                  className="text-lg font-medium leading-6 text-gray-900"
                >
                  Confirmar Exclusão
                </Dialog.Title>
                <div className="mt-2">
                  <p className="text-sm text-gray-500">
                    Tem certeza que deseja excluir o usuário {usuarioParaExcluir?.nome}?
                  </p>
                </div>
                <div className="mt-6 flex justify-end space-x-3">
                  <button
                    type="button"
                    onClick={() => setEstaAbertoConfirmacao(false)}
                    className="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 hover:bg-gray-200 rounded-md"
                  >
                    Cancelar
                  </button>
                  <button
                    type="button"
                    onClick={confirmarExclusao}
                    className="px-4 py-2 text-sm font-medium text-white bg-red-600 hover:bg-red-700 rounded-md"
                  >
                    Excluir
                  </button>
                </div>
              </div>
            </div>
          </Dialog>
        </Transition>
      </div>
    </div>
  )
}