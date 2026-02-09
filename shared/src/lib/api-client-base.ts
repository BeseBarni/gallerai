import { AxiosError, type AxiosInstance, type AxiosRequestConfig } from "axios";

let _instance: AxiosInstance | null = null;

export const setAxiosInstance = (instance: AxiosInstance) => {
  _instance = instance;
};

export const getAxiosInstance = () => {
  if (!_instance) {
    throw new Error(
      "Axios Instance not configured! Call setAxiosInstance() first.",
    );
  }
  return _instance;
};

export const customInstance = <T>(
  config: AxiosRequestConfig,
  options?: AxiosRequestConfig,
): Promise<T> => {
  return getAxiosInstance()({ ...config, ...options }).then(({ data }) => data);
};

export type ErrorType<Error> = AxiosError<Error>;
