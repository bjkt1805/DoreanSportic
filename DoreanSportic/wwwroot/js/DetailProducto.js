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

    //Obtener los datos del formulario
    const form = e.target;
    //Crear el formulario (FormData) con los valores recibidos (form)
    const formData = new FormData(form);

    //Acceder al componente Alpine de personalización
    const componentePersonalizacion = document.querySelector('[x-data="{ deseaPersonalizar: \'no\' }"]');
    const deseaPersonalizar = componentePersonalizacion?.__x?.$data?.deseaPersonalizar === "si";

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

    // Validaciones condicionales  de los atributos personalizables (solo si se quiere personalizar)
    if (deseaPersonalizar) {

        // Validar tipo de empaque
        if (!empaqueSelect || empaqueSelect.value === "") {
            document.getElementById("error-tipoEmpaque").innerText = "El tipo de empaque es requerido.";
            valido = false;
        }

        // Validar mensaje
        if (!mensajeInput || mensajeInput.value.trim() === "") {
            document.getElementById("error-mensajePersonalizado").innerText = "El mensaje personalizado es requerido.";
            valido = false;
        }


        // Validar si hay imagen en el dropzone de imágenes
        const tieneArchivo = dropzone && dropzone.__x && dropzone.__x.$data.file != null;

        // Si el formulario no tiene imagen, mostrar error de imagen
        if (!tieneArchivo) {

            if (!tieneArchivo) {
                const erroresFoto = document.getElementById("error-foto");
                if (erroresFoto) {
                    erroresFoto.innerText = "Debe insertar al menos una imagen";
                    valido = false;
                }
            }
        }
    }

    // Validación general del formulario (HTML = Cliente + Data Annotations = Modelo)
    const esValido = $(form).valid();
    if (!esValido || !valido) {
        $(form).find("input, select, textarea").each(function () {
            $(this).valid();
        });
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

            // Para notificar a la función de calcular subTotal
            // que se ha agregado o removido una imagen en el dropzone
            window.dispatchEvent(new Event("imagen-personalizada-cambiada"));
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

            // Para notificar a la función de calcular subTotal
            // que se ha agregado o removido una imagen en el dropzone
            window.dispatchEvent(new Event("imagen-personalizada-cambiada"));
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
        // y mantener solo digitos
        input.value = input.value.replace(/[^\d]/g, ""); 

        // Limitar el input solo a 3 dígitos
        if (input.value.length > 3) {
            input.value = input.value.slice(0, 3);
        }

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

// Función para bloquear el borrado en el input de cantidad
function bloquearBorradoInputCantidad() {
    const input = document.getElementById("inputCantidad");

    if (!input) return;

    // Escuchar el evento de key down cuando se 
    // inserta valor en el input de cantidad
    input.addEventListener("keydown", (e) => {
        // Bloquear teclas de borrado
        const teclasBloqueadas = ["Backspace", "Delete"];

        if (teclasBloqueadas.includes(e.key)) {
            e.preventDefault();
        }

        // También bloquear CTRL + X
        if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === "x") {
            e.preventDefault();
        }
    });

    // Bloquear pegar
    input.addEventListener("paste", (e) => e.preventDefault());
    input.addEventListener("cut", (e) => e.preventDefault());
}

// Función para cargar el campo SubTotal de manera dinámica en el frontend
function calcularSubtotal() {

    // Obtener los valores de los inputs del formulario para 
    // calcular dinámicamente el subTotal
    const form = document.getElementById("formCarritoDetalle");
    const inputCantidad = document.getElementById("inputCantidad");
    const selectEmpaque = document.querySelector("select[name='IdEmpaque']");
    const radioPersonalizar = document.querySelectorAll("input[name='DeseaPersonalizar']");
    const subtotalSpan = document.getElementById("subTotalValor");

    // Obtener el valor del input de cantidad
    const cantidad = parseInt(inputCantidad.value) || 0;

    // Obtener el valor del radio Button de personalizar
    const deseaPersonalizar = [...radioPersonalizar].find(r => r.checked)?.value === "si";

    // Obtener el valor del precio del producto
    const precioProducto = parseFloat(form.dataset.precioDescuento || form.dataset.precioBase || 0);

    //Asignar inicialmente el valor de subproducto (precioProducto * cantidad)
    let subtotal = precioProducto * cantidad;

    // Si el usuario quiere personalizar (deseaPersonalizar = true)
    if (deseaPersonalizar) {

        // Obtener la referencia ACTUAL al select insertado dinámicamente
        const selectEmpaque = document.querySelector("select[name='IdEmpaque']");

        // Inicializar el precio del empaque en 0
        let precioEmpaque = 0;
        // Obtener el precio del empaque

        //Validar que se seleccione algo en el select de Empaque
        if (selectEmpaque && selectEmpaque.value !== "") {

            // asignar la opción seleccionada del empaque
            const opcionSeleccionada = selectEmpaque.selectedOptions[0];

            // obtener el precio del empaque de la opción seleccionada 
            // a través del dataset
            precioEmpaque = parseFloat(opcionSeleccionada?.dataset.precio || 0);
            console.log("Precio empaque seleccionado:", precioEmpaque);
        }

        // Agregarle a subtotal precioEmpaque * cantidad Producto
        subtotal += precioEmpaque * cantidad;

        // Sumarle al subTotal el precio por defecto del mensaje personalizado
        subtotal += parseFloat(form.dataset.precioMensaje || 0);

        // Sumarle al subTotal el precio por defecto de la foto
        subtotal += parseFloat(form.dataset.precioFoto || 0);
    }

    // Darle formato al campo de SubTotal (separador de miles y 2 decimales)
    subtotalSpan.textContent = subtotal.toLocaleString("es-CR", {
        style: "decimal",
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
}

// INICIALIZACIÓN DE FUNCIONES NECESARIAS CUANDO SE CARGA EL DOM

// Asignar función que maneja el envio de la reseña 
// al evento submit solo cuando el DOM esté listo
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

// Cuando el DOM esté listo, realizar la validación del input de cantidad para
// añadir el producto al carrito
document.addEventListener("DOMContentLoaded", function () {
    escucharInputCantidad();
    bloquearBorradoInputCantidad();
});

// Cuando el DOM esté listo ,asignar el evento de submit del formulario de detalle de carrito 
document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("formCarritoDetalle");
    if (form) {
        form.addEventListener("submit", manejarEnvioDetalleCarrito);
    }
});

// Cuando el DOM esté listo, cargar los datos de los inputs para poder calcular
// dinámicamente el subTotal
document.addEventListener("DOMContentLoaded", () => {

    // Obtener los valores de los inputs del formulario para 
    // calcular dinámicamente el subTotal
    const form = document.getElementById("formCarritoDetalle");
    const inputCantidad = document.getElementById("inputCantidad");
    const mensajeTextarea = document.querySelector("textarea[name='MensajePersonalizado']");
    const dropzone = document.querySelector('[x-data="dataSingleFileDnD()"]');
    const radioPersonalizar = document.querySelectorAll("input[name='DeseaPersonalizar']");
    const subtotalSpan = document.getElementById("subTotalValor");

    // Escuchar cambio en el input de Cantidad para llamar a la función de calcularSubTotal
    inputCantidad.addEventListener("input", calcularSubtotal);

    // Escuchar cambio en el input de mensajePersonalizado para llamar a la función de calcularSubTotal
    mensajeTextarea?.addEventListener("input", calcularSubtotal);

    // Escuchar cambio en el radioButton de personalizar
    radioPersonalizar.forEach(r => {
        r.addEventListener("change", () => {
            calcularSubtotal(); // recalcular subtotal al cambiar opción

            // Si hay un radioButton chequeado y
            // la opción Sí está seleccionada,
            // disparar el evento manual de select de 
            // tipo de empaque listo
            if (r.value === "si" && r.checked) {
                console.log("Radio sí seleccionado → disparando evento manual");
                window.dispatchEvent(new Event("select-empaque-listo"));
            }
        });
    });

    // En Alpine: emitir evento desde dropzone cuando cambie imagen
    window.addEventListener("imagen-personalizada-cambiada", calcularSubtotal);

    // Iniciar la función de calcular subTotal
    calcularSubtotal();

    // Alpine.js tiene envuelto el select de tipoEmpaque en una etiqueta template (x-if)
    // lo cual está ocasionando problemas con cargar el select en el DOM
    // correctamente.
    // Para que la función "calcularSubtotal" pueda escuchar los cambios del select
    // de tipo de empaque hay que manualmente registrar un evento listener manual
    // para poder manejar el cambio del select dinámico. 
    //window.addEventListener("select-empaque-listo", () => {
    //    // Esperar al siguiente "tick" para que el DOM esté completamente actualizado
    //    setTimeout(() => {
    //        const selectEmpaque = document.querySelector("select[name='IdEmpaque']");
    //        if (selectEmpaque && !selectEmpaque.dataset.listenerAttached) {
    //            console.log("Select encontrado y listener agregado");
    //            selectEmpaque.addEventListener("change", () => {
    //                const opcion = selectEmpaque.selectedOptions[0];
    //                const precio = opcion?.dataset.precio || 0;
    //                console.log("Cambio en select:", selectEmpaque.value, "→ Precio:", precio);
    //                calcularSubtotal();
    //            });

    //            selectEmpaque.dataset.listenerAttached = "true";
    //        } else {
    //            console.log("Select no encontrado al hacer dispatch del evento");
    //        }

    //        calcularSubtotal();
    //    }, 0); // usar 0 para que espere al siguiente ciclo de render
    //});


});

window.addEventListener("select-empaque-listo", () => {
    // Esperar al siguiente "tick" para que el DOM esté completamente actualizado
    setTimeout(() => {
        const selectEmpaque = document.querySelector("select[name='IdEmpaque']");
        if (selectEmpaque && !selectEmpaque.dataset.listenerAttached) {
            console.log("Select encontrado y listener agregado");
            selectEmpaque.addEventListener("change", () => {
                const opcion = selectEmpaque.selectedOptions[0];
                const precio = opcion?.dataset.precio || 0;
                console.log("Cambio en select:", selectEmpaque.value, "→ Precio:", precio);
                calcularSubtotal();
            });

            selectEmpaque.dataset.listenerAttached = "true";
        } else {
            console.log("Select no encontrado o listener ya agregado");
        }

        calcularSubtotal();
    }, 0);
});

// Cuando el DOM esté listo, habilitar o deshabilitar el botón "Agregar al carrito"
// si el input de cantidad es 0
document.addEventListener("DOMContentLoaded", () => {

    // Obtener los elementos del DOM por id (inputCantidad y btnAgregarCarrito)
    const inputCantidad = document.getElementById("inputCantidad");
    const btnAgregarCarrito = document.getElementById("btnAgregarCarrito");

    // Función para actualizar el estado del Botón de "Agregar al carrito"
    function actualizarEstadoBoton() {
        const cantidad = parseInt(inputCantidad.value);
        if (isNaN(cantidad) || cantidad <= 0) {
            btnAgregarCarrito.disabled = true;
            btnAgregarCarrito.classList.add("btn-disabled", "btn", "btn-soft", "text-black", "cursor-not-allowed");
        } else {
            btnAgregarCarrito.disabled = false;
            btnAgregarCarrito.classList.remove("btn-disabled", "text-white", "cursor-not-allowed");
        }
    }

    // Agregar un listener activo para cuando cambie el valor del
    // input cantidad para habilitar o deshabilitar el botón
    inputCantidad.addEventListener("input", actualizarEstadoBoton);

    // Ejecutar una vez al cargar la página
    actualizarEstadoBoton();
});





