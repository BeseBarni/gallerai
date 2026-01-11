import React from 'react'

import { isProdMode } from './general-helpers'

export const TanStackRouterDevtools = isProdMode()
  ? () => null
  : React.lazy(() =>
      import('@tanstack/react-router-devtools').then((res) => ({
        default: res.TanStackRouterDevtools,
      })),
    )

export const ReactQueryDevtools = isProdMode()
  ? () => null
  : React.lazy(() =>
      import('@tanstack/react-query-devtools').then((res) => ({
        default: res.ReactQueryDevtools,
      })),
    )
