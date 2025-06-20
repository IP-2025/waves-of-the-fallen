import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue(), vueDevTools()],
  resolve: {
    alias: {
      vue: 'vue/dist/vue.esm-bundler.js',
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  build: {
    commonjsOptions: {
      include: [/node_modules/]
    }
  },
  base: '/waves-of-the-fallen',
  server: {
    proxy: {
      '/api/v1': {
        target: 'http://waves-of-the-fallen.duckdns.org:32424',
        // target: 'http://localhost:3000',
        ws: true,
        changeOrigin: true,
        rewrite: (path) => path
      }
    }
  }
})
