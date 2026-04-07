/**
 * dispatch.js
 * Location: wwwroot/js/dispatch.js
 *
 * All JavaScript for the ABG Production Dispatch page.
 * No external CDN dependencies — works 100% offline.
 * Only dependency: html5-qrcode.min.js (also local)
 */

var Dispatch = (function () {

    /* ================================================
       PRIVATE STATE
    ================================================ */
    var _qr           = null;
    var _useBackCam   = true;
    var _poData       = {};
    var _items        = [];

    /* viewer state */
    var _vScale  = 1;
    var _vRot    = 0;
    var _vOff    = { x: 0, y: 0 };
    var _isDrag  = false;
    var _dragFrom = { x: 0, y: 0 };

    /* counters */
    var _cntScanned   = 0;
    var _cntWeightSum = 0;
    var _cntConfirmed = 0;

    /* demo attachments — remove in production */
    //var _demoFiles = [
    //    { name: 'Packing_List.jpg',   size: '1.2 MB', type: 'JPG',
    //      url: 'https://placehold.co/400x300/e5e7eb/6b7280?text=Packing+List' },
    //    { name: 'QC_Certificate.png', size: '843 KB', type: 'PNG',
    //      url: 'https://placehold.co/400x300/dbeafe/2563eb?text=QC+Certificate' },
    //    { name: 'Weight_Slip.jpg',    size: '520 KB', type: 'JPG',
    //      url: 'https://placehold.co/400x300/dcfce7/16a34a?text=Weight+Slip' },
    //    { name: 'Barcode_Sheet.png',  size: '256 KB', type: 'PNG',
    //      url: 'https://placehold.co/400x300/fef3c7/d97706?text=Barcode+Sheet' },
    //    { name: 'Delivery_Note.jpg',  size: '712 KB', type: 'JPG',
    //        url: 'https://placehold.co/400x300/fce7f3/9d174d?text=Delivery+Note'
    //    },
    //    {
    //        name: 'Disp1.avif', size: '2187 KB', type: 'PNG',
    //        url: '/images/disp1.png'
    //    },
    //];


    /* ================================================
       INIT
    ================================================ */
    function _init() {
        _startClock();
        _bindViewerDrag();
        _loadScanCount();  
        _loadConfirmCount();
    }


    /* ================================================
       LIVE CLOCK
    ================================================ */
    function _startClock() {
        function _tick() {
            var d = new Date();
            var dateStr = d.toLocaleDateString('en-IN', {
                weekday: 'short', day: 'numeric',
                month: 'short', year: 'numeric'
            });
            var timeStr = d.toLocaleTimeString('en-IN', {
                hour: '2-digit', minute: '2-digit'
            });
            var el = document.getElementById('liveDate');
            if (el) el.textContent = dateStr + ' \u00B7 ' + timeStr;

            var fel = document.getElementById('footerDate');
            if (fel) fel.textContent = dateStr;
        }
        _tick();
        setInterval(_tick, 1000);
    }


    /* ================================================
       QR SCANNER
    ================================================ */
    function openScanner() {
        var overlay = document.getElementById('qrOverlay');
        if (overlay) overlay.classList.add('open');
        _startCamera();
    }

    function _startCamera() {
        _qr = new Html5Qrcode('reader');
        _qr.start(
            { facingMode: _useBackCam ? 'environment' : 'user' },
            { fps: 10, qrbox: 220 },
            function (text) {
                var code = text.indexOf('|') !== -1 ? text.split('|')[0] : text;
                var inp = document.getElementById('inputCode');
                if (inp) inp.value = code;
                stopScanner();
                _doFetch(code);
            }
        ).catch(function (err) {
            console.error('Camera error:', err);
            _toast('Camera Error', 'Could not start camera', 'error');
        });
    }

    function stopScanner() {
        if (_qr) {
            _qr.stop()
                .then(function () { _qr.clear(); })
                .catch(function () {});
        }
        var overlay = document.getElementById('qrOverlay');
        if (overlay) overlay.classList.remove('open');
    }

    function switchCamera() {
        _useBackCam = !_useBackCam;
        if (_qr) {
            _qr.stop().then(function () {
                _qr.clear();
                _startCamera();
            });
        }
    }



    /* ================================================
       CORE DATA FETCH
    ================================================ */
    function _doFetch(code) {
        _showLoader();
        clearUI();
        fetch('/api/dispatch/po?code=' + encodeURIComponent(code))
            .then(function (res) {
                if (!res.ok) throw new Error('No data');
                return res.json();
            })
            .then(function (data) {

                /* Save global PO state */
                _poData = {
                    poNo:     data.poNo,
                    huCode:   data.huCode,
                    material: data.material,
                    cone:     data.cone,
                    EXIDV:    data.exidv,
                    SONUM:    data.sonum,
                    WERKS:    data.werks,
                    VEMEH:    data.vemeh
                };

                _items = data.items || [];
                //_items.push(
                //    { name: "PH-P2227AS", coneWeight: 100, material: "R2001NTR6535NO00" },
                //    { name: "PH-P2227BS", coneWeight: 110, material: "R2001NTR6535NO00" },
                //    { name: "PH-P2227CS", coneWeight: 95, material: "R2001NTR6535NO00" },
                //    { name: "PH-P2227DS", coneWeight: 120, material: "R2001NTR6535NO00" },
                //    { name: "PH-P2227ES", coneWeight: 105, material: "R2001NTR6535NO00" },
                //    { name: "PH-P2227ES", coneWeight: 105, material: "R2001NTR6535NO00" },
                //    { name: "PH-P2227ES", coneWeight: 105, material: "R2001NTR6535NO00" },
                //    { name: "PH-P2227ES", coneWeight: 105, material: "R2001NTR6535NO00" },
                //    { name: "PH-P2227ES", coneWeight: 105, material: "R2001NTR6535NO00" },
                //    { name: "PH-P2227ES", coneWeight: 105, material: "R2001NTR6535NO00" },
                //    { name: "PH-P2227ES", coneWeight: 105, material: "R2001NTR6535NO00" },
                //    { name: "PH-P2227ES", coneWeight: 105, material: "R2001NTR6535NO00" },
                //    { name: "PH-P2227ES", coneWeight: 105, material: "R2001NTR6535NO00" },
                //    { name: "PH-P2227ES", coneWeight: 105, material: "R2001NTR6535NO00" },
                //    { name: "PH-P2227ES", coneWeight: 105, material: "R2001NTR6535NO00" },
                //    { name: "PH-P2227ES", coneWeight: 105, material: "R2001NTR6535NO00" },
                //    { name: "PH-P2227ES", coneWeight: 105, material: "R2001NTR6535NO00" },
                //    { name: "PH-P2227ES", coneWeight: 105, material: "R2001NTR6535NO00" },
                //    { name: "PH-P2227ES", coneWeight: 105, material: "R2001NTR6535NO00" },
                //    { name: "PH-P2227ES", coneWeight: 105, material: "R2001NTR6535NO00" },
                //    { name: "PH-P2227ES", coneWeight: 105, material: "R2001NTR6535NO00" }
                //);

                /* PO Table */
                document.querySelector('#poTable tbody').innerHTML =
                    '<tr>' +
                    '<td><span class="tag tag-blue">' + data.poNo + '</span></td>' +
                    '<td>' + data.huCode + '</td>' +
                    '<td><strong>' + data.material + '</strong></td>' +
                    '<td><span class="tag tag-amber">' + data.cone + '</span></td>' +
                    '</tr>';

                /* Line Items Table */
                var rows = '';
                if (_items.length > 0) {
                    for (var i = 0; i < _items.length; i++) {
                        rows +=
                            '<tr>' +
                            '<td>' + (i + 1) + '</td>' +
                            '<td>' + _items[i].name + '</td>' +
                            '<td><strong>' + _items[i].coneWeight + '</strong></td>' +
                            '<td>' + _items[i].material + '</td>' +
                            '</tr>';
                    }
                } else {
                    rows = '<tr class="empty-row"><td colspan="4">' +
                           '<span class="empty-ico">&#128203;</span>No line items found</td></tr>';
                }
                document.querySelector('#lineTable tbody').innerHTML = rows;

                /* Standard Weight */
                var swEl = document.getElementById('stdWeight');
                if (swEl) swEl.value = data.stdWeight || '';

                /* Material Banner */
                var mnEl = document.getElementById('matName');
                var mcEl = document.getElementById('matCode');
                var ban  = document.getElementById('matBanner');
                if (mnEl) mnEl.textContent = data.material || '—';
                if (mcEl) mcEl.textContent = 'HU: ' + (data.huCode || '—');
                if (ban)  ban.classList.add('show');

                /* Material Image */
                _loadMaterialImage(data.material);

                /* Stats */
                //_cntScanned++;
                updateScanCount();
                //var se = document.getElementById('statScanned');
                //if (se) se.textContent = _cntScanned;

                /* Attachments — call renderAttachments(data.attachments) here */
                /* if (data.attachments) renderAttachments(data.attachments); */

                _toast('Data Loaded \u2713', 'PO data fetched successfully', 'success');
            })
            .catch(function (err) {
                console.error(err);
                clearUI();
                _toast('Not Found', 'No data found ', 'error');
            })
            .then(function () { _hideLoader(); });
    }


    /* ================================================
       MATERIAL IMAGE
    ================================================ */
    function _loadMaterialImage(material) {
        var el = document.getElementById('matImage');
        if (!el) return;

        // ✅ If no material → clear image immediately
        if (!material) {
            el.src = '';
            el.style.display = 'none';
            return;
        }

        fetch('/api/dispatch/material-image?material=' + encodeURIComponent(material))
            .then(function (r) {
                if (!r.ok) throw new Error('No image');
                return r.json();
            })
            .then(function (img) {
                if (img && img.image) {
                    el.src = img.image;
                    el.style.display = 'block';   // ✅ show image
                } else {
                    // ✅ No image from API → clear
                    el.src = '';
                    el.style.display = 'none';
                }
            })
            .catch(function () {
                // ✅ API error → clear image
                el.src = '';
                el.style.display = 'none';
            });
    }
    /* ================================================
      increse scanned count
   ================================================ */
    function updateScanCount() {
        fetch('/api/ScannedNoController/increment-scan', {
            method: 'POST'
        })
            .then(() => {
                _cntScanned++;

                var se = document.getElementById('statScanned');
                if (se) {
                    se.textContent = _cntScanned;

                    // 🔥 animation trigger (optional)
                    se.classList.remove("animate");
                    void se.offsetWidth;
                    se.classList.add("animate");
                }
            })
            .catch(err => console.error("Scan API error:", err));
    }
    

    /* ================================================
   Update Scan Count
================================================ */

    function _loadScanCount() {
        fetch('/api/ScannedNoController/get-scan-count')
            .then(res => {
                if (!res.ok) throw new Error("API failed");
                return res.json();
            })
            .then(data => {
                _cntScanned = data;
                document.getElementById('statScanned').textContent = _cntScanned;
            })
            .catch(err => console.error("Load count error:", err));
    }

    /* ================================================
     FETCH wrong PO NO then wrong number
  ================================================ */
    function fetchByCode() {
        var inp = document.getElementById('inputCode');
        var code = inp ? inp.value.trim() : '';

        if (!code) {
            _toast('Empty Code', 'Please enter or scan a code', 'warning');

            return;
        }

        // ✅ CLEAR OLD DATA FIRST
        clearUI();

        _doFetch(code);
    }
   
    function clearUI() {
        // Tables
        var po = document.querySelector('#poTable tbody');
        if (po) po.innerHTML = '';

        var line = document.querySelector('#lineTable tbody');
        if (line) line.innerHTML = '';

        // Weights
        var swEl = document.getElementById('stdWeight');
        if (swEl) swEl.value = '';

        // Material text
        var mnEl = document.getElementById('matName');
        if (mnEl) mnEl.textContent = '—';

        var mcEl = document.getElementById('matCode');
        if (mcEl) mcEl.textContent = 'HU: —';

        // ✅ CLEAR IMAGE (IMPORTANT)
       _loadMaterialImage(''); // ✅ clears image properly
    }
    /* ================================================
       FETCH WEIGHT
    ================================================ */
    function getWeight() {
        var awEl = document.getElementById('actWeight');
        if (awEl) awEl.value = '';
        _showLoader();

        fetch('/api/weighment/weight')
            .then(function (r) { return r.text(); })
            .then(function (raw) {
                console.log('Device raw:', raw);

                if (raw.indexOf('ERROR') === 0) {
                    _toast('Device Error', raw, 'error');
                    return;
                }

                var match = raw.match(/[\d.]+/);
                if (match) {
                    var w = parseFloat(match[0]);
                    if (awEl) awEl.value = w;
                    //_cntWeightSum += w;
                    var we = document.getElementById('statWeight');
                    if (we) we.textContent = _cntWeightSum.toFixed(1);
                    _toast('Weight Fetched', 'Actual weight: ' + w + ' kg', 'success');
                } else {
                    _toast('Invalid Data', 'Could not parse weight from device', 'warning');
                }
            })
            .catch(function () {
                _toast('Device Offline', 'Weight machine not responding', 'error');
            })
            .then(function () { _hideLoader(); });
    }
    /* ================================================
    after confirmed add confirm numbers
  ================================================ */
    function _loadConfirmCount() {
        fetch('/api/ScannedNoController/get-confirm-count')
            .then(res => res.json())
            .then(data => {
                _cntConfirmed = data;

                var ce = document.getElementById('statConfirmed');
                if (ce) ce.textContent = _cntConfirmed;
            });
    }
    
    /* ================================================
       CONFIRM & SAVE TO SAP
    ================================================ */
    function confirmData() {
        if (!_poData || !_poData.poNo) {
            _toast('Scan First', 'Please scan a QR code first', 'warning');
            return;
        }

        var awEl = document.getElementById('actWeight');
        var act  = awEl ? awEl.value : '';
        if (!act) {
            _toast('Weight Missing', 'Please fetch actual weight first', 'warning');
            return;
        }

        var swEl = document.getElementById('stdWeight');

        var coneWtTotal = 0;
        for (var i = 0; i < _items.length; i++) {
            coneWtTotal += (_items[i].coneWeight || 0);
        }

        var payload = {
            AUFNR:      _poData.poNo,
            VENUM:      _poData.huCode,
            EXIDV:      _poData.EXIDV,
            SONUM:      _poData.SONUM,
            MATNR:      _poData.material,
            WERKS:      _poData.WERKS || '',
            KDMAT:      (_items[0] ? _items[0].name : ''),
            STD_WEIGHT: parseFloat(swEl ? swEl.value : 0),
            VEMEH:      _poData.VEMEH,
            ZCONE_NO:   String(_poData.cone).padStart(5, '0'),
            ZCONE_WT:   coneWtTotal,
            ACT_WEIGHT: parseFloat(act)
        };

        console.log('Payload:', payload);
        _showLoader();

        fetch('/api/dispatch/save', {
            method:  'POST',
            headers: { 'Content-Type': 'application/json' },
            body:    JSON.stringify(payload)
        })
            .then(function (res) {
                if (!res.ok) throw new Error('Save failed');
                //_cntConfirmed++;
                updateScanCount();
                //var ce = document.getElementById('statConfirmed');
                //if (ce) ce.textContent = _cntConfirmed;
                _toast('Saved to SAP \u2713', 'Dispatch record confirmed', 'success');
            })
            .catch(function () {
                _toast('Save Failed', 'Could not push data to SAP', 'error');
            })
            .then(function () { _hideLoader(); });
    }


    /* ================================================
       ATTACHMENTS
    ================================================ */
    //function loadDemoAttachments() {
    //    renderAttachments(_demoFiles);
    //}

    /**
     * renderAttachments(files)
     * Call this from your API response with:
     *   Dispatch.renderAttachments(data.attachments)
     *
     * Each file object: { name, size, type, url }
     */
    //function renderAttachments(files) {
    //    var cnt  = document.getElementById('attachCount');
    //    var grid = document.getElementById('attachGrid');
    //    if (!grid) return;

    //    if (cnt) cnt.textContent = (files ? files.length : 0) + ' file' + (files && files.length !== 1 ? 's' : '');

    //    if (!files || !files.length) {
    //        grid.innerHTML =
    //            '<div class="attach-empty">' +
    //            '<svg width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.2">' +
    //            '<rect x="3" y="3" width="18" height="18" rx="2"/>' +
    //            '<circle cx="8.5" cy="8.5" r="1.5"/>' +
    //            '<polyline points="21 15 16 10 5 21"/>' +
    //            '</svg>' +
    //            '<p>No attachments found.</p>' +
    //            '</div>';
    //        return;
    //    }

    //    var html = '';
    //    for (var i = 0; i < files.length; i++) {
    //        var f = files[i];
    //        html +=
    //            '<div class="attach-card" ' +
    //                'onclick="Dispatch.openViewer(\'' + f.url + '\',\'' + f.name + '\')" ' +
    //                'style="animation:fadeUp .3s ease ' + (i * 0.05) + 's both">' +
    //                '<span class="attach-badge">' + (f.type || 'IMG') + '</span>' +
    //                '<img class="attach-thumb" src="' + f.url + '" alt="' + f.name + '" loading="lazy" />' +
    //                '<div class="attach-hover">&#128269;</div>' +
    //                '<div class="attach-foot">' +
    //                    '<div class="attach-fname">' + f.name + '</div>' +
    //                    '<div class="attach-fsize">' + (f.size || '') + '</div>' +
    //                '</div>' +
    //            '</div>';
    //    }
    //    grid.innerHTML = html;
    //}


    /* ================================================
       IMAGE VIEWER
    ================================================ */
    function openViewer(src, name) {
        _vScale = 1; _vRot = 0; _vOff = { x: 0, y: 0 };

        var img   = document.getElementById('viewerImg');
        var title = document.getElementById('viewerTitle');
        if (img)   img.src = src;
        if (title) title.textContent = name;

        _applyTransform();
        var ov = document.getElementById('viewerOverlay');
        if (ov) ov.classList.add('open');
    }

    function closeViewer(e) {
        /* if called from backdrop click, only close if click was on overlay itself */
        if (e && e.target !== document.getElementById('viewerOverlay')) return;
        var ov = document.getElementById('viewerOverlay');
        if (ov) ov.classList.remove('open');
    }

    function zoomImg(delta) {
        _vScale = Math.min(6, Math.max(0.15, _vScale + delta));
        _applyTransform();
    }

    function rotateImg(deg) {
        _vRot = (_vRot + deg) % 360;
        _applyTransform();
    }

    function resetImg() {
        _vScale = 1; _vRot = 0; _vOff = { x: 0, y: 0 };
        _applyTransform();
    }

    function _applyTransform() {
        var img = document.getElementById('viewerImg');
        if (img) {
            img.style.transform =
                'translate(' + _vOff.x + 'px,' + _vOff.y + 'px) ' +
                'rotate(' + _vRot + 'deg) ' +
                'scale(' + _vScale + ')';
        }
        var lbl = document.getElementById('zoomLbl');
        if (lbl) lbl.textContent = Math.round(_vScale * 100) + '%';
    }


    /* ================================================
       VIEWER DRAG — Mouse + Touch
    ================================================ */
    function _bindViewerDrag() {
        var stage = document.getElementById('viewerStage');
        if (!stage) return;

        /* Mouse */
        stage.addEventListener('mousedown', function (e) {
            _isDrag = true;
            _dragFrom = { x: e.clientX - _vOff.x, y: e.clientY - _vOff.y };
        });

        stage.addEventListener('mousemove', function (e) {
            if (!_isDrag) return;
            _vOff = { x: e.clientX - _dragFrom.x, y: e.clientY - _dragFrom.y };
            _applyTransform();
        });

        stage.addEventListener('mouseup',    function () { _isDrag = false; });
        stage.addEventListener('mouseleave', function () { _isDrag = false; });

        /* Touch */
        stage.addEventListener('touchstart', function (e) {
            if (e.touches.length === 1) {
                _isDrag = true;
                _dragFrom = {
                    x: e.touches[0].clientX - _vOff.x,
                    y: e.touches[0].clientY - _vOff.y
                };
            }
        }, { passive: true });

        stage.addEventListener('touchmove', function (e) {
            if (!_isDrag || e.touches.length !== 1) return;
            e.preventDefault();
            _vOff = {
                x: e.touches[0].clientX - _dragFrom.x,
                y: e.touches[0].clientY - _dragFrom.y
            };
            _applyTransform();
        }, { passive: false });

        stage.addEventListener('touchend', function () { _isDrag = false; });

        /* Scroll to zoom */
        stage.addEventListener('wheel', function (e) {
            e.preventDefault();
            zoomImg(e.deltaY < 0 ? 0.12 : -0.12);
        }, { passive: false });
    }


    /* ================================================
       LOADER
    ================================================ */
    function _showLoader() {
        var el = document.getElementById('loader');
        if (el) el.classList.add('show');
    }

    function _hideLoader() {
        var el = document.getElementById('loader');
        if (el) el.classList.remove('show');
    }


    /* ================================================
       TOAST
    ================================================ */
    function _toast(title, msg, type) {
        type = type || 'info';

        var icons = { success: '\u2713', error: '\u2715', warning: '\u26A0', info: '\u2139' };

        var wrap = document.getElementById('toastWrap');
        if (!wrap) return;

        var el = document.createElement('div');
        el.className = 'toast toast-' + type;
        el.innerHTML =
            '<span class="toast-ico">' + (icons[type] || '\u2139') + '</span>' +
            '<div class="toast-body">' +
                '<div class="toast-ttl">' + title + '</div>' +
                (msg ? '<div class="toast-msg">' + msg + '</div>' : '') +
            '</div>' +
            '<span class="toast-x" onclick="Dispatch._dismissToast(this)">\u2715</span>';

        wrap.appendChild(el);

        /* Auto-dismiss */
        setTimeout(function () {
            Dispatch._dismissToast(el.querySelector('.toast-x'));
        }, 3200);
    }

    function _dismissToast(xBtn) {
        var el = xBtn && xBtn.closest ? xBtn.closest('.toast') : null;
        if (!el || el.classList.contains('out')) return;
        el.classList.add('out');
        setTimeout(function () {
            if (el.parentNode) el.parentNode.removeChild(el);
        }, 300);
    }


    /* ================================================
       PUBLIC API
    ================================================ */
    return {
        init:                 _init,
        openScanner:          openScanner,
        stopScanner:          stopScanner,
        switchCamera:         switchCamera,
        fetchByCode:          fetchByCode,
        getWeight:            getWeight,
        confirmData:          confirmData,
        //loadDemoAttachments:  loadDemoAttachments,
        //renderAttachments:    renderAttachments,
        openViewer:           openViewer,
        closeViewer:          closeViewer,
        zoomImg:              zoomImg,
        rotateImg:            rotateImg,
        resetImg:             resetImg,
        _dismissToast:        _dismissToast
    };

})();

/* Auto-init on DOM ready */
document.addEventListener('DOMContentLoaded', function () {
    Dispatch.init();
});
