// Script para escuchar el evento submit del formulario para crear la Resenna

function manejarEnvioResenna(e) {
    e.preventDefault();

    const form = e.target;
    const formData = new FormData(form);
    const data = Object.fromEntries(formData.entries());

    // Activar validación unobtrusive
    $.validator.unobtrusive.parse('#formResenna');

    fetch("/ResennaValoracion/Create", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    })
        .then(resp => {
            if (!resp.ok) throw new Error("Error al enviar reseña");
            return resp.json();
        })
        .then(result => {
            mostrarToast("Reseña agregada exitosamente", "success");
            modalResenna.close();
            form.reset();

            // Agregar reseña sin recargar
            const contenedor = document.getElementById("zona-resennas");
            const nuevaResenna = document.createElement("div");
            nuevaResenna.classList.add("p-4", "mb-2", "border", "rounded", "bg-base-100", "shadow");

            nuevaResenna.innerHTML = `
                <div class="flex justify-between items-center mb-1">
                    <span class="font-bold text-[#004AAD]">${data.Usuario}</span>
                    <span class="text-xs text-gray-500">${new Date(data.Fecha).toLocaleDateString()}</span>
                </div>
                <div class="mb-1">${renderEstrellas(data.Calificacion)}</div>
                <p class="text-sm text-justify">${data.Comentario}</p>
            `;
            contenedor.prepend(nuevaResenna);
        })
        .catch(err => {
            console.error(err);
            mostrarToast("Error al enviar reseña", "error");
        });
}

function renderEstrellas(valor) {
    let estrellas = '';
    for (let i = 1; i <= 5; i++) {
        estrellas += `<svg class="inline w-5 h-5 ${i <= valor ? 'text-yellow-400' : 'text-gray-300'}" fill="currentColor" viewBox="0 0 24 24">
                <path d="M12 .587l3.668 7.431 8.2 1.192-5.934 5.782
                         1.402 8.175L12 18.896l-7.336 3.861
                         1.402-8.175-5.934-5.782 8.2-1.192z"/>
            </svg>`;
    }
    return estrellas;
}

function mostrarToast(mensaje, tipo = "info") {
    const toast = document.createElement("div");
    toast.className = `toast toast-top toast-center z-50`;
    toast.innerHTML = `
            <div class="alert alert-${tipo}">
                <span class="text-black font-bold">${mensaje}</span>
            </div>
        `;
    document.body.appendChild(toast);

    setTimeout(() => {
        toast.remove();
    }, 4000);
}

// Asignar función al evento submit solo cuando el DOM esté listo
document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("formResenna");
    if (form) {
        form.addEventListener("submit", manejarEnvioResenna);
    }
});