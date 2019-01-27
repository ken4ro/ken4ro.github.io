'use strict';

// もろもろの準備
var video = document.getElementById("my-video");           // video 要素を取得
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
    localStream = stream;
}).catch(function (error) {
    // Error
    console.error('getUserMedia() error: ', error);
});

// Peer オブジェクト作成
var peer = new Peer({
    key: '22bed81b-406a-4fec-a08c-998f8cd7588d',
    debug: 3
});

// SkyWay のシグナリングサーバと接続完了
peer.on('open', function () {
    $('#my-peer-id').text(peer.id);
    console.log("peer open. Connection to SkyWay is succeeded");
})

// 部屋作成ボタンクリック
$('create-room').submit(function (e) {
    // e.preventDefault();
    var formText = document.getElementById("my-peer-id-text").formText;
    console.log("my peer id form text: ", formText);
});

// 入室ボタンクリック
$('enter-room').submit(function (e) {
    // e.preventDefault();
    // 指定した Peer ID と接続
    var formText = document.getElementById("her-peer-id-text").formText;
    console.log("her peer id form text: ", formText);
    var herPeerId = formText;
    callToTargetPeer(peer, herPeerId);
    // 現在入室中の部屋のIDを表示
    document.getElementById("now-id").formText = herPeerId;
});

// 相手から接続要求があった
peer.on('call', function (call) {
    // 接続要求に応答
    call.answer(localStream);
    // call オブジェクト更新
    setupCallEventHandlers(call);
});

// エラー発生
peer.on('error', function (err) {
    alert(err.message);
});

// シグナリングサーバーとの接続が切断された
peer.on('disconnected', function () {
    console.log("Connection to SkyWay is disconnected");
});

// 対象の Peer ID に発信
function callToTargetPeer(peer, targetPeerId) {
    // 発信
    const call = peer.call(targetPeerId, localStream);
    // call オブジェクト更新
    setupCallEventHandlers(call);
}

// 発信 or 着信
let existingCall = null;
function setupCallEventHandlers(call) {
    // 既存の接続を切断
    if (existingCall) {
        existingCall.close();
    };
    existingCall = call;
    // 相手のカメラ映像・マイク音声を受信
    call.on('stream', function (stream) {
        $('#her-video').get(0).srcObject = stream;
        $('#make-call').hide();
        $('#end-call').show();
        $('#her-peer-id').text(call.remoteId);
    });
    // 相手との接続が切断
    call.on('close', function () {
        $('#' + call.remoteId).remove();
        $('#make-call').show();
        $('#end-call').hide();
    });
}

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
