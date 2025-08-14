// Función para ayudar al toast a mostrar
// el texto traducido. 
window.getTranslation = function (key) {
    const lang = getCurrentLangShort();
    // Si existe traducción para la clave y el idioma, la devuelve
    if (translations[key] && translations[key][lang]) {
        return translations[key][lang];
    }
    // Fallback: español, o la clave si no existe nada
    return translations[key]?.es || key;
}

// Función para traducir mensajes de éxito y error dentro de
// la ventana global
window.translations = {
    "ProductoAgregado": {
        es: "¡Producto creado correctamente!",
        en: "Product created successfully!"
    },
    "ProductoActualizado": {
        es: "¡Producto actualizado correctamente!",
        en: "Product updated successfully!"
    },
    "PromocionCreada": {
        es: "¡Promoción creada correctamente!",
        en: "Promotion created successfully!"
    },
    "PromocionActualizada": {
        es: "¡Promoción actualizada correctamente!",
        en: "Promotion updated successfully!"
    }
};


//Función general para cargar la vista parcial
function cargarVista(ruta) {
    const loader = document.getElementById('loader');
    const container = document.getElementById('contenido-dinamico');

    loader.classList.remove('hidden');
    container.innerHTML = "";

    fetch(ruta)
        .then(response => {
            if (!response.ok) {
                return response.text().then(txt => { throw new Error(txt); });
            }
            return response.text();
        })
        .then(html => {
            setTimeout(() => {
                container.innerHTML = html;
                loader.classList.add('hidden');

                // Cargar validación AJAX para cuando se cargue la vista de _CreatePromocion y _EditPromocion
                $.validator.unobtrusive.parse('#formPromocion');

                // Inicializar la función que escucha el cambio del select de TipoPromoción (_CreatePromocion y _EditPromocion)
                escucharCambioSelectTipoPromocion();

                // Inicializar la función que escucha el input de descuento en las vistas de Promocion (_CreatePromocion y _EditPromocion)
                revisarInputDescuento();

                // Cargar comportamiento de fechas
                cargaFechas();

                // Cargar la función JavaScript si se carga la vista de productos
                if (typeof inicializarVistaProductos === 'function') {
                    inicializarVistaProductos();
                }

                // Cargar la función JavaScript si se hace drag and drop (etiquetas) en crear producto 
                if (document.getElementById('dp1') && typeof inicializarDragAndDropEtiquetas === 'function') {
                    inicializarDragAndDropEtiquetas();
                }

                // Cargar la función JavaScript para inicializar el drop down de productos
                if (typeof inicializarDropdownProductos === 'function') {
                    inicializarDropdownProductos();
                }

                // Inicializar función que escucha el input de precioBase (_CreateProducto)
                if (ruta == "/Producto/Create") {
                    escucharInputPrecioBase();
                }

                // Inicializar función que escucha el input de cantidad (_CreateProducto)
                if (ruta == "/Producto/Create") {
                    escucharInputCantidad();
                }

                // Cargar reseñas del producto
                cargarResennasProducto(null, false);


            }, 300);
        })
        .catch(error => {
            console.error("Error al cargar la vista:", error);
            container.innerHTML = `<p class="text-red-600 font-semibold">ERROR<br> ${error.message}</p>`;
        });
}

//Función para cargar la vista de cards de los productos
function inicializarVistaProductos() {
    const toggle = document.querySelector('input[type="checkbox"]');
    const loader = document.getElementById('loader');
    const container = document.getElementById('card-productos-body-admin');

    if (!toggle || !loader || !container) return;

    toggle.addEventListener('change', () => {
        const idCategoria = toggle.checked ? 2 : 1;
        cargarProductosPorCategoria(idCategoria);
    });

    function cargarProductosPorCategoria(idCategoria) {
        loader.classList.remove('hidden');
        container.innerHTML = "";
        fetch(`/Producto/FiltrarPorCategoriaAdmin?idCategoria=${idCategoria}`)
            .then(res => res.text())
            .then(html => {
                setTimeout(() => {
                    container.innerHTML = html;
                    loader.classList.add('hidden');
                }, 500);
            })
            .catch(error => {
                console.error("Error al cargar productos:", error);
                setTimeout(() => {
                    container.innerHTML = "<p class='text-red-500'>ERROR</p>";
                    loader.classList.add('hidden');
                }, 500);
            });
    }

    cargarProductosPorCategoria(1);
    toggle.checked = false;
}

// Función para cargar la vista de detalles del producto
function cargarDetalleProducto(idProducto) {
    const loader = document.getElementById('loader');
    const container = document.getElementById('contenido-dinamico');
    const esVistaDetalle = true;

    loader.classList.remove('hidden');
    container.innerHTML = "";

    fetch(`/Producto/DetailsAdmin/${idProducto}`)
        .then(res => {
            if (!res.ok) throw new Error(`HTTP ${res.status}`);
            return res.text();
        })
        .then(html => {
            setTimeout(() => {
                container.innerHTML = html;
                loader.classList.add('hidden');

                if (typeof inicializarCarrusel === 'function') {
                    inicializarCarrusel();
                }

                // Cargar reseñas del producto
                cargarResennasProducto(idProducto, esVistaDetalle);


            }, 300);
        })
        .catch(err => {
            console.error("Error al cargar detalles:", err);
            container.innerHTML = `<p class="text-red-500">ERROR</p>`;
            loader.classList.add('hidden');
        });
}

// Función para cargar la vista de detalles de la reseña
function cargarDetalleResenna(idResenna) {
    const loader = document.getElementById('loader');
    const container = document.getElementById('contenido-dinamico');

    loader.classList.remove('hidden');
    container.innerHTML = "";

    fetch(`/ResennaValoracion/DetailsAdmin/${idResenna}`)
        .then(res => {
            if (!res.ok) throw new Error(`HTTP ${res.status}`);
            return res.text();
        })
        .then(html => {
            setTimeout(() => {
                container.innerHTML = html;
                loader.classList.add('hidden');
            }, 300);
        })
        .catch(err => {
            console.error("Error al cargar detalles:", err);
            container.innerHTML = `<p class="text-red-500">ERROR</p>`;
            loader.classList.add('hidden');
        });
}

// Función para cargar la vista de detalles de la promocion
function cargarDetallePromocion(idPromocion) {
    const loader = document.getElementById('loader');
    const container = document.getElementById('contenido-dinamico');

    loader.classList.remove('hidden');
    container.innerHTML = "";

    fetch(`/Promocion/DetailsAdmin/${idPromocion}`)
        .then(res => {
            if (!res.ok) throw new Error(`HTTP ${res.status}`);
            return res.text();
        })
        .then(html => {
            setTimeout(() => {
                container.innerHTML = html;
                loader.classList.add('hidden');
            }, 300);
        })
        .catch(err => {
            console.error("Error al cargar detalles:", err);
            container.innerHTML = `<p class="text-red-500">ERROR</p>`;
            loader.classList.add('hidden');
        });
}

// Función para inicializar el carrusel de la vista de detalles del producto
function inicializarCarrusel() {
    let currentSlide = 1;

    function mostrarSlide(n) {
        document.querySelectorAll('.carousel-item').forEach(div => div.classList.add('hidden'));
        const actual = document.getElementById(`slide${n}`);
        if (actual) actual.classList.remove('hidden');
        actualizarBotonActivo(`#slide${n}`);
    }

    function actualizarBotonActivo(hash) {
        document.querySelectorAll('.btn-xs').forEach(btn => {
            if (btn.getAttribute('href') === hash) {
                btn.classList.add('bg-[#004AAD]', 'text-white');
                btn.classList.remove('bg-base-200', 'text-black');
            } else {
                btn.classList.remove('bg-[#004AAD]', 'text-white');
                btn.classList.add('bg-base-200', 'text-black');
            }
        });
    }

    document.querySelectorAll('.btn-xs').forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            const hash = this.getAttribute('href');
            const match = hash.match(/#slide(\d+)/);
            if (match) {
                currentSlide = parseInt(match[1]);
                mostrarSlide(currentSlide);
            }
        });
    });

    mostrarSlide(currentSlide);
}

// Función para cargar las reseñas del producto dentro de la vista de detalle de producto
function cargarResennasProducto(_idProducto, esVistaDetalle) {
    const zonaDetalle = document.getElementById("detalle-producto");
    //_idProducto = zonaDetalle?.dataset?.id;

    // Para poder traducir el texto "Promedio de valoraciones:" hay que buscar el dataset
    // donde se va a traducir el texto (Promedio de valoraciones / Average Rating)

    const promedio = document.getElementById("contenedor-resennas")
        .dataset.promedio || "Average rating:";

    if (_idProducto == null) {
        const html = `
        <div class="flex w-full items-start gap-2 mb-4">
            <span class="font-bold">${promedio}: 0.0</span>
            <div class="flex gap-1">
                ${[...Array(5)].map(() => `
                    <svg xmlns="http://www.w3.org/2000/svg"
                         fill="currentColor"
                         viewBox="0 0 24 24"
                         stroke="none"
                         class="w-5 h-5 text-gray-300">
                        <path d="M12 .587l3.668 7.431 8.2 1.192-5.934 5.782
                                 1.402 8.175L12 18.896l-7.336 3.861
                                 1.402-8.175-5.934-5.782 8.2-1.192z" />
                    </svg>
                `).join('')}
            </div>
        </div>
        `;

        document.getElementById("contenedor-resennas").innerHTML = html;
        return;
    }

    if (esVistaDetalle == true) {
        fetch(`/ResennaValoracion/GetResennasPorProducto?idProducto=${_idProducto}`)
            .then(response => response.text())
            .then(html => {
                document.getElementById("zona-resennas").innerHTML = html;
            })
            .catch(error => {
                document.getElementById("zona-resennas").innerHTML = "<p>ERROR</p>";
            });
    }
    else {
        fetch(`/ResennaValoracion/GetResennasPorProductoAdmin?idProducto=${_idProducto}`)
            .then(response => response.text())
            .then(html => {
                document.getElementById("contenedor-resennas").innerHTML = html;
            })
            .catch(error => {
                document.getElementById("contenedor-resennas").innerHTML = "<p>ERROR</p>";
            });
    }


}

// Función javascript para manejar el drag and drop de las etiquetas al crear un producto
function inicializarDragAndDropEtiquetas() {

    console.log('Drag and Drop inicializado');
    let dragTemp;

    $('.drag').on('dragstart', function (e) {
        dragTemp = e.target;
        console.log('Start', dragTemp);
    });

    $('.drop').on('dragover', function (e) {
        e.preventDefault();
    });

    $('.drop').on('dragenter', function () {
        $(this).addClass('ring ring-blue-400');
    });

    $('.drop').on('dragleave drop', function () {
        $(this).removeClass('ring ring-blue-400');
    });

    $('.drop').on('drop', function (e) {
        e.preventDefault();
        if (dragTemp && this !== dragTemp.parentElement) {
            this.appendChild(dragTemp);
            console.log('Drop en:', this.id);
        }
        dragTemp = null;

        if (this.id === 'dp2') {
            $('#dp2').children('.drag').each(function () {
                console.log(this.innerText);
            });
        }
    });
}

// Función javascript para cargar la vista de editar producto (_EditProducto)
// Función para cargar la vista parcial de edición de un producto
function cargarEditarProducto(idProducto) {
    const loader = document.getElementById('loader');
    const container = document.getElementById('contenido-dinamico');
    const esVistaDetalle = false;


    loader.classList.remove('hidden');
    container.innerHTML = "";

    fetch(`/Producto/Edit/${idProducto}`)
        .then(res => {
            if (!res.ok) throw new Error(`HTTP ${res.status}`);
            return res.text();
        })
        .then(html => {
            setTimeout(() => {
                container.innerHTML = html;
                loader.classList.add('hidden');

                // Inicializar función de drag and drop para las etiquetas
                if (document.getElementById('dp1') && typeof inicializarDragAndDropEtiquetas === 'function') {
                    inicializarDragAndDropEtiquetas();
                }

                // Cargar reseñas del producto
                cargarResennasProducto(idProducto, esVistaDetalle);

                // Inicializar función que escucha el input de precioBase (_CreateProducto y _EditProducto)
                escucharInputPrecioBase();

                // Inicializar función que escucha el input de cantidad (_CreateProducto y _EditoProducto)
                escucharInputCantidad();


            }, 300);
        })
        .catch(err => {
            console.error("Error al cargar la vista de edición:", err);
            container.innerHTML = `<p class="text-red-500">ERROR</p>`;
            loader.classList.add('hidden');
        });
}

// Función javascript para cargar la vista de editar promocion (_EditPromocion)
// Función para cargar la vista parcial de edición de una promoción
function cargarEditarPromocion(idPromocion) {
    const loader = document.getElementById('loader');
    const container = document.getElementById('contenido-dinamico');

    loader.classList.remove('hidden');
    container.innerHTML = "";

    fetch(`/Promocion/Edit/${idPromocion}`)
        .then(res => {
            if (!res.ok) throw new Error(`HTTP ${res.status}`);
            return res.text();
        })
        .then(html => {
            setTimeout(() => {
                container.innerHTML = html;
                loader.classList.add('hidden');

                // Cargar validación AJAX para cuando se cargue la vista de _CreatePromocion y _EditPromocion
                $.validator.unobtrusive.parse('#formPromocion');

                // Cargar comportamiento de fechas
                cargaFechas();

                // Inicialiar la función que se encarga de mostrar el select de categoría o el dropdown de productos
                mostrarGrupoSegunTipoInicial();

                // Inicializar la función para mostrar el select de categoria o drop down de productos 
                escucharCambioSelectTipoPromocion();

                // Inicializar función para escuchar los cambios de checkboxes en el drop down de productos
                // seleccionados (en _EditPromocion)
                escucharCheckboxProductos();

                // Inicializar la función que escucha el input de descuento en las vistas de Promocion (_CreatePromocion y _EditPromocion)
                revisarInputDescuento();

                // Inicializar la función que controla la visibilidad del select de tipo, categoria y el checkbox de estado
                // según el estado de la promoción
                controlarVisibilidadSegunEstado();



            }, 300);
        })
        .catch(err => {
            console.error("Error al cargar la vista de edición:", err);
            container.innerHTML = `<p class="text-red-500">ERROR</p>`;
            loader.classList.add('hidden');
        });
}

// Función para mostrar el toast en las vistas parciales
function mostrarToast(mensaje, tipo = "info") {
    const toast = document.createElement("div");
    toast.className = `toast toast-top toast-center z-50`;
    if (tipo === "error") {
        toast.innerHTML = `
            <div class="alert alert-${tipo}" style="background-color:#FF0000">
                <span class="text-white font-bold">${mensaje}</span>
            </div>
        `;
    }
    else {
        toast.innerHTML = `
            <div class="alert alert-${tipo}">
                <span class="text-black font-bold">${mensaje}</span>
            </div>
        `;
    }

    document.body.appendChild(toast);

    setTimeout(() => {
        toast.remove();
    }, 4000);
}

// Función para "escuchar" mientras el usuario escribe en el campo de "Precio base"
// en _CreateProducto.cshtml y _Editproducto.cshtml

function escucharInputPrecioBase() {
    const input = document.getElementById("inputPrecioBase");
    const mensajeError = document.querySelector("span[data-valmsg-for='PrecioBase']");


    // Prevenir ingreso de "e", "+", "-" y otros caracteres no deseados
    input.addEventListener("keydown", (event) => {
        const teclasNoPermitidas = ["e", "E", "+", "-"];
        if (teclasNoPermitidas.includes(event.key)) {
            event.preventDefault();
        }
    });

    // Bloquear pegado de caracteres no permitidos
    input.addEventListener("paste", (event) => {
        const textoPegado = (event.clipboardData || window.clipboardData).getData("text");
        if (/[eE+\-]/.test(textoPegado)) {
            event.preventDefault();
        }
    });

    // Para mostrar error internacionalizado
    const msjPrecioBase = document.getElementById("msj-precio-base").dataset.msjRango;

    // Evento para escuchar el evento "input" de
    // precioBase y realizar las validacines necesarias
    // como insertar solo 2 decimales, revisar que el precio 
    // esté en el rango deseado (5000 y 100000)
    input.addEventListener("input", () => {
        // Limitar a 2 decimales
        let valor = input.value;

        if (valor.includes(".")) {
            const [entero, decimales] = valor.split(".");
            if (decimales.length > 2) {
                input.value = `${entero}.${decimales.slice(0, 2)}`;
                return;
            }
        }

        // Validar rango
        const numero = parseFloat(input.value);

        if (!isNaN(numero)) {
            if (numero < 5000 || numero > 100000) {
                if (mensajeError) {
                    mensajeError.textContent = msjPrecioBase;
                }

            } else {
                if (mensajeError) {
                    mensajeError.textContent = "";
                }
                // Esta clase no se está borrando en el
                // código
                input.classList.remove("border-red-500");
            }
        } else {
            if (mensajeError) mensajeError.textContent = "";
            // Esta clase no se está borrando en el
            // código
            input.classList.remove("border-red-500");
        }
    });
}

// Función para "escuchar" mientras el usuario escribe en el campo de "Cantidad"
// en _CreateProducto.cshtml y _EditProducto.cshtml
function escucharInputCantidad() {
    const input = document.getElementById("inputCantidad");
    const mensajeError = document.querySelector("span[data-valmsg-for='Stock']");
    const msjCantidad = document.getElementById("msj-cantidad")?.dataset.msjCantidad;

    if (!input) return;

    input.addEventListener("input", () => {
        // Eliminar decimales si el usuario los escribe
        input.value = input.value.replace(/[^\d]/g, ""); // Solo dígitos

        const numero = parseInt(input.value);

        if (!isNaN(numero)) {
            if (numero < 1 || numero > 100) {
                if (mensajeError) {
                    mensajeError.textContent = msjCantidad;
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

// Función para poder cargar las páginas de la tablas de las vistas parciales
// sin recargar la página

// Lógica para manejar paginación de reseñas y promociones sin recargar toda la página
document.addEventListener('click', function (e) {
    // Paginación de reseñas
    if (e.target.matches('a.pagina-resenna')) {
        e.preventDefault();
        const url = e.target.getAttribute('href');

        fetch(url)
            .then(res => {
                if (!res.ok) throw new Error("ERROR");
                return res.text();
            })
            .then(html => {
                document.getElementById("zona-resennas").innerHTML = html;
            })
            .catch(err => {
                console.error("Error al cargar reseñas:", err);
                document.getElementById("zona-resennas").innerHTML = "<p class='text-red-500'>ERROR</p>";
            });
    }

    // Paginación de promociones
    if (e.target.matches('a.pagina-promocion')) {
        e.preventDefault();
        const url = e.target.getAttribute('href');

        fetch(url)
            .then(res => {
                if (!res.ok) throw new Error("Error al obtener promociones");
                return res.text();
            })
            .then(html => {
                document.getElementById("zona-promociones").innerHTML = html;
            })
            .catch(err => {
                console.error("Error al cargar promociones:", err);
                document.getElementById("zona-promociones").innerHTML = "<p class='text-red-500'>ERROR</p>";
            });
    }
});

// Función para escuchar el evento submit del formulario de _CreatePromocion para mostrar errores
// (si los hay)
// Escuchar el evento submit del formulario de _CreatePromocion y _EditPromocion
// para realizar validación del lado del cliente
function validacionFormularioPromocion() {
    const form = document.getElementById('formPromocion');
    if (form) {

        const tipoSelect = document.getElementById('tipoPromocion');
        const errorSpan = document.getElementById('errorTipoPromocion');
        const fechaInicioInput = document.getElementById('fechaInicio');
        const fechaFinInput = document.getElementById('fechaFin');
        const errorInicio = document.getElementById('errorFechaInicio');
        const errorFin = document.getElementById('errorFechaFin');

        let isValid = true;

        // Para mostrar mensaje de error internacionalizado
        const msjTipoPromocion = document.getElementById("msj-tipo-promocion").dataset.msjTipo;

        // Validar tipo de promoción
        if (tipoSelect && tipoSelect.value === "") {
            isValid = false;
            if (errorSpan) {
                errorSpan.textContent = msjTipoPromocion;
                errorSpan.classList.remove("hidden");
            }
            tipoSelect.classList.add("border-red-500");
        } else {
            if (errorSpan) {
                errorSpan.textContent = "";
                errorSpan.classList.add("hidden");
            }
            tipoSelect?.classList.remove("border-red-500");
        }

        // Validar fechas
        const fechaInicioStr = fechaInicioInput?.value;
        const fechaFinStr = fechaFinInput?.value;

        // Convertir dd/mm/yyyy a objeto Date
        function convertirFecha(fechaStr) {
            const [dia, mes, anio] = fechaStr.split('/');
            return new Date(`${anio}-${mes}-${dia}`);
        }

        const hoy = new Date();
        hoy.setHours(0, 0, 0, 0); // Limpiar hora

        const fechaInicio = convertirFecha(fechaInicioStr);
        const fechaFin = convertirFecha(fechaFinStr);

        // Validar fecha de inicio
        // Traer el valor del localizer para mostrar el error internacionalizado
        const msjFechaInicio = document.getElementById("errorFechaInicio").dataset.msjFechaInicio
        if (!fechaInicioStr || isNaN(fechaInicio.getTime())) {
            isValid = false;
            errorInicio.textContent = msjFechaInicio;
            errorInicio.classList.remove("hidden");
            fechaInicioInput.classList.add("border-red-500");
        } else {
            errorInicio.textContent = "";
            errorInicio.classList.add("hidden");
            fechaInicioInput.classList.remove("border-red-500");
        }

        // Validar fecha de fin
        // Traer el valor del localizer para mostrar el error internacionalizado
        const msjFechaFin = document.getElementById("errorFechaFin").dataset.msjFechaFin
        if (!fechaFinStr || isNaN(fechaFin.getTime())) {
            isValid = false;
            errorFin.textContent = msjFechaFin;
            errorFin.classList.remove("hidden");
            fechaFinInput.classList.add("border-red-500");
        } else {
            errorFin.textContent = "";
            errorFin.classList.add("hidden");
            fechaFinInput.classList.remove("border-red-500");
        }

        // Validar selección de productos solo si el grupo está visible
        const grupoProducto = document.getElementById('grupoProducto');
        if (!grupoProducto.classList.contains('hidden')) {
            const productoChecks = document.querySelectorAll('.producto-checkbox');
            const errorProductos = document.getElementById('errorProductos');

            const algunoSeleccionado = Array.from(productoChecks).some(cb => cb.checked);

            if (!algunoSeleccionado) {
                isValid = false;

                if (errorProductos) {
                    errorProductos.classList.remove("hidden");
                }
            } else {
                if (errorProductos) {
                    errorProductos.classList.add("hidden");
                }
            }
        }


        // Validaciones unobtrusive de ASP.NET
        if (!$(form).valid()) {
            isValid = false;
        }

        // Convertir fechas antes de enviar si todo es válido
        if (isValid) {
            const formatoNET = "Y-m-d"; // yyyy-MM-dd
            const pickerInicio = flatpickr.parseDate(fechaInicioInput.value, "d/m/Y");
            const pickerFin = flatpickr.parseDate(fechaFinInput.value, "d/m/Y");

            if (pickerInicio) {
                fechaInicioInput.value = flatpickr.formatDate(pickerInicio, formatoNET);
            }
            if (pickerFin) {
                fechaFinInput.value = flatpickr.formatDate(pickerFin, formatoNET);
            }


            // Si el formulario es válido
            // enviar la información a "revisarRespuestaPostPromocion"
            if (isValid) {
                revisarRespuestaPostPromocion(new FormData(form));
            }
        }
    }
}

// Función que se encarga de escuchar el cambio del select de tipo de 
// promoción
function escucharCambioSelectTipoPromocion() {
    const tipoSelect = document.getElementById('tipoPromocion');
    const grupoCategoria = document.getElementById('grupoCategoria');
    const grupoProducto = document.getElementById('grupoProducto');
    const selectCategoria = document.querySelector('select[name="IdCategoriaSeleccionada"]');
    const checkboxesProducto = document.querySelectorAll('.producto-checkbox');
    const selectAllCheckbox = document.getElementById('checkboxSelectAll');
    const dropdownLabel = document.getElementById('dropdownLabel');

    if (!tipoSelect) return;

    //Esta parte se ejecuta una vez al cargar(modo edición) para poder seleccionar los productos
    // de forma automática cuando se carga la edición de promoción

    const tipoInicial = tipoSelect.value;

    tipoSelect.addEventListener('change', function () {
        const tipo = tipoSelect.value;

        if (tipo === "Categoria") {
            grupoCategoria.classList.remove('hidden');
            grupoProducto.classList.add('hidden');

            // Limpiar selección de productos si se oculta
            checkboxesProducto.forEach(cb => cb.checked = false);
            if (selectAllCheckbox) selectAllCheckbox.checked = false;
            if (dropdownLabel) {
                // Obtener el texto traducido del atributo data-text-none (desde la vista)
                const textNone = dropdownLabel.dataset.textNone || "Select one or more products";
                dropdownLabel.textContent = textNone;
            }

        } else if (tipo === "Producto") {
            grupoProducto.classList.remove('hidden');
            grupoCategoria.classList.add('hidden');

            // Limpiar selección de categoría si se oculta
            if (selectCategoria) selectCategoria.value = "";

        } else {
            // Si no se selecciona tipo, ocultar ambos
            grupoCategoria.classList.add('hidden');
            grupoProducto.classList.add('hidden');

            if (selectCategoria) selectCategoria.value = "";
            checkboxesProducto.forEach(cb => cb.checked = false);
            if (selectAllCheckbox) selectAllCheckbox.checked = false;
            if (dropdownLabel) {
                // Obtener el texto traducido del atributo data-text-none (desde la vista)
                const textNone = dropdownLabel.dataset.textNone || "Select one or more products";
                dropdownLabel.textContent = textNone;
            }
        }

    });
}

// Función que se encarga de mostrar los productos seleccionados 
// cuando hayan productos seleccionados en la vista precargada de _EditPromocion
function actualizarLabelProductosSeleccionados() {
    const label = document.getElementById("dropdownLabel");
    const seleccionados = document.querySelectorAll(".producto-checkbox:checked").length;

    if (label) {
        // Lee los textos traducidos desde data-attributes
        const textN = label.dataset.textN || "{0} product(s) selected";
        const textNone = label.dataset.textNone || "Select one or more products";

        label.textContent = seleccionados > 0
            ? textN.replace("{0}", seleccionados)
            : textNone;
    }
}

// Función para marcar automáticamente de mostrar el select de
// categoria (si la promocion es por categoria) o la lista de 
// productos (si la promocion es por producto)
function mostrarGrupoSegunTipoInicial() {
    const tipoSelect = document.getElementById('tipoPromocion');
    const grupoCategoria = document.getElementById('grupoCategoria');
    const grupoProducto = document.getElementById('grupoProducto');

    if (!tipoSelect) return;

    const valorTipo = tipoSelect.value;
    console.log("Valor actual de tipoPromocion al cargar:", valorTipo);

    if (valorTipo === "Categoria") {
        grupoCategoria.classList.remove('hidden');
        grupoProducto.classList.add('hidden');
    } else if (valorTipo === "Producto") {
        grupoProducto.classList.remove('hidden');
        grupoCategoria.classList.add('hidden');

        actualizarLabelProductosSeleccionados();
    } else {
        grupoCategoria.classList.add('hidden');
        grupoProducto.classList.add('hidden');
    }
}


// Función para inicializar el dropdown de productos
function inicializarDropdownProductos() {
    const dropdown = document.querySelector('.dropdown');
    const dropdownToggle = document.getElementById('dropdownToggle');
    const dropdownContent = document.getElementById('listaProductos');
    const checks = document.querySelectorAll('.producto-checkbox');
    const label = document.getElementById('dropdownLabel');
    const selectAll = document.getElementById('checkboxSelectAll');

    if (!dropdown || !dropdownToggle || !dropdownContent) return;

    // Mostrar u ocultar el dropdown al hacer clic en el botón
    dropdownToggle.addEventListener('click', function (e) {
        e.stopPropagation();
        dropdownContent.classList.toggle('hidden');
    });

    // Evitar que clics en cualquier parte del menú (etiquetas, texto) cierren el dropdown
    dropdownContent.addEventListener('click', function (e) {
        e.stopPropagation(); //
    });

    // Cerrar si se hace clic fuera del dropdown
    document.addEventListener('click', function (e) {
        if (!dropdown.contains(e.target)) {
            dropdownContent.classList.add('hidden');
        }
    });
    function actualizarLabelDropdownProducto() {
        const label = document.getElementById("dropdownLabel");
        const seleccionados = Array.from(checks).filter(c => c.checked);

        // Si noa hay label, devolverse
        if (!label) return;

        // Tomar los textos traducidos de los data-attributes desde el frontend
        const textNone = label.dataset.textNone || "Select one or more products";
        const textN = label.dataset.textN || "{0} products selected";

        if (seleccionados.length === 0) {
            label.textContent = textNone;
        } else if (seleccionados.length === 1) {
            label.textContent = seleccionados[0].nextElementSibling.textContent;
        } else {
            label.textContent = textN.replace("{0}", seleccionados.length);
        }

        if (selectAll) {
            selectAll.checked = seleccionados.length === checks.length;
        }
    }

    checks.forEach(cb => {
        cb.addEventListener("change", actualizarLabelDropdownProducto);
    });

    if (selectAll) {
        selectAll.addEventListener("change", () => {
            const marcado = selectAll.checked;
            checks.forEach(cb => cb.checked = marcado);
            actualizarLabelDropdownProducto();
        });
    }

    actualizarLabelDropdownProducto();
}

// Función para escuchar cuando se seleccionen checkboxes en 
// el drop down de la vista de _EditPromocion
function escucharCheckboxProductos() {
    const checkboxes = document.querySelectorAll('.producto-checkbox');
    const selectAllCheckbox = document.getElementById('checkboxSelectAll');

    // Escuchar cambios individuales
    checkboxes.forEach(cb => {
        cb.addEventListener('change', actualizarLabelProductosSeleccionados);
    });

    // Escuchar "Seleccionar todos"
    if (selectAllCheckbox) {
        selectAllCheckbox.addEventListener('change', () => {
            const checked = selectAllCheckbox.checked;
            checkboxes.forEach(cb => cb.checked = checked);
            actualizarLabelProductosSeleccionados();
        });
    }
}


// Función de Flatpickr para poder bloquear que la fecha de Fin sea menor a fecha de Inicio
// y también que fecha de inicio tenga como mínimo hoy
function cargaFechas() {
    const hoy = new Date().toISOString().split("T")[0];

    const pickerInicio = flatpickr("#fechaInicio", {
        dateFormat: "d/m/Y",
        locale: "es",
        minDate: "today",
        defaultDate: hoy,
        onChange: function (selectedDates, dateStr, instance) {
            // Cuando se cambia la fecha de inicio, actualizar minDate en fechaFin
            if (selectedDates.length > 0) {
                pickerFin.set('minDate', selectedDates[0]);
            }
        }
    });

    const pickerFin = flatpickr("#fechaFin", {
        dateFormat: "d/m/Y",
        locale: "es",
        minDate: hoy
    });
}


// Función para evitar que el usuario digite para el campo descuento (_CreatePromocion y _EditPromocion)
// más de 3 dígitos y tampoco decimales
function revisarInputDescuento() {
    const descuentoInput = document.querySelector('[name="PorcentajeDescuento"]');

    if (descuentoInput) {
        descuentoInput.addEventListener('input', function (e) {
            // Remover caracteres no numéricos (excepto números)
            this.value = this.value.replace(/[^0-9]/g, '');

            // Limitar a 3 dígitos
            if (this.value.length > 3) {
                this.value = this.value.slice(0, 3);
            }

            // Validar valor máximo permitido (100)
            const valor = parseInt(this.value);
            if (valor > 100) {
                this.value = '100';
            }
        });

        // Previene que se escriban puntos o comas directamente
        descuentoInput.addEventListener('keypress', function (e) {
            if (e.key === '.' || e.key === ',' || e.key === '-') {
                e.preventDefault();
            }
        });
    }
}

// Función para revisar respuesta al crear o actualizar Promoción
// y mostrar el toast correspondiente
async function revisarRespuestaPostPromocion(formData) {
    try {
        const id = formData.get("Id"); // null o "" si es nueva
        const esEdicion = id !== null && id !== "" && !isNaN(parseInt(id));

        const url = esEdicion ? `/Promocion/Edit/${id}` : "/Promocion/Create";
        const method = "POST";

        const response = await fetch(url, {
            method: method,
            body: formData
        });

        if (response.ok) {
            const data = await response.json();

            if (data.success) {
                mostrarToast(getTranslation(data.mensaje), "success");
                cargarVista("/Promocion/IndexAdmin"); 

            } else {
                console.warn("Error lógico:", data);
            }

        } else {
            // Cargar vista parcial con errores de validación
            const html = await response.text();
            document.getElementById("contenido-dinamico").innerHTML = html;
        }

    } catch (err) {
        console.error("Error en revisarRespuestaPostPromocion:", err);
    }
}

// Función para controlarVisibilidad de los controles de categoria y producto
// si la promoción no está activa
function controlarVisibilidadSegunEstado() {
    const estadoCheckbox = document.getElementById("estadoPromocion");
    const tipoPromocion = document.getElementById("grupoTipoPromocion");
    const grupoCategoria = document.getElementById("grupoCategoria");
    const grupoProducto = document.getElementById("grupoProducto");
    //const tipoSelect = document.getElementById("tipoPromocion");

    function actualizarVisibilidad() {
        const estaActiva = estadoCheckbox.checked;

        if (!estaActiva) {
            // Si está inactiva, ocultar todo
            tipoPromocion.classList.add("hidden");
            grupoCategoria.classList.add("hidden");
            grupoProducto.classList.add("hidden");
        } else {
            // Si está activa, mostrar tipoPromocion
            tipoPromocion.classList.remove("hidden");

        }
    }

    estadoCheckbox.addEventListener("change", actualizarVisibilidad);
    console.log("Estado cambiado a:", estadoCheckbox.checked);

    // Llamada inicial al cargar la vista
    actualizarVisibilidad();
}

// Función para obtener la cookie inyectada por ASP.NET Core para el idioma
function getAspNetCultureCookie() {
    // Obtener el valor de la cookie .AspNetCore.Culture
    const name = '.AspNetCore.Culture=';
    // Decodificar y separar las cookies
    const decoded = decodeURIComponent(document.cookie);
    // Separar las cookies por punto y coma
    const ca = decoded.split(';');
    // Recorrer las cookies para encontrar la que nos interesa
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i].trim();
        // Si la cookie comienza con el nombre que buscamos, devolver su valor
        if (c.indexOf(name) === 0) {
            return c.substring(name.length, c.length);
        }
    }
    return null;
}

// Función para obtener la cultura actual del usuario desde la cookie .AspNetCore.Culture
function getCurrentCulture() {
    // Obtener la cookie de cultura inyectada por ASP.NET Core
    const cookie = getAspNetCultureCookie();
    // Si no existe la cookie, devolver el idioma por defecto (español)
    if (!cookie) return "en"; // idioma por defecto
    // El formato es: c=en-US|uic=en-US
    const match = cookie.match(/c=([^|]+)/);
    return match ? match[1] : "en";
}

// Función para obtener el idioma actual en formato corto (en o es)
function getCurrentLangShort() {
    // Devuelve "en" o "es"
    const culture = getCurrentCulture();
    return culture.split('-')[0];
}






















