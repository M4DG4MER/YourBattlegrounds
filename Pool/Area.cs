using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace tutoriales
{
    public class Area : MonoBehaviour {

        private static Area _area;
        public static Area AREA()
        {
            if (_area == null)
                _area = FindObjectOfType<Area>();
            return _area;
        }


        public MeshRenderer Cylinder;

        public AreaStep[] Steps;
        public float timeElapsed;
        public AreaStep actualArea;

        int step = 0;
        private Transform mTransform;

        private void OnEnable()
        {
            mTransform = this.transform;
        }

        void NextStep()
        {
            if (step < Steps.Length)
            {
                AreaStep prevArea = actualArea;
                actualArea = Steps[step++];
                actualArea.SelectPos(prevArea);
                if (actualArea.Drop)
                {
                    Airplane.AIRPLANE().ResetForDrop(actualArea.position);
                }
            }
        }


        private void FixedUpdate()
        {
            timeElapsed += Time.deltaTime;
            
            if (actualArea.CheckStart(timeElapsed))
            {
                if (step == 0)
                {
                    Cylinder.enabled = true;
                }

                float timePercent = actualArea.EndPercentage(timeElapsed);
                float t = timePercent * Time.deltaTime;
                Vector3 fpos = new Vector3(actualArea.position.x, this.transform.position.y, actualArea.position.z);
                mTransform.position = Vector3.Lerp(mTransform.position, fpos, timePercent * Time.deltaTime);
                mTransform.localScale = Vector3.Lerp(mTransform.localScale, new Vector3(actualArea.Radius, mTransform.localScale.y, actualArea.Radius), timePercent * Time.deltaTime);


                if (timePercent > 1)
                {
                    NextStep();
                }
           
            }


        }

    }

    [System.Serializable]
    public class AreaStep
    {
        public enum AreaStepMode
        {
            random = 0,
            interest = 1
        }

        public AreaStepMode mode = AreaStepMode.random;
        public bool Drop;

        public Vector3 position;
        public float Radius;
        public float timeStart;
        public float timeEnd;
        public float Damage;
        public float DamageFrequency;


        public void SelectPos(AreaStep prevStep)
        {
            switch (mode)
            {
                case AreaStepMode.random:
                    RandomPos(prevStep);
                    break;
                case AreaStepMode.interest:
                    InterestPos(prevStep);
                    break;
                default:
                    break;
            }
        }


        public void RandomPos(AreaStep prevStep)
        {
            Vector3 pos = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            pos.Normalize();

            float nRadius = this.Radius / 2;
            pos *= nRadius;

            Vector3 finalPos = prevStep.position + pos;
            this.position = finalPos;
        }


        public void InterestPos(AreaStep prevStep)
        {
            Collider[] col = Physics.OverlapSphere(prevStep.position, prevStep.Radius / 2, LayerMask.GetMask("interest"));
            
            if (col != null && col.Length > 0)
            {
                int indexRandom = Random.Range(0, col.Length - 1);
                Collider c = col[indexRandom];
                this.position = c.bounds.center;
            }

        }


        public bool CheckStart(float timeElapsed)
        {
            return timeElapsed >= timeStart;
        }

        public float EndPercentage(float timeElapsed)
        {
            float timeDifference = timeEnd - timeStart;
            float timeStep = timeElapsed - timeStart;

            return timeStep / timeDifference;
        }

        public bool MustDamage(float timeElapsed)
        {
            return timeElapsed >= DamageFrequency;
        }


    }


}


