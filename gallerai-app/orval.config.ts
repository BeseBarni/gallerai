import { defineConfig } from 'orval'

export default defineConfig({
  gallerai: {
    input: './swagger.json',
    output: {
      mode: 'split',
      target: 'src/api/gallerai.gen.ts',
      schemas: 'src/api/schemas',
      client: 'react-query',
      clean: true,
      httpClient: 'axios',
      prettier: true,
      override: {
        mutator: {
          path: 'src/lib/api-client.ts',
          name: 'axiosInstance',
        },
      },
    },
  },
})
