import axios from 'axios'

import { env } from './env'

const baseURL = (env.VITE_API_URL ?? '').replace(/\/$/, '')

const axiosInstance = axios.create({
  baseURL,
  headers: { 'Content-Type': 'application/json' },
})

export default axiosInstance
