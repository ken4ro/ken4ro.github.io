// もろもろの準備
var video = document.getElementById("video");           // video 要素を取得
var canvas = document.getElementById("canvas");         // canvas 要素の取得
var context = canvas.getContext("2d");                  // canvas の context の取得

// getUserMedia によるカメラ映像の取得
let localStream = null
var media = navigator.mediaDevices.getUserMedia({       // メディアデバイスを取得
    video: true,                                         // カメラの映像を使う（スマホならインカメラ）
    audio: false                                          // マイクの音声は使わない
});
media.then((stream) => {                                // メディアデバイスが取得できたら
    video.srcObject = stream; // video 要素にストリームを渡す
    $('#my-video').get(0).srcObject = stream;
    localStream = stream;
}).catch(function (error) {
    // Error
    console.error('getUserMedia() error: ', error);
});

// clmtrackr の開始
var tracker = new clm.tracker();  // tracker オブジェクトを作成
tracker.init(pModel);             // tracker を所定のフェイスモデル（※）で初期化
tracker.start(video);             // video 要素内でフェイストラッキング開始

// 描画ループ
function drawLoop() {
    requestAnimationFrame(drawLoop);                      // drawLoop 関数を繰り返し実行
    var positions = tracker.getCurrentPosition();         // 顔部品の現在位置の取得
    // showData(positions);                                  // データの表示
    context.clearRect(0, 0, canvas.width, canvas.height); // canvas をクリア
    tracker.draw(canvas);                                 // canvas にトラッキング結果を描画
}
drawLoop();                                             // drawLoop 関数をトリガー

// 顔部品（特徴点）の位置データを表示する showData 関数
function showData(pos) {
    var str = "";                                         // データの文字列を入れる変数
    for (var i = 0; i < pos.length; i++) {                 // 全ての特徴点（71個）について
        str += "特徴点" + i + ": ("
            + Math.round(pos[i][0]) + ", "                 // X座標（四捨五入して整数に）
            + Math.round(pos[i][1]) + ")<br>";             // Y座標（四捨五入して整数に）
    }
    var dat = document.getElementById("dat");             // データ表示用div要素の取得
    dat.innerHTML = str;                                  // データ文字列の表示
}

// Peer オブジェクト作成
let existingCall = null;
peer = new Peer({
    key: 'apikey',
    debug: 3
});

// 発信処理
let targetPeerId = 1;
const call = peer.call($('#callto-id').val(), localStream);
setupCallEventHandlers(call);

// 切断処理
existingCall.close();

// 着信処理
peer.on('call', function(call){
    call.answer(localStream);
    setupCallEventHandlers(call);
});

// Callオブジェクト必要なイベントリスナー
function setupCallEventHandlers(call){
    if (existingCall) {
        existingCall.close();
    };

    existingCall = call;

    call.on('stream', function(stream){
        addVideo(call, stream);
        setupEndCallUI();
        $('#their-id').text(call.remoteId);
    });
    call.on('close', function(){
        removeVideo(call.remoteId);
        setupMakeCallUI();
    });
}

function addVideo(call, stream){
    $('#their-video').get(0).srcObject = stream;
}

function removeVideo(peerId){
    $('#'+peerId).remove();
}

function setupMakeCallUI(){
    $('#make-call').show();
    $('#end-call').hide();
}

function setupEndCallUI(){
    $('#make-call').hide();
    $('#end-call').show();
}

// 各種コールバック
peer.on('open', function(){
    // SkyWay のシグナリングサーバと接続完了
    $('#my-id').text(peer.id);
    var dat = document.getElementById("dat");
    dat.innerText = "Connection to SkyWay is succeeded";
})

peer.on('error', function(err){
    alert(err.message);
});

peer.on('close', function(){
    // Peer (相手) との接続が切断された
    console.log("Peer close");
});

peer.on('disconnected', function(){
    // シグナリングサーバーとの接続が切断された
    console.log("Connection to SkyWay is disconnected");
});

