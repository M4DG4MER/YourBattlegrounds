using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace tutoriales.Sounds
{
    [CreateAssetMenu(fileName = "Sound Collection Manager", menuName = "tutoriales/Sounds/Collection Manager", order = 1)]
    public class TextureMaterialCollection : ScriptableObject, SoundCollectionManager<TextureEvent>
    {
        [Range(0f, 1f)]
        public float GeneralVolume = 1;

        public List<TextureCollection> managedCollections;


        public SoundCollection Get(TextureEvent key)
        {
            return Get(key.Name);
        }

        public SoundCollection Get(string key)
        {
            return managedCollections.FirstOrDefault(c => c.Name.Name == key)?.Collection;
        }

        public bool Play(TextureEvent key, AudioSource s, float d)
        {
            return Play(key.Name, s, d);
        }

        public bool Play(string key, AudioSource s, float d)
        {
            return Get(key)?.PlayRandom(s, GeneralVolume, d) ?? false;
        }
    }

    [System.Serializable]
    public class TextureCollection : KeyCollection<TextureEvent>
    { }

    [System.Serializable]
    public class TextureEvent
    {
        public Texture _Texture;
        public SoundEvent EventType;

        public string Name {  get { return EventName() + "_" + TextureName(); } }

        public string TextureName() { return _Texture?.name ?? "texture"; }
        public string EventName() { return EventType?.name ?? "event"; }

    }
}