import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'

import './index.css'

import { LoadingBoundary } from './app/app-loading-boundary'
import AppProvider from './app/providers'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <LoadingBoundary>
      <AppProvider />
    </LoadingBoundary>
  </StrictMode>,
)
