using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace tutoriales
{
    public class StatsViewer : MonoBehaviour
    {
        static private StatsViewer viewer;
        static public StatsViewer Viewer()
        {
            if (viewer == null)
            {
                viewer = FindObjectOfType<StatsViewer>();
            }
            return viewer;
        }

        public  Slider[] sliders;


        private void OnEnable()
        {
            sliders = GetComponentsInChildren<Slider>();
        }

        public void Add(Stat stat)
        {
            stat.StatChanged += new StatEvent(OnStatChange);
            OnStatChange(stat);
        }

        public void OnStatChange(Stat stat)
        {
            foreach (Slider sl in sliders)
            {
                if (sl.name.Contains(stat.Name))
                {
                    sl.value = stat.Percentage();
                }
            }
        }



    }
}