using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class target_inview : MonoBehaviour {
    private List<GameObject> targetList = new List<GameObject>();
    private int currentTarget = 0;
    private Material defaultMaterial;
    private Renderer currentRenderer;
    private int maxTargets;

    private float timeStamp = 0f;
    private float targetDelay = 0.25f;
    private float powerScalar = 0.025f;

    public float targetRadius;
    public Material outlineMaterial;

    void Start () {
        //Runs before Renders actually render anything so will be empty
        //getObjects();
    }
	
	
	void Update () {
        //Target Cycling
        if (Input.GetKey(KeyCode.R))
        {   
            if (Time.time > timeStamp + targetDelay)
            {   
                getObjectsInView();
                //if updating target list every time, avoid indexing non existent unit
                if (targetList.Count - 1 < currentTarget) {
                    return;
                } 
                if (currentRenderer != null)
                {
                    currentRenderer.material = defaultMaterial;
                }

                if (targetList[currentTarget].GetComponent<Renderer>() == currentRenderer)
                {
                    Debug.Log("Wasted Keypress, Targeted Same Object");
                    //this will index out of range, but don't target the same thing twice
                    //currentTarget++;
                    //should select from other available objects instead of this
                }

                currentRenderer = targetList[currentTarget].GetComponent<Renderer>();
                defaultMaterial = currentRenderer.material;
                currentRenderer.material = outlineMaterial;
                currentTarget++;
                
                if (currentTarget >= maxTargets)
                {
                    currentTarget = 0;
                }
                timeStamp = Time.time;
            }
        }

        //Scale Up
        if (Input.GetKey(KeyCode.F))
        {
            if (currentRenderer != null)
            {
                Vector3 curScale = currentRenderer.gameObject.transform.localScale;
                if (currentRenderer.gameObject.tag == "targetable_vert")
                {
                    currentRenderer.gameObject.transform.localScale = new Vector3(curScale.x,curScale.y * (1 + powerScalar),curScale.z);
                } else if (currentRenderer.gameObject.tag == "targetable_hor")
                {
                    currentRenderer.gameObject.transform.localScale = new Vector3(curScale.x, curScale.y, curScale.z * (1 + powerScalar));
                } else
                {
                    currentRenderer.gameObject.transform.localScale = currentRenderer.gameObject.transform.localScale * (1 + powerScalar);
                }
            }
        }

        //Scale Down
        if (Input.GetKey(KeyCode.G))
        {
            Vector3 curScale = currentRenderer.gameObject.transform.localScale;
            if (currentRenderer != null)
            {
                if (currentRenderer.gameObject.tag == "targetable_vert")
                {
                    currentRenderer.gameObject.transform.localScale = new Vector3(curScale.x, curScale.y * (1 - powerScalar), curScale.z);
                } else if (currentRenderer.gameObject.tag == "targetable_hor")
                {
                    currentRenderer.gameObject.transform.localScale = new Vector3(curScale.x, curScale.y, curScale.z * (1 - powerScalar));
                } else 
                {
                    currentRenderer.gameObject.transform.localScale = currentRenderer.gameObject.transform.localScale * (1 - powerScalar);
                }
            }
        }
    }

    void getObjectsInView()
    {
        maxTargets = 0;
        targetList.Clear(); //Should be returning a new List<> instead of using a global >.>
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, targetRadius);
        for (int i = 0; i < hitColliders.Length; i++)
        {   
            if (hitColliders[i].gameObject.tag.Contains("targetable"))
            {
                Renderer colRenderer = hitColliders[i].gameObject.GetComponentInParent<Renderer>();
                if (colRenderer.isVisible)
                {
                    targetList.Add(hitColliders[i].gameObject);
                    maxTargets++;
                }
            }
        }
    }
}