using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales.Sounds
{
    [System.Serializable]
    public class RandomSound
    {
        public string Name;

        public SoundCollection Collection;

        public bool PlaySound(AudioSource AudioSource, float d = float.MaxValue)
        {
            if (AudioSource == null || Collection == null)
            {
                Debug.LogWarning("Audio Source or Collection is no estan configurados");
                return false;
            }

            return Collection.PlayRandom(AudioSource, 1, d);
        }
    }
}
