using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet3 : MonoBehaviour
{
    private float orbitScale = 8f;
    private float rotationScale = 20f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(GameObject.Find("Star").transform.position, new Vector3(0f, 1f, 0f), Time.deltaTime * orbitScale);
        transform.Rotate(new Vector3(0f, 1f, 0f), Time.deltaTime * rotationScale);
    }
}
