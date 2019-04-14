using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{
    public class GunController : WeaponController
    {

        Transform crosshair;

        public Transform ModelRoot;

        public Transform shootPoint;
        public Transform leftHandPosition;
        public Transform leftElbowPosition;

        public BulletController bulletPrefab;


        private bool showCrosshair = false;



        public AudioClip shotNoise;
        private AudioSource shotSource;
        private ParticleSystem muzzle;



        private Quaternion defaultModelRotation;
        private Vector3 defaultModelPosition;

        private int ShootedBullets = 0;
        private float deltaTime = 0;
        public int ActualAmmo = 0;

        int fState = 0;
        public FireState FireState
        {
            get
            {
                return (getGunStats().States.Count > fState ?  getGunStats().States[fState] : new FireState());
            }
        }
        public void NextFireState()
        {
            int state = fState + 1;
            fState = (state >= getGunStats().States.Count ? 0 : state);
            ShootedBullets = 0;
        }


        internal ConsumableItem reloading;
        Vector3 recoilTarget;

        

        protected override void Initialize()
        {
            crosshair = GetComponentInChildren<Canvas>().transform;
            crosshair.gameObject.SetActive(false);

            muzzle = shootPoint.GetComponentInChildren<ParticleSystem>();
            muzzle.gameObject.SetActive(false);
            muzzle.Stop();
            shotSource = shootPoint.GetComponent<AudioSource>();


            defaultModelPosition = ModelRoot.localPosition;
            defaultModelRotation = ModelRoot.localRotation;


            ActualAmmo = getGunStats().maxClip;
            
        }

        public GunStats getGunStats()
        {
            if (Stats is GunStats)
                return Stats as GunStats;
            GunStats defect = new GunStats();
            Stats = defect;
            return defect;
        }


        private void FixedUpdate()
        {
            deltaTime += Time.deltaTime;
            if (reloading != null && reloading.GetActualUseState() >= 0.99)
            {
                reloading.usingChar.ik.DisableLeftHand = !reloading.getConsumableStats().DisableLeftHand;
                reloading.usingChar.ik.DisableRightHand = !reloading.getConsumableStats().DisableRightHands;
                reloading = null;
            }
        }

        public override void Show()
        {
            base.Show();

            if (!showCrosshair)
            {
                ShowCrosshair();
            }
        }
        public override void Hide()
        {
            base.Hide();

            if (showCrosshair)
            {
                ShowCrosshair();
            }
        }


        public override bool Attack(CharController character)
        {
            if (ActualAmmo <= 0 || reloading != null)
            {
                return false;
            }

            if (deltaTime > FireState.FireLapse() || ShootedBullets == 0)
            {
                deltaTime = 0;
                switch (FireState.mode)
                {
                    case FireMode.auto:
                        Shoot(character);
                        return true;
                    case FireMode.burst:
                        Shoot(character);
                        if (ShootedBullets >= FireState.rate)
                        {
                            ShootedBullets = 0;
                            return false;
                        }
                        return true;
                    case FireMode.unique:
                        if (ShootedBullets == 0)
                        {
                            Shoot(character);
                            ShootedBullets = 0;
                        }
                        return false;
                }

            }

            return true;
        }


        private void Shoot(CharController character)
        {
            viewer.RPC("Shoot_RPC", Photon.Pun.RpcTarget.AllBuffered, shootPoint.position, shootPoint.rotation, character.photonView.ViewID);
        }

        [Photon.Pun.PunRPC]
        public void Shoot_RPC(Vector3 position, Quaternion rotation, int ID)
        {
            Photon.Pun.PhotonView character = Photon.Pun.PhotonView.Find(ID);

            BulletController bullet = BulletsPool.Pool.GetBullet();
            GunStats gstats = getGunStats();
            bullet.Initilize(position, rotation, gstats.power, gstats.Damage, gstats.lifeTime, character.IsMine);

            shotSource.PlayOneShot(shotNoise, GameConfiguration.EffectsLevel);
            muzzle.gameObject.SetActive(true);
            muzzle.Play(true);

            float max = getGunStats().MaxRecoil;
            recoilTarget.x += Random.Range(-max, max);
            recoilTarget.y += Random.Range(-max, max);
            recoilTarget.z += Random.Range(-max, max);


            ActualAmmo--;
            ShootedBullets++;

            bullet.gameObject.SetActive(true);

        }





        public override void Drop(CharController character, Transform itemTransform, bool selected)
        {
            base.Drop(character, itemTransform, selected);
            ModelRoot.localPosition = defaultModelPosition;
            ModelRoot.localRotation = defaultModelRotation;
        }




        public void ShowCrosshair()
        {
            crosshair.gameObject.SetActive(showCrosshair = !showCrosshair);
        }


        public void DrawCrossHair(Transform camera)
        {
            //if (!showCrosshair)
            //{
            //    ShowCrosshair();
            //}

            if (reloading != null)
            {
                return;
            }
            
            RaycastHit hit;
            Vector3 rotationPos = camera.position + camera.forward * getGunStats().Range;
            if (Physics.Raycast(camera.position, camera.forward, out hit, getGunStats().Range))
            {
                rotationPos = hit.point;
            }
            ModelRoot.LookAt(2 * ModelRoot.position - rotationPos - recoilTarget);

            recoilTarget = Vector3.Lerp(recoilTarget, Vector3.zero, deltaTime);

        }



        public override void Use(CharController character)
        {
            InventaryGroup g = character.inventary.GetGroup("Ammo");

            if (g != null)
            {
                ItemController it = g.GetItemByName(getGunStats().ammoName);
                if (it != null && it is ConsumableItem)
                {
                    ConsumableItem consumable = (it as ConsumableItem);

                    int lack = (this.getGunStats().maxClip - ActualAmmo);
                    if (lack <= 0)
                    {
                        return;
                    }
                    else
                    {
                        int getUnits = (consumable.Units < lack ? consumable.Units : lack);

                        reloading = consumable;
                        consumable.Use(character);

                        consumable.Units -= getUnits;
                        ActualAmmo += getUnits;


                        int index = character.anim.GetLayerIndex(Stats.animLayer);
                        character.anim.Play(Stats.animation, index);
                        character.anim.SetLayerWeight(index, 1);
                        character.ik.DisableLeftHand = consumable.getConsumableStats().DisableLeftHand;
                        character.ik.DisableRightHand = consumable.getConsumableStats().DisableRightHands;

                        Debug.Log(Stats.animation);
                        Debug.Log(index);
                    }
                }
            }
            base.Use(character);
        }

        
    }
}
