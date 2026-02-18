import axios from 'axios'
import { toast } from 'react-toastify'

import { env } from './env'

const baseURL = (env.VITE_API_URL ?? '').replace(/\/$/, '')

const axiosInstance = axios.create({
  baseURL,
  headers: { 'Content-Type': 'application/json' },
})

axiosInstance.interceptors.request.use((config) => {
  const authStorage = localStorage.getItem('auth-storage')
  if (authStorage) {
    try {
      const { state } = JSON.parse(authStorage)
      if (state?.token) {
        config.headers.Authorization = `Bearer ${state.token}`
      }
    } catch {
      // Invalid storage, ignore
    }
  }
  return config
})

axiosInstance.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('auth-storage')
      window.location.href = '/login'
    }

    toast.error(error.response?.data?.message ?? 'An error occurred')

    return Promise.resolve(error.response) // Resolve with response to prevent unhandled rejections
  },
)

export default axiosInstance
