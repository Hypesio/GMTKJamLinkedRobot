using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimation : MonoBehaviour
{
    public Image image;
    public Sprite[] sprites;
    public float framePerSecond = 6;
    public bool playOnAwake = false;
    public bool loop = false;

    private Sprite _startSprite;
    
    void Start()
    {
        _startSprite = image.sprite;
        if (playOnAwake)
            PlayAnimation();
    }
    
    public void PlayAnimation()
    {
        StartCoroutine(IAnimation());
    }

    public void StopAnimation()
    {
        StopAllCoroutines();
    }


    IEnumerator IAnimation()
    {
        float timeBetweenFrame = 1 / framePerSecond;

        for (int i = 0; i < sprites.Length; i++)
        {
            image.sprite = sprites[i];
            yield return new WaitForSeconds(timeBetweenFrame);
        }

        image.sprite = _startSprite;

        if (loop)
        {
            StartCoroutine(IAnimation());
        }
    }
}
