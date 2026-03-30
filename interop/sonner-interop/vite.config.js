import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  define: {
    'process.env.NODE_ENV': JSON.stringify('production')
  },
  build: {
    outDir: '../src/ShadcnBlazor/Components/Sonner/Stubs',
    lib: {
      entry: './sonner-interop.js',
      name: 'Sonner',
      formats: ['iife'],
      fileName: () => 'SonnerStub.razor.js'
    },
    rollupOptions: {
      external: [],
      output: {
        globals: {}
      }
    }
  }
});
