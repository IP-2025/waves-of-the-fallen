import { defineConfig } from 'vite';

export default defineConfig({
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost', // ← backend exposed via Ingress
        changeOrigin: true
      }
    }
  }
});
