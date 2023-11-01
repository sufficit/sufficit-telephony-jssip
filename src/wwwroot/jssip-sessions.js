export var DotNetObjectReference;

/**
 * Get session by id
 * @param {any} id
 * @returns
 */
export const GetSession = function (id) {
    const session = WebPhone._sessions[id];
    if (!session) {
        throw new Error(`session id: ${id} not found`);
    }
    return session;
}

/**
 * Saving dotnet object reference for service
 * @param {any} dotNetObjectRef
 */
export function Reference(dotNetObjectRef) { DotNetObjectReference = dotNetObjectRef; }

/**
 * Attaches events handlers to recent created sessions
 * @param {any} id
 * @param {any} events
 */
export const AttachEventHandlers = function (id, events) {
    const session = GetSession(id);
    events.forEach(function (title) {
        const method = `On${title.charAt(0).toUpperCase()}${title.slice(1)}`;
        session.on(title, e => dispatchSessionEvent(session, method, e));
    });
}

/**
 * Usado para vincular os eventos a uma seção recem criada
 */
export const Monitor = async function (id, reference) {
    console.debug(`monitor session: ${id}`);
    const session = GetSession(id);

    session.on('newDTMF', e => reference.invokeMethodAsync('OnNewDTMF', e));
    session.on('newInfo', e => reference.invokeMethodAsync('OnNewInfo', e));

    session.on('hold', e => reference.invokeMethodAsync('OnHold', e));
    session.on('muted', e => reference.invokeMethodAsync('OnMuted', e));
    session.on('unhold', e => reference.invokeMethodAsync('OnUnhold', e));
    session.on('unmuted', e => reference.invokeMethodAsync('OnUnmuted', e));

    session.on('progress', e => reference.invokeMethodAsync('OnProgress', e));
    session.on('accepted', e => reference.invokeMethodAsync('OnAccepted', e));
    session.on('succeeded', e => reference.invokeMethodAsync('OnSucceeded', e));
    session.on('failed', e => reference.invokeMethodAsync('OnFailed', e));
    session.on('ended', e => reference.invokeMethodAsync('OnEnded', e));

    session.on('confirmed', e => reference.invokeMethodAsync('OnConfirmed', e));

    // emitido ai iniciar uma tentativa de conexão de seção
    session.on('connecting', e => reference.invokeMethodAsync('OnConnecting', e));
    session.on('peerconnection', e => reference.invokeMethodAsync('OnPeerconnection', e));    
}


function dispatchSessionEvent(session, title, e) {
    
    if (title === 'OnPeerconnection' || title === 'OnConnecting') {
        console.debug(`on ${title}, connection:`, session.connection);

        // Events to Media Stream
        if (session.connection)
            session.connection.addEventListener('addstream', onAddstream);
    }

    // include json converter
    e.toJSON = JsSIPSessionEventToJson;

    console.debug(`dispatchSessionEvent: ${title}`, session, e);
    DotNetObjectReference.invokeMethodAsync(title, session, e);
}

/** JSON Replacer for the sessions events */
function JsSIPSessionEventToJson() {
    let properties = ['originator', 'cause'];
    let result = {}
    for (var x in this) {
        if (properties.includes(x)) {
            let key = x;
            if (key.startsWith("_")) key = key.slice(1);
            result[key] = this[x];
        }
    }
    //console.debug("JsSIPSessionEventToJson: ", this, result);
    return result;
}

function onAddstream(event) {
    
    const tracks = event.stream.getTracks();
    console.debug('onAddstream, event, tracks: ', event, tracks);

    tracks.forEach(eachMediaTrack);
}

const audioRemoteElementID = 'audio-player-remote';
const videoRemoteElementID = 'media-player-remote';
function eachMediaTrack(track, index, array) {
    let htmlElement;
    if (track.kind === 'audio') {
        htmlElement = document.getElementById(audioRemoteElementID);
        if (!htmlElement) {
            htmlElement = document.createElement('audio');
            htmlElement.id = audioRemoteElementID;
            document.body.appendChild(htmlElement);
        }
    } else if (track.kind === 'video') {
        htmlElement = document.getElementById(videoRemoteElementID);
        if (!htmlElement) {
            htmlElement = document.createElement('video');
            htmlElement.id = videoRemoteElementID;
            document.body.appendChild(htmlElement);
        }
    } else {
        console.error('unknown media stream');
        return;
    }

    const mediaStream = new MediaStream([ track ]);
    if ('srcObject' in htmlElement) {
        htmlElement.srcObject = mediaStream;
    } else {
        // Avoid using this in new browsers, as it is going away.
        htmlElement.src = URL.createObjectURL(mediaStream);
    }

    htmlElement.play();
}


//#region SESSION METHODS

/**
 * Execute actions to sessions
 * @param {any} info Session basic information
 * @param {any} action 
 */
export const JsSIPSessionActions = async function (info, action, args) {
    console.debug(`action ${action} for session: ${info.id}`);
    const session = GetSession(info.id);

    switch (action) {
        case 'answer':
            session.answer(args); // atender
            break;
        case 'terminate':
            session.terminate(); // rejeitar
            break;
        case 'mute':
            session.mute(args); // mutar audio ou/e video
            break;
        case 'unmute':
            session.unmute(args); // habilitar audio ou/e video
            break;
        case 'hold':
            session.hold(); // colocar chamada em espera
            break;
        case 'unhold':
            session.unhold(); // continuar chamada em espera
            break;


        case 'Papayas':
            console.log('Mangoes and papayas are $2.79 a pound.');
            // expected output: "Mangoes and papayas are $2.79 a pound."
            break;
        default:
            console.log(`sorry, we are out of ${action}.`);
            break;
    }
}

/**
 * Originate a call session
 * @param {any} uri
 * @param {any} args
 */
export const Originate = async function (uri, args) {
    console.debug('originate with args:', args);
    return WebPhone.call(uri, args);
}

/**
 * Terminate a call session
 */
export const Terminate = function (id) {
    console.debug(`terminate session: ${id}`);
    const session = GetSession(id);
    session.terminate();
}

/**
 * Answer a call session
 * @param {any} id
 * @param {any} args
 */
export const Answer = function (id, args) {
    const session = GetSession(id);
    session.answer(args);
}

//#endregion
