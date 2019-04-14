using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace tutoriales
{

    [CreateAssetMenu(fileName = "throwableStats", menuName = "tutoriales/throwable stats")]
    public class ThrowableStats : ConsumableStats
    {

        public bool Area;
        public float maxDamage;
        public float damageArea;

        public float throwDelayTime;
        public float animDelayTime;
        public float explosionDelayTime;

        public ThrowableController prefab;

    }
}
