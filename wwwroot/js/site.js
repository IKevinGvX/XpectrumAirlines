document.addEventListener('DOMContentLoaded', () => {
    const boletos = window.boletoData || [];
    boletos.forEach((boleto, i) => {
        const container = document.getElementById(`qrcode-${i}`);
        if (container) {
            container.innerHTML = '';
            const qrText = boleto.CodigoBoleto; // Solo el código
            new QRCode(container, {
                text: qrText,
                width: 120,
                height: 120,
                colorDark: "#000000",
                colorLight: "#ffffff",
                correctLevel: QRCode.CorrectLevel.H
            });
        }
    });
});

function downloadQRCode(index, code) {
    const qrDiv = document.getElementById('qrcode-' + index);
    const img = qrDiv.querySelector('img');
    if (img) {
        const link = document.createElement('a');
        link.href = img.src;
        link.download = code + '.png';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    } else {
        alert('Código QR no disponible aún.');
    }
}
