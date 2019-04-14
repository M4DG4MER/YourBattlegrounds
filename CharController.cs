using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tutoriales
{
    public class CharController : Photon.Pun.MonoBehaviourPun
    {
        public bool Active = true;
        public bool Player = true;

        public CharStats Stats;
        public Stat life;
        public Stat shield;
        public Stat armor;



        internal Transform tr;
        Rigidbody rg;
        internal Animator anim;
        RagdollController ragdoll;
        CapsuleCollider coll;
        internal InventaryController inventary;
        internal IkHandler ik;

        public Transform CameraShoulder;
        public Transform CameraHolder;
        public Transform AimingCameraHolder;
        private Transform getHolder()
        {
            return (states.Aiming ? AimingCameraHolder : CameraHolder);
        }
     

        public Transform LookAt;
        private Transform cam;

        internal float rotY = 0f;
        internal float rotX = 0f;

        public Transform HandsPivot;
        public Transform throwOrigin;
        public List<HandsStates> HandsStates = new List<HandsStates>();
        private HandsStates currentState;
        public void SetHandState(string stateName)
        {
            if (currentState == null || currentState.Name != stateName)
            {
                foreach (HandsStates hstate in HandsStates)
                {
                    if (hstate.Name == stateName)
                    {
                        currentState = hstate;
                        break;
                    }
                }
            }
        }




        public States states = new States();
        private bool notShooting = false;


        private Vector2 moveDelta;
        private Vector2 mouseDelta;

        internal Vector2 moveAnim;
        private float deltaT;


        public InputController _input;

        private bool initialized = false;


        private void OnEnable()
        {
            if (!initialized){
                Initialize();
            }
        }

        // Use this for initialization
        void Initialize()
        {
            tr = this.transform;
            rg = GetComponent<Rigidbody>();
            coll = GetComponent<CapsuleCollider>();
            anim = GetComponentInChildren<Animator>();
            ik = GetComponentInChildren<IkHandler>();
            inventary = GetComponent<InventaryController>();
            ragdoll = GetComponentInChildren<RagdollController>();
            
            ik.LookAtPosition = LookAt;
            cam = Camera.main.transform;

            inventary.Initiliaze(Player, this);

            life = new Stat("life", this.Stats.MaxLife, this.Stats.MaxLife);
            shield = new Stat("shield", this.Stats.MaxLife, 0);
            armor = new Stat("armor", this.Stats.MaxLife, 0);

            Active = true;

            initialized = true;
        }

        void InitStatsInventary()
        {
            StatsViewer.Viewer().Add(life);
            StatsViewer.Viewer().Add(shield);
            StatsViewer.Viewer().Add(armor);
            inventary.Initiliaze(Player, this);
        }



        // Update is called once per frame
        void FixedUpdate()
        {
            if (life.Value <= 0)
                return;

            if (Player)
            {
                PlayerControl();
                AreaControl();
                CameraControl();
            }
            else
            {
                if (Player = (Photon.Pun.PhotonNetwork.CurrentRoom == null || photonView.IsMine))
                {
                    InitStatsInventary();
                }
            }

            if (!Active)
            {
                return;
            }

            MoveControl();
            ItemsControl();
            AnimControl();
        }

        private void PlayerControl()
        {
            _input.Update();

            
            float deltaX = _input.CheckF("Horizontal");
            float deltaZ = _input.CheckF("Vertical");
            float mouseX = _input.CheckF("Mouse X");
            float mouseY = _input.CheckF("Mouse Y");
            states.Jumping = _input.Check("Jump");
            states.Crouching = _input.Check("Crouch");
            states.Running = _input.Check("Run");
            states.Interacting = _input.Check("Interact");
            states.Consuming = _input.Check("Consume") && states.OnGround;
            states.W1 = _input.Check("Weapon1");
            states.W2 = _input.Check("Weapon2");
            states.W3 = _input.Check("Weapon3");
            states.Throwing = (_input.Check("Throw") ? !states.Throwing : states.Throwing);


            if (_input.Check("Inventary"))
            {
                states.Inventary = !states.Inventary;
                if (!states.Inventary)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    inventary.inventaryViewer.SceneViewer.gameObject.SetActive(false);
                }
                else
                {
                    inventary.inventaryViewer.SceneViewer.gameObject.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;
                }
            }

            states.Reloading = _input.Check("Reload");
            states.FireMode = _input.Check("FireMode");
            states.Aiming = _input.Check("Fire2") && !states.Running;
            bool shoot  = _input.Check("Fire1") && !states.Running && !EventSystem.current.IsPointerOverGameObject();
            //

            if (!shoot)
            {
                notShooting = true;
            }
            states.Shooting = shoot && notShooting;

            moveDelta = new Vector2(deltaX, deltaZ);
            mouseDelta = new Vector2(mouseX, mouseY);
            deltaT = Time.deltaTime;


            if (Input.GetKeyDown(KeyCode.Y))
            {
                TakeDamage(10);
            }

        }

        private void MoveControl()
        {
            if (!states.Inventary)
            {
                rotY += mouseDelta.y * deltaT * Stats.rotationSpeed;
                float xrot = mouseDelta.x * deltaT * Stats.rotationSpeed;
                tr.Rotate(0, xrot, 0);
                rotY = Mathf.Clamp(rotY, Stats.minAngle, Stats.maxAngle);

                Quaternion localRotation = Quaternion.Euler(-rotY, 0, 0);

                CameraShoulder.localRotation = localRotation;
            }
            


            Vector3 side = Stats.speed * moveDelta.x * deltaT * tr.right;
            Vector3 forward = Stats.speed * moveDelta.y * deltaT * tr.forward;

            Vector3 endSpeed = side + forward;


            RaycastHit hit;
            states.OnGround = Physics.Raycast(this.tr.position, -tr.up, out hit, .2f);
            if (states.OnGround)
            {
                if (states.Crouching)
                    OnCrouch();
                else
                {
                    if (states.Running)
                        endSpeed *= Stats.runningSpeedIncrement;
                }

                if (states.Jumping)
                {
                    if (states.Crouch)
                        OnCrouch();
                    else
                        Jump();
                }

                Vector3 sp = rg.velocity;
                endSpeed.y = sp.y;

                rg.velocity = endSpeed;
            }
            else
            {
                Vector3 sp = rg.velocity;
                sp.y = Mathf.Clamp(sp.y, -Stats.maxFallSpeed, 0);
                rg.velocity = sp;


                if (states.Crouch)
                    OnCrouch();
            }


            moveAnim = moveDelta * (states.Running ? 2 : 1);
        }


        private void ItemsControl()
        {

            Collider[] checking = Physics.OverlapSphere(LookAt.position, 2f, LayerMask.GetMask("Item"));


            if (states.Inventary)
            {
                Collider[] checkingArround = Physics.OverlapSphere(this.tr.position, 2f, LayerMask.GetMask("Item"));
                inventary.inventaryViewer.SceneViewer.ShowItems(checkingArround);
            }


            if (checking.Length > 0)
            {
                float near = 2f;
                Collider nearest = null;
                
                foreach (Collider c in checking)
                {
                    Vector3 collisionpos = c.ClosestPoint(LookAt.position);
                    float distance = Vector3.Distance(collisionpos, LookAt.position);
                    if (distance < near)
                    {
                        nearest = c;
                        near = distance;
                    }
                }
                
                if (nearest != null)
                {
                    ItemController item = nearest.GetComponent<ItemController>();
                    if (item != null )
                    {
                        if (Player)
                        {
                            inventary.ItemViewer.DrawItemViewer(item.Stats, item.mTransform, cam);
                        }
                        if (states.Interacting)
                        {
                            Take(item);
                            ///take MULTI// inventary.AddItem(this, item); 
                        }
                    }
                }
            }
            else
            {
                if (this.Player)
                {
                    inventary.ItemViewer.HideViewer();
                }
            }
            
            if (!states.Throwing)
            {
                this.GunControl();
            }
            else
            {
                this.ThrowControl();
            }
            


            if (states.Consuming)
            {
                ItemController selectedMed = inventary.GetSelectedAt("Meds");
                if (selectedMed != null)
                {
                    if (selectedMed is ConsumableItem)
                    {
                        ConsumableItem consumable = selectedMed as ConsumableItem;
                        consumable.Use(this);
                    }
                }

            }

        }
        
        public void Take(ItemController item)
        {
            item.photonView.RequestOwnership();
            photonView.RPC("RPCTake", Photon.Pun.RpcTarget.OthersBuffered, item.photonView.ViewID);
            inventary.AddItem(this, item);
        }

        [Photon.Pun.PunRPC]
        void RPCTake(int itemID)
        {
            Photon.Pun.PhotonView pv = Photon.Pun.PhotonView.Find(itemID);
            ItemController item = pv.GetComponent<ItemController>();
            if (item != null)
                inventary.AddItem(this, item);
        }



        public void GunControl()
        {
            ItemController selectedWeapon = inventary.GetSelectedAt("PrimaryWeapon");

            if (selectedWeapon != null)
            {
                selectedWeapon.Show();
                if (selectedWeapon is GunController)
                {
                    states.Armed = true;
                    GunController GUN = (selectedWeapon as GunController);
                    this.SetHandState(GUN.getGunStats().HandsState);

                    ik.LeftHandPosition = GUN.leftHandPosition;
                    ik.LeftElbowPosition = GUN.leftElbowPosition;

                    if (Player)
                    {
                        GUN.DrawCrossHair(cam);

                        if (!states.Inventary)
                        {
                            if (states.Shooting)
                            {
                                if (!GUN.Attack(this))
                                {
                                    notShooting = false;
                                }
                            }
                            else if (states.Reloading)
                            {
                                GUN.Use(this);
                            }
                            else if (states.FireMode)
                            {
                                GUN.NextFireState();
                            }
                        }
                    }
                }
                else
                {
                    states.Armed = false;
                }
            }
            else
            {
                states.Armed = false;
            }



            InventaryGroup weaponGroup = inventary.GetGroup("PrimaryWeapon");

            if (weaponGroup != null)
            {
                if (states.W1)
                {
                    weaponGroup.Select(0);
                }
                if (states.W2)
                {
                    weaponGroup.Select(1);
                }
                if (states.W3)
                {
                    weaponGroup.Select(2);
                }
            }

        }


        public void ThrowControl()
        {

            ItemController selectedWeapon = inventary.GetSelectedAt("PrimaryWeapon");
            if (selectedWeapon != null)
            {
                selectedWeapon.Hide();
            }


            ItemController selectedThrowable = inventary.GetSelectedAt("Throwable");
            this.states.Armed = true;

            if (selectedThrowable != null)
            {
                if (selectedThrowable is ThrowableItem)
                {
                    if (states.Shooting && !states.Inventary)
                    {
                        selectedThrowable.Use(this);
                    }

                }
            }

        }


        public void Jump()
        {
            rg.AddForce(tr.up * Stats.jumpForce);
        }


        public void OnCrouch()
        {
            states.Crouch = !states.Crouch;
            states.Crouching = false;
            float mult = (states.Crouch ? 1 : -1);
            coll.center = coll.center + new Vector3(0, Stats.crouchPosOffset, 0) * mult;
            coll.height += Stats.crouchHeightOffset * mult;
            CameraShoulder.position = CameraShoulder.position + new Vector3(0, Stats.crouchPosOffset, 0) * mult;
        }


        public void TakeDamage(float damage)
        {
            photonView.RPC("TakeDamage_RPC", Photon.Pun.RpcTarget.AllBuffered, damage);
        }

        [Photon.Pun.PunRPC]
        public void TakeDamage_RPC(float damage)
        {
            life.Value -= damage;
            if (life.Value <= 0)
            {
                ragdoll.Active(true);
                this.Active = false;
            }
        }





        internal void Consume(float effectStrength, string effectStat)
        {
            if (effectStat == "life")
            {
                if ((life.Value + effectStrength) <= life.Max)
                {
                    life.Value += effectStrength;
                }
                else
                {
                    life.Value = life.Max;
                }
            }
            else if (effectStat == "shield")
            {
                if ((shield.Value + effectStrength) <= shield.Max)
                {
                    shield.Value += effectStrength;
                }
                else
                {
                    shield.Value = shield.Max;
                }
            }
            else if (effectStat == "armor")
            {
                if ((armor.Value + effectStrength) <= armor.Max)
                {
                    armor.Value += effectStrength;
                }
                else
                {
                    armor.Value = shield.Max;
                }
            }


        }


        private void CameraControl()
        {
           
            
            Vector3 heading = getHolder().position - CameraShoulder.position;
            float distance = heading.magnitude;
            Vector3 direction = heading / distance;

            RaycastHit hit;
            bool cameraCollision = Physics.Raycast(CameraShoulder.position, direction, out hit, distance, ~LayerMask.GetMask(new string[] { "player", "area", "camera" }));
            Vector3 finalP = (cameraCollision ? hit.point - direction * Stats.camRadius : getHolder().position);

            if (cameraCollision)
            {
                Debug.Log(hit.collider.gameObject);
            }

            cam.position = Vector3.Lerp(cam.position, finalP, Stats.cameraSpeed * deltaT);
            cam.rotation = Quaternion.Lerp(cam.rotation, getHolder().rotation, Stats.cameraSpeed * deltaT);
        }

        private float deltaDamage = 0;
        public bool onzone = true;
        private void AreaControl()
        {
            Area a = Area.AREA();
            if (a != null && !onzone)
            {
                this.deltaDamage += Time.deltaTime;
                if (a.actualArea.MustDamage(this.deltaDamage))
                {
                    this.deltaDamage = 0;
                    this.TakeDamage(a.actualArea.Damage);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Area a = Area.AREA();
            if (a != null && other.gameObject == a.Cylinder.gameObject)
            {
                onzone = true;
                this.deltaDamage = 0;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            Area a = Area.AREA();
            if (a != null && other.gameObject == a.Cylinder.gameObject)
            {
                onzone = false;
            }
        }



        private void AnimControl()
        {
            HandsPivot.position = anim.GetBoneTransform(HumanBodyBones.RightShoulder).position;
            Quaternion localRotation = Quaternion.Euler(-rotY, HandsPivot.localRotation.y, HandsPivot.localRotation.z);
            HandsPivot.localRotation = localRotation;
            
            anim.SetBool("ground", states.OnGround);
            anim.SetBool("crouch", states.Crouch);
            anim.SetBool("armed", states.Armed);

            anim.SetFloat("X", moveAnim.x);
            anim.SetFloat("Y", moveAnim.y);

            ik.Aiming = (this.states.Aiming);
            ik.Shooting = (this.states.Shooting);
            ik.DisableHands = !this.states.Armed;

            ik.DisableLeftHand = this.states.Throwing;
            ik.DisableRightHand = this.states.Throwing;

            if (currentState != null)
            {
                ik.RightHandPosition = (states.Aiming ? currentState.RightHand : currentState.RightHandRelax);
                ik.RightElbowPosition = (states.Aiming ? currentState.RightElbow : currentState.RightElbowRelax);
            }
        }
    }
    
    [System.Serializable]
    public class States
    {
        public bool OnGround = false;
        public bool Jumping = false;
        public bool Crouch = false;
        public bool Crouching = false;
        public bool Running = false;
        public bool Aiming = false;
        public bool Armed = false;

        public bool Throwing = false;

        public bool Shooting = false;
        public bool Interacting = false;
        public bool Consuming = false;
        public bool W1 = false;
        public bool W2 = false;
        public bool W3 = false;

        public bool Reloading = false;
        public bool FireMode = false;

        public bool Inventary = false;
    }


    [System.Serializable]
    public class HandsStates
    {
        public string Name;
        public Transform RightHand;
        public Transform RightElbow;
        public Transform RightHandRelax;
        public Transform RightElbowRelax;


    }
}
