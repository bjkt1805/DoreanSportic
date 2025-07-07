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

                if (typeof inicializarVistaProductos === 'function') {
                    inicializarVistaProductos();
                }



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
    const container = document.getElementById('card-productos-body');

    if (!toggle || !loader || !container) return;

    toggle.addEventListener('change', () => {
        const idCategoria = toggle.checked ? 2 : 1;
        cargarProductosPorCategoria(idCategoria);
    });

    function cargarProductosPorCategoria(idCategoria) {
        loader.classList.remove('hidden');
        container.innerHTML = "";
        fetch(`/Producto/FiltrarPorCategoria?idCategoria=${idCategoria}`)
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

// JavaScript para poder cargar imágenes por drag and drop
document.addEventListener("DOMContentLoaded", () => {
    const dropzone = document.getElementById("dropzone");
    const fileInput = document.getElementById("fileInput");
    const preview = document.getElementById("preview");

    // Click en zona de drop abre el selector
    dropzone.addEventListener("click", () => fileInput.click());

    // Detectar archivos seleccionados manualmente
    fileInput.addEventListener("change", handleFiles);

    // Detectar archivos arrastrados
    dropzone.addEventListener("dragover", e => {
        e.preventDefault();
        dropzone.classList.add("border-blue-500", "bg-blue-50");
    });

    dropzone.addEventListener("dragleave", () => {
        dropzone.classList.remove("border-blue-500", "bg-blue-50");
    });

    dropzone.addEventListener("drop", e => {
        e.preventDefault();
        dropzone.classList.remove("border-blue-500", "bg-blue-50");
        const files = e.dataTransfer.files;
        fileInput.files = files; // vincula con el input hidden
        handleFiles({ target: { files } });
    });

    // Función para mostrar la vista previa
    function handleFiles(e) {
        const files = e.target.files;
        preview.innerHTML = ""; // limpiar previews previas
        Array.from(files).forEach(file => {
            if (!file.type.startsWith("image/")) return;

            const reader = new FileReader();
            reader.onload = function (e) {
                const img = document.createElement("img");
                img.src = e.target.result;
                img.classList.add("w-full", "h-32", "object-cover", "rounded");
                preview.appendChild(img);
            };
            reader.readAsDataURL(file);
        });
    }
});



