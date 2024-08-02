using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReciteSpell : MonoBehaviour
{
    [SerializeField] protected float timeToFade = 2f;
    [SerializeField] protected float timeOfExistence = 5f;
    [SerializeField] protected float timer = 0f;

    protected SpriteRenderer spriteRenderer;
    protected Color startColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startColor = spriteRenderer.color;
    }


    void Start()
    {
        timer = timeOfExistence;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }

        if (timer <= timeToFade)
        {
            Fading();
        }

    }

    protected void Fading()
    {
        float newApha = startColor.a * (timer / timeToFade);
        spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, newApha);
    }
}
