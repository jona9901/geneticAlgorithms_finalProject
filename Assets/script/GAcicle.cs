// AI for videogames - Maria Luisa Cruz Lopez - 2022
// @author: Jonathan Castillo, Diego Iniguez, Sebastian Astiazaran

using UnityEngine;

public class GAcicle : MonoBehaviour
{
   
    Population1 pop1;
    int numGen;

    // Use this for initialization
    void Start()
    {
        numGen = 1;
         GameObject myObj = this.gameObject;
      
        pop1 = myObj.GetComponent<Population1>();
        Debug.Log("Generation " + numGen);
       
    }

    // Update is called once per frame
    void Update()
    {
        if (pop1.allArrive())
        {
            Debug.Log("termino generacion");
            pop1.selecBest();
            pop1.crossover();
            pop1.mutation();
            numGen++;

            if (pop1.gen1)
                pop1.gen1 = false;
            else
                pop1.gen1 = true;
         
            Debug.Log("Generation " + numGen);
        }
    }
}
