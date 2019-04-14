using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{
    public class ThrowableItem : ConsumableItem
    {

        protected bool iniciando;
        protected bool finalizando;
        protected bool throwed;




        public ThrowableStats getThrowableStats()
        {
            if (Stats is ThrowableStats)
                return Stats as ThrowableStats;
            ThrowableStats defect = new ThrowableStats();
            Stats = defect;
            return defect;
        }


        private void FixedUpdate()
        {
            if (iniciando && finalizando)
            {
                ThrowableStats stats = getThrowableStats();

                usingChar.anim.Play(stats.animation + ".Throw", usingChar.anim.GetLayerIndex(stats.animLayer));

                deltaTime += Time.deltaTime;

                if (!throwed && stats.throwDelayTime < deltaTime)
                {
                    Throw();
                    throwed = true;
                    deltaTime = 0;
                }else if (throwed && stats.animDelayTime < deltaTime)
                {
                    Finish(false);
                }
            }


            finalizando = true;

        }

        public void Throw()
        {

            ThrowableStats stats = getThrowableStats();
            ThrowableController granade = Instantiate<ThrowableController>(stats.prefab, usingChar.throwOrigin.position, usingChar.throwOrigin.rotation);

            granade.Initilize(usingChar.throwOrigin.position, usingChar.throwOrigin.rotation,  stats.Area, stats.damageArea, usingChar.Stats.throwForce, stats.maxDamage, stats.explosionDelayTime, usingChar.photonView.IsMine);
        }


        public override void Use(CharController character)
        {

            ThrowableStats stats = getThrowableStats();
            iniciando = true;
            finalizando = false;

            if ((Units - stats.ConsumeRate) >= 0)
            {
                usingChar = character;
                int index = usingChar.anim.GetLayerIndex(stats.animLayer);
                usingChar.anim.Play(stats.animation + ".Prepare", index);

                if (stats.ShowOnUse)
                {
                    usingChar.ik.DisableLeftHand = stats.DisableLeftHand;
                    usingChar.ik.DisableRightHand = stats.DisableRightHands;
                    this.Show();
                }

            }

        }


        public override float GetActualUseState()
        {
            return 0f;
        }

        public override void Finish(bool interrupt)
        {
            iniciando = false;
            finalizando = false;
            throwed = false;
            usingChar.states.Throwing = false;
            base.Finish(interrupt);
        }

    }
}
