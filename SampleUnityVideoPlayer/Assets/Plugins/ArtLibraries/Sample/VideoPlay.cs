/**
 * \copyright Copyright 2018 Applibot Inc.
 */

using UnityEngine;
using UnityEngine.Video;

namespace Art.Sample
{
    /// <summary>VideoPlayerによる動画再生クラス</summary>
    public class VideoPlay : MonoBehaviour
    {
        public enum PlayState
        {
            Stoped = 0,
            Loading,
            Loaded,
            Playing,
            Paused
        }

        /// <summary>エラー通知</summary>
        public System.Action<string> OnErrorReceived = null;
        /// <summary>ロード完了通知</summary>
        public System.Action OnPrepareCompleted = null;
        /// <summary>再生開始通知</summary>
        public System.Action OnStarted = null;
        /// <summary>再生完了通知</summary>
        public System.Action OnEnd = null;

        /// <summary>ステータス</summary>
        public PlayState State
        {
            get { return _playState; }
        }

        /// <summary>インスタンスの生成</summary>
        public static VideoPlay Create(Camera camera = null)
        {
            // シーンを横断する場合はDontDestroyOnLoadにする
            GameObject movieGameObject = new GameObject(kMovieGameObjectName);
            VideoPlay movieBehaviour = movieGameObject.AddComponent<VideoPlay>();
            movieBehaviour.Init(movieGameObject, camera);

            return movieBehaviour;
        }

        /// <summary>インスタンスの破棄</summary>
        public void Dispose()
        {
            // イベント設定解除
            _videoPlayer.errorReceived -= ErrorReceived;
            _videoPlayer.prepareCompleted -= PrepareCompleted;
            _videoPlayer.started -= PlayStart;
            _videoPlayer.loopPointReached -= PlayEnd;
            OnErrorReceived = null;
            OnPrepareCompleted = null;
            OnStarted = null;
            OnEnd = null;

            GameObject.Destroy(_movieGameObject);
        }

        /// <summary>事前ローディング</summary>
        public void Preload(string filePath)
        {
            _playState = PlayState.Loading;
#if WITH_DEVELOP
            _movieGameObject.name = kMovieGameObjectName + System.IO.Path.GetFileName(filePath);
#endif
            _videoPlayer.url = filePath;
            _videoPlayer.Prepare();
        }

        /// <summary>事前ロードした動画の再生開始</summary>
        public void PlayPrepared(bool loop = false, bool tapSkip = true)
        {
            if (_playState != PlayState.Loaded
                && _playState != PlayState.Stoped)
            {
                const string message = "not Prepared";
                ErrorReceived(_videoPlayer, message);
                return;
            }

            _tapSkip = tapSkip;
            _videoPlayer.isLooping = loop;
            _playState = PlayState.Playing;
            _videoPlayer.Play();
        }

        /// <summary>動画の再生開始(パス指定)</summary>
        public void Play(string filePath, bool loop = false, bool tapSkip = true)
        {
            if (_playState == PlayState.Playing || _playState == PlayState.Paused)
            {
                const string message = "already playing";
                ErrorReceived(_videoPlayer, message);
                return;
            }

            if (_playState == PlayState.Loading || _playState == PlayState.Loaded)
            {
                const string message = "already Prepared";
                ErrorReceived(_videoPlayer, message);
                return;
            }
#if WITH_DEVELOP
            _movieGameObject.name = kMovieGameObjectName + System.IO.Path.GetFileName(filePath);
#endif
            _tapSkip = tapSkip;
            _videoPlayer.url = filePath;
            _videoPlayer.isLooping = loop;

            _playState = PlayState.Playing;
            _videoPlayer.Play();
        }

        /// <summary>再生停止</summary>
        public void Stop()
        {
            if (_videoPlayer.isPlaying)
            {
                _videoPlayer.Stop();
            }

            PlayEnd(_videoPlayer);
        }

        /// <summary>再生一時停止</summary>
        public void Pause()
        {
            if (_playState != PlayState.Playing)
            {
                return;
            }

            _playState = PlayState.Paused;
            _videoPlayer.Pause();
        }

        /// <summary>再生一時停止再開</summary>
        public void Resume()
        {
            if (_playState != PlayState.Paused)
            {
                return;
            }

            _playState = PlayState.Playing;
            _videoPlayer.Play();
        }

        /// <summary>表示の有効無効設定</summary>
        public void SetEnabled(bool enabled)
        {
            _videoPlayer.enabled = enabled;
        }

        private void Init(GameObject gameObject, Camera camera)
        {
            _movieGameObject = gameObject;

            _audioSource = gameObject.AddComponent<AudioSource>();
            _videoPlayer = gameObject.AddComponent<VideoPlayer>();

            // 表示設定
            _videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
            _videoPlayer.targetCamera = camera;
            _videoPlayer.aspectRatio = VideoAspectRatio.FitInside;
            _videoPlayer.playOnAwake = false;

            // サウンド設定
            //TODO: エディタだと音声が出力されない(Unity 2017.2.1f1)
            //      PrepareCompleted後に設定すると2回目以降は再生される…
            _audioSource.playOnAwake = false;
            _videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            _videoPlayer.controlledAudioTrackCount = 1;
            _videoPlayer.EnableAudioTrack(0, true);
            _videoPlayer.SetTargetAudioSource(0, _audioSource);

            // イベント設定
            _videoPlayer.errorReceived += ErrorReceived;
            _videoPlayer.prepareCompleted += PrepareCompleted;
            _videoPlayer.started += PlayStart;
            _videoPlayer.loopPointReached += PlayEnd;
        }

        private void ErrorReceived(VideoPlayer player, string message)
        {
            if (OnErrorReceived != null)
            {
                OnErrorReceived(message);
            }
        }

        private void PrepareCompleted(VideoPlayer player)
        {
            _playState = PlayState.Loaded;
            if (OnPrepareCompleted != null)
            {
                OnPrepareCompleted();
            }
        }

        private void PlayStart(VideoPlayer player)
        {
            if (OnStarted != null)
            {
                OnStarted();
            }
        }

        private void PlayEnd(VideoPlayer player)
        {
            if (_playState == PlayState.Stoped)
            {
                return;
            }
            _playState = PlayState.Stoped;

            if (OnEnd != null)
            {
                OnEnd();
            }
        }

        void Update ()
        {
            if (_playState != PlayState.Playing || !_tapSkip)
            {
                return;
            }

            // タップ確認
            if ( Input.GetMouseButtonUp(0) )
            {
                Stop();
            }
        }

        private const string kMovieGameObjectName = "Movie_";
        private PlayState _playState = PlayState.Stoped;
        private GameObject _movieGameObject = null;
        private VideoPlayer _videoPlayer = null;
        private AudioSource _audioSource = null;
        private bool _tapSkip = false;

    } // class VideoPlay
} // namespace Art.Sample
