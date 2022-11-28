using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveVelSimple : MonoBehaviour
{

    // general data
    public Vector3 vc_Velocity;
    public Vector3 vc_Heading;
    Vector3 vn_Velocity;
    public float s_rotSpeed = 20.0f;

    public float s_MaxSpeed = 10.0f;
    public float s_MinSpeed = 1.0f;

    private Vector3 newPosition;

    public GameObject TargetSeek;
    public GameObject TargetFlee;
    public GameObject TargetPursuit;


    public bool OnSeek = false;
    public bool OnFlee = false;


    public float s_panicDist;

    // Use this for initialization
    void Start()
    {
        // s_MaxSpeed = 8.0f;
        s_MinSpeed = 0.2f;
        s_panicDist = 30.0f;
        vn_Velocity = new Vector3(0.0f, 0.0f, 0.0f);
        vc_Velocity = new Vector3(0.0f, 0.0f, 0.0f);

    }

    // Update is called once per frame
    void Update()
    {

        vn_Velocity = Vector3.zero;

        if (OnSeek)
        {
            if (Vector3.Distance(TargetSeek.transform.position, transform.position) > 1.0)
                vn_Velocity = vn_Velocity + Seek(TargetSeek.transform.position);
            else
                vn_Velocity = Vector3.zero;
                //vn_Velocity = vc_Velocity * -1.0f;
        }

        if (OnFlee)
        {
            vn_Velocity = vn_Velocity + Flee(TargetFlee.transform.position);
        }

        //**********************************************************
        if (vn_Velocity.magnitude > 0.025f)
        {
            vc_Velocity += vn_Velocity;
            vc_Velocity = Vector3.ClampMagnitude(vc_Velocity, s_MaxSpeed);
            newPosition = transform.position + (vc_Velocity * Time.deltaTime);

            if (vc_Velocity.magnitude > s_MinSpeed)
                transform.position = newPosition;
        }
        else {
            vc_Velocity= Vector3.zero;
        }
        
        // update the direction of the boid
        vc_Heading = vc_Velocity.normalized;

        //****************************************

        float angle = Vector3.SignedAngle(transform.forward, vc_Heading, Vector3.up);
        //Debug.Log(angle);
        float rotAngle = 0.0f;

        if (angle < -0.1f)
            rotAngle = Time.deltaTime * -1.0f * s_rotSpeed;
        if (angle > 0.1f)
            rotAngle = Time.deltaTime * s_rotSpeed;
        if (angle >= -0.1f && angle <= 0.1f)
        {
            if (Vector3.Dot(transform.forward, vc_Heading) >= 0.9)
                rotAngle = 0.0f;
            if (Vector3.Dot(transform.forward, vc_Heading) <= -0.9)
                rotAngle = 10.0f;
        }

        //Debug.Log("rotAngle" + rotAngle);
        transform.Rotate(0.0f, rotAngle, 0.0f, Space.Self);
        //*****************************************
    }
    //******************************************************************

    public Vector3 Seek(Vector3 targetseek)
    {
        Vector3 direction;

        direction = targetseek - transform.position;
        direction.y = 0;

        if (direction.magnitude < 0.25f)
        {

            return (Vector3.zero);
        }
        direction.Normalize();
        Vector3 DesiredVelocity = direction * s_MaxSpeed;
        DesiredVelocity = Vector3.ClampMagnitude(DesiredVelocity, s_MaxSpeed);

        return (DesiredVelocity - vc_Velocity);


    }

    //******************************************************************
    public Vector3 Flee(Vector3 targetFlee)
    {
        Vector3 direction;

        direction = transform.position - targetFlee;
        direction.y = 0;

        if (direction.magnitude > s_panicDist)
        {

            return (Vector3.zero);
        }
        direction.Normalize();
        Vector3 DesiredVelocity = direction * s_MaxSpeed;
        DesiredVelocity = Vector3.ClampMagnitude(DesiredVelocity, s_MaxSpeed);

        return (DesiredVelocity - vc_Velocity);


    }

    //******************************************************************
    ///  ************** function pursuit*********


    //*************************************************************************
    void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, vc_Heading * 3.0f + transform.position, Color.red);
        Debug.DrawLine(transform.position, transform.forward * 3.0f + transform.position, Color.green);

    }
}
