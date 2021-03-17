using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AddPractice : MonoBehaviour
{
    private int currentImage;

    float deltaTime = 0.0f;

    public float timerStatic = 5.0f;
    public float timer = 5.0f;
    public bool timer1IsRunning = true;
    public Sprite[] sprites;

    SpriteRenderer spriteRenderer;
    void Start()
    {
        currentImage = 0;
        timer = timerStatic;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {

        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        if (timer1IsRunning)

        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;

            }

            else
            {
                //Debug.Log("Time has run out!");
                currentImage++;

                if (currentImage >= sprites.Length)
                    currentImage = 0;
                spriteRenderer.sprite = sprites[currentImage];
                spriteRenderer.size = new Vector2(10.8f, 2f);
                timer = timerStatic;
            }
        }

    }
}
