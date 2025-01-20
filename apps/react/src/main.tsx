import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { RouterProvider } from 'react-router-dom'
import router from './routes'
import { AppSessionProvider } from '@shared/session'
import { AppThemeProvider } from '@shared/theme'

import '@assets/css/style.bundle.css'
import '@assets/plugins/global/plugins.bundle.css'
import 'sweetalert2/dist/sweetalert2.min.css'
import "primereact/resources/themes/mdc-dark-indigo/theme.css"
import '@assets/primeng/primeng-customize.css'
import '@assets/primeng/primeng-customize-dark.css'
import './index.css'
import { AppCartProvider } from '@shared/carts'

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <AppThemeProvider>
            <AppSessionProvider>
                <AppCartProvider>
                    <RouterProvider router={router} />
                </AppCartProvider>
            </AppSessionProvider>
        </AppThemeProvider>
    </StrictMode>
)
