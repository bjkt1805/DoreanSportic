//Función de Alpine.js para poder cargar las imágenes en Create Producto (Dashboard Admin) y hacer la validación 
// client-side a través del formulario de productos
function dataFileDnD() {
    return {
        files: [],
        fileDragging: null,
        fileDropping: null,

        loadFile(file) {
            return URL.createObjectURL(file);
        },

        addFiles(event) {
            let droppedFiles = event.target.files;
            if (!droppedFiles) return;

            [...droppedFiles].forEach(file => this.files.push(file));
        },

        remove(index) {
            this.files.splice(index, 1);
        },

        dragstart(event) {
            this.fileDragging = event.target.dataset.index;
            event.dataTransfer.effectAllowed = 'move';
        },

        dragenter(event) {
            this.fileDropping = event.target.dataset.index;
        },

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

        humanFileSize(size) {
            const i = size === 0 ? 0 : Math.floor(Math.log(size) / Math.log(1024));
            return (size / Math.pow(1024, i)).toFixed(2) * 1 + ' ' + ['B', 'kB', 'MB', 'GB', 'TB'][i];
        },

        async submitForm(e) {
            e.preventDefault();

            const form = e.target;

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
            if (etiquetasAsignadas.length === 0) {
                document.getElementById("error-etiquetas").innerText = "* Debe asignar al menos una etiqueta *";
                hasError = true;
            }

            // Validar que haya al menos una imagen cargada
            if (this.files.length === 0) {
                const zonaErrores = document.getElementById("zona-errores-validacion");
                if (zonaErrores) {
                    zonaErrores.innerText = "* Debe subir al menos una imagen del producto *";
                }
                hasError = true;
            }

            // Si hay errores o el form no es válido por parte del cliente, 
            //detener el proceso
            if (hasError && !$(form).valid()) {
                return;
            }

            // Preparar FormData
            const formData = new FormData(form);

            // Agregar imágenes
            this.files.forEach(file => {
                formData.append("imagenesProducto", file);
            });

            // Agregar etiquetas seleccionadas
            etiquetasAsignadas.forEach(el => {
                const id = el.dataset.id;
                if (id) {
                    formData.append("selectedEtiquetas", id);
                }
            });

            try {
                const response = await fetch(form.action, {
                    method: "POST",
                    body: formData
                });

                if (response.redirected) {
                    window.location.href = response.url;
                } else {
                    const html = await response.text();
                    document.body.innerHTML = html;
                }
            } catch (err) {
                console.error("Error al enviar el formulario", err);
            }
        }
    };
}
