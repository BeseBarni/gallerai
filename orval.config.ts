import { defineConfig } from 'orval'

export default defineConfig({
  gallerai: {
    input: './swagger.json',
    output: {
      mode: 'split',
      target: './shared/src/api/gallerai/api.gen.ts',
      schemas: './shared/src/api/schemas',
      client: 'react-query',
      clean: true,
      httpClient: 'axios',
      prettier: true,
      override: {
        mutator: {
          path: './shared/src/lib/api-client-base.ts',
          name: 'customInstance',
        },
      },
    },
  },
  'gallerai-worker': {
    input: './swagger.json',
    output: {
      client: 'fetch',
      mode: 'split',
      target: './shared/src/api/worker/worker.gen.ts',
      schemas: './shared/src/api/schemas',
      httpClient: 'fetch',
      prettier: true,
      clean: true,
    },
  },
})
