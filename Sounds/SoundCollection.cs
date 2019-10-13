using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales.Sounds
{

    [CreateAssetMenu(fileName = "Sound Collection", menuName = "tutoriales/Sounds/Sound Collection", order = 1)]
    public class SoundCollection : ScriptableObject
    {
        public List<AudioClip> Clips = new List<AudioClip>();

        public bool _2D = false;

        public Vector2 PitchRange = new Vector2(1.0f, 1.5f);
        public Vector2 VolumeRange = new Vector2(0.75f, 1f);

        public float SoundDuration;


        public AudioClip GetRandom()
        {
            return Clips[Random.Range(0, Clips.Count)];
        }

        private bool CanPlay(float d)
        {
            return d > SoundDuration;
        }

        public bool PlayRandom(AudioSource AudioSource, float VolumeMaster, float d)
        {
            if (!CanPlay(d))
                return false;

            if (Clips.Count == 0)
                return false;

            AudioClip soundToPlay = GetRandom();

            if (soundToPlay == null)
                return false;

            AudioSource.clip = soundToPlay;
            AudioSource.volume = Random.Range(VolumeRange.x, VolumeRange.y) * VolumeMaster;
            AudioSource.pitch = Random.Range(PitchRange.x, PitchRange.y);

            AudioSource.spatialBlend = (!_2D ? 1f : 0f);
            AudioSource.Play();

            return true;
        }
    }
}
