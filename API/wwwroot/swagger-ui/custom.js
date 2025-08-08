
(function () {
     const originalFetch = window.fetch;

     window.fetch = async (...args) => {
          const [resource, config] = args;

          const response = await originalFetch(...args);

          if (resource.includes('/login') && response.ok) {
               const clone = response.clone();
               const data = await clone.json();

               const token = data.token;
               if (token && window.ui) {
                    window.ui.preauthorizeApiKey('Bearer', `${token}`);
                    console.log('[Swagger] Token JWT aplicado automaticamente.');
               }
          }

          return response;
     };
})();
