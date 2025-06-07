// Nom et version du cache pour l'application
const CACHE_NAME = 'gameOnTonight-cache-v1';

// Liste des ressources à mettre en cache pour le fonctionnement hors ligne
const RESOURCES_TO_CACHE = [
    '/',
    '/index.html',
    '/css/app.css',
    '/favicon.png',
    '/icon-192.png',
    '/icon-512.png',
    '/_framework/blazor.webassembly.js',
    '/manifest.webmanifest'
];

// Installation du service worker et mise en cache des ressources essentielles
self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => {
                console.log('[Service Worker] Mise en cache des ressources');
                return cache.addAll(RESOURCES_TO_CACHE);
            })
            .then(() => self.skipWaiting())
    );
});

// Nettoyage des anciennes versions du cache lors de l'activation
self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys().then(cacheNames => {
            return Promise.all(
                cacheNames.map(cacheName => {
                    if (cacheName !== CACHE_NAME) {
                        console.log('[Service Worker] Suppression de l\'ancien cache:', cacheName);
                        return caches.delete(cacheName);
                    }
                })
            );
        }).then(() => self.clients.claim())
    );
});

// Stratégie de récupération des ressources: d'abord depuis le réseau, puis depuis le cache si hors ligne
self.addEventListener('fetch', event => {
    // Ne pas intercepter les requêtes vers l'API (elles échoueront naturellement hors ligne)
    if (event.request.url.includes('/api/')) {
        return;
    }

    event.respondWith(
        fetch(event.request)
            .then(response => {
                // Si la requête a réussi, mettre à jour le cache avec la nouvelle réponse
                if (response && response.status === 200) {
                    const responseClone = response.clone();
                    caches.open(CACHE_NAME)
                        .then(cache => {
                            cache.put(event.request, responseClone);
                        });
                }
                return response;
            })
            .catch(() => {
                // En cas d'échec de la requête réseau, essayer de servir depuis le cache
                console.log('[Service Worker] Récupération depuis le cache pour:', event.request.url);
                return caches.match(event.request);
            })
    );
});
