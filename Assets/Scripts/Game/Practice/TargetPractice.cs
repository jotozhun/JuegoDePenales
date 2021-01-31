using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TargetPractice : MonoBehaviour
{
    [Header("Target")]
    /*float deltaTime = 0.0f;
    public float timerStatic = 5.0f;
    public float timer = 5.0f;
    public bool timer1IsRunning = true;*/
    public Sprite sprite;
    private Vector3 randPos;
    private bool level1;
    private bool level2;
    private BoxCollider targetCollider;
    public bool isTargetPractice;

    private SpriteRenderer spriteRenderer;
    private void Start()
    {
        //timer = timerStatic;
        spriteRenderer = GameUIPractice.instance.target.GetComponent<SpriteRenderer>();
        targetCollider = GameUIPractice.instance.target.GetComponent<BoxCollider>();
        spriteRenderer.sprite = sprite;
        //spriteRenderer.enabled = false;
        level1 = false;
        level2 = false;
        isTargetPractice = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            if (level1)
                randPos = new Vector3(Random.Range(-92.49f, -79.8f), Random.Range(0.25f, -2.11f), -66f);
            else
                randPos = new Vector3(Random.Range(-93.07f, -79.28f), Random.Range(0.65f, -2.67f), -66f);
            spriteRenderer.transform.position = randPos;
            //Debug.Log(randPos);
        }
    }

    public void OnLevel1( )
    {
        GameUIPractice.instance.target.SetActive(true);
        level1 = true;
        level2 = false;
        isTargetPractice = true;
        //spriteRenderer.enabled = true;
        spriteRenderer.size = new Vector2(2.4f, 2.4f);
        targetCollider.size = new Vector3(2f, 2f, 0.1f);

    }
    public void OnLevel2()
    {
        GameUIPractice.instance.target.SetActive(true);
        level2 = true;
        level1 = false;
        isTargetPractice = true;
        //spriteRenderer.enabled = true;
        spriteRenderer.size = new Vector2(1.5f, 1.5f);
        targetCollider.size = new Vector3(1f, 1f, 0.1f);
    }
    public void OnLevelNormal()
    {
        GameUIPractice.instance.target.SetActive(false);
        isTargetPractice = false;
    }
    // Update is called once per frame
    /*void Update()
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
                spriteRenderer.sprite = sprites;
                spriteRenderer.size = new Vector2(10.8f, 2f);
                timer = timerStatic;
            }
        }

    }*/
}
