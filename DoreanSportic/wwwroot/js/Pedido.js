// Función para inicializar detalles del pedido
function initPedidoDetails(pedidoId, localizer) {

    // Carga vista parcial de detalles con el id de pedido
    fetch(`/PedidoDetalle/GetDetallesPorPedido?idPedido=${pedidoId}`)
        .then(response => response.text())
        .then(html => {
            document.getElementById("tabla-detalles").innerHTML = html;
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
});

// Cuando el DOM esté listo, llamar a la función para cargar provincias
document.addEventListener('DOMContentLoaded', cargarProvincias);