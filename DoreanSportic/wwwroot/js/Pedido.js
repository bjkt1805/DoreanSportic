// Función para formatear números a dos decimales y agregar el símbolo de colón
function formatColones(value) {
    // Convertir a número válido
    const numero = Number(value ?? 0);

    // Formatear con separador de miles por defecto (usando coma o punto según locale)
    let partes = numero
        .toFixed(2) // Siempre 2 decimales
        .split('.'); // Separar parte entera y decimal usando punto fijo

    // Agregar separador de miles como espacio
    partes[0] = partes[0].replace(/\B(?=(\d{3})+(?!\d))/g, ' ');

    // Reconstruir usando punto como separador decimal
    return `₡${partes[0]}.${partes[1]}`;
}

// Función para asociar eventos a la tabla de detalles del pedido
function bindParcialEventos(root) {
    if (!root) return;

    // Obtener el id del pedido desde el elemento raíz (root) en este caso sería <div id="detalles-root">
    const pedidoId = root.dataset.pedidoId;

    // Obtener el contenido de detalles del pedido (body de la tabla = <tbody id="pedido-detalles-body">)
    const body = root.querySelector('#pedido-detalles-body');

    // Función para actualizar totales en pantalla
    function actualizarTotales(total) {

        // Si no hay totales, no hacer nada
        if (!total) return;

        // Obtener el valor de subtotal
        const subTotal = root.querySelector('#totals-sub');

        // Obtener el valor del impuesto
        const impuesto = root.querySelector('#totals-tax');

        // Obtener el valor del total general
        const granTotal = root.querySelector('#totals-grand');

        // Si existe subTotal, darle formato de dos decimales y agregar el símbolo de colón
        if (subTotal) subTotal.textContent = formatColones(total.sub);

        // Si existe impuesto, darle formato de dos decimales y agregar el símbolo de colón
        if (impuesto) impuesto.textContent = formatColones(total.imp);

        // Si existe grandTotal, darle formato de dos decimales y agregar
        if (granTotal) granTotal.textContent = formatColones(total.total);
    }

    // Configuración de un debounce para evitar múltiples llamadas al servidor en poco tiempo
    let t = null;
    const debounce = (fn, ms) => { clearTimeout(t); t = setTimeout(fn, ms); };

    // Cambios de cantidad (0 => elimina el detalle)
    // Escuchar el evento de input en el body de la tabla
    body?.addEventListener('input', (e) => {

        // Si el elemento que disparó el evento no tiene clase de cantidad, salir de la función
        if (!e.target.classList.contains('qty-input')) return;

        // Obtener la fila de la tabla (tr) m cercana al elemento que disparó el evento
        const filaTabla = e.target.closest('tr');

        // Asignar el dato detalleId desde el atributo data-detalle-id de la fila
        const detalleId = filaTabla.dataset.detalleId;

        // Obtener el valor numérico de la cantidad ingresada, asegurando que sea un número entero
        // Si no hay valor, asignar 0 
        const cantidad = parseInt(e.target.value || '0', 10);

        // Obtener el token de verificación anti-CSRF desde el formulario de encabezado del pedido
        const token = document.querySelector('#pedido-encabezado-form input[name="__RequestVerificationToken"]')?.value;

        // Si la cantidad es NaN o menor que 0, asignar 0 al input y salir de la función
        if (isNaN(cantidad) || cantidad < 0) { e.target.value = 0; return; }

        // Llamar a la API para actualizar la cantidad del detalle
        debounce(async () => {

            // Crear un objeto FormData para enviar los datos al servidor
            const form = new FormData();

            // Añadir los datos necesarios al FormData (detalleId y cantidad))
            form.append('detalleId', detalleId);
            form.append('cantidad', cantidad);

            // Añadir el token de verificación anti-CSRF al FormData
            form.append('__RequestVerificationToken', token);

            // Realizar la petición al servidor para actualizar la cantidad del detalle
            const response = await fetch('/PedidoDetalle/ActualizarCantidad', {
                method: 'POST',
                body: form
            });

            // Obtener la respuesta en formato JSON
            const data = await response.json();

            // Si la respuesta no es exitosa, salir de la función
            if (!data.success) return;

            // Si la respuesta indica que el detalle fue eliminado, eliminar la fila de la tabla
            if (data.eliminado) {
                filaTabla.remove();

                // Si no hay más filas en el body, recargar la vista parcial para mostrar mensaje de "no hay detalles"
                const quedan = body.querySelectorAll('tr[data-detalle-id]').length > 0;
                if (!quedan) {
                    await recargarParcial(pedidoId);
                    return;
                }

                // Si hay detalle, hay que actualizar las filas subtotal y precio unitario 
            } else if (data.detalle) {

                // Actualizar el contenido de las celdas subTotal y Precio Unitario con los nuevos valores
                filaTabla.querySelector('.cell-subtotal').textContent = formatColones(data.detalle.subTotal);
                const punit = data.detalle.cantidad > 0 ? (data.detalle.subTotal / data.detalle.cantidad) : 0;
                filaTabla.querySelector('.cell-punit').textContent = formatColones(punit);
            }

            // Actualizar los totales en pantalla con los nuevos valores
            actualizarTotales(data.totals);
        }, 350);
    });

    // Función para eliminar un detalle del pedido
    body?.addEventListener('click', async (e) => {

        // Si el elemento que disparó el evento no tiene la clase btn-eliminar-detalle, salir de la función
        if (!e.target.classList.contains('btn-eliminar-detalle')) return;

        // Asignar a filatabla la fila de la tabla (tr) más cercana al elemento que disparó el evento
        const filaTabla = e.target.closest('tr');

        // Asignar a detalleId el valor del atributo data-detalle-id de la fila
        const detalleId = filaTabla.dataset.detalleId;

        // Obtener el token de verificación anti-CSRF desde el formulario de encabezado del pedido
        const token = document.querySelector('#pedido-encabezado-form input[name="__RequestVerificationToken"]')?.value;

        // Crear un objeto FormData para enviar los datos al servidor
        const form = new FormData();

        // Añadir los datos necesarios al FormData (detalleId y pedidoId))
        form.append('detalleId', detalleId);
        form.append('pedidoId', pedidoId);

        // Añadir el token de verificación anti-CSRF al FormData
        form.append('__RequestVerificationToken', token);

        // Realizar la petición al servidor para eliminar el detalle del pedido
        const response = await fetch('/PedidoDetalle/EliminarDetalle', {
            method: 'POST',
            body: form
        });

        // Obtener la respuesta en formato JSON
        const data = await response.json();

        // Si la respuesta es exitosa, eliminar la fila de la tabla y actualizar los totales
        if (data.success) {
            filaTabla.remove();
            actualizarTotales(data.totals);

            // Si no hay más filas en el body, recargar la vista parcial para mostrar mensaje de "no hay detalles"
            const quedan = body.querySelectorAll('tr[data-detalle-id]').length > 0;
            if (!quedan) {
                await recargarParcial(pedidoId);
                return;
            }
        }
    });

    // Completar compra (guardar encabezado con dirección)
    root.parentElement // contenedor fuera de la tabla (el encabezado del pedido)
        // buscar el botón de completar compra dentro del contenedor
        ?.querySelector('#btn-completar-compra')

        // Si existe el botón, agregar el evento click
        ?.addEventListener('click', async () => {

            // Asignar a userIdEl el elemento input con id cliente-userid
            const userIdEl = document.getElementById('cliente-userid');

            // Asignar a userId el valor numérico de userIdEl, asegurando que sea un número
            const userId = Number(userIdEl?.value);

            // Obtener el token de verificación anti-CSRF desde el formulario de encabezado del pedido
            const token = document.querySelector('#pedido-encabezado-form input[name="__RequestVerificationToken"]')?.value;

            // Ubicar los selects de provincia, cantón y distrito
            const selectProv = document.getElementById('sel-provincia');
            const selectCant = document.getElementById('sel-canton');
            const selectDist = document.getElementById('sel-distrito');

            // Asignar a otrasSennas el valor del input con id otras-senas, asegurando que sea un string sin espacios al inicio o final
            const otrasSennas = document.getElementById('otras-senas')?.value?.trim();

            // Asignar a constantes de tipo String el texto de las opciones seleccionadas en los selects de provincia, cantón y distrito
            const provinciaTxt = selectProv?.selectedOptions[0]?.text || '';
            const cantonTxt = selectCant?.selectedOptions[0]?.text || '';
            const disttritoTxt = selectDist?.selectedOptions[0]?.text || '';

            // Si los textos no existen o están vacíos, mostrar un mensaje de alerta y salir de la función
            if (!provinciaTxt || !cantonTxt || !disttritoTxt) {
                alert('Por favor seleccione provincia, cantón y distrito.');
                return;
            }

            // Armar la dirección compuesta con los textos de provincia, cantón, distrito y otras señas (si existen)
            const direccionCompuesta =
                `${provinciaTxt}, ${cantonTxt}, ${disttritoTxt}${otrasSennas ? `. ${otrasSennas}` : ''}`;

            // Asignar a status el elemento con id save-status
            const status = document.getElementById('save-status');

            // Realizar la llamada al servidor para actualizar el encabezado del pedido con la dirección de envío
            try {
                // Realizar la llamada al servidor para actualizar el encabezado del pedido con la dirección de envío
                const response = await fetch('/Pedido/ActualizarEncabezado', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': token,
                    },
                    body: JSON.stringify({
                        pedidoId: pedidoId,
                        userId,
                        direccionEnvio: direccionCompuesta
                    })
                });

                // Obtener la respuesta en formato JSON
                const data = await response.json();

                // Si la respuesta es exitosa, actualizar el estado del pedido y mostrar mensaje de éxito
                if (data?.success) {
                    if (data.estadoNombre) {
                        document.getElementById('pedido-estado').textContent = data.estadoNombre;
                    }

                    // Actualizar el texto del estado de guardado
                    status.textContent = (window.PedidoConfig?.localizer || {})["Encabezado guardado"] || "Encabezado guardado";
                    // Cambiar la clase del estado a texto verde
                    status.className = 'text-sm text-green-600';
                } else {
                    // Si la respuesta no es exitosa, mostrar mensaje de error
                    status.textContent = data?.mensaje || (window.PedidoConfig?.localizer || {})["No fue posible guardar"] || "No fue posible guardar";
                    // Cambiar la clase del estado a texto rojo
                    status.className = 'text-sm text-red-600';
                }
            } catch {
                // Si ocurre un error al hacer la llamada, mostrar mensaje de error
                status.textContent = (window.PedidoConfig?.localizer || {})["Error inesperado"] || "Error inesperado";

                // Cambiar la clase del estado a texto rojo
                status.className = 'text-sm text-red-600';

                // Finalmente el mensaje de estado después de 2.5 segundos
            } finally {
                setTimeout(() => { status.textContent = ""; }, 2500);
            }
        });
}

// Función para inicializar detalles del pedido
function initPedidoDetails(pedidoId, localizer) {

    // Carga vista parcial de detalles con el id de pedido
    fetch(`/PedidoDetalle/GetDetallesPorPedido?idPedido=${pedidoId}`)
        .then(response => response.text())
        .then(html => {
            document.getElementById("tabla-detalles").innerHTML = html;
            // Inicializar la validación de cantidad en los inputs de cantidad
            initDetalleCantidadValidation();

            // Inicializar el bloqueo de borrado en los inputs de cantidad
            initBloqueoBorradoCantidad();
            // Una vez cargada la vista parcial, asociar los eventos a la tabla de detalles del pedido
            const root = document.getElementById("detalles-root");
            bindParcialEventos(root);
        })
        .catch(() => {
            document.getElementById("tabla-detalles").innerHTML =
                "<p>Error</p>";
        });

    // Completar la compra
    document.getElementById('btn-completar-compra')?.addEventListener('click', async () => {
        const userIdEl = document.getElementById('cliente-userid');
        const pedidoIdAttr = userIdEl?.dataset?.pedidoId;
        const userId = Number(userIdEl?.value);
        const status = document.getElementById('save-status');


        // Obtener la dirección de envío
        const selectProvincia = document.getElementById('sel-provincia');
        const selectCanton = document.getElementById('sel-canton');
        const selectDistrito = document.getElementById('sel-distrito');
        const inputOtrasSenas = document.getElementById('otras-senas');

        // Obtener el texto de la selección de provincia, cantón y distrito (para armar dirección de envío)
        const provinciaText = selectProvincia?.selectedOptions[0]?.text || '';
        const cantonText = selectCanton?.selectedOptions[0]?.text || '';
        const distritoText = selectDistrito?.selectedOptions[0]?.text || '';

        // Validación mínima en caso de que no haya seleccionado provincia, cantón o distrito
        if (!provinciaText || !cantonText || !distritoText) {
            alert('Por favor seleccione provincia, cantón y distrito para la dirección de envío.');
            return;
        }

        // Armar la dirección de envío completa
        const direccionCompuesta = `${provinciaText}, ${cantonText}, ${distritoText}${otras ? `. ${inputOtrasSenas}` : ''}`;

        // Configurar el atributo al elemento input de dirección compuesta
        document.getElementById('direccion-compuesta')?.setAttribute('value', direccionCompuesta);

        // Actualizar el pedido (encabezado) con la dirección de envío)
        try {
            const resp = await fetch('/Pedido/ActualizarEncabezado', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ pedidoId: pedidoIdAttr, userId, direccionEnvio: direccionCompuesta })
            });

            const data = await resp.json();
            if (data?.success) {
                if (data.estadoNombre) {
                    document.getElementById('pedido-estado').textContent = data.estadoNombre;
                }
                status.textContent = localizer["Encabezado guardado"];
                status.className = 'text-sm text-green-600';
            } else {
                status.textContent = data?.mensaje || localizer["No fue posible guardar"];
                status.className = 'text-sm text-red-600';
            }
        } catch {
            status.textContent = localizer["Error inesperado"];
            status.className = 'text-sm text-red-600';
        } finally {
            setTimeout(() => { status.textContent = ""; }, 2500);
        }
    });
}

//  Función para manejar el detalle del cliente (email/telefono)
function initClienteDetalle() {

    // Obtener los detalles del cliente de forma dinámica
    const radios = document.querySelectorAll('input[name="mostrar-detalle"]');
    const userIdEl = document.getElementById('cliente-userid');
    const labelEl = document.getElementById('cliente-detalle-label');
    const valorEl = document.getElementById('cliente-detalle-valor');

    // Validar que los elementos existen antes de continuar
    if (!radios.length || !userIdEl) return;

    // Caché simple para evitar múltiples fetch del mismo usuario
    let clienteCache = null;
    let cargando = false;

    // Función asincrónica para obtener el cliente por userId
    async function obtenerCliente() {

        // Si ya tenemos el cliente en caché o estamos cargando, no hacemos nada más
        if (clienteCache || cargando) return clienteCache;

        // Cambio de estado para indicar que estamos cargando
        cargando = true;

        // mediante un try se hace el llamado por AJAX al controlador de Cliente
        try {

            // Obtener el userId del elemento input
            const userId = Number(userIdEl.value);

            // Almacenar el restultado en caché para evitar múltiples llamadas
            const respuesta = await fetch(`/Cliente/GetByUserId?userId=${encodeURIComponent(userId)}`);

            if (!respuesta.ok) {
                clienteCache = null;
                return null;
            }

            // Almacenar el resultado del llamado en clienteCache
            clienteCache = await respuesta.json();

            // retornar el cliente obtenido
            return clienteCache;

        } catch {
            return null;
        } finally {
            cargando = false;
        }
    }

    // Función asincrónica para actualizar el detalle del cliente
    async function actualizarDetalle(valor) {

        // Esperar a que se obtenga el cliente para almacenar respuesta en constante cliente
        const cliente = await obtenerCliente();

        // Si no hay cliente, no hacemos nada más
        if (!cliente) return;

        // Si el parámetro valor es telefono, actualizar los labels y valores con la información del teléfono
        if (valor === 'telefono') {
            labelEl.textContent = labelEl.textContent.replace(/Correo|Email/i, 'Teléfono') || 'Teléfono:';
            valorEl.textContent = cliente.telefono ?? '';

            // Si el parámetro valor es email, actualizar los labels y valores con la información del correo
        } else {
            labelEl.textContent = labelEl.textContent.replace(/Teléfono/i, 'Correo') || 'Correo:';
            valorEl.textContent = cliente.email ?? '';
        }
    }

    // Inicializar el detalle con el valor por defecto (email)
    actualizarDetalle('email');

    // Configurar los eventos de cambio de los radios
    radios.forEach(r => {
        r.addEventListener('change', (e) => {
            if (e.target.checked) actualizarDetalle(e.target.value);
        });
    });
}

// Función para escuchar el input de cantidad en la tabla de detalles para manejar
// la cantidad de productos a añadir al pedido
function initDetalleCantidadValidation() {

    // Obtener el body de la tabla de detalles del pedido (tbody con id pedido-detalles-body)
    const body = document.getElementById('pedido-detalles-body');

    // Si no existe el body, salir de la función
    if (!body) return;

    // Escuchar el evento de input en el body de la tabla
    body.addEventListener('input', (e) => {

        // Asignar a input el elemento que disparó el evento
        const input = e.target;

        // Si el elemento que disparó el evento no tiene la clase qty-input, salir de la función
        if (!input.classList.contains('qty-input')) return;

        // Solo dígitos, sin signos ni decimales
        input.value = input.value.replace(/[^\d]/g, '');

        // Limitar a 3 dígitos
        if (input.value.length > 3) {
            input.value = input.value.slice(0, 3);
        }

        // Convertir el valor del input a un número entero, o 0 si no hay valor
        const numero = parseInt(input.value || '0', 10);

        //// Mensaje localizado desde data-* en algún contenedor global (ajusta a tu caso)
        //const msjCantidad = document.body.dataset.msjCantidad || 'Cantidad entre 1 y 100';

        // Buscar el elemento de error en la misma fila (td) del input
        const errorEl = input.closest('td')?.querySelector('.qty-error');

        // Validar el número ingresado
        if (!isNaN(numero)) {

            // Si el número es menor que 1 o mayor que 100, mostrar mensaje de error
            if (numero < 1 || numero > 100) {
                if (errorEl) errorEl.textContent = msjCantidad;
                input.classList.add('border-red-500');

                // Si el número es válido, limpiar el mensaje de error
            } else {
                if (errorEl) errorEl.textContent = '';
                input.classList.remove('border-red-500');
            }

            // Si el número no es un número válido, mostrar mensaje de error
        } else {
            if (errorEl) errorEl.textContent = '';
            input.classList.remove('border-red-500');
        }
    });
}

// Función para inicializar el bloqueo de borrado en los inputs de cantidad
function initBloqueoBorradoCantidad() {

    // Obtener el body de la tabla de detalles del pedido (tbody con id pedido-detalles-body)
    const body = document.getElementById('pedido-detalles-body');

    // Si no existe el body, salir de la función
    if (!body) return;

    // Bloquear borrar si deja vacío
    body.addEventListener('keydown', (e) => {
        const input = e.target;
        if (!input.classList.contains('qty-input')) return;

        const key = e.key;
        const blocked = (key === 'Backspace' || key === 'Delete');

        // Evitar Ctrl/Cmd + X
        if ((e.ctrlKey || e.metaKey) && key.toLowerCase() === 'x') {
            e.preventDefault();
            return;
        }

        // Si la tecla es Backspace o Delete, verificar si se debe bloquear
        if (blocked) {
            const val = input.value ?? '';
            const start = input.selectionStart ?? 0;
            const end = input.selectionEnd ?? 0;
            const allSelected = (start === 0 && end === val.length);

            // Si el valor es vacío o solo un dígito, bloquear el borrado
            const dejarVacio = allSelected || (val.length <= 1 && start === end);
            if (dejarVacio) {
                e.preventDefault();
            }
        }
    });

    // Bloquear pegar/cortar
    body.addEventListener('paste', (e) => {
        if (e.target.classList.contains('qty-input')) e.preventDefault();
    });
    body.addEventListener('cut', (e) => {
        if (e.target.classList.contains('qty-input')) e.preventDefault();
    });

    // En blur, nunca permitir vacío o 0
    body.addEventListener('blur', (e) => {
        const input = e.target;
        if (!input.classList.contains('qty-input')) return;
        if (input.value === '' || input.value === '0') input.value = '1';
    }, true);
}

// Función para recargar la vista parcial si ya no hay detalles en el pedido
async function recargarSiNoHayDetalles(pedidoId) {

    // Hacer fetch a la API para obtener los detalles del pedido
    const html = await fetch(`/PedidoDetalle/GetDetallesPorPedido?idPedido=${pedidoId}`)

    // asignar el resultado a la tabla de detalles
    document.getElementById("tabla-detalles").innerHTML = html;

    // obtener el elemento raíz de detalles
    const root = document.getElementById("detalles-root");

    // Reenganchar los eventos a la tabla de detalles del pedido
    bindParcialEventos(root);
}

// Función para cargar provincias, cantones y distritos para el envío

async function cargarProvincias() {
    // obtener el select de provincias
    const selectProvincia = document.getElementById('sel-provincia');

    // Si el select no existe, salir de la función
    if (!selectProvincia) return;

    try {
        // Hacer fetch a la API para obtener las provincias
        const response = await fetch('/Ubicacion/Provincias');

        // Si la respuesta no es exitosa, salir de la función
        if (!response.ok) return;

        // Parsear la respuesta JSON
        const data = await response.json();
        // Ir cargando las provincias en el select y resetear cantones y distritos
        selectProvincia.innerHTML = `<option value="">Provincia</option>` +
            data.map(p => `<option value="${p.id}">${p.nombre}</option>`).join('');
        document.getElementById('sel-canton').innerHTML = `<option value="">Cantón</option>`;
        document.getElementById('sel-distrito').innerHTML = `<option value="">Distrito</option>`;
        document.getElementById('sel-canton').disabled = true;
        document.getElementById('sel-distrito').disabled = true;
    } catch {
        return null;
    }

    // Función para cargar cantones dado el id de la provincia
    async function cargarCantones(provId) {

        // Obtener los selects de cantones y distritos
        const selectCanton = document.getElementById('sel-canton');
        const selectDistrito = document.getElementById('sel-distrito');

        // Si no existen los selects de Canton o Distrito, salir de la función
        if (!selectCanton || !selectDistrito) return;

        try {
            // Hacer fetch a la API para obtener los cantones de la provincia seleccionada
            const response = await fetch(`/Ubicacion/Cantones?provinciaId=${encodeURIComponent(provId)}`);

            // Si la respuesta no es exitosa, salir de la función
            if (!response.ok) return;

            // Parsear la respuesta JSON
            const data = await response.json();
            selectCanton.innerHTML = `<option value="">Cantón</option>` +
                data.map(c => `<option value="${c.id}">${c.nombre}</option>`).join('');
            selectCanton.disabled = false;
            selectDistrito.innerHTML = `<option value="">Distrito</option>`;
            selectDistrito.disabled = true;
        } catch {
            return null;
        }
    }

    // Función para cargar distritos dado el id de la provincia y del cantón
    async function cargarDistritos(provId, cantonId) {

        // Obtener el select de distritos
        const selectDistrito = document.getElementById('sel-distrito');

        // Si no existe el select de Distrito, o no se recibio id de provincia o cantón, salir de la función
        if (!selectDistrito || !provId || !cantonId) return;

        try {

            // Hacer fetch a la API para obtener los distritos del cantón seleccionado
            const response = await fetch(`/Ubicacion/Distritos?provinciaId=${encodeURIComponent(provId)}&cantonId=${encodeURIComponent(cantonId)}`);

            // Si la respuesta no es exitosa, salir de la función
            if (!response.ok) return;

            // Parsear la respuesta JSON
            const data = await response.json();
            selectDistrito.innerHTML = `<option value="">Distrito</option>` +
                data.map(d => `<option value="${d.id}">${d.nombre}</option>`).join('');
            selectDistrito.disabled = false;
        } catch {
            return null;
        }
    }

    // listeners de cascada
    document.addEventListener('change', (e) => {

        // Si el cambio fue en el select de provincia, cargar los cantones correspondientes
        if (e.target.id === 'sel-provincia') {
            const provId = e.target.value;
            cargarCantones(provId);
        }

        // Si el cambio fue en el select de cantón, cargar los distritos correspondientes
        if (e.target.id === 'sel-canton') {
            const provId = document.getElementById('sel-provincia').value;
            const cantonId = e.target.value;
            cargarDistritos(provId, cantonId);
        }
    });
}

// Cuando el DOM esté listo, cargar las funciones de inicialización necesarias para manejar el pedido, detalles y cliente
document.addEventListener("DOMContentLoaded", () => {
    // Obtener configuración del pedido desde window.PedidoConfig
    let pedidoId = window.PedidoConfig?.pedidoId;
    let localizer = window.PedidoConfig?.localizer;

    // Fallback si pedidoId no viene por window.PedidoConfig
    if (!pedidoId) {
        pedidoId = document.getElementById('selector-usuario')?.dataset?.pedidoId;
    }

    // Si no existe localizer, inicializar con valores por defecto
    if (!localizer) {
        localizer = {
            "Encabezado guardado": "Encabezado guardado",
            "No fue posible guardar": "No fue posible guardar",
            "Error inesperado": "Error inesperado"
        };
    }

    // Inicializar la vista parcial de detalles del pedido si existe pedidoId
    if (pedidoId) {

        // Inicializar detalles del pedido con el pedidoId y localizer
        initPedidoDetails(pedidoId, localizer);
    }

    // Inicializar la función para manejar el detalle del cliente (radios de email/telefono))
    initClienteDetalle();
})

// Cuando el DOM esté listo, llamar a la función para cargar provincias
document.addEventListener('DOMContentLoaded', cargarProvincias);
