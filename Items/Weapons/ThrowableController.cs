using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{
    public class ThrowableController : BulletController
    {
        private bool Area;
        private float damageArea;


        public ParticleSystem particles;


        public void Initilize(Vector3 pos, Quaternion rotation, bool Area, float damageArea, float power, float damage, float lifeTime, bool is_Mine)
        {
            this.Area = Area;
            this.damageArea = damageArea;
            
            base.Initilize(pos, rotation, power, damage, lifeTime, is_Mine);
        }


        private void FixedUpdate()
        {
            deltatime += Time.deltaTime;

            if (Area)
            {

                if( deltatime >= lifeTime)
                {
                    Explosion();
                    Destroy(this.gameObject);
                }
            }
            else
            {
                detectCollision();
                if (deltatime >= lifeTime)
                {
                    Destroy(this.gameObject);
                }
            }


        }

        public void Explosion()
        {
            Vector3 pos = this.mtransform.position;
            ParticleSystem explosion = Instantiate<ParticleSystem>(particles, pos, Quaternion.identity);

            Collider[] checking = Physics.OverlapSphere(pos, this.damageArea, ~hitboxMask);

            if (checking.Length > 0)
            {
                foreach (Collider c in checking)
                {
                    GameObject go = c.gameObject;
                    if (go.layer == hitboxMask)
                    {
                        BodyPart bp = go.GetComponent<BodyPart>();
                        if (bp != null)
                        {
                            Vector3 collisionPos = c.ClosestPoint(pos);
                            float distance = Vector3.Distance(pos, collisionPos);

                            float damageDisminution = distance / damageArea;

                            float finalDamage = damage - damage * damageDisminution;

                            bp.TakeHit(finalDamage);

                        }

                    }

                }
            }

        }
    }
}
