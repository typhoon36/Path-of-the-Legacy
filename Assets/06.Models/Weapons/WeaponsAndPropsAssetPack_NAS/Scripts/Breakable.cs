using System.Collections;
using UnityEngine;

namespace WeaponsAndPropsAssetPack_NAS.Scripts
{
    public class Breakable : MonoBehaviour
    {
        [SerializeField]  Transform wholeObject;
        [SerializeField]  Transform fracturedObject;
        [SerializeField]  bool isCyclic;

        // Core Variables
         bool isBroken;
         bool isClean;
         bool objectReseted = true;
         Transform fracturedObjectInstance;
         bool shouldBreak;

        // Variables to showcase in cycle
         const float timeToCleanUp = 5f;
         const float timeToStartDestruction = 2f;
         const float timeToReconstructObject = 2f;
         const float cycleTime = 0.2f;
         const float timerTimeUnit = 1f;
        
         void Start()
        {
            TriggerBreak();
        }

         void TriggerBreak()
        {
            // Methods For Cyclic Use (I.E Destroy On Loop -  For Showcase)
            if (isCyclic)
            {
                StartCoroutine(CycleDestruction());
            }
            // Methods For Single Use (I.E Destroy Once)
            else
            {
                StartCoroutine(DestroyOnce());
            }
        }

        // Core Methods For Single Use (I.E Destroy Once)
         IEnumerator DestroyOnce()
        {
            objectReseted = false;
            shouldBreak = true;
            yield return null;
        }

         void Update()
        {
            if (shouldBreak)
            {
                BreakObject();
            }
        }

         void BreakObject()
        {
            wholeObject.gameObject.SetActive(false);
            fracturedObjectInstance = Instantiate(fracturedObject);
            fracturedObjectInstance.position = wholeObject.position;
            fracturedObjectInstance.gameObject.SetActive(true);
            isBroken = true;
            shouldBreak = false;
            StartCoroutine(CleanUpCoroutine());
        }

         void CleanUp()
        {
            isClean = true;
            Destroy(fracturedObjectInstance.gameObject);
        }

         IEnumerator ResetObject()
        {
            if (isClean)
            {
                yield return new WaitForSeconds(timeToReconstructObject);
                wholeObject.gameObject.SetActive(true);
                isBroken = false;
                isClean = false;
                objectReseted = true;
            }
        }

         IEnumerator CleanUpCoroutine()
        {
            float timer = 0f;
            while (isBroken && !isClean)
            {
                if (timer >= timeToCleanUp)
                {
                    CleanUp();
                }

                yield return new WaitForSeconds(timerTimeUnit);
                timer += 1f;
            }

            // Methods For Cyclic Use (I.E Destroy On Loop -  For Showcase)
            if (isCyclic)
            {
                yield return ResetObject();
            }

            yield return null;
        }

        // Methods For Cyclic Use (I.E Destroy On Loop -  For Showcase)
         IEnumerator CycleDestruction()
        {
            while (true)
            {
                if (objectReseted)
                {
                    yield return new WaitForSeconds(timeToStartDestruction);
                    objectReseted = false;
                    shouldBreak = true;
                }
                yield return new WaitForSeconds(cycleTime);
            }
        }
    }
}