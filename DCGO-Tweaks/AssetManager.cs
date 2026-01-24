using Il2Cpp;
using Il2CppInterop.Runtime;
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
            LoadBackgroundData();
        }

        #region UI Assets
        DirectoryInfo _ui_directory;
        public Sprite UnitFrame { get; private set; }
        public Sprite UnitFrameMaskLeft { get; private set; }
        public Sprite UnitFrameMaskRight { get; private set; }

        public Sprite UnitFrameStats { get; private set; }

        public Sprite CostCircle { get; private set; }

        public Sprite CostCircleRotated { get; private set; }

        public Sprite LinkIcon { get; private set; }

        public Sprite CardMask { get; private set; }
        public Shader AnimatedImageShader { get; private set; }

        public void LoadUIAssets()
        {
            string path = Application.dataPath;
            _ui_directory = new DirectoryInfo(path);
            _ui_directory = new DirectoryInfo(_ui_directory.Parent.Parent.FullName + "/Assets/Textures/UI");

            UnitFrame = GetSpriteFromFile(Path.Combine(_ui_directory.FullName, "UnitFrame_Base.png"), true);
            UnitFrameMaskLeft = GetSpriteFromFile(Path.Combine(_ui_directory.FullName, "UnitFrame_Mask_Left.png"), true);
            UnitFrameMaskRight = GetSpriteFromFile(Path.Combine(_ui_directory.FullName, "UnitFrame_Mask_Right.png"), true);

            UnitFrameStats = GetSpriteFromFile(Path.Combine(_ui_directory.FullName, "UnitFrame_Stats.png"), true);

            CostCircle = GetSpriteFromFile(Path.Combine(_ui_directory.FullName, "Cost_Circle.png"), true);
            CostCircleRotated = GetSpriteFromFile(Path.Combine(_ui_directory.FullName, "Cost_Circle_Rotated.png"), true);
            LinkIcon = GetSpriteFromFile(Path.Combine(_ui_directory.FullName, "LinkIcon.png"), true);
            CardMask = GetSpriteFromFile(Path.Combine(_ui_directory.FullName, "CardMask.png"), true);
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
            Settings settings = Settings.Instance;

            DataBase.CardColor_ColorLightDictionary[CardColor.Red] = settings.RedColor();
            DataBase.CardColor_ColorLightDictionary[CardColor.Blue] = settings.BlueColor();
            DataBase.CardColor_ColorLightDictionary[CardColor.Yellow] = settings.YellowColor();
            DataBase.CardColor_ColorLightDictionary[CardColor.Green] = settings.GreenColor();
            DataBase.CardColor_ColorLightDictionary[CardColor.Black] = settings.BlackColor();
            DataBase.CardColor_ColorLightDictionary[CardColor.Purple] = settings.PurpleColor();
            DataBase.CardColor_ColorLightDictionary[CardColor.White] = settings.WhiteColor();

            DataBase.CardColor_ColorDarkDictionary[CardColor.Red] = settings.RedColor();
            DataBase.CardColor_ColorDarkDictionary[CardColor.Blue] = settings.BlueColor();
            DataBase.CardColor_ColorDarkDictionary[CardColor.Yellow] = settings.YellowColor();
            DataBase.CardColor_ColorDarkDictionary[CardColor.Green] = settings.GreenColor();
            DataBase.CardColor_ColorDarkDictionary[CardColor.Black] = settings.BlackColor();
            DataBase.CardColor_ColorDarkDictionary[CardColor.Purple] = settings.PurpleColor();
            DataBase.CardColor_ColorDarkDictionary[CardColor.White] = settings.WhiteColor();
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

        #region Animated Images


        struct AnimatedImageEntry
        {
            public AnimatedImage Image;
            public float LastAccessTime;
        }

        private Dictionary<string, AnimatedImageEntry> _animated_image_look_up = new Dictionary<string, AnimatedImageEntry>();

        DirectoryInfo _animated_cards_directory;
        DirectoryInfo _animated_ui_directory;
        GameObject _animated_image_holder;

        public void FindAnimatedImages()
        {
            Settings settings = Settings.Instance;

            _animated_image_holder = new GameObject();
            GameObject.DontDestroyOnLoad(_animated_image_holder);

            string path = Application.dataPath;
            _animated_cards_directory = new DirectoryInfo(path);
            _animated_ui_directory = new DirectoryInfo(_animated_cards_directory.Parent.Parent.FullName + "/Assets/Textures/UI/Animated");
            _animated_cards_directory = new DirectoryInfo(_animated_cards_directory.Parent.Parent.FullName + "/Assets/Textures/Card/Animated");

            AnimatedImage.SetLoadLimit(settings.AnimatedCardsLoadingThreads());

            foreach (var file in _animated_cards_directory.GetFiles())
            {
                if (file.Extension == ".webp" || file.Extension == ".gif")
                {
                    string key = System.IO.Path.GetFileNameWithoutExtension(file.Name);
                    if (!_animated_image_look_up.ContainsKey(key))
                    {
                        AnimatedImage image = _animated_image_holder.AddComponent<AnimatedImage>();

                        int width = (int)(430 * settings.AimatedCardsRenderScale());
                        int height = (int)(600 * settings.AimatedCardsRenderScale());

                        image.Init(file.FullName, true, settings.AnimatedCardsMaxFps(), width, height);
                        _animated_image_look_up.Add(key, new AnimatedImageEntry() { Image = image, LastAccessTime = 0.0f});
                    }
                }
            }
        }

        public void CleanUpAnimatedImages(bool inculde_in_use = true)
        {
            foreach (var item in _animated_image_look_up)
            {
                if ((item.Value.Image.IsLoaded() || item.Value.Image.IsLoading()) && ShouldUnloadAnimtedImage(item.Value.Image, inculde_in_use))
                {
                    item.Value.Image.Unload();
                }
            }
        }

        public bool ShouldUnloadAnimtedImage(AnimatedImage image, bool inculde_in_use = true)
        {
            if ((!inculde_in_use && image.InUse()) || !image.IsCard)
            {
                return false;
            }

            List<AnimatedImageEntry> list = _animated_image_look_up.Values.ToList();

            // Annoying, cant use .Sort so do it myself
            for (int i = 0; i < list.Count - 1; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    if (list[i].LastAccessTime < list[j].LastAccessTime)
                    {
                        AnimatedImageEntry temp = list[i];
                        list[i] = list[j];
                        list[j] = temp;
                    }
                }
            }

            int target = Settings.Instance.AnimatedCardsCacheSize();

            for (int i = list.Count - 1; i >= target; i--)
            {
                if (list[i].Image == image)
                {
                    return true;
                }
            }

            return false;
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

        public AnimatedImage GetAnimatedImage(string image_name, bool load = true)
        {
            if (!HasAnimatedImage(image_name))
            {
                return null;
            }

            AnimatedImageEntry _animated_image = _animated_image_look_up[image_name];

            _animated_image.LastAccessTime = Time.time;

            if (!_animated_image.Image.IsLoading() && !_animated_image.Image.IsLoaded())
            {
                _animated_image.Image.LoadAsync();
            }

            _animated_image_look_up[image_name] = _animated_image;

            CleanUpAnimatedImages(false);

            return _animated_image.Image;
        }
        #endregion
    }
}
