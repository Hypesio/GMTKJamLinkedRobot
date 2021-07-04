using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GridGestion : MonoBehaviour
{
    public Transform target;
    public GameObject objectToPlace;
    public float gridSize;

    private Vector2 _truePos;
   
    void LateUpdate()
    {
        //Deplacement de l'objet tenu en main selon la position de la cible arrondit pour suivre la grille
        if (objectToPlace != null && target != null)
        {
            _truePos.x = Mathf.Floor(target.position.x / gridSize) * gridSize;
            _truePos.y = Mathf.Floor(target.position.y / gridSize) * gridSize;

            objectToPlace.transform.position = _truePos;
        }
        
    }
    
}
