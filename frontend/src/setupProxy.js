const { createProxyMiddleware } = require('http-proxy-middleware');

module.exports = function(app) {
  app.use(
    '/api',
    createProxyMiddleware({
      target: 'http://localhost:5000',
      xfwd: true
    })
  );
  app.use(
    '/swagger',
    createProxyMiddleware({
      target: 'http://localhost:5000',
      xfwd: true
    })
  );
  app.use(
    '/docs',
    createProxyMiddleware({
      target: 'http://localhost:5000',
      xfwd: true
    })
  );
  app.use(
    '/jobs',
    createProxyMiddleware({
      target: 'http://localhost:5000',
      xfwd: true
    })
  );
};