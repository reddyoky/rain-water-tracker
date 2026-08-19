/**
 * dashboard.js
 * 
 * Fetch API ile sayfa YENİLENMEDEN su ekleme işlemi.
 * Bardak butonuna tıklanır → POST /api/water/drink → DOM güncellenir.
 */

/**
 * Bardak butonuna tıklanınca çağrılır.
 * @param {number} amountMl - İçilen su miktarı (ml)
 */
async function drinkWater(amountMl) {
    try {
        // 1. API'ye POST isteği gönder (sayfa yenilenmez!)
        const response = await fetch('/api/water/drink', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ amountMl })
        });

        if (!response.ok) throw new Error('Sunucu hatası');

        // 2. Sunucudan güncel veriyi al
        const data = await response.json();

        // 3. Ekrandaki progress bar'ları güncelle
        updateUserCards(data.users);

        // 4. Timeline'a yeni kayıt ekle
        addTimelineItem(data.recentLogs[0]);

        // 5. Toast bildirimi göster
        showToast(`✅ ${amountMl} ml eklendi! 💧`);

    } catch (err) {
        showToast('❌ Bir hata oluştu, tekrar dene.', true);
        console.error(err);
    }
}

/**
 * Tüm kullanıcı kartlarını güncel verilerle günceller.
 * @param {Array} users - [{id, displayName, dailyGoalMl, totalMl, percentage}]
 */
function updateUserCards(users) {
    users.forEach(user => {
        // Su dolum animasyonu (height CSS transition ile)
        const fill = document.getElementById(`fill-${user.id}`);
        if (fill) fill.style.height = `${user.percentage}%`;

        // Yüzde yazısı
        const pct = document.getElementById(`pct-${user.id}`);
        if (pct) pct.textContent = `${user.percentage}%`;

        // ml yazısı
        const total = document.getElementById(`total-${user.id}`);
        if (total) {
            // Sayaç animasyonu
            animateNumber(total, parseInt(total.textContent), user.totalMl, ' ml');
        }
    });
}

/**
 * Sayıyı animasyonla günceller (500ms içinde).
 */
function animateNumber(el, from, to, suffix = '') {
    const duration = 500;
    const start = performance.now();

    function step(timestamp) {
        const elapsed = timestamp - start;
        const progress = Math.min(elapsed / duration, 1);
        // Easing
        const eased = 1 - Math.pow(1 - progress, 3);
        const current = Math.round(from + (to - from) * eased);
        el.textContent = current + suffix;
        if (progress < 1) requestAnimationFrame(step);
    }

    requestAnimationFrame(step);
}

/**
 * Timeline'ın en üstüne yeni kayıt ekler.
 * @param {object} log - {displayName, amountMl, drankAt}
 */
function addTimelineItem(log) {
    if (!log) return;

    const timeline = document.getElementById('timeline');
    if (!timeline) return;

    // "az önce" metni
    const when = 'az önce';

    // Yeni timeline elemanı oluştur
    const item = document.createElement('div');
    item.className = 'timeline-item';
    item.innerHTML = `
        <span class="tl-dot"></span>
        <div class="tl-content">
            <strong>${escHtml(log.displayName)}</strong>
            <span class="tl-amount">${log.amountMl} ml içti</span>
            <span class="tl-time">${when}</span>
        </div>
    `;

    // Boş mesaj varsa kaldır
    const empty = timeline.querySelector('.timeline-empty');
    if (empty) empty.remove();

    // En üste ekle (en yeni en üstte)
    timeline.insertBefore(item, timeline.firstChild);

    // Maksimum 10 kayıt göster
    const items = timeline.querySelectorAll('.timeline-item');
    if (items.length > 10) items[items.length - 1].remove();
}

/**
 * XSS koruması için HTML karakterlerini kaçır.
 */
function escHtml(str) {
    const div = document.createElement('div');
    div.textContent = str;
    return div.innerHTML;
}

/**
 * Ekranın altında kısa süre bir toast mesajı gösterir.
 * @param {string} msg     - Mesaj
 * @param {boolean} isError - true ise kırmızı gösterilir
 */
function showToast(msg, isError = false) {
    const toast = document.getElementById('toast');
    if (!toast) return;

    toast.textContent = msg;
    toast.style.background = isError
        ? 'linear-gradient(135deg, #c0392b, #922b21)'
        : 'linear-gradient(135deg, #2d5986, #1a3a5c)';

    toast.classList.add('show');

    setTimeout(() => {
        toast.classList.remove('show');
    }, 2800);
}
