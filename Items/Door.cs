using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{
    public class Door : ItemController
    {
        [Range(0f, 1f)]
        public float DoorState = 0.5f;
        [Range(0f, 1f)]
        public float TargetDoorState = 0.5f;
        [Range(0f, 1f)]
        public float CurrDoorState = 0.5f;
        public float dir = -1f;

        public float OpenSpeed = .05f;
        public Vector3 MinAngle = new Vector3(0, -90, 0);
        public Vector3 MaxAngle = new Vector3(0, 90, 0);
        

        protected AudioSource AudioSource = null;
        public AudioClip OpenDoor;


        protected override void Initialize()
        {
            AudioSource = GetComponent<AudioSource>();
            base.Initialize();
        }


        public override void Take(CharController character, Transform slotPosition, bool selected)
        {
            Use(character);
        }

        public override void Use(CharController character)
        {
            Vector3 position = character.tr.position;

            Vector3 localPosition = mTransform.InverseTransformPoint(position);
            dir = localPosition.z > 0 ? 1f : -1f;

            TargetDoorState = Mathf.PingPong(TargetDoorState + 0.5f * dir, 1);

            StartCoroutine(Open());
        }

        IEnumerator Open()
        {
            float d = 0;
            while(Mathf.Abs(DoorState - TargetDoorState) >= 0.001f)
            {
                d += OpenSpeed * Time.deltaTime;
                DoorState = Mathf.Lerp(CurrDoorState, TargetDoorState, d);

                mTransform.localRotation = Quaternion.Euler(Vector3.Lerp(MinAngle, MaxAngle, DoorState));

                yield return null;
            }

            CurrDoorState = (float)decimal.Round((decimal)DoorState, 1);
        }


    }
}
