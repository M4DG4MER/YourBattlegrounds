using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{
    public class MaterialHole : MonoBehaviour
    {
        public AudioClip ImpactClip;

        ParticleSystem particles;
        AudioSource audioSource;
        Transform mtransform;


        // Use this for initialization
        void Start()
        {
            mtransform = this.transform;
            audioSource = GetComponent<AudioSource>();
            particles = GetComponentInChildren<ParticleSystem>();
            this.gameObject.SetActive(false);



        }


        public void Reubicate(Vector3 pos, Vector3 normal)
        {
            audioSource.Stop();
            this.mtransform.position = pos + normal * 0.01f;
            this.mtransform.LookAt(pos + normal);
            this.gameObject.SetActive(true);

            audioSource.PlayOneShot(ImpactClip, GameConfiguration.EffectsLevel);
            particles.gameObject.SetActive(true);
            particles.Play(true);

        }

    }
}