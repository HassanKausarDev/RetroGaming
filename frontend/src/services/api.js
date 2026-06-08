import axios from 'axios'

const api = axios.create({
  baseURL: 'https://localhost:7262/api',
  headers: {
    'Content-Type': 'application/json'
  }
})

api.interceptors.response.use(
  response => response,
  error => {
    const message = error.response?.data?.message || 'An unexpected error occurred'
    return Promise.reject(new Error(message))
  }
)

export default api