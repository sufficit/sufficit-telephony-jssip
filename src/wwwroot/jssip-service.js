export var WebPhone;
export var DotNetObjectReference;

/** Recupera o estado atual do serviço  */
export function GetStatus() {
    return WebPhone.status;
}

/**
 * Saving dotnet object reference for service
 * @param {any} dotNetObjectRef
 */
export const Reference = async function(jssip, dotNetObjectRef) {
    DotNetObjectReference = dotNetObjectRef;
    let JsSIP = await new Promise(resolve => {
        require([jssip], function (JsSIP) { resolve(JsSIP); });
    });

    window.JsSIP = JsSIP;
    await dotNetObjectRef.invokeMethodAsync('onDependenciesLoaded', JsSIP);
}

/**
 * Ocorre assim que o arquivo base JsSIP for carregado completamente
 * @param {any} config Configurações passadas pelo backend (JsSIPConfiguration)
 */
export function onJsSIPLoaded(config) {

    // Criando sockets apartir dos textos passados
    let sockets = [];
    config.sockets.forEach(function (address) {
        let socket = new JsSIP.WebSocketInterface(address);
        sockets.push(socket);
    });

    // Atualizando o vetor com os sockets criados
    config.sockets = sockets;

    // Criando softphone padrão
    WebPhone = new JsSIP.UA(config);

    // Exposing WebPhone for Debug, insecure , dont problem
    window.WebPhone = WebPhone;

    // Vinculando eventos
    WebPhone.on('connected',            WPEvent.bind(this, 'onConnected'));
    WebPhone.on('disconnected',         WPEvent.bind(this, 'onDisconnected'));
    WebPhone.on('newMessage',           WPEvent.bind(this, 'onNewMessage'));
    WebPhone.on('registered',           WPEvent.bind(this, 'onRegistered'));
    WebPhone.on('unregistered',         WPEvent.bind(this, 'onUnregistered'));
    WebPhone.on('registrationFailed',   WPEvent.bind(this, 'onRegistrationFailed'));
    WebPhone.on('ringing',              WPEvent.bind(this, 'onRinging'));
    WebPhone.on('ack',                  WPEvent.bind(this, 'onAck'));    
    WebPhone.on('newRTCSession',        WPEvent.bind(this, 'onNewRTCSession'));

    // Inicializando
    WebPhone.start();


    doTest(function (e) {
        console.debug('test error: ', e);
    }, function (e) { });

    JsSIPTestVideo();
}

/**
 * Usada para filtrar o evento de nova seção
 * Gera informação demais no JSON e da erro
 * @param {any} mappedEvent
 * @param {any} data
 */
function WPEvent(mappedEvent, data) {
    switch (mappedEvent) {
        case "onNewRTCSession": {

            data = data.session;

            // include json converter
            data.toJSON = JsSIPSessionToJson;
            break;
        }
        case "onDisconnected": {
            console.debug(data);
            break;
        }
        default: break;
    }

    DotNetObjectReference.invokeMethodAsync(mappedEvent, data);
}

/** JSON Replacer for the sessions */
function JsSIPSessionToJson() {
    let properties = ['id', 'direction', 'status'];
    var result = {};
    for (const element of properties) {
        result[element] = this[element];
    }
    //console.debug("JsSIPSessionToJson: ", this, result);
    return result;
}

function doTest(errorCallback, successCallback) {
    navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia || navigator.mozGetUserMedia;
    navigator.getUserMedia({
        audio: true,
        video: true
    },
        successCallback,
        errorCallback);

    MediaDevices();
}

let mediaSelfElementID = 'media-player-self';
async function JsSIPTestVideo() {
    console.debug(navigator.mediaDevices);
    console.debug(await navigator.mediaDevices.getSupportedConstraints());

    Testtt();
    return;
    //return;
    const mediaStream = await navigator.mediaDevices.getUserMedia({ video: true });

    //console.debug('JsSIPTestVideo: ', mediaStream);
    let videoElement = document.getElementById(mediaSelfElementID);
    if (!videoElement) {
        videoElement = document.createElement('video');
        videoElement.id = mediaSelfElementID;
        document.body.appendChild(videoElement);
    }

    if ('srcObject' in videoElement) {
        videoElement.srcObject = mediaStream;
    } else {
        // Avoid using this in new browsers, as it is going away.
        videoElement.src = URL.createObjectURL(mediaStream);
    }
}

async function Testtt() {
    let devices = await MediaDevices();

    let cameraDevice = undefined;
    devices.forEach(function (dev) {
        if (!cameraDevice && dev.kind === 'videoinput') {
            cameraDevice = dev;
        }
    });

    if (cameraDevice) {
        let videoElement = document.getElementById(mediaSelfElementID);
        if (!videoElement) {
            videoElement = document.createElement('video');
            videoElement.id = mediaSelfElementID;
            document.body.appendChild(videoElement);
        }


        const mediaStream = await navigator.mediaDevices.getUserMedia({ video: { deviceId: { exact: cameraDevice.deviceId } } });

        if ('srcObject' in videoElement) {
            videoElement.srcObject = mediaStream;
        } else {
            // Avoid using this in new browsers, as it is going away.
            videoElement.src = URL.createObjectURL(mediaStream);
        }
    }
}

/** Recupera a lista de dispositivos de mídia disponiveis no navegador (cameras e microfones) */
export const MediaDevices = async function () {
    if (!navigator.mediaDevices || !navigator.mediaDevices.enumerateDevices) {
        console.warn("enumerateDevices() not supported.");
        return;
    }

    return await new Promise(resolve => {
        // List cameras and microphones.
        navigator.mediaDevices.enumerateDevices()
            .then(resolve)
            .catch(function (err) {
                console.error(err.name + ": " + err.message);
            });
    });
}