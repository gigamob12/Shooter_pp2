using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallingPlatform : MonoBehaviour
{


    [SerializeField] float floorDropTime;
    [SerializeField] Rigidbody rigi;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("stepped on me");
            StartCoroutine(FloorDropTime());
        }
    }
    IEnumerator FloorDropTime()
    {
        yield return new WaitForSeconds(floorDropTime);
        rigi.useGravity = true;

    }
}
