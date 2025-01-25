import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tsconfigPaths from 'vite-tsconfig-paths';
import { viteStaticCopy } from 'vite-plugin-static-copy';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    react(),
    tsconfigPaths(),
    viteStaticCopy({
      targets: [
        {
          src: 'src/assets/images/**',
          dest: 'assets/images'
        },
        {
          src: 'src/assets/primeng/**.min.css',
          dest: 'assets/primeng'
        },
        {
          src: 'node_modules/primereact/resources/themes/mdc-light-indigo/theme.css',
          dest: 'assets/primeng/themes/mdc-light-indigo'
        },
        {
          src: 'node_modules/primereact/resources/themes/mdc-dark-indigo/theme.css',
          dest: 'assets/primeng/themes/mdc-dark-indigo'
        }
      ]
    })
  ],
  define: {
    "process.env": {},
  }
})
