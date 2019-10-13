using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace tutoriales.Sounds
{
    public interface SoundCollectionManager<T>
    {

        SoundCollection Get(T key);

        SoundCollection Get(string key);

        bool Play(T key, AudioSource s, float d);

        bool Play(string key, AudioSource s, float d);
    }

    public class KeyCollection<T>
    {
        public T Name;
        public SoundCollection Collection;
    }
}

