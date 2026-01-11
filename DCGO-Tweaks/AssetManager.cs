using Il2CppInterop.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
    }
}
