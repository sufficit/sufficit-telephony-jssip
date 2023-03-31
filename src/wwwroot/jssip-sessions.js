export var DotNetObjectReference;

/**
 * Saving dotnet object reference for service
 * @param {any} dotNetObjectRef
 */
export function Reference(dotNetObjectRef) { DotNetObjectReference = dotNetObjectRef; }

/**
 * Usado para vincular os eventos a uma seção recem criada
 * @param {any} JsSIPSession
 */
export function onJsSIPSession(JsSIPSession) {
    console.debug('onJsSIPSession: ', JsSIPSession);
    const session = WebPhone._sessions[JsSIPSession.id];    
    
    session.on('newDTMF', e => dispatchSessionEvent(session, 'newDTMF', e));
    session.on('newInfo', e => dispatchSessionEvent(session, 'newInfo', e));

    session.on('hold', e => dispatchSessionEvent(session, 'hold', e));
    session.on('muted', e => dispatchSessionEvent(session, 'muted', e));
    session.on('unhold', e => dispatchSessionEvent(session, 'unhold', e));
    session.on('unmuted', e => dispatchSessionEvent(session, 'unmuted', e));

    session.on('progress', e => dispatchSessionEvent(session, 'progress', e));
    session.on('succeeded', e => dispatchSessionEvent(session, 'succeeded', e));
    session.on('failed', e => dispatchSessionEvent(session, 'failed', e));
    session.on('ended', e => dispatchSessionEvent(session, 'ended', e));

    session.on('confirmed', e => dispatchSessionEvent(session, 'confirmed', e));

    // emitido ai iniciar uma tentativa de conexão de seção
    session.on('connecting', e => dispatchSessionEvent(session, 'connecting', e));
    session.on('accepted', e => dispatchSessionEvent(session, 'accepted', e));
    session.on('peerconnection', e => dispatchSessionEvent(session, 'peerconnection', e));

    //session.on('peerconnection:createofferfailed', e => dispatchSessionEvent(session, 'peerconnection:createofferfailed', e));
}

function dispatchSessionEvent(session, title, e) {
    
    if (title === 'peerconnection' || title === 'connecting') {
        // Events to Media Stream
        if (session.connection)
            session.connection.addEventListener('addstream', onAddstream);
    }

    // adjust event name
    let eventTitle = `on${title.charAt(0).toUpperCase()}${title.slice(1)}`;

    // include json converter
    e.toJSON = JsSIPSessionEventToJson;

    console.debug(`dispatchSessionEvent: ${eventTitle}`, session);
    DotNetObjectReference.invokeMethodAsync(eventTitle, session, e);
}

/** JSON Replacer for the sessions events */
function JsSIPSessionEventToJson() {
    let properties = ['originator', 'cause'];
    let result = {};
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

let audioRemoteElementID = 'audio-player-remote';
let videoRemoteElementID = 'media-player-remote';
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
export function JsSIPSessionActions(info, action, args) {
    //console.debug("JsSIPSessionActions: ", info, action);
    const session = WebPhone._sessions[info.id];
    //console.debug(`JsSIPSessionActions: ${action}: `, session, args);
    switch (action) {
        case 'answer':
            session.answer(args); // atender
            break;
        case 'terminate':
            session.terminate(); //rejeitar
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
            console.log(`Sorry, we are out of ${action}.`);
    }
}

/**
 * Originate a call session
 * @param {any} uri
 * @param {any} arguments
 */
export const Originate = function (uri, arguments) {
    WebPhone.call(uri, arguments);
}

/**
 * Answer a call session
 * @param {any} sessionId
 * @param {any} arguments
 */
export const Answer = function (sessionId, arguments) {
    const session = WebPhone._sessions[sessionId];
    session.answer(arguments);
}

//#endregion
