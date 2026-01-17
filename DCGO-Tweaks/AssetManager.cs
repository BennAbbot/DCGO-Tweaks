using Harmony;
using Il2Cpp;
using Il2CppInterop.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DCGO_Tweaks
{
    using CardColorList = Il2CppSystem.Collections.Generic.List<CardColor>;

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
            LoadBackgroundData();
        }

        #region UI Assets
        DirectoryInfo _ui_directory;
        public Sprite DPHolder { get; private set; }
        public Sprite DPHolderMaskLeft { get; private set; }
        public Sprite DPHolderMaskRight { get; private set; }

        public Sprite CostCircle { get; private set; }

        public Sprite CostCircleRotated { get; private set; }

        Color[] _colour_table = new Color[7];
        public void LoadUIAssets()
        {
            string path = Application.dataPath;
            _ui_directory = new DirectoryInfo(path);
            _ui_directory = new DirectoryInfo(_ui_directory.Parent.Parent.FullName + "/Assets/UI");

            DPHolder = GetSpriteFromFile(Path.Combine(_ui_directory.FullName, "UnitFrame_Base.png"), true);
            DPHolderMaskLeft = GetSpriteFromFile(Path.Combine(_ui_directory.FullName, "UnitFrame_Mask_Left.png"), true);
            DPHolderMaskRight = GetSpriteFromFile(Path.Combine(_ui_directory.FullName, "UnitFrame_Mask_Right.png"), true);

            CostCircle = GetSpriteFromFile(Path.Combine(_ui_directory.FullName, "Cost_Circle.png"), true);
            CostCircleRotated = GetSpriteFromFile(Path.Combine(_ui_directory.FullName, "Cost_Circle_Rotated.png"), true);
        }

        public Texture2D GetTextureFromFile(string path)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                texture.LoadImage(bytes);
                return texture;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Sprite GetSpriteFromFile(string path, bool never_unload)
        {
            Sprite sprite = null;
            Texture2D texture = GetTextureFromFile(path);
            if (texture != null)
            {
                sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                if (never_unload)
                {
                    texture.hideFlags = HideFlags.DontUnloadUnusedAsset;
                    sprite.hideFlags = HideFlags.DontUnloadUnusedAsset;
                }
            }

            return sprite;
        }

        #endregion

        #region Colours
        public void InitColours()
        {
            DataBase.CardColor_ColorLightDictionary[CardColor.Red] = new Color(230f / 255f, 0, 46f / 255f);
            DataBase.CardColor_ColorLightDictionary[CardColor.Blue] = new Color(0, 151f / 255f, 224f / 255f);
            DataBase.CardColor_ColorLightDictionary[CardColor.Yellow] = new Color(254f / 255f, 225f / 255f, 1f / 255f);
            DataBase.CardColor_ColorLightDictionary[CardColor.Green] = new Color(0, 155f / 255f, 107f / 255f);
            DataBase.CardColor_ColorLightDictionary[CardColor.Black] = new Color(34f / 255f, 22f / 255f, 22f / 255f);
            DataBase.CardColor_ColorLightDictionary[CardColor.Purple] = new Color(100f / 255f, 86f / 255f, 163f / 255f);
            DataBase.CardColor_ColorLightDictionary[CardColor.White] = new Color(1, 1, 1);

            DataBase.CardColor_ColorDarkDictionary[CardColor.Red] = new Color(230f / 255f, 0, 46f / 255f);
            DataBase.CardColor_ColorDarkDictionary[CardColor.Blue] = new Color(0, 151f / 255f, 224f / 255f);
            DataBase.CardColor_ColorDarkDictionary[CardColor.Yellow] = new Color(254f / 255f, 225f / 255f, 1f / 255f);
            DataBase.CardColor_ColorDarkDictionary[CardColor.Green] = new Color(0, 155f / 255f, 107f / 255f);
            DataBase.CardColor_ColorDarkDictionary[CardColor.Black] = new Color(34f / 255f, 22f / 255f, 22f / 255f);
            DataBase.CardColor_ColorDarkDictionary[CardColor.Purple] = new Color(100f / 255f, 86f / 255f, 163f / 255f);
            DataBase.CardColor_ColorDarkDictionary[CardColor.White] = new Color(1, 1, 1);
        }
        #endregion

        #region Backgrounds
        static List<int> _recent_backgrounds = new List<int>();
        static int _background_ring_index = -1;

        DirectoryInfo _background_directory;

        void LoadBackgroundData()
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
