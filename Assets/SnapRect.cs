using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnapRect : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform content;
    public Scrollbar scrollbar;
    public float[] pos;
    float distance;
    float oldpos;
    int currentPos;

    public Button next;
    public Button back;

    void Start()
    {
        distance = 0.5f; // 1f / (content.childCount -1) = 1/3-2= 0.5
        pos = new float[3];

        for (int i = 0; i < 3 ; i++) // content.childCount = elementos de mi objeto
        {
            pos[i] = distance * i;
        }

        next.onClick.AddListener(() => {
            StartCoroutine(nextbtn());
        });

        back.onClick.AddListener(() => {
            StartCoroutine(backbtn());
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            oldpos = scrollbar.value;
            Debug.Log("El oldpos es:"+ oldpos);
            Debug.Log("El currentpos es:" + currentPos);
            Debug.Log("La distancia es: " + distance);
            Debug.Log("El childcount :" + content.childCount);
            Debug.Log("el valor del scroll:" + scrollbar.value);
        }
        else
        {
            for (int i = 0; i < pos.Length; i++)
            {
                if (oldpos < pos[i] + (distance / 2) && oldpos > pos[i] - (distance / 2))
                {
                    scrollbar.value = Mathf.Lerp(scrollbar.value, pos[i], 0.3f);
                    currentPos = i;
                }
            }
        }
    }

    IEnumerator nextbtn()
    {
        while ((scrollbar.value < pos[currentPos + 1] - 0.2f))
        {
            if(scrollbar.value > 0.9f)
            {
                scrollbar.value = 0;
                currentPos = 0;
            }
            else
            {
                scrollbar.value = Mathf.Lerp(scrollbar.value, pos[currentPos + 1], 0.3f);
            }
            
        }
        oldpos = scrollbar.value;
        yield return null;
    }

    IEnumerator backbtn()
    {
        while ((scrollbar.value > pos[currentPos - 1] + 0.2f) && scrollbar.value > 0)
        {
            scrollbar.value = Mathf.Lerp(scrollbar.value, pos[currentPos - 1], 0.3f);
        }
        oldpos = scrollbar.value;
        yield return null;
    }
}