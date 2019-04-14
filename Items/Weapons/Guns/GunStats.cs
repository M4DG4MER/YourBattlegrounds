using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{
    [CreateAssetMenu(fileName = "gunStats", menuName = "tutoriales/gun stats")]
    public class GunStats : WeaponStats
    {
        public BulletController bulletPrefab;

        public float Range = 10f;
        public float MaxRecoil = 1f;
        public float ShootModifier = 2f;

        public float power = 100f;
        public float lifeTime = 10f;

        public string HandsState = "rifle";

        public string ammoName = "";
        public int maxClip = 10;

        public List<FireState> States = new List<FireState>();

    }

    [System.Serializable]
    public class FireState
    {
        public FireMode mode = FireMode.unique;
        public int rate = 1;

        public float FireLapse()
        {
            return 1f / (float)rate;
        }
    }


    public enum FireMode
    {
        unique = 0,
        burst = 1,
        auto = 2
    }
    
}