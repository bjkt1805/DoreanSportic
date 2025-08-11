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
    "UsuarioYaExiste": {
        es: "El usuario ya existe",
        en: "Username already exists"
    },
    "CorreoElectronicoYaRegistrado": {
        es: "El correo electrónico ya está registrado",
        en: "Email address is already registered"
    },
    "CredencialesIncorrectos": {
        es: "Usuario o contraseña inválidos",
        en: "Incorrect username or password"
    },
    "UsuarioNoEncontradoInactivo": {
        es: "Usuario no encontrado o inactivo",
        en: "User not found or inactive",
    }
}

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

// Función para escuchar el input ed usuario y validar formato
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

// Función para manejar el envío del formulario de login
function manejarSubmitLogin(event) {
    // Prevenir el envío del formulario por defecto
    event.preventDefault();

    // Obtener el formulario
    const form = event.target;

    // Habilitar validación unobtrusive del formulario
    $.validator.unobtrusive.parse(form);

    // Si el formulario no es válido, salir de la función
    if (!$(form).valid()) return;

    // Crear un objeto FormData con los datos del formulario
    const formData = new FormData(form);

    // Hacer la petición AJAX al servidor
    fetch("/Login/Login", {
        method: form.method,
        body: formData
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Si el login es exitoso, mostrar Toast y redirigir a la página principal o a la URL indicada
                mostrarToast(getMensaje("msjloginok"), "success");
                // Resetear el formulario si hay respuesta exitosa
                form.reset();
                // Esperar .75 segundos para redirigir a la página 
                // donde se envió la solicitud de autenticación
                // para que el toast sea visible en pantalla
                setTimeout(() => {
                    window.location.href = data.redirectUrl || "/";
                }, 750);  

            } else if (data.errors) {
                // Si hay errores, pintarlos en el formulario
                mostrarToast(getTranslation("CredencialesIncorrectos"), "error");
            } else if (data.errores) {
                pintarErroresFormulario(form, data.errores);
                mostrarToast(getMensaje?.("msjLoginError") || "Revisa tus credenciales", "error");
            }
        })
        // Capturar cualquier error de la petición
        .catch(error => {
            console.error('Error:', error);
            mostrarToast(getMensaje?.("msjErrorInesperado") || "Error inesperado", "error");
        });
}

// Función para manejar el envío del formulario de registro
function manejarSubmitRegistro(event) {
    // Prevenir el envío del formulario por defecto
    event.preventDefault();

    // Obtener el formulario
    const form = event.target;

    // Habilitar validación unobtrusive del formulario
    $.validator.unobtrusive.parse(form);

    // Si el formulario no es válido, salir de la función
    if (!$(form).valid()) return;

    // Crear un objeto FormData con los datos del formulario
    const formData = new FormData(form);

    // Hacer la petición AJAX al servidor
    fetch(form.action || "/Login/Registrar", {
        method: form.method,
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Si el registro es exitoso, mostrar Toast y redirigir a la página de Login o a la URL indicada
                mostrarToast(getMensaje("registrook"), "success");
                // Resetear el formulario si hay respuesta exitosa
                form.reset();
                // Esperar .75 segundos para redirigir a "/Login/Login"
                // para que el toast sea visible en pantalla
                setTimeout(() => {
                    window.location.href = "/Login/Login";
                }, 750);
                return;
            }


            // Si viene un mensaje simple del backend, mostrar el error en el formulario
            if (data.errors) {
                mostrarToast(getMensaje("CredencialesIncorrectos"), "error");
                return;
            }

            // Si algún caso devuelve diccionario de errores por campo
            if (data.errors || data.errores) {
                pintarErroresFormulario(form, data.errors || data.errores);
                return;
            }

            // fallback
            mostrarToast(getMensaje?.("msjErrorInesperado") || "Error inesperado", "error");
        })
        // Capturar cualquier error de la petición
        .catch(error => {
            console.error('Error:', error);
            mostrarToast(getMensaje?.("msjErrorInesperado") || "Error inesperado", "error");
        });
}

// Función para manejar el envío del formulario de cambio de contraseña
function manejarSubmitRecuperarContrasenna(event) {
    // Prevenir el envío del formulario por defecto
    event.preventDefault();

    // Obtener el formulario
    const form = event.target;

    // Habilitar validación unobtrusive del formulario
    $.validator.unobtrusive.parse(form);

    // Si el formulario no es válido, salir de la función
    if (!$(form).valid()) return;

    // Crear un objeto FormData con los datos del formulario
    const formData = new FormData(form);

    // Hacer la petición AJAX al servidor
    fetch("/Login/RecuperarContrasenna", {
        method: form.method,
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Si el login es exitoso, mostrar Toast y redirigir a la página principal o a la URL indicada
                mostrarToast(getMensaje("recuperacionok"), "success");
                // Resetear el formulario si hay respuesta exitosa
                form.reset();
                // Esperar .75 segundos para redirigir a "/Login/Login"
                // para que el toast sea visible en pantalla
                setTimeout(() => {
                    window.location.href = "/Login/Login";
                }, 750);
                return;
            }

            // Si viene un mensaje simple del backend, mostrar el error en el formulario
            if (data.errors) {
                // Si el login es exitoso, mostrar Toast y redirigir a la página principal o a la URL indicada
                mostrarToast(getTranslation("UsuarioNoEncontradoInactivo"), "error");
                return;
            }

            // Si algún caso devuelve diccionario de errores por campo
            if (data.errors || data.errores) {
                pintarErroresFormulario(form, data.errors || data.errores);
                return;
            }

            // fallback
            mostrarToast(getMensaje?.("msjErrorInesperado") || "Error inesperado", "error");
        })
        // Capturar cualquier error de la petición
        .catch(error => {
            console.error('Error:', error);
            mostrarToast(getMensaje?.("msjErrorInesperado") || "Error inesperado", "error");
        });
}

// Función para mostrar el toast a la hora de login/registro/cambio de contraseña
// ya sea exitoso o error
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
    }, 4000);
}

// Cuando el DOM esté listo, cargar los eventos de envío de los formularios al cargar la página
document.addEventListener("DOMContentLoaded", () => {

    // Obtener el formulario de login
    const formLogin = document.querySelector("#formLogin");
    // Obtener el formulario de registro
    const formRegistro = document.querySelector("#formRegistro");
    // Obtener el formulario de cambio de contraseña
    const formRecuperarContrasenna = document.querySelector("#formRecuperarContrasenna");

    // Si el formulario de login existe, agregar el evento de envío
    if (formLogin) {
        formLogin.addEventListener("submit", manejarSubmitLogin);
    }

    // Si el formulario de registro existe, agregar el evento de envío
    if (formRegistro) {
        formRegistro.addEventListener("submit", manejarSubmitRegistro);
    }

    // Si el formulario de cambio de contraseña existe, agregar el evento de envío
    if (formRecuperarContrasenna) {
        formRecuperarContrasenna.addEventListener("submit", manejarSubmitRecuperarContrasenna);
    }

});

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

// Cuando el DOM esté listo, cargar los eventos de validación de los campos del formulario de registro
document.addEventListener("DOMContentLoaded", () => {
    const formularioRegistro = document.getElementById("formRegistro");
    if (!formularioRegistro) return;

    configurarValidacionUsuario("UserName", "UserName", "msj-usuario");
    configurarValidacionPassword("Password", "Password", "msj-password");
    configurarValidacionConfirmacionPassword("ConfirmPassword", "Password", "ConfirmPassword", "msj-confirm");
    configurarValidacionEmail("Email", "Email", "msj-email");
    configurarValidacionNombreApellido("Nombre", "Nombre", "msj-nombre");
    configurarValidacionNombreApellido("Apellido", "Apellido", "msj-apellido");
    configurarValidacionTelefono("Telefono", "Telefono", "msj-telefono");
});

// Cuando el DOM esté listo, cargar los eventos de validación de los campos del formulario de recuperar contraseña
document.addEventListener("DOMContentLoaded", () => {
    const formularioRecuperarContrasenna = document.getElementById("formRecuperarContrasenna");
    if (!formularioRecuperarContrasenna) return;

    configurarValidacionUsuario("UserName", "UserName", "msj-usuario");
    configurarValidacionPassword("Password", "Password", "msj-password");
    configurarValidacionConfirmacionPassword("ConfirmPassword", "Password", "ConfirmPassword", "msj-confirm");
});

