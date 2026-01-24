using MelonLoader;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using UnityEngine;
using UnityEngine.UI;
using static Il2CppSystem.Threading.Volatile;

namespace DCGO_Tweaks
{
    [RegisterTypeInIl2Cpp]
    internal class AnimatedImage : MonoBehaviour
    {
        public AnimatedImage(IntPtr ptr) : base(ptr) { }

        List<Texture2D> _frames = new List<Texture2D>();
        List<float> _frame_durations = new List<float>();

        int _current_frame = 0;
        float _current_frame_time = 0f;

        HashSet<RawImage> _subscribers = new HashSet<RawImage>();

        ManualResetEventSlim _new_frame_added = new ManualResetEventSlim(true);
        Color32[] _new_frame_pixels = null;
        readonly object _new_frame_pixels_lock = new object();

        volatile int _width = 0;
        volatile int _height = 0;

        volatile bool _is_loading = false;
        volatile bool _is_loaded = false;

        static SemaphoreSlim _load_limit = new SemaphoreSlim(4);

        CancellationTokenSource _load_cancellation = null;
        string _file_path { get; set; }
        int _max_width;
        int _max_height;
        int _target_fps;
        public bool IsCard { get; private set; }

        public void Init(string filePath, bool is_card, int target_fps = int.MaxValue, int max_width = int.MaxValue, int max_height = int.MaxValue)
        {
            _file_path = filePath;
            _target_fps = target_fps;
            _max_width = max_width;
            _max_height = max_height;
            IsCard = is_card;
        }

        public static void SetLoadLimit(int limit)
        {
            _load_limit = new SemaphoreSlim(limit);
        }

        public bool IsLoading()
        {
            return _is_loading;
        }

        public bool IsLoaded()
        {
            return _is_loaded;
        }

        public void SubscribeRawImage(RawImage raw_image)
        {
            if (!_subscribers.Contains(raw_image))
            {
                _subscribers.Add(raw_image);
            }

            if (_frames.Count > 0)
            {
                raw_image.texture = _frames[_current_frame];
                raw_image.enabled = true;
            }

        }

        public void UnsubscribeRawImage(RawImage raw_image)
        {
            raw_image.enabled = false;
            int old_count = _subscribers.Count;
            _subscribers.Remove(raw_image);

            if (AssetManager.Instance.ShouldUnloadAnimtedImage(this, false))
            {
                Unload();
            }
        }

        public bool InUse()
        {
            return _subscribers.Count > 0;
        }

        public void Update()
        {
            if (!_new_frame_added.IsSet)
            {
                lock (_new_frame_pixels_lock)
                {
                    if (_new_frame_pixels != null)
                    {
                        AddFrame(_width, _height, _new_frame_pixels);
                        _new_frame_added.Set();
                    }
                }
            }

            if (_frames.Count == 0 || _subscribers.Count == 0)
            {
                return;
            }

            _current_frame_time -= Time.deltaTime;

            if (_current_frame_time >= 0f)
            {
                return;
            }

            int next_frame = _current_frame;

            while (_current_frame_time <= 0.0)
            {
                next_frame = (next_frame + 1) % _frames.Count;
                lock (_frame_durations)
                {
                    _current_frame_time += _frame_durations[_current_frame];
                }
            }

            if (next_frame != _current_frame)
            {
                _current_frame = next_frame;
                foreach (var item in _subscribers)
                {
                    item.texture = _frames[next_frame];
                }
            }
        }

        public void Unload()
        {
            //MelonLogger.Msg($"Unloading: {FilePath}");

            _load_cancellation?.Cancel();

            foreach (var frame in _frames)
            {
                Destroy(frame);
            }

            _frames.Clear();

            lock (_frame_durations)
            {
                _frame_durations.Clear();
            }

            _is_loading = false;
            _is_loaded = false;
            _current_frame = 0;
            _current_frame_time = 0;
            _width = 0;
            _height = 0;
            _new_frame_added.Set();

            foreach (var item in _subscribers)
            {
                item.enabled = false;
            }
        }

        public Task LoadAsync()
        {
            //MelonLogger.Msg($"Loading: {FilePath}");

            if (_is_loading || _is_loaded)
            {
                return null;
            }

            _is_loading = true;

            _load_cancellation = new CancellationTokenSource();
            CancellationToken token = _load_cancellation.Token;

            return Task.Run(() =>
            {
                try
                {
                    _load_limit.Wait(token);
                }
                catch (Exception)
                {
                    return;
                }

                try
                {
                    token.ThrowIfCancellationRequested();

                    using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load<Rgba32>(_file_path))
                    {
                        _width = Math.Min(_max_width, image.Width);
                        _height = Math.Min(_max_height, image.Height);

                        image.Mutate(x => x.Resize(_width, _height, KnownResamplers.Bicubic));

                        token.ThrowIfCancellationRequested();

                        lock (_new_frame_pixels_lock)
                        {
                            _new_frame_pixels = new Color32[_width * _height];
                        }

                        if (image.Frames.Count > 1)
                        {
                            float max_frame_duration = 1f / _target_fps;
                            float accumulatedTime = 0f;
                            for (int i = 0; i < image.Frames.Count; i++)
                            {
                                token.ThrowIfCancellationRequested();

                                var frame = image.Frames[i];
                                var metadata = frame.Metadata.GetWebpMetadata();
                                float duration = metadata.FrameDelay / 1000f;

                                if (max_frame_duration > duration)
                                {
                                    accumulatedTime += duration;
                                    if (accumulatedTime >= max_frame_duration || i == image.Frames.Count - 1)
                                    {
                                        accumulatedTime -= max_frame_duration;
                                        duration = max_frame_duration;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }

                                lock (_frame_durations)
                                {
                                    _frame_durations.Add(duration);
                                }

                                FillCurrentTexturePixels(frame, _width, _height);

                                _new_frame_added.Reset();
                                _new_frame_added.Wait(token);

                            }
                        }
                        else
                        {
                            lock (_frame_durations)
                            {
                                _frame_durations.Add(0);
                            }


                            FillCurrentTexturePixels(image.Frames[0], _width, _height);
                            _new_frame_added.Reset();
                        }

                        _is_loaded = true;
                    }
                }
                catch (Exception)
                {
                    _is_loaded = false;
                }
                finally
                {
                    lock (_new_frame_pixels_lock)
                    {
                        _new_frame_pixels = null;
                    }

                    _is_loading = false;
                    _load_limit.Release();

                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    System.GC.Collect();
                }
            }, token);
        }

        void FillCurrentTexturePixels(ImageFrame<Rgba32> image_frame, int width, int height)
        {
            lock (_new_frame_pixels_lock)
            {
                if (_new_frame_pixels == null || _new_frame_pixels.Length != width * height)
                {
                    return;
                }

                image_frame.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < height; y++)
                    {
                        var row = accessor.GetRowSpan(y);
                        int unity_y = height - 1 - y;

                        for (int x = 0; x < width; x++)
                        {
                            Rgba32 pixel = row[x];
                            _new_frame_pixels[unity_y * width + x] = new Color32(
                                pixel.R, pixel.G, pixel.B, pixel.A
                            );
                        }
                    }
                });
            }
        }

        public void AddFrame(int width, int height, Color32[] pixels)
        {
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, true);
            texture.hideFlags = HideFlags.DontUnloadUnusedAsset;
            texture.filterMode = FilterMode.Bilinear;

            texture.SetPixels32(pixels);
            texture.Apply(true, true);

            if (_frames.Count == 0)
            {
                foreach (var item in _subscribers)
                {
                    item.enabled = true;
                    item.texture = texture;
                }
            }

            _frames.Add(texture);
        }
    }
}
