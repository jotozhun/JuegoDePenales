using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

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
    private BoxCollider targetCollider;
    public bool isTargetPractice;

    [Header("TargetPoints")]
    public GameObject pointsObject;
    public TextMeshProUGUI puntajeJugadorText;
    public TextMeshProUGUI puntajeObjetivoText;
    public int puntajeJugador;
    public int puntajeObjetivoLevel1;
    public int puntajeObjetivoLevel2;

    [Header("TargetAttempts")]
    public GameObject attemptsObject;
    public TextMeshProUGUI intentoJugadorText;
    public TextMeshProUGUI cantidadIntentosText;
    public int intentosJugador;
    public int cantidadIntentosLevel1;
    public int cantidadIntentosLevel2;

    public GameObject score1Object;
    public GameObject score2Object;

    [Header("Messages")]
    public GameObject allPoints;
    public GameObject gameOver;


    private SpriteRenderer spriteRenderer;
    public void Start()
    {
        //timer = timerStatic;
        spriteRenderer = GameUIPractice.instance.target.GetComponent<SpriteRenderer>();
        targetCollider = GameUIPractice.instance.target.GetComponent<BoxCollider>();
        level1 = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            puntajeJugador++;
            intentosJugador++;
            actualizarUIText();
            if (level1)
            {
                
                randPos = new Vector3(Random.Range(-92.49f, -79.8f), Random.Range(0.25f, -2.11f), -66f);
            }
            else
            {
                randPos = new Vector3(Random.Range(-93.07f, -79.28f), Random.Range(0.65f, -2.67f), -66f);
            }
            spriteRenderer.transform.position = randPos;
            //Debug.Log(randPos);
        }
    }

    public void OnLevel1()
    {
        modeLevelTarget();
        GameUIPractice.instance.target.SetActive(true);
        level1 = true;
        puntajeObjetivoText.text = puntajeObjetivoLevel1.ToString();
        cantidadIntentosText.text = cantidadIntentosLevel1.ToString();
        spriteRenderer.size = new Vector2(2.4f, 2.4f);
        targetCollider.size = new Vector3(2f, 2f, 0.1f);
        puntajeJugador = 0;
        intentosJugador = 0;
        actualizarUIText();
        isTargetPractice = true;
    }
    public void OnLevel2()
    {
        modeLevelTarget();
        GameUIPractice.instance.target.SetActive(true);
        level1 = false;
        puntajeObjetivoText.text = puntajeObjetivoLevel2.ToString();
        cantidadIntentosText.text = cantidadIntentosLevel2.ToString();
        spriteRenderer.size = new Vector2(1.5f, 1.5f);
        targetCollider.size = new Vector3(1f, 1f, 0.1f);
        puntajeJugador = 0;
        intentosJugador = 0;
        actualizarUIText();
        isTargetPractice = true;
    }
    public void OnLevelNormal()
    {
        pointsObject.SetActive(false);
        attemptsObject.SetActive(false);
        score1Object.SetActive(true);
        score2Object.SetActive(true);
        GameUIPractice.instance.target.SetActive(false);
        isTargetPractice = false;
    }

    public void modeLevelTarget()
    {
        pointsObject.SetActive(true);
        attemptsObject.SetActive(true);
        score1Object.SetActive(false);
        score2Object.SetActive(false);
    }

    public void actualizarUIText()
    {
        puntajeJugadorText.text = puntajeJugador.ToString();
        intentoJugadorText.text = intentosJugador.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        /*
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
        }*/
        spriteRenderer.sprite = sprite;
        if (puntajeJugador==puntajeObjetivoLevel1 || puntajeJugador==puntajeObjetivoLevel2)
        {
            StartCoroutine(showaAllPoints());
        }
        else if ((intentosJugador==cantidadIntentosLevel1 && puntajeJugador<puntajeObjetivoLevel1) || (intentosJugador==cantidadIntentosLevel2 && puntajeJugador<puntajeObjetivoLevel2))
        {
            StartCoroutine(showGameOver());
        }

    }
    IEnumerator showaAllPoints()
    {
        allPoints.SetActive(true);
        yield return new WaitForSeconds(4f);
        allPoints.SetActive(false);
        puntajeJugador = 0;
        intentosJugador = 0;
        actualizarUIText();
    }
    IEnumerator showGameOver()
    {
        gameOver.SetActive(true);
        yield return new WaitForSeconds(4f);
        gameOver.SetActive(false);
        puntajeJugador = 0;
        intentosJugador = 0;
        actualizarUIText();
    }

}
