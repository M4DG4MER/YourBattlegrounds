using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace tutoriales
{
    public class SceneScrollView : MonoBehaviour
    {
        public InventaryGroupViewer viewer;
        VerticalLayoutGroup vr;
        private Scrollbar scrollbar;
        public float MaxSize;
        private float despl;

        public void Initialize()
        {
            vr = viewer.GetComponent<VerticalLayoutGroup>();
            scrollbar = this.GetComponent<Scrollbar>();
            scrollbar.size = 1f / MaxSize;
        }



        public void ScrollChanged()
        {
            despl = scrollbar.value * MaxSize;
            vr.padding.top = -(int)despl;
            vr.SetLayoutVertical();
        }


    }
}
