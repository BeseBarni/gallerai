let _baseUrl: string | undefined;

export const setBaseUrl = (url: string) => {
  if (!_baseUrl) {
    _baseUrl = url.replace(/\/$/, '');
  }
};

export const workerFetch = async <T>(
  url: string,
  options?: RequestInit
): Promise<T> => {
  if (!_baseUrl) {
    throw new Error('❌ API Client not initialized! Call setBaseUrl() in your worker entry.');
  }

  const response = await fetch(`${_baseUrl}${url}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...options?.headers,
    },
  });

  if (!response.ok) {
    throw new Error(`Worker API Error: ${response.statusText}`);
  }

  return response.json() as Promise<T>;
};

export default workerFetch;