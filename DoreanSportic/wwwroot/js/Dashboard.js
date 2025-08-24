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
    },
    "UsuarioYaExiste": {
        es: "El usuario ya existe",
        en: "Username already exists"
    },
    "CorreoElectronicoYaRegistrado": {
        es: "El correo electrónico ya está registrado",
        en: "Email address is already registered"
    },
};

// Para internacionalización de la información de los gráficos del dashboard
Object.assign(window.translations, {
    Pedidos: { es: "Pedidos", en: "Orders" },
    CantidadVendida: { es: "Cantidad vendida", en: "Units sold" },
    Cargando: { es: "Cargando...", en: "Loading..." },
    SinResenas: { es: "Sin reseñas", en: "No reviews" },
    Usuario: { es: "Usuario", en: "User" },
    Calificacion: { es: "Calificación", en: "Rating" }
});



// Función que sirve como handler para pintar los errores que vienen del servidor/modelo
// para los formularios de login, cambio de contraseña y registro
function mostrarErrorEnCampo(nombreCampoModelo, mensaje, elementoInput) {
    // Buscar el span que tiene el atributo data-valmsg-for con el nombre del campo del modelo
    const spanError = document.querySelector(`span[data-valmsg-for='${nombreCampoModelo}']`);

    // Si se encuentra el span, poner el mensaje de error dentro
    if (spanError) spanError.textContent = mensaje || "";

    // Añadir o quitar la clase de error al input correspondiente
    if (elementoInput) elementoInput.classList.toggle("border-red-500", !!mensaje);
}

// Función utilitaria para obtener todos los mensajes internacionalizados (data-)
// del contenedor utilitario para mostrarlos en pantalla
function getMensaje(key, ...params) {
    // Llamar al contenedor que tiene los mensajes internacionalizados
    const div = document.getElementById("mensajes-internacionalizados");
    let mensaje = div?.dataset[key] || "";
    // Si el mensaje tiene parámetros tipo {0}, {1}, etc.
    params.forEach((param, i) => {
        mensaje = mensaje.replace(`{${i}}`, param);
    });
    return mensaje;
}

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

                // Inicializar el form de creación de usuario
                if (ruta == "/Usuario/Create") {
                    inicializarCrearUsuario();
                }

                // Al cargar la vista parcial, resetear guard para permitir una nueva inicialización “limpia”
                window.__dashboardInitBound = false;

                // Inicializar gráficos del dashboard si el parcial los trae
                if (document.getElementById('chart-ventas-dia')) {
                    if (typeof inicializarDashboardGraficos === 'function') {
                        inicializarDashboardGraficos();
                    }
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

//Delegación de submit para EditUsuario (funciona aunque el form se inyecte luego) 
(function initEditUsuarioDelegation() {
    // Asegurar que el script no se ejecute más de una vez
    if (window.__editUsuarioDelegationBound) return;

    // Se asigna true a la variable para evitar múltiples asignaciones
    window.__editUsuarioDelegationBound = true;

    // Usar captura (tercer parámetro true) para asegurar que llegue el evento
    document.addEventListener('submit', async function onEditUsuarioSubmit(ev) {

        // Evitar que el evento se propague a otros formularios
        const form = ev.target;

        // Si no existe formulario o no es el correcto, salir de la función
        if (!form || form.id !== 'formEditUsuario') return;

        // Evitar el comportamiento por defecto del formulario
        ev.preventDefault();

        // Asegurar unobtrusive validation para este form (por si se inyectó después)
        if ($ && $.validator && $.validator.unobtrusive) {
            $.validator.unobtrusive.parse(form);
            if (!$(form).valid()) return;
        }

        // Crear un objeto FormData con los datos del formulario
        const formData = new FormData(form);

        // Enviar solicitud POST al servidor
        try {
            const resp = await fetch(form.action || '/Usuario/Edit', {
                method: form.method || 'POST',
                body: formData
            });
            const data = await resp.json();

            // Si la respuesta es exitosa, data.success será true
            if (data.success) {

                // Si la respuesta es exitosa, data.success será true
                if (data.success) {

                    // Si el registro es exitoso, mostrar Toast y redirigir a la página de Login o a la URL indicada
                    mostrarToast(getMensaje("editarok"), "success");
                    // Resetear el formulario si hay respuesta exitosa
                    form.reset();
                    // Esperar .60 segundos para redirigir a "/Usuario/IndexAdmin"
                    // y para que el toast sea visible en pantalla
                    if (typeof cargarVista === "function") {
                        setTimeout(() => cargarVista("/Usuario/IndexAdmin"), 600);
                    }

                    // Limpiar el formulario
                    form.reset();
                    return;
                }
            }

            // Manejo de errores por campo
            const errs = data.errors || data.errores;
            if (errs && typeof pintarErroresFormulario === 'function') {
                pintarErroresFormulario(form, errs);

                // Toast corto con el primer error (opcional)
                const firstKey = Object.keys(errs)[0];
                const firstMsg = Array.isArray(errs[firstKey]) ? errs[firstKey][0] : errs[firstKey];
                if (firstMsg && typeof mostrarToast === 'function') {
                    const text = typeof getTranslation === 'function' ? getTranslation(firstMsg) : firstMsg;
                    mostrarToast(text, 'error');
                }
                return;
            }

            // Fallback
            const msgs = document.getElementById('mensajes-internacionalizados');
            typeof mostrarToast === 'function' &&
                mostrarToast(msgs?.dataset?.errorInesperado || (typeof getMensaje === 'function' && getMensaje('msjErrorInesperado')) || 'Error inesperado', 'error');

        } catch (error) {
            console.error('Error al editar usuario:', error);
            const msgs = document.getElementById('mensajes-internacionalizados');
            typeof mostrarToast === 'function' &&
                mostrarToast(msgs?.dataset?.errorInesperado || (typeof getMensaje === 'function' && getMensaje('msjErrorInesperado')) || 'Error inesperado', 'error');
        }
    }, true);
})();

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

// Función para cargar la vista de detalles del usuario
function cargarDetalleUsuario(idUsuario) {

    const loader = document.getElementById('loader');
    const container = document.getElementById('contenido-dinamico');

    loader.classList.remove('hidden');
    container.innerHTML = "";

    fetch(`/Usuario/DetailsAdmin/${idUsuario}`)
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

// Función javascript para cargar la vista de editar usuario (_EditUsuario)
// Función para cargar la vista parcial de edición de un usuario
function cargarEditarUsuario(idUsuario) {
    const loader = document.getElementById('loader');
    const container = document.getElementById('contenido-dinamico');


    loader.classList.remove('hidden');
    container.innerHTML = "";

    fetch(`/Usuario/Edit/${idUsuario}`)
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

// Función para inicializar el form de registro/crear usuario
function inicializarCrearUsuario() {

    // Asignar a form el formulario de registro
    const form = document.getElementById("formRegistro");

    // Si no existe el formulario, no hacer nada
    if (!form) return;

    // unobtrusive
    $.validator.unobtrusive.parse(form);

    // Validaciones “en vivo” para los campos (reutiliza helpers de Login.js)
    if (typeof configurarValidacionUsuario === "function") {
        configurarValidacionUsuario("UserName", "UserName", "msj-usuario");
    }

    // Validaciones “en vivo” para el campo de Contraseña
    if (typeof configurarValidacionPassword === "function") {
        configurarValidacionPassword("Password", "Password", "msj-password");
    }

    // Validaciones “en vivo” para el campo de Confirmar Contraseña
    if (typeof configurarValidacionConfirmacionPassword === "function") {
        configurarValidacionConfirmacionPassword("ConfirmPassword", "Password", "ConfirmPassword", "msj-confirm");
    }

    // Validaciones “en vivo” para el campo de Email
    if (typeof configurarValidacionEmail === "function") {
        configurarValidacionEmail("Email", "Email", "msj-email");
    }

    // Validaciones “en vivo” para los campos de Nombre y Apellido
    if (typeof configurarValidacionNombreApellido === "function") {
        configurarValidacionNombreApellido("Nombre", "Nombre", "msj-nombre");
        configurarValidacionNombreApellido("Apellido", "Apellido", "msj-apellido");
    }

    // Validaciones “en vivo” para el campo de Teléfono
    if (typeof configurarValidacionTelefono === "function") {
        configurarValidacionTelefono("Telefono", "Telefono", "msj-telefono");
    }

    // Hacer "submit" mediante AJAX al endpoint Login/Registrar
    form.addEventListener("submit", async (ev) => {

        // Evitar el comportamiento por defecto del formulario
        ev.preventDefault();

        // Validar el formulario con jQuery Validate y no continuar si hay errores
        if (!$(form).valid()) return;

        // Crear un objeto FormData con los datos del formulario
        const formData = new FormData(form);

        // Enviar solicitud POST al servidor
        try {
            const resp = await fetch(form.action || "/Login/Registrar", {
                method: form.method || "POST",
                body: formData
            });

            const data = await resp.json();

            // Si la respuesta es exitosa, data.success será true
            if (data.success) {

                // Si el registro es exitoso, mostrar Toast y redirigir a la página de Login o a la URL indicada
                mostrarToast(getMensaje("crearok"), "success");
                // Resetear el formulario si hay respuesta exitosa
                form.reset();
                // Esperar .60 segundos para redirigir a "/Usuario/IndexAdmin"
                // y para que el toast sea visible en pantalla
                if (typeof cargarVista === "function") {
                    setTimeout(() => cargarVista("/Usuario/IndexAdmin"), 600);
                }

                // Limpiar el formulario
                form.reset();
                return;
            }

            // Si viene un mensaje simple del backend, mostrar el error en el formulario
            if (data.errors) {
                pintarErroresFormulario(form, data.errors || data.errores);
                return;
            }

            // Si algún caso devuelve diccionario de errores por campo
            if (data.errors || data.errores) {
                pintarErroresFormulario(form, data.errors || data.errores);
                return;
            }

            // fallback
            mostrarToast(getMensaje?.("msjErrorInesperado") || "Error inesperado", "error");

        } catch (error) {
            console.error('Error:', error);
            mostrarToast(getMensaje?.("msjErrorInesperado") || "Error inesperado", "error");
        }
    });
}

// Función para convertir una cadena a PascalCase (UpperCamelCase)
function toPascalCase(k) { return k ? k.charAt(0).toUpperCase() + k.slice(1) : k; }

// Función para pintar los errores del formulario de forma dinámica
function pintarErroresFormulario(form, errors) {
    // Limpiar errores previos en el formulario
    form.querySelectorAll("[data-valmsg-for]").forEach(span => span.textContent = "");

    // Convertir los nombres de los campos a PascalCase si es necesario
    for (const [key, msj] of Object.entries(errors)) {

        // Si el campo no existe en el formulario, convertir a PascalCase
        const campo = form.querySelector(`[name='${key}']`)
            ? key
            : toPascalCase(key);

        // Obtener el input correspondiente al campo
        const input = form.querySelector(`[name='${campo}']`);

        // Si el mensaje es un array, tomar el primer mensaje
        let mensaje = Array.isArray(msj) ? msj[0] : msj;

        // Si el backend mandó una *clave*, se traduce con getTranslation
        if (typeof mensaje === "string") {
            mensaje = getTranslation(mensaje);
        }

        // Buscar el span con ambas opciones (nombreCampo o NombreCampo) por si acaso
        const span = form.querySelector(
            `span[data-valmsg-for='${campo}'], span[data-valmsg-for='${toPascalCase(key)}']`
        );

        // Mostrar el mensaje de error en el span correspondiente
        if (span) span.textContent = mensaje || "";
        // Añadir o quitar la clase de error al input correspondiente (opcional)
        if (input) input.classList.toggle("border-red-500", !!mensaje);
    }
}

// Función para obtener los mensajes de error a través de datasets (data-)
// de un elemento HTML, dado su id
function obtenerMensajesDesdeElemento(idElemento) {
    return document.getElementById(idElemento)?.dataset || {};
}

// Regex para los inputs según el tipo de dato
const patronUsuario = /^[A-Za-z0-9._-]+$/;
const patronNombreApellido = /^[A-Za-zÀ-ÿ' ]+$/;
const patronPasswordFuerte = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$/;
const patronEmail = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.((com|org|net|es|edu|gov|mil|info|co|io)|[a-zA-Z]{2,6})$/;
const patronTelefono = /^(\+?\d{7,15}|(\+?\d{1,4}[-.\s]?)?(\d{2,4}[-.\s]?){2,4}\d{2,4})$/;

// Función para escuchar el input de usuario y validar formato
function configurarValidacionUsuario(idInput, nombreCampoModelo, idMensajes) {
    // Obtener el input
    const input = document.getElementById(idInput);

    // Si no existe el input, salir de la función
    if (!input) return;

    // Obtener los mensajes desde el elemento HTML
    const mensajes = obtenerMensajesDesdeElemento(idMensajes);

    // Escuchar el evento keydown para evitar espacios
    input.addEventListener("keydown", e => { if (e.key === " ") e.preventDefault(); });

    // Escuchar el evento input para validar el formato
    input.addEventListener("input", () => {

        // Obtener el valor del input y quitar espacios al inicio y final
        const valor = input.value.trim();

        // Si el valor está vacío, mostrar mensaje de campo obligatorio
        if (valor === "") { mostrarErrorEnCampo(nombreCampoModelo, "", input); return; }

        // Validar longitud del usuario 
        const min = parseInt(mensajes.min || "8"), max = parseInt(mensajes.max || "30");

        // Validar longitud y formato del usuario
        if (valor.length < min || valor.length > max) {
            const mensaje = (mensajes.msjLen || "").replace("{0}", min).replace("{1}", max);
            mostrarErrorEnCampo(nombreCampoModelo, mensaje, input); return;
        }

        // Validar formato del usuario
        if (!patronUsuario.test(valor)) {
            mostrarErrorEnCampo(nombreCampoModelo, mensajes.msjForm || "", input); return;
        }

        // Si todo es correcto, quitar mensaje de error
        mostrarErrorEnCampo(nombreCampoModelo, "", input);
    });
}

// Función para escuchar el input de password y validar formato
function configurarValidacionPassword(idInput, nombreCampoModelo, idMensajes) {

    // Obtener el input
    const input = document.getElementById(idInput);

    // Si no existe el input, salir de la función
    if (!input) return;

    // Obtener los mensajes desde el elemento HTML
    const mensajes = obtenerMensajesDesdeElemento(idMensajes);

    // Escuchar el evento input para validar el formato
    input.addEventListener("input", () => {

        // Obtener el valor del input
        const valor = input.value;

        // Chequear la longitud mínima y máxima de la contraseña
        const min = parseInt(mensajes.min || "8"), max = parseInt(mensajes.max || "64");

        // Si la longitud no es válida, mostrar mensaje de error
        if (valor.length < min || valor.length > max) {

            // Mostrar mensaje de error con los valores mínimos y máximos
            const mensaje = (mensajes.msjLen || "").replace("{0}", min).replace("{1}", max);

            // Mostrar el error en el campo correspondiente
            mostrarErrorEnCampo(nombreCampoModelo, mensaje, input); return;
        }

        // Validar que la contraseña sea fuerte (mayúscula, minúscula, número, símbolo)
        if (!patronPasswordFuerte.test(valor)) {
            mostrarErrorEnCampo(nombreCampoModelo, mensajes.msjReq || "", input); return;
        }

        // Si todo es correcto, quitar mensaje de error
        mostrarErrorEnCampo(nombreCampoModelo, "", input);
    });
}

// Función para escuchar el input de confirmación de password y validar que coincida
function configurarValidacionConfirmacionPassword(idInputConfirmar, idInputPassword, nombreCampoModelo, idMensajes) {

    // Obtener los inputs
    const inputConfirmar = document.getElementById(idInputConfirmar);
    const inputPassword = document.getElementById(idInputPassword);

    // Si no existen los inputs, salir de la función
    if (!inputConfirmar || !inputPassword) return;

    // Obtener los mensajes desde el elemento HTML
    const mensajes = obtenerMensajesDesdeElemento(idMensajes);

    // Función para validar que las contraseñas coincidan
    const validar = () => {
        if (inputConfirmar.value !== inputPassword.value) {
            mostrarErrorEnCampo(nombreCampoModelo, mensajes.msj || "", inputConfirmar);
        } else {
            mostrarErrorEnCampo(nombreCampoModelo, "", inputConfirmar);
        }
    };

    // Escuchar el evento input del input de confirmación de contraseña 
    inputConfirmar.addEventListener("input", validar);

    // También validar cuando cambie la contraseña original
    inputPassword.addEventListener("input", validar);
}

// Función para escuchar el input de email y validar formato
function configurarValidacionEmail(idInput, nombreCampoModelo, idMensajes) {

    // Obtener el input
    const input = document.getElementById(idInput);

    // Si no existe el input, salir de la función
    if (!input) return;

    // Obtener los mensajes desde el elemento HTML
    const mensajes = obtenerMensajesDesdeElemento(idMensajes);

    // Escuchar el evento keydown para evitar espacios
    input.addEventListener("keydown", e => { if (e.key === " ") e.preventDefault(); });

    // Escuchar el evento input para validar el formato
    input.addEventListener("input", () => {

        // Obtener el valor del input y quitar espacios al inicio y final
        const valor = input.value.trim();

        // Si el valor está vacío, mostrar mensaje de campo obligatorio
        if (valor === "") { mostrarErrorEnCampo(nombreCampoModelo, "", input); return; }

        // Validar formato del email
        if (!patronEmail.test(valor)) {
            mostrarErrorEnCampo(nombreCampoModelo, mensajes.msj || "", input);
        } else {

            // Si todo es correcto, quitar mensaje de error
            mostrarErrorEnCampo(nombreCampoModelo, "", input);
        }
    });
}

// Función para escuchar el input de nombre y apellido y validar formato
function configurarValidacionNombreApellido(idInput, nombreCampoModelo, idMensajes) {
    // Obtener el input
    const input = document.getElementById(idInput);

    // Si no existe el input, salir de la función
    if (!input) return;

    // Obtener los mensajes desde el elemento HTML
    const mensajes = obtenerMensajesDesdeElemento(idMensajes);

    // Escuchar el evento input para validar el formato
    input.addEventListener("input", () => {

        // Obtener el valor del input
        const valor = input.value;

        // Revisar el minimo y el máximo de caracteres
        const min = parseInt(mensajes.min || "2"), max = parseInt(mensajes.max || "30");

        // Si el valor es menor que el mínimo o mayor que el máximo, mostrar mensaje de error
        if (valor.length < min || valor.length > max) {

            // Mostrar mensaje de error con los valores mínimos y máximos
            const mensaje = (mensajes.msjLen || "").replace("{0}", min).replace("{1}", max);
            mostrarErrorEnCampo(nombreCampoModelo, mensaje, input); return;
        }

        // Validar formato del nombre o apellido
        if (!patronNombreApellido.test(valor)) {

            // Si el formato no es válido, mostrar mensaje de error
            mostrarErrorEnCampo(nombreCampoModelo, mensajes.msj || "", input); return;
        }

        // Si todo es correcto, quitar mensaje de error
        mostrarErrorEnCampo(nombreCampoModelo, "", input);
    });
}

// Función para escuchar el input de teléfono y validar formato
function configurarValidacionTelefono(idInput, nombreCampoModelo, idMensajes) {

    // Obtener el input
    const input = document.getElementById(idInput);

    // Si no existe el input, salir de la función
    if (!input) return;

    // Obtener los mensajes desde el elemento HTML
    const mensajes = obtenerMensajesDesdeElemento(idMensajes);

    // Escuchar el evento keydown para evitar espacios
    input.addEventListener("input", () => {
        // Obtener el valor del input y quitar espacios al inicio y final
        const valor = input.value.trim();

        // Si el valor está vacío, mostrar mensaje de campo obligatorio
        if (valor === "") { mostrarErrorEnCampo(nombreCampoModelo, "", input); return; }

        // Validar formato del teléfono
        if (!patronTelefono.test(valor)) {
            mostrarErrorEnCampo(nombreCampoModelo, mensajes.msj || "", input);
        } else {
            mostrarErrorEnCampo(nombreCampoModelo, "", input);
        }
    });
}

// Función para poder cargar las páginas de la tablas de las vistas parciales
// sin recargar la página

// Lógica para manejar paginación de reseñas, promociones y usuarios sin recargar toda la página
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

    // Paginación de usuarios
    if (e.target.matches('a.pagina-usuario')) {
        e.preventDefault();
        const url = e.target.getAttribute('href');

        fetch(url)
            .then(res => {
                if (!res.ok) throw new Error("ERROR");
                return res.text();
            })
            .then(html => {
                document.getElementById("zona-usuarios").innerHTML = html;
            })
            .catch(err => {
                console.error("Error al cargar usuarios:", err);
                document.getElementById("zona-usuarios").innerHTML = "<p class='text-red-500'>ERROR</p>";
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

// Helpers i18n/format para los gráficos del dashboard

const t = (k) => (typeof getTranslation === 'function' ? getTranslation(k) : k);
const culture = (typeof getCurrentCulture === 'function' ? getCurrentCulture() : 'en');
const nf = new Intl.NumberFormat(culture);
const df = new Intl.DateTimeFormat(culture, { dateStyle: 'medium' });

function formatDateLocal(v) {
    try { return df.format(new Date(v)); } catch { return v; }
}

// Función para cargar el dashboard de estadísticas junto con sus helpers y carga de gráficos/kpis
(function () {

    // Variables para los gráficos (Chart.js)
    let chartVentasDia, chartVentasMes, chartPedidosEstado, chartTopProductos;

    // Función para crear o actualizar un gráfico
    function ensureChart(ctx, type, data, options) {

        // Si ya existe un gráfico en este contexto, destruirlo antes de crear uno nuevo
        if (ctx._chart) {
            ctx._chart.destroy();
        }

        // Crear un nuevo gráfico y almacenarlo en el contexto para futuras referencias
        ctx._chart = new Chart(ctx, { type, data, options });
        return ctx._chart;
    }

    // Función para obtener JSON desde una URL con manejo básico de errores
    async function fetchJson(url) {

        // Usar fetch con encabezado para indicar que es una solicitud AJAX
        const r = await fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } });

        // Si la respuesta no es OK, lanzar un error
        if (!r.ok) throw new Error("HTTP " + r.status);

        // Devolver el JSON
        return await r.json();
    }

    // Ventas por día
    async function loadVentasDia() {

        // Obtener los valores de los inputs de fecha
        const desde = document.getElementById('ventas-dia-desde')?.value;
        const hasta = document.getElementById('ventas-dia-hasta')?.value;

        // Construir la query string si hay fechas
        const qs = new URLSearchParams();
        if (desde) qs.append('from', desde);
        if (hasta) qs.append('to', hasta);

        // Llamar al endpoint con las fechas (si existen)
        const url = '/Reporte/VentasPorDia' + (qs.toString() ? '?' + qs.toString() : '');
        const json = await fetchJson(url);

        // Obtener el canvas y su contexto
        const canvas = document.getElementById('chart-ventas-dia');

        // Si no existe el canvas, salir
        if (!canvas) return;

        // Obtener el contexto 2D
        const ctx = canvas.getContext('2d');

        // Crear o actualizar el gráfico
        chartVentasDia = ensureChart(ctx, 'bar', {
            labels: json.labels,
            datasets: [{ label: t('Pedidos'), data: json.data }]
        }, { responsive: true, plugins: { legend: { display: true } } });
    }

    // Ventas por mes (por año)
    async function loadVentasMes() {

        // Obtener el valor del input de año
        const anio = document.getElementById('ventas-mes-anio')?.value;

        // Construir la URL con el año si existe
        const url = '/Reporte/VentasPorMes' + (anio ? ('?year=' + anio) : '');
        const json = await fetchJson(url);

        // Obtener el canvas y su contexto
        const canvas = document.getElementById('chart-ventas-mes');

        // Si no existe el canvas, salir
        if (!canvas) return;

        // Obtener el contexto 2D
        const ctx = canvas.getContext('2d');

        // Crear o actualizar el gráfico
        chartVentasMes = ensureChart(ctx, 'line', {
            labels: json.labels,
            datasets: [{ label: t('Pedidos'), data: json.data, tension: .3, fill: false }]

        }, { responsive: true, plugins: { legend: { display: true } } });
    }

    // Pedidos por estado
    async function loadPedidosEstado() {

        // Llamar al endpoint
        const json = await fetchJson('/Reporte/PedidosPorEstado');

        // Obtener el canvas y su contexto
        const canvas = document.getElementById('chart-pedidos-estado');

        // Si no existe el canvas, salir
        if (!canvas) return;

        // Obtener el contexto 2D
        const ctx = canvas.getContext('2d');

        // Crear o actualizar el gráfico
        chartPedidosEstado = ensureChart(ctx, 'doughnut', {
            labels: json.labels,
            datasets: [{ label: t('Pedidos'), data: json.data }]

        }, { responsive: true });
    }

    // Top 3 productos
    async function loadTopProductos() {

        // Obtener el valor del input de N (3)
        const n = document.getElementById('top-n')?.value || 3;

        // Llamar al endpoint con N(3)
        const json = await fetchJson('/Reporte/TopProductos?n=' + encodeURIComponent(n));

        // Obtener el canvas y su contexto
        const canvas = document.getElementById('chart-top-productos');

        // Si no existe el canvas, salir
        if (!canvas) return;

        // Obtener el contexto 2D
        const ctx = canvas.getContext('2d');

        // Crear o actualizar el gráfico
        chartTopProductos = ensureChart(ctx, 'bar', {
            labels: json.labels,
            datasets: [{ label: t('CantidadVendida'), data: json.data }]

        }, { responsive: true });
    }

    // Reseñas recientes
    async function loadResennas() {

        // Obtener el valor del input de N (3)
        const n = document.getElementById('res-n')?.value || 3;

        // Obtener el elemento UL donde se mostrarán las reseñas
        const list = document.getElementById('lista-resennas');

        // Si no existe el UL, salir
        if (!list) return;

        // Mostrar mensaje de cargando
        list.innerHTML = `<li class="text-sm opacity-60">${t('Cargando')}</li>`;
        const json = await fetchJson('/Reporte/ResennasRecientes?n=' + encodeURIComponent(n));

        // Si no hay reseñas, mostrar mensaje
        if (!Array.isArray(json) || json.length === 0) {
            list.innerHTML = `<li class="text-sm opacity-60">${t('SinResenas')}</li>`;
            return;
        }

        // Mapear las reseñas a elementos LI
        list.innerHTML = json.map(r => `
            <li class="p-3 rounded border flex flex-col gap-1 mb-3">
                <div class="flex justify-between">
                    <b>${r.producto}</b>
                    <span class="text-xs opacity-70">${formatDateLocal(r.fecha)}</span>
                </div>
                <div class="text-sm">
                    <b>${t('Usuario')}:</b> ${r.usuario} &nbsp;&nbsp; <b>${t('Calificacion')}:</b> ${r.calificacion}/5
                </div>
                <div class="text-sm italic">"${r.comentario ?? ''}"</div>
            </li>
        `).join('');
    }

    // Inicializador público que se llama tras cargar el parcial del dashboard
    window.inicializarDashboardGraficos = function () {

        // Evitar múltiples inicializaciones sobre el mismo DOM
        if (window.__dashboardInitBound) return;
        window.__dashboardInitBound = true;

        // Defaults de filtros (si existen los inputs)
        try {
            const d = new Date();
            const hoy = d.toISOString().slice(0, 10);
            const hace14 = new Date(d.getTime() - 14 * 24 * 60 * 60 * 1000).toISOString().slice(0, 10);
            if (document.getElementById('ventas-dia-desde')) document.getElementById('ventas-dia-desde').value = hace14;
            if (document.getElementById('ventas-dia-hasta')) document.getElementById('ventas-dia-hasta').value = hoy;
            if (document.getElementById('ventas-mes-anio')) document.getElementById('ventas-mes-anio').value = d.getFullYear();
        } catch { /* no-op */ }

        // Blindaje de los inputs de fechas y spinner de año
        const inDesde = document.getElementById('ventas-dia-desde');
        const inHasta = document.getElementById('ventas-dia-hasta');
        const inYear = document.getElementById('ventas-mes-anio');

        // Bloquear escritura (pero permitir navegación con Tab/Arrows)
        function blockTyping(inp, { blockBeforeInput = true } = {}) {
            if (!inp) return;
            // **Solo bloquear beforeinput si se indica**
            if (blockBeforeInput) {
                inp.addEventListener('beforeinput', e => e.preventDefault());
            }
            inp.addEventListener('keydown', e => {
                const nav = ['Tab', 'ArrowLeft', 'ArrowRight', 'Home', 'End'];
                if (!nav.includes(e.key)) e.preventDefault();
            });
            inp.addEventListener('paste', e => e.preventDefault());
            inp.addEventListener('drop', e => e.preventDefault());
        }

        // Bloquear escritura en los inputs de fecha y año
        blockTyping(inDesde);
        blockTyping(inHasta);

        // Para el año: NO bloquear beforeinput para que funcione el spinner
        blockTyping(inYear, { blockBeforeInput: false });

        // Mantener el date-picker visible
        [inDesde, inHasta].forEach(inp => {
            if (!inp) return;
            // Evita que se abra el teclado en móviles
            inp.setAttribute('inputmode', 'none');
            // Abre el picker al enfocar o hacer click (si el navegador soporta showPicker)
            if ('showPicker' in HTMLInputElement.prototype) {
                inp.addEventListener('focus', () => inp.showPicker());
                inp.addEventListener('click', () => inp.showPicker());
            }
        });

        // Evitar cambiar el año con la rueda del mouse (pero conservar flechas)
        if (inYear) {
            inYear.setAttribute('inputmode', 'numeric');
            inYear.setAttribute('pattern', '[0-9]*');
            inYear.addEventListener('wheel', e => e.preventDefault(), { passive: false });
        }

        // Reglas de rango para fechas (hasta ≥ desde)
        if (inDesde && inHasta) {
            if (inDesde.value) inHasta.min = inDesde.value;
            if (inHasta.value) inDesde.max = inHasta.value;

            inDesde.addEventListener('change', () => {
                inHasta.min = inDesde.value || '';
                if (inHasta.value && inHasta.value < inDesde.value) {
                    inHasta.value = inDesde.value;
                }
            });

            inHasta.addEventListener('change', () => {
                inDesde.max = inHasta.value || '';
                if (inDesde.value && inDesde.value > inHasta.value) {
                    inDesde.value = inHasta.value;
                }
            });
        }

        // Rango para el año [2000, año actual] manteniendo el spinner
        if (inYear) {
            const curr = new Date().getFullYear();
            const minY = 2000;
            inYear.min = String(minY);
            inYear.max = String(curr);

            const clampYear = () => {
                let val = parseInt(inYear.value, 10);
                if (isNaN(val)) { inYear.value = String(curr); return; }
                if (val < minY) val = minY;
                if (val > curr) val = curr;
                inYear.value = String(val);
            };
            inYear.addEventListener('input', clampYear);
            clampYear();
        }


        // Eventos de filtros (botones)

        // Obtener el botón de filtrar por día y cargar ventas por día mediante el evento click
        const btnDia = document.getElementById('btn-filtrar-dia');
        if (btnDia) btnDia.addEventListener('click', () => loadVentasDia().catch(console.error));

        // Obtener el botón de filtrar por mes y cargar ventas por mes mediante el evento click
        const btnMes = document.getElementById('btn-filtrar-mes');
        if (btnMes) btnMes.addEventListener('click', () => loadVentasMes().catch(console.error));

        // Obtener el select de top 3 productos y cargar top productos mediante el evento change
        const selTopN = document.getElementById('top-n');
        if (selTopN) selTopN.addEventListener('change', () => loadTopProductos().catch(console.error));

        // Obtener el select de 3 reseñas y cargar reseñas mediante el evento change
        const selResN = document.getElementById('res-n');
        if (selResN) selResN.addEventListener('change', () => loadResennas().catch(console.error));

        // Cargas iniciales (sólo si los elementos existen)
        if (document.getElementById('chart-ventas-dia')) loadVentasDia().catch(console.error);
        if (document.getElementById('chart-ventas-mes')) loadVentasMes().catch(console.error);
        if (document.getElementById('chart-pedidos-estado')) loadPedidosEstado().catch(console.error);
        if (document.getElementById('chart-top-productos')) loadTopProductos().catch(console.error);
        if (document.getElementById('lista-resennas')) loadResennas().catch(console.error);
    };
})();


// Cuando el DOM esté listo, cargar los eventos de validación de los campos del formulario de registro
document.addEventListener("DOMContentLoaded", () => {

    configurarValidacionUsuario("UserName", "UserName", "msj-usuario");
    configurarValidacionPassword("Password", "Password", "msj-password");
    configurarValidacionConfirmacionPassword("ConfirmPassword", "Password", "ConfirmPassword", "msj-confirm");
    configurarValidacionEmail("Email", "Email", "msj-email");
    configurarValidacionNombreApellido("Nombre", "Nombre", "msj-nombre");
    configurarValidacionNombreApellido("Apellido", "Apellido", "msj-apellido");
    configurarValidacionTelefono("Telefono", "Telefono", "msj-telefono");
});






















