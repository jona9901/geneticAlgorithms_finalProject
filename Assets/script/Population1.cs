using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population1 : MonoBehaviour
{
    int s = 100;

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
    List<Individual> parents;


    public bool gen1 = true;
    MoveVelSimple objmove;
    Individual objInd;
    //Come objCome;

    // Crossover algorithms
    [SerializeField]
    private bool fifthyFifthy = true;
    [SerializeField]
    private bool randomMidpoint = false;
    [SerializeField]
    private bool coinFlip = false;

    // pool parent
    [Range(0, 100)]
    public int parentSelection = 25;
    [Range(0, 100)]
    public int mutationPercentage = 5;

    public int mutatedGens = 10;

    public Vector2 Vx;
    public Vector2 Vy;

    // Use this for initialization
    void Start()
    {
        Time.timeScale = 10;

        //generate the first population of the algorithm
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
        //resetBools();
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

        poolParent(distancia);

        Debug.Log(let);
    }

    public void poolParent(float[] distancia)
    {
        parents = new List<Individual>();

        List<float> d = new List<float>(distancia);
        d.Sort();
        //d.Reverse();

        float min_distance = d[parentSelection];
        

        foreach(Individual agent in evalAgent)
        {
            if (agent.distDest < min_distance) parents.Add(agent);
        }

        evalAgent = parents;

        Debug.Log("Pool parent : " + s);
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

        //int NumParents = (int)(totalPop / 4);   //mitad de población
        int NumParents = parents.Count;

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

            /*
            refInd_F1 = evalAgent[father1].gameObject.GetComponent<Individual>();
            refInd_F2 = evalAgent[father2].gameObject.GetComponent<Individual>();
            */
            
            refInd_F1 = parents[father1].gameObject.GetComponent<Individual>();
            refInd_F2 = parents[father2].gameObject.GetComponent<Individual>();

            refInd = Gdestino[i].GetComponent<Individual>();  // del hijo
            refInd.reset();
            if (refInd != null)
            {
                if (fifthyFifthy) refInd = fifthy_fifthy(refInd, refInd_F1, refInd_F2);
                else if (randomMidpoint) refInd = random_midpoint(refInd, refInd_F1, refInd_F2);
                else if (coinFlip) refInd = coin_flip(refInd, refInd_F1, refInd_F2);
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

    public Individual fifthy_fifthy(Individual refInd, Individual refInd_F1, Individual refInd_F2)
    {
        int halfF = refInd.genSize / 2;
        for (int j = 0; j < halfF; j = j + 1)
        {
            refInd.genes[j] = refInd_F1.genes[j];
            refInd.genes[j + halfF] = refInd_F2.genes[j + halfF];
        }
        return refInd;
    }

    public Individual random_midpoint(Individual refInd, Individual refInd_F1, Individual refInd_F2)
    {
        int randomMidpoint = Random.Range(0, refInd.genSize);
        for (int j = 0; j < randomMidpoint; j = j + 1)
        {
            refInd.genes[j] = refInd_F1.genes[j];
            refInd.genes[randomMidpoint - j] = refInd_F2.genes[randomMidpoint - j];
        }
        return refInd;
    }

    public Individual coin_flip(Individual refInd, Individual refInd_F1, Individual refInd_F2)
    {
        int randInt = Random.Range(0, 2);
        for (int j = 0; j < randInt; j = j + 1)
        {
            if (randInt == 0)
            {
                refInd.genes[j] = refInd_F1.genes[j];
            } else
            {
                refInd.genes[j] = refInd_F2.genes[j];
            }
            
        }
        return refInd;
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
        for (int i = 0; i < mutatedGens; i++)
        {
            muta = Random.Range(0, totalPop);
            obj = agents[muta];
           
            objInd = obj.GetComponent<Individual>();
            genmuta = Random.Range(0, objInd.genSize);

            Vector3 genTmp = new Vector3(UnityEngine.Random.Range(Vx[0], Vx[1]), 0, UnityEngine.Random.Range(Vy[0], Vy[1]));
            genTmp.Normalize();
            genTmp *= mutationPercentage;
            objInd.genes[genmuta]=genTmp;

        }

    }

    public void resetBools()
    {
        if (fifthyFifthy)
        {
            randomMidpoint = false;
            coinFlip = false;
        }
        else if (randomMidpoint)
        {
            fifthyFifthy = false;
            coinFlip = false;
        }
        else if (coinFlip)
        { 
            fifthyFifthy = false;
            randomMidpoint = false;
        }
        /*if(!fifthyFifthy && !randomMidpoint && !coinFlip)
        {
            fifthyFifthy = true;
        }
        */
    }

}
