using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLayer : MonoBehaviour
{
    public int layerWanted;

    private int _originalLayer;
    private SpriteRenderer _spriteRenderer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        _spriteRenderer = other.GetComponent<SpriteRenderer>();
        _originalLayer = _spriteRenderer.sortingOrder;
        _spriteRenderer.sortingOrder = layerWanted;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _spriteRenderer.sortingOrder = _originalLayer;
    } 
}
