using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Towers : MonoBehaviour
{
    public float CoolDown = 0;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CoolDown -= Time.deltaTime;
    }

    void OnTriggerStay(Collider other)
    {
        Debug.Log("hi");
        if (other.gameObject.tag == "Enemy" && CoolDown <= 0)
        {
            CoolDown = 5.0f;
            Debug.Log("hit!");
            Destroy(other.gameObject);
        }
    }
}
