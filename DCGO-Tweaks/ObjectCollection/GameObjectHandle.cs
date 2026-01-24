using Il2CppSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DCGO_Tweaks
{
    public class GameObjectHandle
    {
        string _path = null;
        GameObjectHandle _parent = null;
        private GameObject _gameObject = null;
        public GameObject GameObject
        {
            get 
            {
                if (_gameObject == null)
                {
                    _gameObject = FindObject(_path, _parent?.GameObject);
                }

                return _gameObject; 
            }
        }

        public GameObjectHandle(string path, GameObjectHandle parent = null)
        {
            _path = path;
            _parent = parent;
        }

        public GameObjectHandle(string path, GameObject parent)
        {
            _gameObject = FindObject(path, parent);
        }

        public GameObjectHandle(GameObject gameObject)
        {
            _gameObject = gameObject;
        }

        public void Unload()
        {
            _gameObject = null;
        }

        public static GameObject FindObject(string path, GameObjectHandle parent)
        {
            if (parent == null || parent.GameObject == null)
            {
                return null;
            }

            return FindObject(path, parent.GameObject);
        }

        public static GameObject FindObject(string path, GameObject parent = null)
        {
            string[] objects = path.Split('.');

            GameObject target = parent != null ? parent : null;

            foreach (string obj in objects)
            {
                if (target == null)
                {
                    target = GameObject.Find(obj);
                }
                else
                {
                    Transform child = target.transform.FindChild(obj);
                    target = child != null ? child.gameObject : null;
                }

                if (target == null)
                {
                    break;
                }
            }

            return target;
        }

        public T GetComponent<T>() where T : Component
        {
            if (GameObject == null)
            {
                return null;
            }

            return GameObject.GetComponent<T>();
        }

        public void SetActive(bool enbaled = true)
        {
            if (GameObject == null)
            {
                return;
            }

            GameObject.SetActive(enbaled);
        }

        public GameObject Child(string path)
        {
            if (GameObject == null)
            {
                return null;
            }

            return FindObject(path, GameObject);
        }

        public void ForEachChild(System.Action<GameObject> func)
        {
            if (GameObject == null)
            {
                return;
            }

            for (int i = 0; i < GameObject.transform.childCount; i++)
            {
                func(GameObject.transform.GetChild(i).gameObject);
            }
        }

        public void ForEachChild(string child_path, System.Action<GameObject> func)
        {
            GameObject target = FindObject(child_path, this);

            if (target == null)
            {
                return;
            }

            for (int i = 0; i < target.transform.childCount; i++)
            {
                func(target.transform.GetChild(i).gameObject);
            }
        }
    }
}
