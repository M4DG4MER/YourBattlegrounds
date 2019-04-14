using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{
    [System.Serializable]
    public class Stat
    {
        public string Name { get; private set; }

        private float max;
        private float value;

        public float Value
        {
            get { return value; }
            set
            {
                this.value = value;
                if (StatChanged != null)
                    StatChanged(this);
            }
        }


        public float Max
        {
            get { return max; }
        }


        public float Percentage()
        {
            return value / max;
        }

        public Stat(string name, float max, float value)
        {
            this.Name = name;
            this.max = max;
            this.value = value;
        }

        public StatEvent StatChanged;

    }



    public delegate void StatEvent(Stat stat);

}
