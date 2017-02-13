﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireController : MonoBehaviour
{
    //Raycast Variables
    [Range(5.0f, 40f)]
    public float maxRange = 25f;
    public Transform rayOrigin;
    //public GameObject tracerEffect; - Particles

    private LineRenderer tracerLine;
    private Camera playerCam;
    private float nextFire;
    private WaitForSeconds shotDuration = new WaitForSeconds(0.05f);
    private RaycastHit hit; //holds info about anything hit by ray casted
    //private float hitForce = 250f; //for debugging

    //Targetting Variables
    public Material outlineMaterial;
    private Material defaultMaterial;
    private Renderer currentRenderer;
    private GameObject currentTarget;
    private int layerMask = 0;

    //Power-Related Variables
    [Range(0.01f, 0.1f)]
    public float powerScalar = 0.025f; //Scaling rate

    void Start()
    {
        //Note: script should be in a child to the player character's camera object
        playerCam = GetComponentInParent<Camera>();
        tracerLine = GetComponentInChildren<LineRenderer>();
        layerMask = (1 << LayerMask.NameToLayer("Interactable")); //Raycast bit mask by shifting index of 'Interactable' layer
    }

    void FixedUpdate()
    {
        //Clear Target if out of range -- Removing when it leaves FoV doesn't feel fun/gud
        if (currentTarget != null && (currentTarget.transform.position - transform.position).magnitude > maxRange)
        {
            ClearTarget(currentTarget);
        }

        //Targetting & Scaling
        if ( (Input.GetButton("Fire1") | Input.GetButton("Fire2"))) 
        {
            //nextFire = Time.time + fireRate; 

            //Renders a line, add in spell effect when/if ready, remove for now b/c Ugly
            //StartCoroutine(ShotEffect());

            /* translates where the point is in the viewport to the world coordinate system
            top left = 0, top right = 1, bot left = 1
            translate middle of viewport in x and y to point in world coordinates */
            Vector3 rayOrigin = playerCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

            //Set LineRenderer position to public origin transform
            //tracerLine.SetPosition(0, this.rayOrigin.position); b/c Ugly

            //Ray registers a hit with an Interactable object
            if (Physics.Raycast(rayOrigin, playerCam.transform.forward, out hit, maxRange, layerMask) && hit.rigidbody.gameObject.tag.Contains("Interactable"))
            {
                //tracerLine.SetPosition(1, hit.point); b/c Ugly
                if (hit.rigidbody != null)
                {
                    //For debugging purposes: hit obj @ normal to surface hit
                    //hit.rigidbody.AddForce(-hit.normal * hitForce);
                    
                    //Selecting/Lock on to new target
                    if (currentTarget != hit.rigidbody.gameObject)
                    {
                        SelectTarget(hit.rigidbody.gameObject);
                    } else if (Input.GetButton("Fire1"))
                    {
                        ScaleObject(hit.rigidbody.gameObject, 1 + powerScalar);
                    } else if (Input.GetButton("Fire2"))
                    {
                        ScaleObject(hit.rigidbody.gameObject, 1 - powerScalar);
                    }     
                }
            }
            else if (currentTarget != null)
            {
                //Just render the line of length maxRange
                //tracerLine.SetPosition(1, rayOrigin + (playerCam.transform.forward * maxRange)); b/c Ugly
                if (Input.GetButton("Fire1"))
                {
                    ScaleObject(currentTarget, 1 + powerScalar);
                }
                else if (Input.GetButton("Fire2"))
                {
                    ScaleObject(currentTarget, 1 - powerScalar);
                }
            }
        }
    }

    /// <summary>
    /// Highlights targetInteractable by changing Renderer material to outlineMaterial and sets curerntTarget object
    /// </summary>
    /// <param name="targetInteractable"></param>
    void SelectTarget(GameObject targetInteractable)
    {
        if (currentRenderer != null)
        {
            currentRenderer.material = defaultMaterial;
        }
        currentRenderer = targetInteractable.GetComponent<Renderer>();
        defaultMaterial = currentRenderer.material;
        currentRenderer.material = outlineMaterial;
        currentTarget = targetInteractable;
    }

    /// <summary>
    /// Clear the current target from lock on, remove visual indication & unset currentTarget
    /// </summary>
    /// <param name="targetInteractable"></param>
    void ClearTarget(GameObject targetInteractable)
    {
        currentRenderer = targetInteractable.GetComponent<Renderer>();
        currentRenderer.material = defaultMaterial;
        currentTarget = null;
    }

    /// <summary>
    /// Scale the target gameObject based on given scalar rate in the dimension specified by the object tag.
    /// Scaling is a muliplication operation to create some input acceleration (smaller: more granular, larger: quicker scaling)
    /// </summary>
    /// <param name="targetInteractable"></param>
    void ScaleObject(GameObject targetInteractable, float scaleRate)
    {
        Vector3 curScale = targetInteractable.transform.localScale;
        //TODO: Cap on max scale
        //TODO: Anchored Scaling
        if (targetInteractable.tag.Contains("XScalable"))
        {
            targetInteractable.transform.localScale = new Vector3(curScale.x * scaleRate, curScale.y, curScale.z);
        }
        else if (targetInteractable.tag.Contains("YScalable"))
        {
            targetInteractable.transform.localScale = new Vector3(curScale.x, curScale.y * scaleRate, curScale.z);
        }
        else if (targetInteractable.tag.Contains("ZScalable"))
        {
            targetInteractable.transform.localScale = new Vector3(curScale.x, curScale.y, curScale.z * scaleRate);
        } else
        {
            targetInteractable.transform.localScale = new Vector3(curScale.x * scaleRate, curScale.y * scaleRate, curScale.z * scaleRate);
        }
    }

    /// <summary>
    /// //coroutine (think separate thread) to render tracer effect for shotDuration seconds
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShotEffect()
    {
        //Show tracer line for shotDuration seconds
        //Instantiate(tracerEffect, transform.position, Quaternion.identity); -- particles
        tracerLine.enabled = true;
        yield return shotDuration;
        tracerLine.enabled = false;
    }
}