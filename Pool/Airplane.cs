using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace tutoriales
{
    public class Airplane : Photon.Pun.MonoBehaviourPun
    {
        private static Airplane _airplane;
        public static Airplane AIRPLANE()
        {
            if (_airplane == null)
                _airplane = FindObjectOfType<Airplane>();
            return _airplane;
        }

        public enum PlaneMode
        {
            player = 0,
            drop =1,
            bomb = 2
        }

        public PlaneMode mode = PlaneMode.player;
        public Drop dropPrefab;
        public float MinDropDelay;
        public float MaxDropDelay;
        public float MinDropDelta;
        public float MinDropTarget;
        public int MaxDrops;
        public int QDroped;



        public CharController charPrefab;

        public Transform Plane;
        public Transform PlaneCam;
        public Transform PlaneCamRotator;
        private Transform cam;

        public float Distance;
        public float MinDistanceJump;
        public float MinDistanceDrop;



        public float Speed;
        public float rotationSpeed;

        public bool PrepareForLaunch = false;
        public bool Launched = false;



        public Text _Text;

        private float d = 0f;
        public float maxd = 2f;
        private PhotonView View;
        private Rigidbody rg;

        private void OnEnable()
        {
            cam = Camera.main.transform;
            View = GetComponent<PhotonView>();
            rg = Plane.GetComponent<Rigidbody>();
            
            if (PhotonNetwork.IsMasterClient)
                ResetRandomPos();
        }

        void ResetRandomPos()
        {
            Vector3 pos = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            pos.Normalize();

            View.RPC("SetPosition", RpcTarget.AllBuffered, pos);
        }


        [Photon.Pun.PunRPC]
        void SetPosition(Vector3 pos)
        {
            Plane.position = this.transform.position + pos * Distance;
            Plane.LookAt(this.transform.position);

            rg.velocity = Plane.forward * Speed;
        }



        // Update is called once per frame
        void FixedUpdate()
        {
            switch (mode)
            {
                case PlaneMode.player:
                    DropPlayer();
                    break;
                case PlaneMode.drop:
                    DropBox();
                    break;
                default:
                    break;
            }
            
        }


        void DropPlayer()
        {
            if (!Launched)
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");
                Vector2 mouseDelta = new Vector2(mouseX, mouseY);
                float xrot = mouseDelta.x * Time.deltaTime * rotationSpeed;

                PlaneCamRotator.Rotate(0, xrot, 0);

                cam.position = PlaneCam.position;
                cam.LookAt(Plane);

                bool distance = (Vector3.Distance(this.transform.position, Plane.position) < MinDistanceJump);
                _Text.enabled = distance;
                if (distance)
                {
                    float jump = Input.GetAxis("Jump");
                    if (jump == 1)
                    {
                        Launched = true;
                        Photon.Pun.PhotonNetwork.Instantiate(charPrefab.name, Plane.position, Quaternion.identity);
                        _Text.enabled = false;
                    }
                }
            }


        }


        void DropBox()
        {
            bool distance = (Vector3.Distance(this.transform.position, Plane.position) < MinDistanceDrop);
            if (distance && QDroped < MaxDrops)
            {
                MinDropDelta += Time.deltaTime;
                if (MinDropDelta >= MinDropTarget)
                {
                    Drop d = Instantiate<Drop>(dropPrefab);
                    d.transform.position = Plane.position;
                    MinDropDelta = 0;
                    QDroped++;
                }

            }


        }


        public void ResetForDrop(Vector3 pos)
        {
            mode = PlaneMode.drop;
            MinDropTarget = Random.Range(MinDropDelay, MaxDropDelay);
            this.transform.position = new Vector3(pos.x, this.transform.position.y, pos.z);
            ResetRandomPos();
        }


    }



}

