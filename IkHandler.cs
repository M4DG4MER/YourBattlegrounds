using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{
    public class IkHandler : MonoBehaviour
    {
        public Transform LookAtPosition;

        public Transform LeftHandPosition;
        public Transform LeftElbowPosition;
        public Transform RightHandPosition;
        public Transform RightElbowPosition;

        public float BodyWeight = 0.5f;



        public float t;

        public bool DisableHands;
        public bool DisableLeftHand;
        public bool DisableRightHand;
        public bool Aiming;
        public bool Shooting;


        Animator anim;
      
        private void OnEnable()
        {
            anim = GetComponent<Animator>();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            float handsMultiplier = (DisableHands ? 0 : 1);
            float leftHandMultiplier = (DisableLeftHand ? 0 : 1);
            float rightHandMultiplier = (DisableRightHand ? 0 : 1);

            anim.SetLayerWeight(anim.GetLayerIndex("UpperBody"), handsMultiplier);

            anim.SetLookAtWeight(1, BodyWeight * handsMultiplier, 1, 1, (Aiming ? 0 : 1));

            anim.SetLookAtPosition(LookAtPosition.position);


            if (RightHandPosition != null && RightElbowPosition != null)
            {
                SetIk(AvatarIKGoal.RightHand, RightHandPosition, AvatarIKHint.RightElbow, RightElbowPosition, handsMultiplier * rightHandMultiplier);
            }

            if (LeftHandPosition != null && LeftElbowPosition != null)
            {
                SetIk(AvatarIKGoal.LeftHand, LeftHandPosition, AvatarIKHint.LeftElbow, LeftElbowPosition, handsMultiplier * leftHandMultiplier);
            }


        }


        private void SetIk(AvatarIKGoal goal, Transform target, AvatarIKHint hint, Transform restraint, float weight)
        {

            anim.SetIKHintPositionWeight(hint, weight);
            anim.SetIKPositionWeight(goal, weight);
            anim.SetIKRotationWeight(goal, weight);
            anim.SetIKPosition(goal, target.position);
            anim.SetIKRotation(goal, target.rotation);
            anim.SetIKHintPosition(hint, restraint.position);
        }




    }
}
