﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHint : MonoBehaviour
{
    [SerializeField] HintElement[] elements;

    [SerializeField] ParticleSystem particleSystem;

    GameObject currentElement;
    private Coroutine particleStopRoutine;

    Camera cam;

    void Awake(){
        cam = FindObjectOfType<Camera>();
        for(int i=0;i<elements.Length;i++){
            elements[i].element.SetActive(false);
        }
    }
    public void Hint(Item item, Vector3 worldPos=default(Vector3))
    {
        var oldElement = currentElement;
        currentElement = null;

        if(worldPos!=default(Vector3)){
            transform.position = worldPos;
            transform.LookAt(cam.transform);
        }

        for(int i=0;i<elements.Length;i++){
            if( elements[i].item == item){
                currentElement = elements[i].element;
                break;
            }
        }

        if( currentElement ){
        
            HideAllElements();
            currentElement.SetActive(true);
            particleSystem.Play();
           // Debug.LogFormat("Start particles {0}",Time.frameCount);
            
            if(particleStopRoutine!=null)
                StopCoroutine(particleStopRoutine);
        }
        else {
            if(particleStopRoutine==null){
                particleStopRoutine = StartCoroutine(DelayedStopParticles(oldElement));
            }
            else {
                HideAllElements();
                 particleSystem.Stop();
                StopCoroutine(particleStopRoutine);
            }
            
        }
    }

    void HideAllElements(){
        for(int i=0;i<elements.Length;i++){
            elements[i].element.SetActive(false);
        }
    }

    IEnumerator DelayedStopParticles(GameObject oldElement){
        yield return new WaitForSeconds(1.2f);
        
       // Debug.LogFormat("Stop particles {0}",Time.frameCount);
        particleSystem.Stop();

        yield return new WaitForSeconds(2.2f);

        if(oldElement){
            oldElement.SetActive(false);
        }

        particleStopRoutine = null;

    }

    [System.Serializable]
    public class HintElement {
        public Item item;
        public GameObject element;
    }
}
