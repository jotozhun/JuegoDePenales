using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using System;

using UnityEngine.UI;

using TMPro;

public class RadioController : MonoBehaviour
{
    public GameObject segmentPrefab;

    private List<GameObject> segmentObjects = new List<GameObject>();

    void Awake() {
        StartCoroutine(GetRequest(DisplaySegments));
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        currentSegmentTransmiting();
    }

    IEnumerator GetRequest(Action<SegmentoModel> onSuccess)
    {
        int idEmisora = 13; //id en la base de datos de la emisora que se quiere obtener los segmentos actuales
        // using (UnityWebRequest webRequest = UnityWebRequest.Get(String.Format("http://127.0.0.1:8000/api/emisoras/{0}/segmentos/today?format=json", idEmisora)))
        using (UnityWebRequest webRequest = UnityWebRequest.Get(String.Format("http://oscarp.pythonanywhere.com/api/emisoras/{0}/segmentos/today?format=json", idEmisora)))
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
                SegmentoModel info = JsonUtility.FromJson<SegmentoModel>("{\"segmentos\":" + segmentosJSON +"}");
                onSuccess(info);
            }
        }
    }

    public void DisplaySegments(SegmentoModel info){

        int segmentsCount = info.segmentos.Count;
        /*Instanciar prefabs*/
        for (int i = 0; i < segmentsCount; i++)
        {
            GameObject segmentItem = Instantiate(segmentPrefab) as GameObject;
            segmentItem.SetActive(true);
            segmentItem.transform.SetParent(segmentPrefab.transform.parent, false);
            segmentItem.transform.Find("programa").GetComponent<TextMeshProUGUI>().text = info.segmentos[i].nombre;
            string horarioFormat = info.segmentos[i].horarios[0].fecha_inicio.Substring(0, 5) + " - " + info.segmentos[i].horarios[0].fecha_fin.Substring(0, 5);
            segmentItem.transform.Find("horario").GetComponent<TextMeshProUGUI>().text = horarioFormat;
            segmentItem.transform.Find("emisora").GetComponent<TextMeshProUGUI>().text = info.segmentos[i].emisora.nombre;
            segmentItem.transform.Find("dias").GetComponent<TextMeshProUGUI>().text = info.segmentos[i].horarios[0].dia;
            GameObject transmisionGameObject = segmentItem.transform.Find("entransmision").gameObject;
            Horario horarioSegmento = info.segmentos[i].horarios[0];
            transmisionGameObject.SetActive(isTransmiting(horarioSegmento));
            segmentObjects.Add(segmentItem);
            // TODO: ACTIVAR EL OBJETO FAVORITO DE LOS QUE EL USUARIO TIENE COMO FAVORITOS
        }

        /*Dar espaciado entre segmentos*/
        RectTransform container;
        float prefabHeight = 65f;
        float spacing = 0f;
        float containerHeight = (prefabHeight + spacing) * (segmentsCount);
        container = (RectTransform)segmentPrefab.transform.parent;
        container.sizeDelta = new Vector2(container.sizeDelta.x, containerHeight);
        Destroy(segmentPrefab);
    }

    public Boolean isTransmiting(Horario horario)
    {
        int[] horaInicio = {int.Parse( horario.fecha_inicio.Substring(0,2)), int.Parse( horario.fecha_inicio.Substring(3,2)), int.Parse( horario.fecha_inicio.Substring(6,2))};
        int[] horaFin = {int.Parse( horario.fecha_fin.Substring(0,2)), int.Parse( horario.fecha_fin.Substring(3,2)), int.Parse( horario.fecha_fin.Substring(6,2))};

        DateTime currentTime = System.DateTime.Now;
        DateTime inicio = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, horaInicio[0], horaInicio[1], horaInicio[2]);
        DateTime fin = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, horaFin[0], horaFin[1], horaFin[2]);
    
        int result1 = DateTime.Compare(currentTime, inicio);    //>=0
        int result2 = DateTime.Compare(currentTime, fin);    //<0

        return (result1 >= 0 && result2 < 0);
    }

    public Boolean isTransmiting(String horario)
    {
        //14:30 - 16:00
        int[] horaInicio = {int.Parse( horario.Substring(0,2)), int.Parse( horario.Substring(3,2)), 0};
        int[] horaFin = {int.Parse( horario.Substring(8,2)), int.Parse( horario.Substring(11,2)), 0};

        DateTime currentTime = System.DateTime.Now;
        DateTime inicio = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, horaInicio[0], horaInicio[1], horaInicio[2]);
        DateTime fin = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, horaFin[0], horaFin[1], horaFin[2]);
    
        int result1 = DateTime.Compare(currentTime, inicio);    //>=0
        int result2 = DateTime.Compare(currentTime, fin);    //<0

        return (result1 >= 0 && result2 < 0);
    }

    void currentSegmentTransmiting()
    {
        /*Recorre los prefabs de segmentos y activa el mensaje de transmitiendo si la hora actual está dentro del horario de transmisión del segmento*/
        for (int i = 0; i < segmentObjects.Count; i++)
        {
            GameObject g = segmentObjects[i];
            String horario = g.transform.Find("horario").GetComponent<TextMeshProUGUI>().text;
            GameObject transmisionGameObject = g.transform.Find("entransmision").gameObject;
            transmisionGameObject.SetActive(isTransmiting(horario));
        }

    }

}
