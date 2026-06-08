import api from './api'

export const consoleService = {
  getAll:  (search = '') => api.get('/consoles', { params: { search } }),
  getById: (id)          => api.get(`/consoles/${id}`),
  create:  (data)        => api.post('/consoles', data),
  update:  (id, data)    => api.put(`/consoles/${id}`, data),
  delete:  (id)          => api.delete(`/consoles/${id}`)
}