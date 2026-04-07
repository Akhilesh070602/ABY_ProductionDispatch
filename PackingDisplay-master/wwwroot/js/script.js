// ================= GLOBAL =================
let qr;
let useBackCamera = true;

// ✅ GLOBAL DATA STORAGE
let poData = {};
let items = [];


// ================= OPEN SCANNER =================
function openScanner() {

    document.getElementById("qrModal").style.display = "block";

    // hide image before scan
    document.getElementById("imageSection").style.display = "none";

    startCamera();
}


// ================= START CAMERA =================
//function startCamera() {

//    qr = new Html5Qrcode("reader");

//    qr.start(
//        { facingMode: useBackCamera ? "environment" : "user" },
//        { fps: 10, qrbox: 220 },
//        (text) => {

//            //document.getElementById("inputCode").value = text;
//            document.getElementById("inputCode").value = cleanCode;
//            let cleanCode = text.split('|')[0];
//            fetch('/api/dispatch/po?code=' + cleanCode)
//                .then(res => {
//                    if (!res.ok) throw "No data";
//                    return res.json();
//                })
//                .then(data => {

//                    // ✅ SAVE GLOBAL PO DATA
//                    poData = {
//                        poNo: data.poNo,
//                        huCode: data.huCode,
//                        material: data.material,
//                        cone: data.cone,
//                        EXIDV: data.exidv,
//                        SONUM: data.sonum
//                    };

//                    // ✅ SAVE GLOBAL ITEMS
//                    items = data.items || [];

//                    // ================= PO TABLE =================
//                    document.querySelector("#poTable tbody").innerHTML = `
//                        <tr>
//                            <td>${data.poNo}</td>
//                            <td>${data.huCode}</td>
//                            <td>${data.material}</td>
//                            <td>${data.cone}</td>
//                        </tr>
//                    `;

//                    // ================= LINE ITEMS =================
//                    let rows = "";

//                    if (items.length > 0) {
//                        items.forEach((x, i) => {
//                            rows += `
//                                <tr>
//                                    <td>${i + 1}</td>
//                                    <td>${x.name}</td>
//                                    <td>${x.coneWeight}</td>
//                                    <td>${x.material}</td>
//                                </tr>`;
//                        });
//                    } else {
//                        rows = `<tr><td colspan="4">No Items Found</td></tr>`;
//                    }

//                    document.querySelector("#lineTable tbody").innerHTML = rows;

//                    // ================= STD WEIGHT =================
//                    document.getElementById("stdWeight").value = data.stdWeight || "";

//                    // ================= IMAGE =================
//                    loadMaterialImage(data.material);

//                })
//                .catch(err => {
//                    console.log(err);
//                    alert("❌ Data not found");
//                });

//            stopScanner();
//        }
//    );
//}

function startCamera() {

    qr = new Html5Qrcode("reader");

    qr.start(
        { facingMode: useBackCamera ? "environment" : "user" },
        { fps: 10, qrbox: 220 },
        (text) => {

            console.log("QR SCANNED:", text);

            // ✅ FIXED ORDER
            let cleanCode = text.includes('|')
                ? text.split('|')[0]
                : text;

            document.getElementById("inputCode").value = cleanCode;

            fetch('/api/dispatch/po?code=' + cleanCode)
                .then(res => {
                    if (!res.ok) throw "No data";
                    return res.json();
                })
                .then(data => {

                    // ✅ SAVE GLOBAL PO DATA
                    poData = {
                        poNo: data.poNo,
                        huCode: data.huCode,
                        material: data.material,
                        cone: data.cone,
                        EXIDV: data.exidv,
                        SONUM: data.sonum,
                        WERKS: data.werks,
                        VEMEH: data.vemeh   
                    };

                    // ✅ SAVE GLOBAL ITEMS
                    items = data.items || [];

                    // PO TABLE
                    document.querySelector("#poTable tbody").innerHTML = `
                        <tr>
                            <td>${data.poNo}</td>
                            <td>${data.huCode}</td>
                            <td>${data.material}</td>
                            <td>${data.cone}</td>
                        </tr>
                    `;

                    // LINE ITEMS
                    let rows = "";

                    if (items.length > 0) {
                        items.forEach((x, i) => {
                            rows += `
                                <tr>
                                    <td>${i + 1}</td>
                                    <td>${x.name}</td>
                                    <td>${x.coneWeight}</td>
                                    <td>${x.material}</td>
                                </tr>`;
                        });
                    } else {
                        rows = `<tr><td colspan="4">No Items Found</td></tr>`;
                    }

                    document.querySelector("#lineTable tbody").innerHTML = rows;

                    // STD WEIGHT
                    document.getElementById("stdWeight").value = data.stdWeight || "";

                    // IMAGE
                    loadMaterialImage(data.material);

                })
                .catch(err => {
                    console.log(err);
                    alert("❌ Data not found");
                });

            stopScanner();
        }
    ).catch(err => {
        console.log("CAMERA ERROR:", err);
        alert("❌ Camera not working");
    });
}
// ================= STOP SCANNER =================
function stopScanner() {
    if (qr) {
        qr.stop().then(() => qr.clear()).catch(() => { });
    }
    document.getElementById("qrModal").style.display = "none";
}


// ================= SWITCH CAMERA =================
function switchCamera() {
    useBackCamera = !useBackCamera;

    if (qr) {
        qr.stop().then(() => {
            qr.clear();
            startCamera();
        });
    }
}


// ================= LOAD IMAGE =================
function loadMaterialImage(material) {

    fetch('/api/dispatch/material-image?material=' + material)
        .then(res => res.json())
        .then(img => {

            const section = document.getElementById("imageSection");
            const image = document.getElementById("materialImage");

            if (img && img.image) {
                image.src = img.image;
                section.style.display = "block";
            } else {
                section.style.display = "none";
            }
        })
        .catch(() => {
            document.getElementById("imageSection").style.display = "none";
        });
}


// ================= FETCH WEIGHT =================
//function getWeightFromMachine() {

//    fetch('/api/weighment/weight')
//        .then(res => res.text())
//        .then(data => {

//            let match = data.match(/[\d.]+/);

//            if (match) {
//                document.getElementById("actWeight").value = parseFloat(match[0]);
//            } else {
//                alert("⚠️ Weight machine not connected!");
//            }
//        })
//        .catch(() => alert("❌ Machine error"));
//}
function getWeightFromMachine() {

    // 🔥 clear old value
    document.getElementById("actWeight").value = "";

    fetch('/api/weighment/weight')
        .then(res => res.text())
        .then(data => {

            console.log("DEVICE RAW:", data);

            if (data.startsWith("ERROR")) {
                alert(data);
                return;
            }

            // ✅ extract number
            let match = data.match(/[\d.]+/);

            if (match) {
                document.getElementById("actWeight").value = parseFloat(match[0]);
            } else {
                alert("⚠️ Invalid weight data");
            }
        })
        .catch(() => {
            alert("❌ Device not connected");
        });
}

// ================= CONFIRM (SAVE TO SAP) =================
function confirmData() {

    if (!poData || !poData.poNo) {
        alert("⚠️ Scan data first!");
        return;
    }

    let std = document.getElementById("stdWeight").value;
    let act = document.getElementById("actWeight").value;

    if (!act) {
        alert("⚠️ Enter weight!");
        return;
    }

    let payload = {
        AUFNR: poData.poNo,
        VENUM: poData.huCode,
            EXIDV: poData.EXIDV ,
            SONUM: poData.SONUM ,

        MATNR: poData.material,
        WERKS: poData.WERKS || "",
        KDMAT: items[0]?.name || "",

        STD_WEIGHT: parseFloat(std),
        VEMEH: poData.VEMEH,

        ZCONE_NO: poData.cone.toString().padStart(5, '0'),
        ZCONE_WT: items.reduce((a, b) => a + b.coneWeight, 0),

        ACT_WEIGHT: parseFloat(act)
    };

    console.log("🚀 FINAL PAYLOAD:", payload);

    fetch('/api/dispatch/save', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    })
        .then(res => {
            if (!res.ok) throw "Error";
            alert("✅ Saved to SAP");
        })
        .catch(err => {
            console.log(err);
            alert("❌ Error saving data");
        });
}