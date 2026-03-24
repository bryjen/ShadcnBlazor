import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  define: {
    'process.env.NODE_ENV': JSON.stringify('production')
  },
  build: {
    lib: {
      entry: './sonner-interop.js',
      name: 'Sonner',
      formats: ['iife'],
      fileName: () => 'sonner-interop.iife.js'
    },
    rollupOptions: {
      external: [],
      output: {
        globals: {}
      }
    }
  }
});
