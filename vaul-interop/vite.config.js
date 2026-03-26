import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  define: {
    'process.env.NODE_ENV': JSON.stringify('production')
  },
  build: {
    lib: {
      entry: './vaul-interop.js',
      name: 'Vaul',
      formats: ['iife'],
      fileName: () => 'vaul-interop.iife.js'
    },
    rollupOptions: {
      external: [],
      output: {
        globals: {}
      }
    }
  }
});
