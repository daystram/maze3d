using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycle : MonoBehaviour
{

    public GameObject sun;
    // Start is called before the first frame update
    void Start()
    {
        sun = gameObject;
        
    }

    // Update is called once per frame
    void Update()
    {
        sun.transform.localEulerAngles = new Vector3(Time.time * 10, -30,0);
    }
}
