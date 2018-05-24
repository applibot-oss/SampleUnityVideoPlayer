# SampleUnityVideoPlayer
- UnityのVideoPlayerの実装サンプル

# サンプルの仕様
- 全画面の動画用に利用する
- UI等を動画の前面に表示する可能性があることを考慮する
- 複数の動画を続けて再生する
- コードだけで再生可能にする

# 使い方
```c#:Usage
video = sample.VideoPlay.Create( camera );
video.Preload( Application.streamingAssetsPath + "ファイルパス" );
 ( 完了待ち : video.State == sample.VideoPlay.PlayState.Loaded) )
video.OnEnd = ( 再生停止時の次の処理 );
video.OnErrorReceived = ( エラー発生時の処理 );
video.OnStarted  = ( 再生開始時の処理、インジケータ非表示等 );
video.PlayPrepared( false(ループしない) , true(タップによってスキップする) );
```
[Sample.cs](https://github.com/applibot-inc/SampleUnityVideoPlayer/blob/master/SampleUnityVideoPlayer/Assets/Scripts/Sample.cs)を参考にしてください。
その他詳細は近日中に[ブログ-てっくぼっと！](http://blog.applibot.co.jp/)に書きますので、しばらくお待ちください

# 動作確認方法
- Sample.unityシーンでPlayボタンを押してください
