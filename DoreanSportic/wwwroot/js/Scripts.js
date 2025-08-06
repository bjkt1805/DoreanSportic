//Función de Alpine.js para poder cargar las imágenes en Create Producto (Dashboard Admin) y hacer la validación 
// client-side a través del formulario de productos
function dataFileDnD() {
    return {
        files: [],
        fileDragging: null,
        fileDropping: null,
        initialImages: [], // Para cargar imágenes desde el backend
        imagenesOriginales: [], 
        

        init() {

            console.log("Imágenes cargadas: ", this.initialImages)
            //Si no hay imágenes iniciales, no hacer nada
            if (!this.initialImages) return;

            // Si hay imágenes iniciales, cargarlas en el dropzone
            this.initialImages.forEach((img, index) => {
                const byteString = atob(img.base64);
                const arrayBuffer = new ArrayBuffer(byteString.length);
                const uint8Array = new Uint8Array(arrayBuffer);

                for (let i = 0; i < byteString.length; i++) {
                    uint8Array[i] = byteString.charCodeAt(i);
                }

                const file = new File([uint8Array], img.nombre || `imagen-${index}.jpg`, {
                    type: "image/jpeg"
                });

                file.isOriginal = true; // Bandera para saber si la imagen venía del backend
                file.nombreOriginal = img.nombre; // Para enviarlo en imagenesConservar

                this.files.push(file);
                this.imagenesOriginales.push(img.nombre); // Guardar nombre como referencia
            });

        },

        loadFile(file) {
            return URL.createObjectURL(file);
        },

        // Función para agregar las imágenes (ya sea por drag and drop o al dar clic al dropzone)
        addFiles(event) {

            // asignar los eventos de dataTransfer (drag and drop) o de subir mediante clic
            // a la variable
            let droppedFiles = event.dataTransfer?.files || event.target.files;

            // Si no hay imágenes cargadas en el array (droppedFiles) simplemente no hacer nada
            if (!droppedFiles || droppedFiles.length === 0) return;

            // Recorrer todo el array de imágenes (droppedFiles) para insertar cada imagen 
            // en el array que se va a enviar para crear el producto
            [...droppedFiles].forEach(file => {
                // Validar que el tipo MIME sea de imagen
                if (!file.type.startsWith("image/")) {
                    mostrarToast(`Solo se permiten imágenes. Archivo inválido: ${file.name}`, "error");
                    return;
                }

                this.files.push(file);
            });

            // Resetear el input para permitir volver a cargar el mismo archivo (si aplica)
            if (event.target.tagName === "INPUT") {
                event.target.value = "";
            }
        },

        remove(index) {
            const file = this.files[index];

            // Si es imagen original, eliminar también de imagenesOriginales
            if (file.isOriginal) {
                const i = this.imagenesOriginales.indexOf(file.nombreOriginal);
                if (i !== -1) {
                    this.imagenesOriginales.splice(i, 1);
                }
            }

            this.files.splice(index, 1);
        },

        // Evento para escuchar el inicio del drag de la imagen
        dragstart(event) {
            this.fileDragging = event.target.dataset.index;
            event.dataTransfer.effectAllowed = 'move';
        },

        // Evento para escuchar el drag hacia al dropzone
        dragenter(event) {
            this.fileDropping = event.target.dataset.index;
        },

        // Evento para escuchar el drop de la imagen en el dropzone
        drop(event) {
            event.preventDefault();
            const from = this.fileDragging;
            const to = event.target.closest('[data-index]').dataset.index;

            if (from === to) return;

            const files = [...this.files];
            files.splice(to, 0, files.splice(from, 1)[0]);

            this.files = files;
            this.fileDragging = null;
            this.fileDropping = null;
        },

        // Función para retornar el valor del tamaño de la imagen
        // en términos entendibles para humanos
        humanFileSize(size) {
            const i = size === 0 ? 0 : Math.floor(Math.log(size) / Math.log(1024));
            return (size / Math.pow(1024, i)).toFixed(2) * 1 + ' ' + ['B', 'kB', 'MB', 'GB', 'TB'][i];
        },

        // Función para manejar el evento de envío del formulario
        async submitForm(e) {
            const estadoCheckbox = document.querySelector("input[type='checkbox'][name='Estado']");
            e.preventDefault();

            const form = e.target;
            // Constante para manejar si se va a crear o editar un producto
            const modo = form.dataset.modo;

            // Limpiar mensajes anteriores
            document.getElementById("error-etiquetas").innerText = "";
            const zonaErrores = document.getElementById("zona-errores-validacion");
            if (zonaErrores) {
                zonaErrores.innerText = "";
            }

            // Activar validación unobtrusive
            if ($.validator && $.validator.unobtrusive) {
                $.validator.unobtrusive.parse(form);
            }

            // Bandera para verificar si hay error
            let hasError = false;

            // Validar etiquetas
            const etiquetasAsignadas = document.querySelectorAll("#dp2 .drag");
            // Obtener el mensaje de error de etiquetas desde el div de errorEtiquetasDiv (internacionalizado))
            const errorEtiquetasDiv = document.getElementById("error-etiquetas");
            const msjEtiqueta = errorEtiquetasDiv.dataset.msjEtiqueta;

            if (etiquetasAsignadas.length === 0) {
                document.getElementById("error-etiquetas").innerText = msjEtiqueta;
                hasError = true;
            }

            // Validar que haya al menos una imagen cargada
            // Obtener el mensaje de error de imagenes desde el div de errorEtiquetasDiv (internacionalizado))
            const errorFotosDiv = document.getElementById("zona-errores-validacion");
            const msjImagen = errorFotosDiv.dataset.msjImagen;

            if  (this.files.length === 0) {
                if (errorFotosDiv) {
                    errorFotosDiv.innerText = msjImagen;
                }
                hasError = true;
            }

            if (!$(form).valid()) {
                console.warn("Errores de validación en el formulario:");
                $(form).find("[data-valmsg-for]").each(function () {
                    const field = $(this).attr("data-valmsg-for");
                    const errorText = $(this).text().trim();
                    if (errorText !== "") {
                        console.warn(`Campo: ${field} → Error: ${errorText}`);
                    }
                });
            }

            // Si hay errores o el form no es válido por parte del cliente, 
            //detener el proceso
            if (hasError && !$(form).valid()) {
                return;
            } else if (hasError || !$(form).valid()) {
                return;
            }

            // Preparar FormData
            const formData = new FormData(form);

            // Agregar todas las imágenes (tanto originales como nuevas)
            this.files.forEach(file => {
                formData.append("imagenes", file);
            });

            // Agregar etiquetas seleccionadas
            etiquetasAsignadas.forEach(el => {
                const id = el.dataset.id;
                if (id) {
                    formData.append("selectedEtiquetas", id);
                }
            });

            // Solo enviar nombres de imágenes originales que aún están presentes
            this.imagenesOriginales.forEach(nombre => {
                formData.append("imagenesConservar", nombre);
            });

            // Configurar el estado del producto (toggle) dependiendo de cuál
            // sea el estado del producto
            formData.set("Estado", estadoCheckbox?.checked ? "true" : "false");

            try {
                const response = await fetch(form.action, {
                    method: "POST",
                    body: formData
                });

                // Código para cargar vista parcial en caso de que haya
                // respuesta exitosa a la hora de crear el producto
                // en el controlador Create (POST)
                if (response.ok) {
                    const data = await response.json();

                    if (data.success) {
                        // Mostrar toast
                        mostrarToast(data.mensaje, "success");

                        // Cargar _IndexAdmin dinámicamente en el contenedor
                        cargarVista('/Producto/IndexAdmin');

                        // Opcional: eliminar estado anterior
                        history.replaceState(null, "", "/Producto/IndexAdmin");
                    } else {
                        // Manejo si viniera algún error
                        console.warn("Error inesperado:", data);
                    }

                } else {
                    const html = await response.text();
                    document.getElementById("contenido-dinamico").innerHTML = html;
                }


                if (response.redirected) {
                    window.location.href = response.url;
                } else {
                    const html = await response.text();
                    document.body.innerHTML = html;
                }
            } catch (err) {
                console.error("Error al enviar el formulario", err);
            }
        }, 

    };
}

// Función para recargar el resumen del carrito 
// en el navbar
function recargarResumenCarritoNavbar() {
    fetch("/CarritoDetalle/NavbarCarrito")
        .then(response => response.text())
        .then(html => {
            // renderizar la vista parcial del carito en el dropdown
            document.getElementById("carrito-navbar-body").innerHTML = html;

            // leer la cantidad del badge oculto que está en la vista parcial
            const badgeVistaParcial = document.getElementById("carrito-navbar-badge-partial");

            // Si el badge está disponible en el DOM
            if (badgeVistaParcial) {

                // Obtener el valor de cantidad del badge
                const cantidad = badgeVistaParcial.innerText || "0";

                // Actualizar el badge visible del navbar (arriba del carrito de compras)
                // con la cantidad correcta
                document.getElementById("carrito-navbar-badge").innerText = cantidad;
            }
        });
}

// Cuando el DOM esté listo, cargar la función que recarga el carrito de compras
// en el navbar cuando se agregan o quitan detalles/productos del carrito de compras
document.addEventListener("DOMContentLoaded", () => {
    recargarResumenCarritoNavbar();
});


