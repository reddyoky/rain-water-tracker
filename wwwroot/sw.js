// RAIN WaterTracker — Service Worker
// Temel cache: offline'da login ekranı gösterilir

const CACHE = 'rain-v1';
const OFFLINE_URLS = [
    '/',
    '/Auth/Login',
    '/css/site.css',
    '/js/dashboard.js',
    '/manifest.json'
];

// Kurulum: cache'e temel dosyaları al
self.addEventListener('install', e => {
    e.waitUntil(
        caches.open(CACHE).then(c => c.addAll(OFFLINE_URLS))
    );
    self.skipWaiting();
});

// Aktifleşme: eski cache'leri temizle
self.addEventListener('activate', e => {
    e.waitUntil(
        caches.keys().then(keys =>
            Promise.all(keys.filter(k => k !== CACHE).map(k => caches.delete(k)))
        )
    );
    self.clients.claim();
});

// Fetch: önce network, fail olursa cache
self.addEventListener('fetch', e => {
    // API istekleri cache'lenmez
    if (e.request.url.includes('/api/')) return;

    e.respondWith(
        fetch(e.request)
            .then(res => {
                // Başarılı yanıtı cache'e de koy
                const clone = res.clone();
                caches.open(CACHE).then(c => c.put(e.request, clone));
                return res;
            })
            .catch(() => caches.match(e.request))
    );
});
