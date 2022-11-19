export var WebPhone;
export var DotNetObjectReference;

let mediaSelfElementID = 'media-player-self';
let audioRemoteElementID = 'audio-player-remote';
let videoRemoteElementID = 'media-player-remote';

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
    if (!window.JsSIP) {

        const el = document.getElementById('jssip');
        if (!el) {
            window.JsSIPLoading = await new Promise(resolve => {
                console.debug("resolving: ", jssip);
                CreateScriptTag(jssip, () => resolve(window.JsSIP), document.body);
            });
        } else {
            await window.JsSIPLoading;
        }
    }

    await dotNetObjectRef.invokeMethodAsync('onDependenciesLoaded', window.JsSIP);
}

export const CreateScriptTag = function (url, implementationCode, location) {
    //url is URL of external file, implementationCode is the code
    //to be called from the file, location is the location to 
    //insert the <script> element

    var scriptTag = document.createElement('script');
    scriptTag.id = 'jssip'; // avoid duplicate tag
    scriptTag.src = url;

    scriptTag.onload = implementationCode;
    scriptTag.onreadystatechange = implementationCode;

    location.appendChild(scriptTag);
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

export function TestDevices() {
    console.debug("Testing Devices");
    navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia || navigator.mozGetUserMedia;
    navigator.getUserMedia({
        audio: true,
        video: true
    }, function () { console.debug('Testing Devices: success'); }, function () { console.debug('Testing Devices: error'); });

    //MediaDevices();
}

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

export const MediaDeviceUpdate = async function (mediaKind, mediaDevice) {
    console.debug(`MediaDeviceUpdate => ${mediaKind} :: ${mediaDevice}`);
    switch (mediaKind) {
        case 'audiooutput': {
            AttachSinkId(mediaDevice); break;
        }
        case 'videoinput': {
            let videoElement = document.getElementById(mediaSelfElementID);
            if (!videoElement) {
                videoElement = document.createElement('video');
                videoElement.id = mediaSelfElementID;
                document.body.appendChild(videoElement);
            }

            const mediaStream = await navigator.mediaDevices.getUserMedia({ video: { deviceId: { exact: mediaDevice } } });

            if ('srcObject' in videoElement) {
                videoElement.srcObject = mediaStream;
            } else {
                // Avoid using this in new browsers, as it is going away.
                videoElement.src = URL.createObjectURL(mediaStream);
            }
            break;
        }
    }
}

/** Indica se o navegador suporte a escolha do dispositivo para saída de áudio */
export const BrowserOutputSelectSupport = function () { return !('sinkId' in HTMLMediaElement.prototype); };

/** Attach audio output device to video element using device/sink ID. */
export const AttachSinkId = function (sinkId) {
    const element = document.getElementById(audioRemoteElementID);
    if (typeof element.sinkId !== 'undefined') {
        element.setSinkId(sinkId)
            .then(() => {
                console.log(`Success, audio output device attached: ${sinkId}`);
            })
            .catch(error => {
                let errorMessage = error;
                if (error.name === 'SecurityError') {
                    errorMessage = `You need to use HTTPS for selecting audio output device: ${error}`;
                }
                console.error(errorMessage);
                // Jump back to first output device in the list as it's the default.
                audioOutputSelect.selectedIndex = 0;
            });
    } else {
        console.warn('Browser does not support output device selection.');
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