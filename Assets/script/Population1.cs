using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population1 : MonoBehaviour
{

    int totalPop = 100;
    int totalArrive = 0;
    public GameObject myagent;
    public GameObject beginPath;
    public GameObject endPath;


    public float maxSpeed = 10.0f;
    public float maxRadio = 8.0f;

    public float minSpeed = 5.0f;
    public float minRadio = 4.0f;
   
    List<GameObject> grid1Agent;
    List<GameObject> newAgents;
    List<Individual> evalAgent;


    public bool gen1 = true;
    MoveVelSimple objmove;
    Individual objInd;
    //Come objCome;

    // Use this for initialization
    void Start()
    {   //generate the first population of the algorithm
        grid1Agent = new List<GameObject>();
        newAgents = new List<GameObject>();
        evalAgent = new List<Individual>();

        Random.InitState(43);
        Vector3 newPosition = beginPath.transform.position;
        GameObject obj;
        GameObject sphereT;

        for (int i = 0; i < totalPop; i = i + 1)
        {
            obj = (GameObject)Instantiate(myagent, newPosition, Quaternion.identity);
            sphereT = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphereT.transform.position = newPosition;

            objmove = obj.GetComponent<MoveVelSimple>();
            objmove.OnSeek = true;
            objmove.TargetSeek = sphereT;
           
            obj.gameObject.SetActive(false);

            objInd = obj.GetComponent<Individual>();
            objInd.InitGenes();
            objInd.destino = endPath;
            objInd.origen = beginPath;

            Vector3 DistTemp = endPath.transform.position - obj.transform.position;
            objInd.distDest = DistTemp.magnitude;

            objInd.list = 1;
            grid1Agent.Add(obj);

            evalAgent.Add(objInd);

            // second list for evolution
            obj = (GameObject)Instantiate(myagent, newPosition, Quaternion.identity);
            sphereT = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphereT.transform.position = newPosition;


            objmove = obj.GetComponent<MoveVelSimple>();
            objmove.OnSeek = true;
            objmove.TargetSeek = sphereT;
            obj.gameObject.SetActive(false);
           

            objInd = obj.GetComponent<Individual>();
            objInd.destino = endPath;
            objInd.origen = beginPath;
            objInd.initmove();
            DistTemp = endPath.transform.position - obj.transform.position;
            objInd.distDest = DistTemp.magnitude;
            objInd.InitGenes();
            objInd.list = 2;

            obj.gameObject.SetActive(false);
            newAgents.Add(obj);

        }
         ActivaGen();
        

    }

    // Update is called once per frame
    void Update()
    {
         
    }

    public bool ActivaGen()
    {
       
        for (int i = 0; i < totalPop; i = i + 1)
        {
            grid1Agent[i].SetActive( true);

        
        }
        return true;
    }



  public bool allArrive()
    {  totalArrive = 0;
        for (int i = 0; i < totalPop; i ++)
        {
            if (evalAgent[i].llego || evalAgent[i].choco)
                totalArrive++;
        }
        if (totalArrive == totalPop)
            return true;
        else return false;

    }
        public void selecBest()
    {
        if (evalAgent.Count > 0)
        {
            //evalAgent.TrimExcess();
            evalAgent.Sort();
        }

        Debug.Log("Evaluacion");
        Debug.Log("valor media" + evalAgent[totalPop / 2].distDest);
        float[] distancia = new float[totalPop];

        string let = "distancia: ";
        for (int i = 0; i < totalPop; i = i + 1)
        {
            distancia[i] = evalAgent[i].distDest;
            let += distancia[i];
            let += ",";
        }
        Debug.Log(let);


    }
    //*********************************************************************
    public void crossover()
    {
        //Come refCome;

        Individual refInd;
        Individual refInd_F1;
        Individual refInd_F2;
        Individual myInd;

        float minDistance = evalAgent[0].distDest;
        //Debug.Log("maxima comida" + maxComida);

        int NumParents = (int)(totalPop / 4);   //mitad de población

        List<GameObject> Gorigen;
        List<GameObject> Gdestino;
        if (gen1)
        {
            Gorigen = new List<GameObject>(grid1Agent);
            Gdestino = new List<GameObject>(newAgents);
        }
        else
        {
            Gorigen = new List<GameObject>(newAgents);
            Gdestino = new List<GameObject>(grid1Agent);
            
        }

        int father1 = 0, father2 = 0;
        for (int i = 0; i < totalPop; i = i + 1)
        {
            father1 = Random.Range(0, NumParents);   //mitad de población
            father2 = Random.Range(0, NumParents);

            refInd_F1 = evalAgent[father1].gameObject.GetComponent<Individual>();
            refInd_F2 = evalAgent[father2].gameObject.GetComponent<Individual>();

            refInd = Gdestino[i].GetComponent<Individual>();  // del hijo
            refInd.reset();
            int halfF = refInd.genSize / 2;
            if (refInd != null)
            {
                for (int j = 0; j < halfF; j = j + 1)
                {
                    refInd.genes[j] = refInd_F1.genes[j];
                    refInd.genes[j + halfF] = refInd_F2.genes[j + halfF];
                }


            }
        }
        // termino de asignar caracteristicas
        //limpia listas
        evalAgent.Clear(); 

        for (int k = 0; k < totalPop; k = k + 1)
        {  // activa los hijos
            Gdestino[k].SetActive(true);
            Gdestino[k].transform.position = beginPath.transform.position;

            Individual refIndTemp = Gdestino[k].gameObject.GetComponent<Individual>();
            refIndTemp.reset();
            evalAgent.Add(refIndTemp); 
            MoveVelSimple refMoveT= Gdestino[k].gameObject.GetComponent<MoveVelSimple>();
            refMoveT.OnSeek = true;
            refMoveT.TargetSeek.SetActive(true);

            // desactiva la generacion padre
        
            refIndTemp = Gorigen[k].gameObject.GetComponent<Individual>();
            refIndTemp.reset();
            Gorigen[k].SetActive(false);

            refMoveT = Gorigen[k].gameObject.GetComponent<MoveVelSimple>();
            refMoveT.OnSeek = false;
            refMoveT.TargetSeek.SetActive(false);


        }
    }
    //*********************************************************************
    public void mutation()
    {
        List<GameObject> agents;
        if (gen1)
            agents = new List<GameObject>(newAgents);
        else
            agents = new List<GameObject>(grid1Agent);

        int muta, genmuta;
        GameObject obj;
        for (int i = 0; i < (totalPop / 10); i++)
        {
            muta = Random.Range(0, totalPop);
            obj = agents[muta];
           
            objInd = obj.GetComponent<Individual>();
            genmuta = Random.Range(0, objInd.genSize);

            Vector3 genTmp = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), 0, UnityEngine.Random.Range(-1.0f, 1.0f));
            genTmp.Normalize();
            genTmp *= 5.0f;
            objInd.genes[genmuta]=genTmp;

        }

    }



}
