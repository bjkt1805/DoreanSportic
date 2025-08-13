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

// Función básica Luhn para validar número de tarjeta
function luhnCheck(numStr) {

    const digits = (numStr || '').replace(/\s+/g, '');
    if (!/^\d+$/.test(digits)) return false;
    let sum = 0, alt = false;
    for (let i = digits.length - 1; i >= 0; i--) {
        let n = parseInt(digits[i], 10);
        if (alt) {
            n *= 2;
            if (n > 9) n -= 9;
        }
        sum += n;
        alt = !alt;
    }
    return (sum % 10) === 0;
}

// Para verificar que la tarjeta no esté expirada (mes/año)
function expValida(mmAA) {

    // 
    if (!/^\d{2}\/\d{2}$/.test(mmAA)) return false;
    const [mmStr, aaStr] = mmAA.split('/');
    const mm = Number(mmStr), aa = Number(aaStr);
    if (mm < 1 || mm > 12) return false;

    // Año base 2000-2099 para “AA”
    const year = 2000 + aa;
    const now = new Date();
    // último día del mes de vencimiento
    const expDate = new Date(year, mm, 0, 23, 59, 59);
    return expDate >= now;
}

function parseMoney(v) {
    if (typeof v !== 'string') return Number(v) || 0;
    return Number(v.replace(/[^\d.]/g, '')) || 0;
}

// Función para pintar totales del modal leyendo los ya mostrados en la página
function pintarTotalesModalDesdePantalla() {
    const sub = document.getElementById('totals-sub')?.textContent ?? '—';
    const imp = document.getElementById('totals-tax')?.textContent ?? '—';
    const tot = document.getElementById('totals-grand')?.textContent ?? '—';
    document.getElementById('pago-sub').textContent = sub;
    document.getElementById('pago-imp').textContent = imp;
    document.getElementById('pago-tot').textContent = tot;
}

// Función para leer la cantidad de una fila de productos por detalleId
function getCantidadProducto(detalleId) {

    // Buscar la fila del producto por detalleId y obtener el valor del input de cantidad
    const filaProd = document.querySelector(`tr[data-detalle-id="${detalleId}"]`);
    if (!filaProd) return 0;
    const val = Number(filaProd.querySelector('input.qty-input')?.value || 0);
    return isNaN(val) ? 0 : val;
}

// Recalcula el subtotal de UNA fila personalizada (por cambio de cantidad)
function actualizarFilaPersonalizacion(detalleId) {

    // Buscar la fila de personalización por detalleId y actualizar el subtotal
    const filaPers = document.querySelector(`#tbody-personal tr[data-detalle-id="${detalleId}"]`);

    // Si no existe la fila de personalización, salir de la función
    if (!filaPers) return;

    // Asignar a cell la celda de subtotal de personalización
    const cell = filaPers.querySelector('.cell-pers-subtotal');

    // Asignar a unit el valor numérico del atributo data-unit-pers de la celda, o 0 si no existe
    const unit = Number(cell?.dataset.unitPers || 0);

    // Obtener la cantidad del producto desde la tabla de productos
    const qty = getCantidadProducto(detalleId);

    // Calcular el nuevo subtotal de personalización
    const nuevo = unit * qty;

    // Actualizar el contenido de la celda con el nuevo subtotal formateado
    cell.textContent = formatColones(nuevo);
}

// Suma todos los subtotales de personalización en pantalla
function calcularSubtotalPersonalizaciones() {
    let total = 0;
    document.querySelectorAll('#tbody-personal tr[data-detalle-id]').forEach(row => {
        const cell = row.querySelector('.cell-pers-subtotal');
        const unit = Number(cell?.dataset.unitPers || 0);
        const detId = row.getAttribute('data-detalle-id');
        const qty = getCantidadProducto(detId);
        total += unit * qty;
    });
    return total;
}

// Función para asociar eventos a la tabla de detalles del pedido
function bindParcialEventos(root) {

    // Constante IVA
    const IVA = 0.13;

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
        const subTotal = document.getElementById('totals-sub'); 

        // Obtener el valor del impuesto
        const impuesto = document.getElementById('totals-tax');  

        // Obtener el valor del total general
        const granTotal = document.getElementById('totals-grand');

        // Calcular el subtotal de personalizaciones presente en el DOM
        // Inicializar subtotal de personalizaciones a 0
        let subPers = 0;

        // Buscar todas las filas de personalizaciones y calcular el subtotal
        const filasPers = document.querySelectorAll('#tbody-personal tr[data-detalle-id]');

        // Iterar sobre cada fila de personalización para calcular el subtotal
        filasPers.forEach(row => {

            // Asignar a cell la celda de subtotal de personalización
            const cell = row.querySelector('.cell-pers-subtotal');

            // Si no existe la celda, salir de la función
            const unit = Number(cell?.dataset.unitPers || 0);

            // Asignar a detId el valor del atributo data-detalle-id de la fila
            const detId = row.getAttribute('data-detalle-id');

            // Buscar la cantidad actual del producto correspondiente en la tabla principal
            const qtyInput = root.querySelector(`tr[data-detalle-id="${detId}"] input.qty-input`);
            const qty = Number(qtyInput?.value || 0);

            // Si la cantidad es un número válido, sumar al subtotal de personalizaciones
            if (!isNaN(qty)) subPers += unit * qty;
        });

        // Pintar "Subtotal personalizaciones" si existe el elemento
        const subPersEl = document.getElementById('sub-pers');
        if (subPersEl) subPersEl.textContent = formatColones(subPers);

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

        // Evitar procesar mientras el valor está vacío ("")
        if (e.target.value === '') return;

        // Clamp por stock (respetar el stock máximo del producto y no sobrepasar el atributo max del input)
        // Obtener el valor máximo del atributo max del input, o 999999 si no existe
        const max = parseInt(e.target.getAttribute('max') || '999999', 10);

        // Si el valor máximo es un número válido, verificar si la cantidad ingresada supera el máximo
        if (!isNaN(max)) {
            const v = parseInt(e.target.value, 10);

            // Si el valor ingresado es un número válido y supera el máximo, establecer el valor del input al máximo
            if (!isNaN(v) && v > max) {
                e.target.value = String(max);
            }
        }

        // Obtener la fila de la tabla (tr) m cercana al elemento que disparó el evento
        const filaTabla = e.target.closest('tr');

        // Asignar el dato detalleId desde el atributo data-detalle-id de la fila
        const detalleId = filaTabla.dataset.detalleId;

        // Obtener el valor numérico de la cantidad ingresada, asegurando que sea un número entero
        // Si no hay valor, asignar 0 
        const cantidad = parseInt(e.target.value || '0', 10);

        // Obtener el token de verificación anti-CSRF desde el formulario de encabezado del pedido
        const token = document.querySelector('#pedido-encabezado-form input[name="__RequestVerificationToken"]')?.value;

        // Si la cantidad no es válida o menor que 0, no continuar (no enviar 0)
        if (isNaN(cantidad) || cantidad < 0) { return; }

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

                // Eliminar también la fila de personalización si existe
                const persRow = document.querySelector(`#tbody-personal tr[data-detalle-id="${detalleId}"]`);
                if (persRow) persRow.remove();

                // Refrescar navbar
                window.recargarResumenCarritoNavbar?.();

                // Si no quedan filas ni en productos ni en personalizados, recargar la vista parcial de detalles
                // para mostrar el mensaje vacío
                const recargado = await verificarVacioYRecargar(pedidoId);
                if (recargado) return;

                // Si hay detalle, hay que actualizar las filas subtotal y precio unitario 
            } else if (data.detalle) {

                // Actualizar el contenido de las celdas subTotal y Precio Unitario con los nuevos valores
                filaTabla.querySelector('.cell-subtotal').textContent = formatColones(data.detalle.subTotal);
                const punit = data.detalle.cantidad > 0 ? (data.detalle.subTotal / data.detalle.cantidad) : 0;
                filaTabla.querySelector('.cell-punit').textContent = formatColones(punit);

                // Calcular el impuesto y total de la fila
                const impFila = Math.round(data.detalle.subTotal * IVA * 100) / 100;
                const totFila = data.detalle.subTotal + impFila;

                // Pintar en la fila
                const taxCell = filaTabla.querySelector('.cell-tax');
                const totalCell = filaTabla.querySelector('.cell-total');
                if (taxCell) taxCell.textContent = formatColones(impFila);
                if (totalCell) totalCell.textContent = formatColones(totFila);

                // Actualizar la fila de personalización si existe
                actualizarFilaPersonalizacion(detalleId);
            }

            // Actualizar los totales en pantalla con los nuevos valores
            actualizarTotales(data.totals);

            // Refrescar navbar
            window.recargarResumenCarritoNavbar?.();
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

            // Eliminar también la fila de personalización si existe
            const persRow = document.querySelector(`#tbody-personal tr[data-detalle-id="${detalleId}"]`);
            if (persRow) persRow.remove();

            actualizarTotales(data.totals);

            // Refrescar navbar
            window.recargarResumenCarritoNavbar?.();

            // Si no quedan filas ni en productos ni en personalizados, recargar la vista parcial de detalles
            // para mostrar el mensaje vacío
            const recargado = await verificarVacioYRecargar(pedidoId);
            if (recargado) return;
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

            // Obtener la opción seleccionada por cada Select
            const provinciaId = selectProv?.value?.trim();
            const cantonId = selectCant?.value?.trim();
            const distritoId = selectDist?.value?.trim();

            // Bloquear si no hay selección real

            if (!provinciaId) { mostrarToast('Seleccione una provincia', "error"); selectProv?.focus(); return; }
            if (!cantonId) { mostrarToast('Seleccione un cantón', "error"); selectCant?.focus(); return; }
            if (!distritoId) { mostrarToast('Seleccione un distrito', "error"); selectDist?.focus(); return; }

            // Asignar a constantes de tipo String el texto de las opciones seleccionadas en los selects de provincia, cantón y distrito
            const provinciaTxt = selectProv?.selectedOptions[0]?.text || '';
            const cantonTxt = selectCant?.selectedOptions[0]?.text || '';
            const disttritoTxt = selectDist?.selectedOptions[0]?.text || '';

            // Armar la dirección compuesta con los textos de provincia, cantón, distrito y otras señas (si existen)
            const direccionCompuesta =
                `${provinciaTxt}, ${cantonTxt}, ${disttritoTxt}${otrasSennas ? `. ${otrasSennas}` : ''}`;

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

                    // Limpia campos/errores
                    document.getElementById('pago-status').textContent = '';
                    document.getElementById('pago-errores-stock').textContent = '';
                    document.getElementById('card-number').value = '';
                    document.getElementById('card-exp').value = '';
                    document.getElementById('card-cvv').value = '';
                    document.getElementById('card-name').value = '';
                    document.getElementById('cash-amount').value = '';
                    document.getElementById('cash-change').value = '';

                    // Método por defecto: tarjeta
                    document.querySelector('input[name="pago-metodo"][value="tarjeta"]').checked = true;
                    document.getElementById('section-tarjeta').style.display = '';
                    document.getElementById('section-efectivo').style.display = 'none';

                    pintarTotalesModalDesdePantalla();
                    // Cargar el modal para proceder con el pago
                    document.getElementById('modalPago').showModal();

                    // Toggle secciones según método
                    document.querySelectorAll('input[name="pago-metodo"]').forEach(r => {
                        r.addEventListener('change', (e) => {
                            const m = e.target.value;
                            document.getElementById('section-tarjeta').style.display = (m === 'tarjeta') ? '' : 'none';
                            document.getElementById('section-efectivo').style.display = (m === 'efectivo') ? '' : 'none';
                        });
                    });

                    // Formateo amigable para número de tarjeta #### #### #### ####
                    document.getElementById('card-number')?.addEventListener('input', (e) => {
                        let v = e.target.value.replace(/[^\d]/g, '').slice(0, 16);
                        v = v.replace(/(\d{4})(?=\d)/g, '$1 ');
                        e.target.value = v;
                    });

                    // Solo “MM/AA”
                    document.getElementById('card-exp')?.addEventListener('input', (e) => {
                        let v = e.target.value.replace(/[^\d]/g, '').slice(0, 4);
                        if (v.length >= 3) v = v.slice(0, 2) + '/' + v.slice(2);
                        e.target.value = v;
                    });

                    // CVV numérico 3 o 4
                    document.getElementById('card-cvv')?.addEventListener('input', (e) => {
                        e.target.value = e.target.value.replace(/[^\d]/g, '').slice(0, 4);
                    });

                    // Efectivo: calcular vuelto en vivo
                    document.getElementById('cash-amount')?.addEventListener('input', (e) => {
                        const totalTxt = document.getElementById('pago-tot')?.textContent || '0';
                        const total = parseMoney(totalTxt);
                        const entregado = parseMoney(e.target.value || '');
                        const vuelto = Math.max(0, entregado - total);
                        document.getElementById('cash-change').value = formatColones(vuelto);
                    });

                    /* ========== Confirmar pago (validaciones + POST CompletarCompra) ========== */
                    document.getElementById('btn-confirmar-pago')?.addEventListener('click', async () => {
                        const statusEl = document.getElementById('pago-status');
                        const errStockEl = document.getElementById('pago-errores-stock');
                        statusEl.textContent = ''; errStockEl.textContent = '';

                        const metodo = document.querySelector('input[name="pago-metodo"]:checked')?.value || 'tarjeta';
                        const token = document.querySelector('#form-anti-forgery input[name="__RequestVerificationToken"]')?.value;
                        const pedidoId = Number(document.getElementById('detalles-root')?.dataset?.pedidoId);

                        // Totales
                        const totalTxt = document.getElementById('pago-tot')?.textContent || '0';
                        const total = parseMoney(totalTxt);

                        // Reset errores
                        const setErr = (id, msg) => { const el = document.getElementById(id); if (el) el.textContent = msg || ''; };

                        // Validaciones según método
                        if (metodo === 'tarjeta') {
                            const cardNum = document.getElementById('card-number').value.trim();
                            const cardExp = document.getElementById('card-exp').value.trim();
                            const cardCVV = document.getElementById('card-cvv').value.trim();
                            const cardName = document.getElementById('card-name').value.trim();

                            let ok = true;

                            // Número: 16 dígitos y Luhn
                            const numDigits = cardNum.replace(/\s+/g, '');
                            if (!/^\d{16}$/.test(numDigits)) {
                                setErr('err-card-number', 'Debe tener 16 dígitos.');
                                ok = false;
                            } else if (!luhnCheck(cardNum)) {
                                setErr('err-card-number', 'Número inválido (Luhn).');
                                ok = false;
                            } else {
                                setErr('err-card-number', '');
                            }

                            // Expiración válida y no pasada
                            if (!expValida(cardExp)) {
                                setErr('err-card-exp', 'Fecha inválida o vencida (MM/AA).');
                                ok = false;
                            } else {
                                setErr('err-card-exp', '');
                            }

                            // CVV 3 o 4 dígitos
                            if (!/^\d{3,4}$/.test(cardCVV)) {
                                setErr('err-card-cvv', 'CVV inválido (3 o 4 dígitos).');
                                ok = false;
                            } else {
                                setErr('err-card-cvv', '');
                            }

                            // Nombre titular no vacío
                            if (!cardName) {
                                setErr('err-card-name', 'El nombre es requerido.');
                                ok = false;
                            } else {
                                setErr('err-card-name', '');
                            }

                            if (!ok) return; // no enviamos al servidor

                        } else { // efectivo
                            const cash = parseMoney(document.getElementById('cash-amount').value || '');
                            let ok = true;

                            if (!(cash > 0)) {
                                document.getElementById('err-cash-amount').textContent = 'Monto inválido (numérico y positivo).';
                                ok = false;
                            } else if (cash < total) {
                                document.getElementById('err-cash-amount').textContent = 'El monto debe ser >= al total.';
                                ok = false;
                            } else {
                                document.getElementById('err-cash-amount').textContent = '';
                            }

                            if (!ok) return;
                        }

                        // Si pasaron validaciones, “procesamos” pago y confirmamos el pedido
                        statusEl.textContent = 'Procesando pago...';
                        statusEl.className = 'text-sm text-slate-600';
                        document.getElementById('btn-confirmar-pago').disabled = true;

                        try {
                            // Nota: aquí **no** enviamos datos de tarjeta al servidor (simulación local).
                            // Solo llamamos a CompletarCompra para cerrar el pedido.
                            const resp = await fetch('/Pedido/CompletarCompra', {
                                method: 'POST',
                                headers: {
                                    'Content-Type': 'application/json',
                                    'RequestVerificationToken': token
                                },
                                body: JSON.stringify({
                                    pedidoId: pedidoId,
                                    direccionEnvio: null // si ya guardaste antes; o manda la que corresponda
                                })
                            });

                            const data = await resp.json();

                            if (data?.success) {
                                statusEl.textContent = 'Pago completado. ¡Gracias!';
                                statusEl.className = 'text-sm text-green-600';

                                // Refrescar navbar del carrito
                                window.recargarResumenCarritoNavbar?.();

                                // Recargar parcial de detalles (mostrará “No hay productos…” si ya quedó vacío)
                                if (typeof recargarParcial === 'function') {
                                    await recargarParcial(pedidoId);
                                }

                                // Cerrar modal
                                document.getElementById('modalPago').close();

                                // (Opcional) redirigir a página de gracias
                                // window.location.href = `/Pedido/Gracias/${pedidoId}`;
                            } else {
                                if (data?.errores && Array.isArray(data.errores)) {
                                    // Errores de stock
                                    const lista = data.errores.map(e =>
                                        `• ${e.nombre}: solicitado ${e.cant}, disponible ${e.stockDisp}`).join('\n');
                                    errStockEl.textContent = lista;
                                }
                                statusEl.textContent = data?.mensaje || 'No fue posible completar la compra.';
                                statusEl.className = 'text-sm text-red-600';
                            }
                        } catch {
                            statusEl.textContent = 'Error inesperado al pagar.';
                            statusEl.className = 'text-sm text-red-600';
                        } finally {
                            document.getElementById('btn-confirmar-pago').disabled = false;
                        }

                    });


                } else {
                    // Si la respuesta no es exitosa, mostrar mensaje de error
                    mostrarToast('Error al procesar el pedido', "error");
                }
            } catch {
                // Si la respuesta no es exitosa, mostrar mensaje de error
                mostrarToast('Error inesperado', "error");
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

    // Bloquear teclas inválidas en inputs number (e, E, +, -, ., , y espacios)
    // En el evento keydown (insertando de texto con el teclado)
    body.addEventListener('keydown', (ev) => {
        const input = ev.target;
        if (!input.classList.contains('qty-input')) return;

        const k = ev.key;
        const bloqueadas = ['e', 'E', '+', '-', '.', ',', ' '];
        if (bloqueadas.includes(k)) {
            ev.preventDefault();
            return;
        }
    });

    // Evitar que la rueda del mouse cambie el número
    body.addEventListener('wheel', (ev) => {
        const input = ev.target;
        if (!input.classList.contains('qty-input')) return;
        // Si el input tiene foco, evita el scroll-change
        if (document.activeElement === input) {
            ev.preventDefault();
        }
    }, { passive: false });

    // Permitir solo números positivos enteros en los inputs de cantidad
    body.addEventListener('paste', (ev) => {
        const input = ev.target;
        if (!input.classList.contains('qty-input')) return;
        const clip = (ev.clipboardData || window.clipboardData).getData('text') || '';
        if (!/^\d+$/.test(clip)) {
            ev.preventDefault();
        }
    });

    // Escuchar el evento de input en el body de la tabla
    body.addEventListener('input', (e) => {

        // Asignar a input el elemento que disparó el evento
        const input = e.target;

        // Si el elemento que disparó el evento no tiene la clase qty-input, salir de la función
        if (!input.classList.contains('qty-input')) return;

        // Solo dígitos, sin signos ni decimales
        input.value = input.value.replace(/[^\d]/g, '');

        // Si quedó vacío, no seguimos (el usuario está escribiendo)
        if (input.value === '') return;

        // Respetar el max (stock)
        const maxAttr = parseInt(input.getAttribute('max') || '999999', 10);
        if (!isNaN(maxAttr)) {
            const v = parseInt(input.value, 10);
            if (!isNaN(v) && v > maxAttr) {
                input.value = String(maxAttr);
            }
        }

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

// Función para mostrar el toast a la hora de dar clic al botón "Pagar" ya sea exitoso o error
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

    // Esperar 4 segundos antes de remover el toast del DOM
    setTimeout(() => {
        toast.remove();
    }, 1500);
}

// Función asincrónica para recargar la vista parcial si ya no hay detalles en el pedido
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

// Función asincrónica para cargar provincias, cantones y distritos para el envío

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

// Función para recargar la vista parcial de detalles del pedido
async function recargarParcial(pedidoId) {
    // Hacer fetch a la API para obtener los detalles del pedido
    const response = await fetch(`/PedidoDetalle/GetDetallesPorPedido?idPedido=${pedidoId}`);

    // Si la respuesta no es exitosa, retornar false
    if (!response.ok) return false;

    // Obtener el HTML de la respuesta
    const html = await response.text();

    // Asignar el HTML a la tabla de detalles del pedido
    document.getElementById("tabla-detalles").innerHTML = html;

    // Reasignar eventos a la nueva tabla
    const root = document.getElementById("detalles-root");
    bindParcialEventos(root);

    return true;
}

// Función asincrónica para verificar si NO quedan filas ni en productos ni en personalización y recargar la vista parcial de detalles
async function verificarVacioYRecargar(pedidoId) {
    // Body de la tabla de productos
    const bodyProd = document.getElementById('pedido-detalles-body');
    const quedanProd = !!bodyProd && bodyProd.querySelectorAll('tr[data-detalle-id]').length > 0;

    // Body de tabla personalizados (puede no existir si nunca hubo personalizados)
    const bodyPers = document.getElementById('tbody-personal');
    const quedanPers = !!bodyPers && bodyPers.querySelectorAll('tr[data-detalle-id]').length > 0;

    // Si NO queda nada en ninguno, pedir de nuevo la parcial (que mostrará el mensaje)
    if (!quedanProd && !quedanPers) {
        await recargarParcial(pedidoId);
        return true; // indicó que recargó
    }
    return false; // no recargó
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
