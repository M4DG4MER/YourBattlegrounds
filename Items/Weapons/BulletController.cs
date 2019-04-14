using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{
    public class BulletController : MonoBehaviour
    {

        protected float lifeTime = 10f;
        protected float damage;
        protected float deltatime = 0f;

        protected Rigidbody rg;

        protected Transform mtransform;
        protected Vector3 lastPosition;
        protected LayerMask hitboxMask;

        public bool IsMine = false;

        private void Awake()
        {
            rg = GetComponent<Rigidbody>();
            mtransform = GetComponent<Transform>();
        }


        public void Initilize(Vector3 pos, Quaternion rotation, float power, float damage, float lifeTime, bool is_Mine)
        {
            mtransform.position = pos;
            mtransform.rotation = rotation;
            rg.velocity = this.transform.forward * power;

            this.damage = damage;
            this.lifeTime = lifeTime;

            hitboxMask = LayerMask.NameToLayer("hitbox");
            lastPosition = mtransform.position;
            this.IsMine = is_Mine;
        }



        private void FixedUpdate()
        {
            deltatime += Time.deltaTime;

            if (IsMine)
            {
                detectCollision();

                if (deltatime >= lifeTime)
                    this.gameObject.SetActive(false);
            }
        }


        protected virtual void detectCollision()
        {
            Vector3 newpos = mtransform.position;

            Vector3 dir = newpos - lastPosition;

            RaycastHit hit;
            
            if (Physics.Raycast(lastPosition, dir.normalized, out hit, dir.magnitude))
            {
                GameObject go = hit.collider.gameObject;

                if (go.layer == hitboxMask)
                {
                    BodyPart bp = go.GetComponent<BodyPart>();
                    if (bp != null)
                    {
                        bp.TakeHit(damage);
                        Debug.Log("Impacto en " + bp.BodyName);
                        this.gameObject.SetActive(false);
                    }
                }
                else
                {
                    HolesPool.Pool.Impact(go.tag, hit.point, hit.normal);
                    this.gameObject.SetActive(false);
                }

            }

            lastPosition = newpos;
        }

    }
}
