using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using System;

using UnityEngine.UI;

using TMPro;

public class emisoraxProvincias : MonoBehaviour
{
    public GameObject segmentPrefab;

    public int idSegmento;
    
    public RectTransform content;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetRequest(DisplaySegments));
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator GetRequest(Action<EmisorasModel> onSuccess)
    {
        int idEmisora = 13; //id en la base de datos de la emisora que se quiere obtener los emisoras actuales
        // using (UnityWebRequest webRequest = UnityWebRequest.Get(String.Format("http://127.0.0.1:8000/api/emisoras/{0}/segmentos/today?format=json", idEmisora)))
        using (UnityWebRequest webRequest = UnityWebRequest.Get("http://oscarp.pythonanywhere.com/api/emisoras"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log("Resultado: " + ": Error: " + webRequest.error);
            }
            else
            {
                byte[] result = webRequest.downloadHandler.data;
                string segmentosJSON = System.Text.Encoding.Default.GetString(result);
                EmisorasModel info = JsonUtility.FromJson<EmisorasModel>("{\"emisoras\":" + segmentosJSON + "}");
                Debug.Log(info.emisoras.Count);
                onSuccess(info);
            }
        }
    }

    public void mostrarInfoPrograma()
    {
    }

    public void DisplaySegments(EmisorasModel info)
    {

        int segmentsCount = info.emisoras.Count;
        for (int i = 0; i < segmentsCount; i++)
        {
            GameObject segmentItem = Instantiate(segmentPrefab) as GameObject;
            segmentItem.SetActive(true);
            segmentItem.transform.SetParent(segmentPrefab.transform.parent, false);
            segmentItem.transform.Find("emisora").GetComponent<TextMeshProUGUI>().text = info.emisoras[i].nombre;
            segmentItem.transform.Find("frecuencia").GetComponent<TextMeshProUGUI>().text = info.emisoras[i].frecuencia_dial + " " + info.emisoras[i].tipo;
           
            //segmentItem.transform.Find("programaBtn").GetComponent<Button>().onClick.AddListener(() => {
                
            //});
        
          
            // TODO: ACTIVAR EL OBJETO FAVORITO DE LOS QUE EL USUARIO TIENE COMO FAVORITOS
        }

        Destroy(segmentPrefab);
    }








}