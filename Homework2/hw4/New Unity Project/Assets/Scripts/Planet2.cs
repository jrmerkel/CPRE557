using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet2 : MonoBehaviour
{
    private float orbitScale = 12.5f;
    private float rotationScale = 50f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(GameObject.Find("Star").transform.position, new Vector3(0f, 1f, 0f), Time.deltaTime* orbitScale);
        transform.Rotate(new Vector3(0f, -1f, 0f), Time.deltaTime * rotationScale);
    }
}
