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
        const direccion = document.getElementById('direccion-envio')?.value ?? '';
        const status = document.getElementById('save-status');


        try {
            const resp = await fetch('/Pedido/ActualizarEncabezado', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ pedidoId: pedidoIdAttr, userId, direccionEnvio: direccion })
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

    document.getElementById('selector-usuario')?.addEventListener('change', async (e) => {
        const userId = e.target.value;
        try {
            const r = await fetch(`/Cliente/GetByUserId?userId=${encodeURIComponent(userId)}`);
            const cliente = await r.json();
            // Refresca panel cliente
            document.getElementById('cliente-nombre').textContent =
                `${cliente?.nombre ?? ''} ${cliente?.apellido ?? ''}`.trim();
            document.getElementById('cliente-email').textContent = cliente?.email ?? '';
            document.getElementById('cliente-telefono').textContent = cliente?.telefono ?? '';
        } catch {
            // opcional: mostrar un pequeño aviso
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