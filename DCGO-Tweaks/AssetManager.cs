using Harmony;
using Il2Cpp;
using Il2CppInterop.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace DCGO_Tweaks
{
    internal class AssetManager
    {
        private static AssetManager _instance;
        public static AssetManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AssetManager();
                }

                return _instance;
            }
        }

        private AssetManager()
        {
            InitBackgroundData();
        }

        #region Backgrounds
        static List<int> _recent_backgrounds = new List<int>();
        static int _background_ring_index = -1;

        DirectoryInfo _background_directory;

        void InitBackgroundData()
        {
            string path = Application.dataPath;
            _background_directory = new DirectoryInfo(path);
            _background_directory = new DirectoryInfo(_background_directory.Parent.Parent.FullName + "/Assets/Textures/Backgrounds");

            _recent_backgrounds = new List<int>();

            for (int i = 0; i < Settings.Instance.BackgroundNoneRepeatingListSize(); i++)
            {
                _recent_backgrounds.Add(-1);
            }
        }

        public Sprite RandomBackground()
        {
            if (_background_directory == null)
            {
                return null;
            }

            string[] files = Directory.GetFiles(_background_directory.FullName, "*.png", SearchOption.TopDirectoryOnly);

            if (files.Length == 0)
            {
                return null;
            }

            if (_recent_backgrounds.Count >= files.Length)
            {
                _recent_backgrounds = new List<int>();

                for (int i = 0; i < Math.Min(files.Length - 1, Settings.Instance.BackgroundNoneRepeatingListSize()); i++)
                {
                    _recent_backgrounds.Add(-1);
                }
            }

            List<int> all_indexs = new List<int>();
            
            for (int i = 0; i < files.Length; i++)
            {
                all_indexs.Add(i);
            }

            List<int> indexs = new List<int>(all_indexs);

            foreach (int recent_index in _recent_backgrounds) 
            {
                indexs.Remove(recent_index);
            }

            if (indexs.Count == 0)
            {
                indexs = all_indexs;
            }

            System.Random random = new System.Random();
            int random_index = indexs[random.Next(indexs.Count)];

            _background_ring_index++;
            _recent_backgrounds[_background_ring_index % _recent_backgrounds.Count] = random_index;

            byte[] bytes = File.ReadAllBytes(files[random_index]);
            Texture2D tex = new Texture2D(1920, 1080);
            tex.LoadImage(bytes);

            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
        #endregion

        #region Scene Sprites

        Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();
        public void LoadSceneSprites()
        {
            _sprites.Clear();

            var sprites = Resources.FindObjectsOfTypeAll(Il2CppType.Of<Sprite>());
            foreach (UnityEngine.Object @object in sprites)
            {
                Sprite sprite = @object.TryCast<Sprite>();
                if (@object.name.Length > 0 && !_sprites.ContainsKey(@object.name) && sprite != null)
                {
                    _sprites.Add(@object.name, sprite);
                }
            }
        }

        public Sprite GetSceneSprite(string name)
        {
            if (!_sprites.ContainsKey(name))
            {
                return null;
            }

            return _sprites[name];
        }
        #endregion

        #region Animated Images

        private struct AnimatedImageEntry
        {
            public AnimatedImageData Image;
            public FileInfo FileInfo;
        }

        private Dictionary<string, AnimatedImageEntry> _animated_image_look_up = new Dictionary<string, AnimatedImageEntry>();

        DirectoryInfo _animated_image_directory;

        public void FindAnimatedImages()
        {
            string path = Application.dataPath;
            _animated_image_directory = new DirectoryInfo(path);
            _animated_image_directory = new DirectoryInfo(_animated_image_directory.Parent.Parent.FullName + "/Assets/Textures/Animated");

            foreach (var file in _animated_image_directory.GetFiles())
            {
                if (file.Extension == ".webp")
                {
                    _animated_image_look_up.Add(System.IO.Path.GetFileNameWithoutExtension(file.Name), new AnimatedImageEntry { Image = null, FileInfo = file });
                }
            }
        }

        // Hack until I can figure out why Sprite are nuked when scene changes
        public void CleanUpAnimatedImages()
        {
            foreach (var item in _animated_image_look_up)
            {
                AnimatedImageEntry entry = item.Value;
                entry.Image = null;
                _animated_image_look_up[item.Key] = entry;
            }
        }

        public void PreLoadImage(string image_name)
        {
            _ = GetAnimatedImage(image_name, true);
        }

        public bool HasAnimatedImage(string image_name)
        {
            return _animated_image_look_up.ContainsKey(image_name);
        }

        public CEntity_Base GetEntityFromCardIndex(CardSource card_source)
        {
            if (card_source == null)
            {
                return null;
            }

            // TODO: Find a better way to do this, getCardEntityByCardID looks slow but the index doesnt match any of the internal lists
            return ContinuousController.instance.getCardEntityByCardID(card_source.CardEntityIndex);
        }

        public AnimatedImageData GetAnimatedImage(string image_name, bool load = true)
        {
            if (!HasAnimatedImage(image_name))
            {
                return null;
            }

            AnimatedImageEntry _animated_image_data = _animated_image_look_up[image_name];

            if (_animated_image_data.Image == null)
            {
                _animated_image_data.Image = new DCGO_Tweaks.AnimatedImageData();
                _animated_image_look_up[image_name] = _animated_image_data;

                if (load)
                {
                    _animated_image_data.Image.AsyncLoad(_animated_image_data.FileInfo);
                }
            }

            return _animated_image_data.Image;
        }
        #endregion
    }
}
