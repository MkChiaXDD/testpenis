using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public List<Transform> WayPoint;
    public Transform CurrentTarget;
    public int CurrentWaypoint = 0;
    public float Speed;
    public Vector3 OrignialPosition;
    public int Dmg;

    private float journeyLength;
    private float startTime;


    // Start is called before the first frame update
    void Start()
    {
       OrignialPosition  = this.gameObject.transform.position;
        CurrentTarget = WayPoint[CurrentWaypoint];
        StartMovement();
    }

    // Update is called once per frame
    void Update()
    {
        movement();
    }

    void StartMovement()
    {
        startTime = Time.time;
        journeyLength = Vector3.Distance(transform.position, CurrentTarget.position);

    }

    void movement()
    {
        //this.gameObject.transform.position -= Vector3.Lerp( new Vector3 (this.gameObject.transform.position.x , 0 , this.gameObject.transform.position.z), new Vector3(0, 0, 0)  , 0.1f* Time.deltaTime).normalized * Speed;

        //Debug.Log(Vector3.Lerp(new Vector3(this.gameObject.transform.position.x, 0, this.gameObject.transform.position.z), new Vector3(CurrentTarget.position.x, 0, CurrentTarget.position.z), 0.1f));
        //Debug.Log(CurrentTarget.position);
        //Debug.Log (this.gameObject.transform.position);
        //Debug.Log(Vector3.Lerp(this.gameObject.transform.position, CurrentTarget.position, 0.1f).normalized);

        float distCovered = (Time.time - startTime) * Speed;

        float fractionOfJourney = distCovered / journeyLength;


        transform.position = Vector3.Lerp(transform.position, CurrentTarget.position, fractionOfJourney);

        if (Vector3.Distance(transform.position, CurrentTarget.position) < 0.1f)
        {
            transform.position = CurrentTarget.position;
            AllocateNextPoint();
        }
    }

    void AllocateNextPoint()
    {
        CurrentWaypoint++;

        if (CurrentWaypoint >= WayPoint.Count)
        {
            return;
        }
        else
        {
            CurrentTarget = WayPoint[CurrentWaypoint];

        }
    }

    void TakeDmg()
    {

    }
}
