// Script para escuchar el evento submit del formulario para crear la Resenna
function manejarEnvioResenna(e) {
    e.preventDefault();

    const form = e.target;

    const formData = new FormData(form);

    // Parsear validaciones unobtrusive (si el formulario fue cargado dinámicamente)
    $.validator.unobtrusive.parse('#formResenna');

    // Validación client-side con jQuery Unobtrusive
    if (!$(form).valid()) {
        return; //No se envía si no es válido
    }

    fetch("/ResennaValoracion/Create", {
        method: "POST",
        body: formData
    })
        .then(resp => {
            if (!resp.ok) {
                // Si es error de validación (400), extraer los mensajes
                if (resp.status === 400) {
                    return resp.json().then(errors => {
                        if (errors.Calificacion) {
                            document.getElementById("error-calificacion").innerText = errors.Calificacion[0];
                        }
                        if (errors.Comentario) {
                            document.getElementById("error-comentario").innerText = errors.Comentario[0];
                        }
                        // Agregar excepción customizado con "Validación fallida"
                        throw new Error("Validación fallida");
                    });
                } else {
                    throw new Error("Error al enviar reseña");
                }
            }
            return resp.json();
        })
        .then(result => {

            mostrarToast("Reseña agregada exitosamente", "success");
            modalResenna.close();
            form.reset();
            $("#formResenna").validate().resetForm(); // limpia mensajes

            // Obtener el ID del producto desde el formulario
            const idProducto = formData.get("IdProducto");

            // Llamar a la función para recargar reseñas y promedio
            recargarZonaResennasYPromedio(idProducto);

        })
        .catch(err => {
            console.error(err);
        });
}

// Script para escuchar el evento submit del formulario para agregar un detalle del carrito
function manejarEnvioDetalleCarrito(e) {
    e.preventDefault();

    // Para validar el estado del formulario
    let valido = true;

    const form = e.target;
    const formData = new FormData(form);

    // Obtener los valores de campos personalizados
    const mensajeInput = document.querySelector("textarea[name='MensajePersonalizado']");
    const empaqueSelect = document.querySelector("select[name='IdEmpaque']");
    const dropzone = document.querySelector('[x-data="dataSingleFileDnD()"]');


    // Limpiar mensajes anteriores de campos personalizables (tipoEmpaque, mensajePersonaliado y foto)

    const erroresTipoEmpaque = document.getElementById("error-tipoEmpaque");
    if (erroresTipoEmpaque) {
        erroresTipoEmpaque.innerText = "";
    }

    const erroresMensajePersonalizado = document.getElementById("error-mensajePersonalizado");
    if (erroresMensajePersonalizado) {
        erroresMensajePersonalizado.innerText = "";
    }

    const erroresFoto = document.getElementById("error-foto");
    if (erroresFoto) {
        erroresFoto.innerText = "";
    }

    // Activar validación unobstrusive
    $.validator.unobtrusive.parse(form);

    // Validar tipo de empaque
    if (!empaqueSelect || empaqueSelect.value === "") {
        document.getElementById("error-mensaje").innerText = "El tipo de empaque es requerido.";
        valido = false;
    }

    // Validar mensaje
    if (!mensajeInput || mensajeInput.value.trim() === "") {
        document.getElementById("error-mensaje").innerText = "El mensaje personalizado es requerido.";
        valido = false;
    }


    // Validar si hay imagen en el dropzone de imágenes
    const tieneArchivo = componenteDropzone && componenteDropzone.__x && componenteDropzone.__x.$data.file != null;

    // Si el formulario no es válido o no tiene imagen, mostrar errores correspondientes
    if (!esValido || !tieneArchivo) {

        // Forzar validación manual para los campos de personalización (ya que hay Alpine JS de por medio)

        // Validación manual de tipoEmpaque


        // Validación manual de mensajePersonalizado

        // Validación manual de foto
        if (!tieneArchivo) {
            const erroresFoto = document.getElementById("error-foto");
            if (erroresFoto) {
                erroresFoto.innerText = "Debe insertar al menos una imagen";
            }
        }

        return;
    }

    // enviar el formulario al controlador si todo está correcto
    fetch("/CarritoDetalle/Create", {
        method: "POST",
        body: formData
    })
        .then(response => response.json())
        .then(result => {
            if (result.success) {
                mostrarToast("", "success");
                // Resetear el formulario si hay respuesta exitosa
                form.reset();
                
            } else if (result.errores) {
                // Mostrar errores manualmente si vienen en el JSON
                result.errores.forEach(mensaje => {
                    mostrarToast(mensaje, "error");
                });
            }
        })
        .catch(error => {
            console.error("Error al enviar detalle:", error);
            mostrarToast("Error inesperado", "error");
        });
}

//Función para recargar los divs de Zona de Reseñas y Promedio de Calificaciones
function recargarZonaResennasYPromedio(idProducto) {
    // Recargar reseñas
    fetch(`/ResennaValoracion/GetResennasPorProducto?idProducto=${idProducto}`)
        .then(response => response.text())
        .then(html => {
            document.getElementById("zona-resennas").innerHTML = html;
        });

    // Recargar promedio
    fetch(`/ResennaValoracion/GetPromedioPorProducto?idProducto=${idProducto}`)
        .then(response => response.text())
        .then(html => {
            document.getElementById("zona-promedio").innerHTML = html;
        });
}

// Función para mostrar el toast a la hora de dejar la reseña del producto 
// ya sea exitoso o error
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

// Función para manejar el dropzone de la imagen del empaque personalizado
function dataSingleFileDnD() {
    return {
        file: null,
        previewUrl: '',

        // Evento/función para menejar el cambio de archivo
        handleFileChange(event) {
            const selectedFile = event.target.files[0];
            if (selectedFile && selectedFile.type.startsWith("image/")) {
                this.file = selectedFile;
                this.previewUrl = URL.createObjectURL(selectedFile);
            }
        },

        // Evento/función para manejar el drop del archivo/imagen
        handleFileDrop(event) {
            const droppedFile = event.dataTransfer.files[0];
            if (droppedFile && droppedFile.type.startsWith("image/")) {
                this.file = droppedFile;
                this.previewUrl = URL.createObjectURL(droppedFile);
            }
            this.$refs.dnd.classList.remove('drag-over');
        },

        // Evento/función para manejar el borrado del archivo/imagen
        removeFile() {
            this.file = null;
            this.previewUrl = '';
        }
    };
}

// Función para "escuchar" mientras el usuario escribe en el campo de "Cantidad"
function escucharInputCantidad() {
    const input = document.getElementById("inputCantidad");
    const mensajeError = document.querySelector("span[data-valmsg-for='Stock']");

    if (!input) return;

    input.addEventListener("input", () => {
        // Eliminar decimales si el usuario los escribe
        input.value = input.value.replace(/[^\d]/g, ""); // Solo dígitos

        const numero = parseInt(input.value);

        if (!isNaN(numero)) {
            if (numero < 1 || numero > 100) {
                if (mensajeError) {
                    mensajeError.textContent = "Debe ingresar una cantidad válida entre 1 y 100";
                }
                input.classList.add("border-red-500");
            } else {
                if (mensajeError) mensajeError.textContent = "";
                input.classList.remove("border-red-500");
            }
        } else {
            if (mensajeError) mensajeError.textContent = "";
            input.classList.remove("border-red-500");
        }
    });
}

// Asignar función al evento submit solo cuando el DOM esté listo
document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("formResenna");
    if (form) {
        form.addEventListener("submit", manejarEnvioResenna);
    }
});

// Cuando el DOM esté listo, cargar el promedio de valoraciones
document.addEventListener("DOMContentLoaded", function () {
    const idProducto = document.querySelector("input[name='IdProducto']")?.value;

    if (idProducto) {
        recargarZonaResennasYPromedio(idProducto);
    }
});

// Cuando el DOM esté listo, realizar las validaciones de los inputs para
// añadir el producto al carrito
document.addEventListener("DOMContentLoaded", function () {
    escucharInputCantidad();
});

// Asignar el evento de submit del formulario de detalle de carrito cuando el DOM esté listo
document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("formCarritoDetalle");
    if (form) {
        form.addEventListener("submit", manejarEnvioDetalleCarrito);
    }
});
