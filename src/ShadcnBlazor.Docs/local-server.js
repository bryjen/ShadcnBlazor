// dotnet publish -c Release -o ./publish && node local-server.js
const http = require('http');
const fs = require('fs');
const path = require('path');
const url = require('url');

// Point to ./publish/wwwroot since we're in the Docs folder
const wwwrootDir = path.join(__dirname, './publish/wwwroot');

const server = http.createServer((req, res) => {
  const parsedUrl = url.parse(req.url, true);
  let pathname = parsedUrl.pathname;

  // Remove /ShadcnBlazor prefix
  if (pathname.startsWith('/ShadcnBlazor')) {
    pathname = pathname.slice('/ShadcnBlazor'.length);
  }

  if (pathname === '' || pathname === '/') {
    pathname = '/index.html';
  }

  let filePath = path.join(wwwrootDir, pathname);

  const mimeTypes = {
    '.html': 'text/html',
    '.css': 'text/css',
    '.js': 'application/javascript',
    '.json': 'application/json',
    '.png': 'image/png',
    '.jpg': 'image/jpeg',
    '.jpeg': 'image/jpeg',
    '.svg': 'image/svg+xml',
    '.ico': 'image/x-icon',
    '.woff': 'font/woff',
    '.woff2': 'font/woff2',
    '.ttf': 'font/ttf',
    '.eot': 'application/vnd.ms-fontobject',
    '.wasm': 'application/wasm',
    '.map': 'application/json',
    '.txt': 'text/plain',
  };

  fs.readFile(filePath, (err, data) => {
    if (err) {
      // File doesn't exist — for SPA routing, return index.html
      fs.readFile(path.join(wwwrootDir, 'index.html'), (err, indexData) => {
        if (err) {
          res.writeHead(500, { 'Content-Type': 'text/plain' });
          res.end('Server error');
          return;
        }
        res.writeHead(200, { 'Content-Type': 'text/html' });
        res.end(indexData);
      });
    } else {
      // File found — serve it with correct MIME type
      const ext = path.extname(filePath).toLowerCase();
      const mimeType = mimeTypes[ext] || 'application/octet-stream';
      res.writeHead(200, { 'Content-Type': mimeType });
      res.end(data);
    }
  });
});

const PORT = 3000;
server.listen(PORT, () => {
  console.log(`\n🚀 Local staging server running at http://localhost:${PORT}/ShadcnBlazor/\n`);
  console.log('Serving from:', wwwrootDir);
  console.log('Press Ctrl+C to stop\n');
});
