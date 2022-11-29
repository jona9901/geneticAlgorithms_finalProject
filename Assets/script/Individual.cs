// AI for videogames - Maria Luisa Cruz Lopez - 2022
// @author: Jonathan Castillo, Diego Iniguez, Sebastian Astiazaran

using System.Collections.Generic;
using System;
using UnityEngine;

public class Individual : MonoBehaviour, IComparable<Individual>, IEquatable<Individual>
{
    public List<Vector3> genes = new List<Vector3>();

    public GameObject destino;
    public GameObject origen;
    Vector3 newposition;
    public int genSize=50;

    public int avance = 0;
    public float distDest = 0.0f;

    public bool llego = false;
    public bool choco = false;

    public int list = 0;
    int numID;

    MoveVelSimple mymove;

    // Run on start
    void Start()
    {
        UnityEngine.Random.InitState(20);
        newposition = gameObject.transform.position;
        mymove = transform.gameObject.GetComponent<MoveVelSimple>();
        mymove.OnSeek = true;
    }

    // Initialize all genes
    public bool InitGenes()
    {
        for (int i = 0; i < genSize; i++)
        {
            Vector3 genTmp = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), 0, UnityEngine.Random.Range(-1.0f, 1.0f));
           genTmp.Normalize();
            genTmp *= 5.0f;
            genes.Add(genTmp);
        }
        return true;
    }

    // Initialice agent movement
    public void initmove()
    {
        mymove = transform.gameObject.GetComponent<MoveVelSimple>();
        mymove.OnSeek = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 difavance = gameObject.transform.position - newposition;
        difavance.y = 0.0f;
        if (avance < 1)
        {
            avance++;
            newposition += genes[avance];
            mymove.TargetSeek.transform.position = newposition;
            return;
        }
        if (difavance.magnitude < 1.0f)
        {
            if (avance < genSize-1)
            {
                avance++;
                newposition += genes[avance];
                mymove.TargetSeek.transform.position = newposition;
            }
            else
            {
                mymove.OnSeek = false;
                mymove.TargetSeek.transform.position = transform.position;

                Vector3 DistTemp = destino.transform.position - gameObject.transform.position;
                distDest = DistTemp.magnitude;
                llego = true;
                avance = 0;
            }
        }
    }

    // On trigger enter collision
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "meta")
        {
      
            llego = true;
            mymove.OnSeek = false;
            
        }
        if (other.gameObject.tag == "obst")
        {
            choco = true;
            mymove.OnSeek = false;
        }
    }

    // Reset the agents
    public bool reset( )
    {
        avance = 0;
        Vector3 tmp= origen.transform.position - destino.transform.position;
        distDest = tmp.magnitude;
        choco = false;
        llego = false;
        newposition= origen.transform.position;
        gameObject.transform.position = origen.transform.position;
        mymove.TargetSeek.transform.position= origen.transform.position;
        return true;
    }

    // Compare the agent with other agent
    public int CompareTo(Individual other)
    {
        if (other == null)
        {
            return 1;
        }

        //return id
        float dist= distDest-other.distDest;
        return (int)dist;
    }

    // Validate if the agent is equals to other agent
    public override bool Equals(System.Object obj)
    {
        Individual tmp = obj as Individual;

        return distDest == tmp.distDest;
    }

    // Get the hashcode of the agent
    public override int GetHashCode()
    {
        return numID;
    }

    // Validate if distDest equals to other agent distDest
    public bool Equals(Individual other)
    {
        return distDest == other.distDest;
    }

    // Draws gizmos
    void OnDrawGizmos()
    {
        //Debug.Log("dibuja Path");
        Vector3 pos0 = origen.transform.position;
        Vector3 pos1 = pos0;
        for (int i = 1; i <= avance; i++)
        {
            if (i < genSize)
            {
                pos1 += genes[i];
                Debug.DrawLine(pos0, pos1, Color.green);
                pos0 = pos1;
            }
        }
       
    }
}
