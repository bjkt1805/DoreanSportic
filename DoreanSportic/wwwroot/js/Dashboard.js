//Función general para cargar la vista parcial
function cargarVista(ruta) {
    const loader = document.getElementById('loader');
    const container = document.getElementById('contenido-dinamico');

    loader.classList.remove('hidden');
    container.innerHTML = "";

    fetch(ruta)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error ${response.status}`);
            }
            return response.text();
        })
        .then(html => {
            setTimeout(() => {
                container.innerHTML = html;
                loader.classList.add('hidden');

                // Cargar validación AJAX para cuando se cargue la vista de _CreatePromocion
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

                // Inicializar función que escucha el input de precioBase (_CreateProducto y _EditProducto)
                escucharInputPrecioBase();

                // Inicializar función que escucha el input de cantidad (_CreateProducto y _EditoProducto)
                escucharInputCantidad();

            }, 300);
        })
        .catch(error => {
            console.error("Error al cargar la vista:", error);
            container.innerHTML = `<p class="text-red-600 font-semibold">Error al cargar la vista.</p>`;
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
                    container.innerHTML = "<p class='text-red-500'>Error al cargar los productos.</p>";
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
                cargarResennasProducto();


            }, 300);
        })
        .catch(err => {
            console.error("Error al cargar detalles:", err);
            container.innerHTML = `<p class="text-red-500">Error al cargar detalles del producto.</p>`;
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
            container.innerHTML = `<p class="text-red-500">Error al cargar detalles de la reseña.</p>`;
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
            container.innerHTML = `<p class="text-red-500">Error al cargar detalles de la promoción.</p>`;
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

// Función para cargar las reseñas del producto den tro de la vista de detalle de producto
function cargarResennasProducto() {
    const zonaDetalle = document.getElementById("detalle-producto");
    const idProducto = zonaDetalle?.dataset?.id;

    if (!idProducto) return;

    fetch(`/ResennaValoracion/GetResennasPorProducto?idProducto=${idProducto}`)
        .then(response => response.text())
        .then(html => {
            document.getElementById("zona-resennas").innerHTML = html;
        })
        .catch(error => {
            document.getElementById("zona-resennas").innerHTML = "<p>Error cargando reseñas.</p>";
        });
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

            }, 300);
        })
        .catch(err => {
            console.error("Error al cargar la vista de edición:", err);
            container.innerHTML = `<p class="text-red-500">Error al cargar la vista de edición del producto.</p>`;
            loader.classList.add('hidden');
        });
}

// Función para mostrar el toast en las vistas parciales
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

// Función para "escuchar" mientras el usuario escribe en el campo de "Precio base"
// en _CreateProducto.cshtml y _Editproducto.cshtml

function escucharInputPrecioBase() {
    const input = document.getElementById("inputPrecioBase");
    const mensajeError = document.querySelector("span[data-valmsg-for='PrecioBase']");

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
                    mensajeError.textContent = "Debe ingresar un valor entre ₡5 000 y ₡100 000";
                }
                input.classList.add("border-red-500");
            } else {
                if (mensajeError) {
                    mensajeError.textContent = "";
                }
                input.classList.remove("border-red-500");
            }
        } else {
            if (mensajeError) mensajeError.textContent = "";
            input.classList.remove("border-red-500");
        }
    });
}

// Función para "escuchar" mientras el usuario escribe en el campo de "Cantidad"
// en _CreateProducto.cshtml y _EditProducto.cshtml
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
                if (!res.ok) throw new Error("Error al obtener reseñas");
                return res.text();
            })
            .then(html => {
                document.getElementById("zona-resennas").innerHTML = html;
            })
            .catch(err => {
                console.error("Error al cargar reseñas:", err);
                document.getElementById("zona-resennas").innerHTML = "<p class='text-red-500'>Error al cargar reseñas.</p>";
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
                document.getElementById("zona-promociones").innerHTML = "<p class='text-red-500'>Error al cargar promociones.</p>";
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

        // Validar tipo de promoción
        if (tipoSelect && tipoSelect.value === "") {
            isValid = false;
            if (errorSpan) {
                errorSpan.textContent = "Debe seleccionar un tipo de promoción.";
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
        if (!fechaInicioStr || isNaN(fechaInicio.getTime())) {
            isValid = false;
            errorInicio.textContent = "Seleccione un valor para la fecha de inicio";
            errorInicio.classList.remove("hidden");
            fechaInicioInput.classList.add("border-red-500");
        } else {
            errorInicio.textContent = "";
            errorInicio.classList.add("hidden");
            fechaInicioInput.classList.remove("border-red-500");
        }

        // Validar fecha de fin
        if (!fechaFinStr || isNaN(fechaFin.getTime())) {
            isValid = false;
            errorFin.textContent = "Seleccione un valor para la fecha de fin";
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
                    errorProductos.textContent = "Debe seleccionar al menos un producto.";
                }
            } else {
                if (errorProductos) {
                    errorProductos.classList.add("hidden");
                    errorProductos.textContent = "";
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


// Mostrar el select correcto según el tipo en la vistas _CreatePromocion y _EditPromocion
//function escucharCambioSelectTipoPromocion() {
//    const select = document.getElementById("tipoPromocion");
//    if (!select) return; // Evita error si el select no existe

//    const grupoCategoria = document.getElementById("grupoCategoria");
//    const grupoProducto = document.getElementById("grupoProducto");

//    if (!grupoCategoria || !grupoProducto) return;

//    select.addEventListener("change", function () {
//        const tipo = this.value;
//        grupoCategoria.classList.add("hidden");
//        grupoProducto.classList.add("hidden");

//        if (tipo === "Categoria") {
//            grupoCategoria.classList.remove("hidden");
//        } else if (tipo === "Producto") {
//            grupoProducto.classList.remove("hidden");
//        }
//    });

//    // Mostrar el grupo correcto si ya viene con valor (modo edición)
//    if (select.value === "Categoria") {
//        grupoCategoria.classList.remove("hidden");
//    } else if (select.value === "Producto") {
//        grupoProducto.classList.remove("hidden");
//    }
//}

function escucharCambioSelectTipoPromocion() {
    const tipoSelect = document.getElementById('tipoPromocion');
    const grupoCategoria = document.getElementById('grupoCategoria');
    const grupoProducto = document.getElementById('grupoProducto');
    const selectCategoria = document.querySelector('select[name="IdCategoriaSeleccionada"]');
    const checkboxesProducto = document.querySelectorAll('.producto-checkbox');
    const selectAllCheckbox = document.getElementById('checkboxSelectAll');
    const dropdownLabel = document.getElementById('dropdownLabel');

    if (!tipoSelect) return;

    tipoSelect.addEventListener('change', function () {
        const tipo = tipoSelect.value;

        if (tipo === "Categoria") {
            grupoCategoria.classList.remove('hidden');
            grupoProducto.classList.add('hidden');

            // Limpiar selección de productos si se oculta
            checkboxesProducto.forEach(cb => cb.checked = false);
            if (selectAllCheckbox) selectAllCheckbox.checked = false;
            if (dropdownLabel) dropdownLabel.textContent = 'Seleccione uno o más productos';

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
            if (dropdownLabel) dropdownLabel.textContent = 'Seleccione uno o más productos';
        }
    });
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
        const seleccionados = Array.from(checks).filter(c => c.checked);
        if (seleccionados.length === 0) {
            label.textContent = 'Seleccione uno o más productos';
        } else if (seleccionados.length === 1) {
            label.textContent = seleccionados[0].nextElementSibling.textContent;
        } else {
            label.textContent = `${seleccionados.length} productos seleccionados`;
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
        const response = await fetch("/Promocion/Create", {
            method: "POST",
            body: formData
        });

        if (response.ok) {
            const data = await response.json();

            if (data.success) {
                mostrarToast(data.mensaje, "success");
                cargarVista('/Promocion/IndexAdmin');
                history.replaceState(null, "", "/Promocion/IndexAdmin");
            } else {
                console.warn("Error inesperado:", data);
            }

        } else {
            const html = await response.text();
            document.getElementById("contenido-dinamico").innerHTML = html;
        }

    } catch (err) {
        console.error("Error al enviar el formulario", err);
    }
}
















