using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class target_cycler : MonoBehaviour {
    private List<GameObject> targetList = new List<GameObject>();
    private int currentTarget = 0;
    private Material defaultMaterial;
    private Renderer currentRenderer;
    private int maxTargets;

    private float timeStamp = 0f;
    private float targetDelay = 0.75f;
    private float powerScalar = 0.025f;

    public float targetRadius;
    public Material outlineMaterial;

    void Start () {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, targetRadius);
        for (int i = 0; i < hitColliders.Length; i++)
        {   
            if (hitColliders[i].gameObject.tag.Contains("targetable"))
            {
                targetList.Add(hitColliders[i].gameObject);
                maxTargets++;
            }
        }
    }
	
	
	void Update () {
        if (Input.GetKey(KeyCode.R))
        {    
            if (Time.time > timeStamp + targetDelay)
            {
                if (currentRenderer != null)
                {
                    currentRenderer.material = defaultMaterial;
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
}