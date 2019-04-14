using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{
    public class BodyMaskManager : MonoBehaviour
    {

        SkinnedMeshRenderer mesh;

        private Dictionary<string, BodyMask> Masks;
        public List<BodyMask> bMasks;

        private void OnEnable()
        {
            Init();
        }

        void Init()
        {
            if (mesh == null)
            {
                mesh = GetComponent<SkinnedMeshRenderer>();
                Masks = new Dictionary<string, BodyMask>();
                bMasks = new List<BodyMask>();
                Material[] materials = mesh.materials;
                foreach(Material m in materials)
                {
                    BodyMask bm = new BodyMask(m);
                    Masks.Add(bm.MaskName, bm);
                    bMasks.Add(bm);
                }
            }
        }


        public bool Show(string name)
        {
            return ShowHide(name, true);
        }
        public bool Hide(string name)
        {
            return ShowHide(name, false);
        }

        private bool ShowHide(string name, bool state)
        {
            if (!Masks.ContainsKey(name))
                return false;

            if (state)
                Masks[name].Show();
            else
                Masks[name].Hide();

            return true;
        }



    }

    [System.Serializable]
    public class BodyMask
    {
        public string MaskName;

        Material m;
        Color c;
        public bool visible = true;

        public BodyMask(Material m)
        {
            this.m = m;
            this.MaskName = m.name.Substring(0, m.name.IndexOf(' '));
            this.c = m.color;
        }

        public void Hide()
        {
            c.a = 0;
            m.color = c;
        }
        public void Show()
        {
            c.a = 1;
            m.color = c;
        }



    }



}
