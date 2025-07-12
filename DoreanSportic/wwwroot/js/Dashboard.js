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

                // Cargar la función JavaScript si se carga la vista de productos
                if (typeof inicializarVistaProductos === 'function') {
                    inicializarVistaProductos();
                }

                // Cargar la función JavaScript si se hace drag and drop (etiquetas) en crear producto 
                if (document.getElementById('dp1') && typeof inicializarDragAndDropEtiquetas === 'function') {
                    inicializarDragAndDropEtiquetas();
                }

                // Inicializar función que escucha el input de precioBase
                escucharInputPrecioBase();

                // Inicializar función que escucha el input de cantidad
                escucharInputCantidad();

                // Inicializar la función que carga páginas en la tabla
                cargarPaginasTabla()

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












