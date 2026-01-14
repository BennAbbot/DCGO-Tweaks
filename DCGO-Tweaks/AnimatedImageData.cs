using DCGO_Tweaks.ModdedComponents;
using MelonLoader;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using UnityEngine;

namespace DCGO_Tweaks
{
    internal class AnimatedImageData
    {
        public struct FrameData
        {
            public Sprite Sprite;
            public float Duration;
        }

        List<FrameData> _frames = new List<FrameData>();
        volatile bool _is_loading = false;
        volatile float _total_duration;

        public Task AsyncLoad(FileInfo file_info)
        {
            return Task.Run(() =>
            {
                Load(file_info.FullName);
            });
        }

        public bool IsLoading()
        {
            return _is_loading;
        }

        public Sprite GetFrameAt(int frame_index)
        {
            lock (_frames)
            {
                if (frame_index >= 0 && frame_index < _frames.Count)
                {
                    return _frames[frame_index].Sprite;
                }
            }

            return null;
        }

        public bool IsAnimated()
        {
            lock (_frames)
            {
                return _frames.Count > 1;
            }
        }

        public int GetFrameIndexFrom(float duration_from, int previous_frame_index = 0)
        {
            lock (_frames)
            {
                if (!IsAnimated())
                {
                    return 0;
                }

                float target_duration = duration_from;

                while (target_duration > _total_duration)
                {
                    target_duration -= _total_duration;
                }

                int index = previous_frame_index;
                float current_duration = _frames[index % _frames.Count].Duration;

                while (target_duration > current_duration)
                {
                    index++;

                    if (IsLoading() && index > _frames.Count - 1)
                    {
                        return _frames.Count - 1;
                    }

                    current_duration += _frames[index % _frames.Count].Duration;
                }

                return index % _frames.Count;
            }
        }

        public void Load(string file_path)
        {
            _is_loading = true;

            FrameData GetFrameData(int width, int hight, Color32[] pixels, float duration)
            {
                Texture2D texture = new Texture2D(width, hight, TextureFormat.RGBA32, true);

                texture.hideFlags = HideFlags.DontUnloadUnusedAsset;

                texture.SetPixels32(pixels);
                texture.Apply();

                Sprite sprtie = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                sprtie.hideFlags = HideFlags.DontUnloadUnusedAsset;

                return new FrameData () { Sprite = sprtie, Duration = duration };
            }

            Color32[] GetPixels(ImageFrame<Rgba32> image_frame)
            {
                Color32[] pixels = new Color32[image_frame.Width * image_frame.Height];

                image_frame.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        var pixel_row = accessor.GetRowSpan(y);
                        for (int x = 0; x < pixel_row.Length; x++)
                        {
                            var pixel = pixel_row[x];
                            int unity_y = accessor.Height - 1 - y;
                            pixels[unity_y * image_frame.Width + x] = new Color32(
                                pixel.R, pixel.G, pixel.B, pixel.A
                            );
                        }
                    }
                });

                return pixels;
            }

            try
            {
                _total_duration = 0.0f;

                byte[] fileData = File.ReadAllBytes(file_path);

                using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load<Rgba32>(fileData))
                {
                    if (image.Frames.Count > 1)
                    {
                        foreach (var frame in image.Frames)
                        {
                            var metadata = frame.Metadata.GetWebpMetadata();
                            float duration = metadata.FrameDelay / 1000f;

                            _total_duration += duration;

                            int width = frame.Width;
                            int hight = frame.Height;
                            Color32[] pixels = GetPixels(frame);

                            MainThreadDispatcher.Instance.EnqueueImageLoad(new System.Action(() =>
                            {
                                _frames.Add(GetFrameData(width, hight, pixels, duration));
                            }));
                        }
                    }
                    else
                    {
                        int width = image.Frames[0].Width;
                        int hight = image.Frames[0].Height;
                        Color32[] pixels = GetPixels(image.Frames[0]);

                        MainThreadDispatcher.Instance.EnqueueImageLoad(new System.Action(() =>
                        {
                            _frames.Add(GetFrameData(width, hight, pixels, 0.0f));
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to load WebP: {ex.Message}");
            }
            finally
            {
                _is_loading = false;
            }
        }
    }
}
