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
    public float contadorTiempo;
    float distance;
    float oldpos;
    int currentPos;
    public Button next;
    public Button back;
    bool ejecutarBackBtn;
    bool ejecutarNextBtn = true;

    void Start()
    {
        
        distance = 1f / (content.childCount -1); // 1f / (content.childCount -1) = 1/3-2= 0.5
        pos = new float[3];

        for (int i = 0; i < content.childCount ; i++) // content.childCount = elementos de mi objeto
        {
            pos[i] = distance * i;
        }

        next.onClick.AddListener(() => {
            StartCoroutine(nextbtn());
        });

        back.onClick.AddListener(() => {
            StartCoroutine(backbtn());
        });

        if(contadorTiempo > 1.0){
            StartCoroutine(nextbtn());
        }
    }

    // Update is called once per frame
    void Update()
    {
        contadorTiempo += Time.deltaTime;
         if(contadorTiempo > 3f && ejecutarNextBtn){
            StartCoroutine(nextbtn());
            contadorTiempo = 0;
         }else if(contadorTiempo > 3f && ejecutarBackBtn){
            StartCoroutine(backbtn());
            contadorTiempo = 0;
         }

        if (Input.GetMouseButton(0))
        {
            oldpos = scrollbar.value;
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
        if(currentPos != pos.Length -1){
            while ((scrollbar.value < pos[currentPos + 1] - 0.2f))
            {
                {
                    scrollbar.value = Mathf.Lerp(scrollbar.value, pos[currentPos + 1], 0.3f);
                }
            
            }
            oldpos = scrollbar.value;
            yield return null;
        }else{
            StartCoroutine(backbtn());
            ejecutarBackBtn = true;
            ejecutarNextBtn = false;
        }
    }

    IEnumerator backbtn()
    {
        if(currentPos != 0 ){
            while (scrollbar.value > pos[currentPos - 1] + 0.2f)
            {
                scrollbar.value = Mathf.Lerp(scrollbar.value, pos[currentPos - 1], 0.3f);
            }
            oldpos = scrollbar.value;
            yield return null;
        }else{
            ejecutarBackBtn = false;
            ejecutarNextBtn = true;
        }
    }


}