using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{
    [CreateAssetMenu(fileName = "stats", menuName = "tutoriales/char stats")]
    public class CharStats : ScriptableObject
    {
        public float speed = 200;
        public float runningSpeedIncrement = 1.5f;
        public float rotationSpeed = 25;
        public float jumpForce = 200;
        public float minAngle = -70;
        public float maxAngle = 70;
        public float cameraSpeed = 24;

        public float throwForce = 50;
        public float maxFallSpeed = 50;
        public float Weight = 20f;

        public float camRadius = 0.25f;

        public float crouchHeightOffset = -.25f;
        public float crouchPosOffset = -.25f;


        public float MaxLife = 100;


        public List<MeleAttack> unarmedAttacks;

    }

    [System.Serializable]
    public class MeleAttack
    {
        public string MeleUnnarmedLayer;
        public string MeleUnnarmedState;
        public float shootTime;
        public float damage;
        public float range;
        public float area;
    }
}
