import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  define: {
    'process.env.NODE_ENV': JSON.stringify('production')
  },
  build: {
    outDir: '../src/ShadcnBlazor/Components/Drawer/Stubs',
    lib: {
      entry: './vaul-interop.js',
      name: 'Vaul',
      formats: ['iife'],
      fileName: () => 'VaulStub.razor.js'
    },
    rollupOptions: {
      external: [],
      output: {
        globals: {}
      }
    }
  }
});
