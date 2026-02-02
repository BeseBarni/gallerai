import axios, { AxiosError, type AxiosRequestConfig } from 'axios'

import { env } from './env'

export type PromiseWithCancel<T> = Promise<T> & {
  cancel: () => void
}
const baseURL = (env.VITE_API_URL ?? '').replace(/\/$/, '')
const AXIOS_INSTANCE = axios.create({
  baseURL: baseURL,
  headers: {
    'Content-Type': 'application/json',
  },
})

export const axiosInstance = <T>(
  config: AxiosRequestConfig,
  options?: AxiosRequestConfig,
): Promise<T> => {
  const promise = AXIOS_INSTANCE({
    ...config,
    ...options,
  }).then(({ data }) => data)
  return promise
}

export type ErrorType<Error> = AxiosError<Error>

export type BodyType<BodyData> = BodyData
