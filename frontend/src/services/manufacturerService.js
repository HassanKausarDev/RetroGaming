import api from './api'

export const manufacturerService = {
  getAll:     ()         => api.get('/manufacturers'),
  getById:    (id)       => api.get(`/manufacturers/${id}`),
  getMapData: ()         => api.get('/manufacturers/map'),
  create:     (data)     => api.post('/manufacturers', data),
  update:     (id, data) => api.put(`/manufacturers/${id}`, data),
  delete:     (id)       => api.delete(`/manufacturers/${id}`)
}