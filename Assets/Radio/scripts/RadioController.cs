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
    public GameObject provinciaPrefab;
    public GameObject emisoraPrefab;

    private List<GameObject> segmentObjects = new List<GameObject>();
    public List<Emisora> emisoras;
    public List<string> provincias;

    public GameObject scrollbarProvincias;
    public GameObject scrollbarEmisoras;
    public RectTransform contentProvincias; 
    public RectTransform contentSegments; 
    public RectTransform contentEmisoras; 
    private float scroll_pos = 0;
    float[] pos;
    private int posProvincia = 0;
    private int posEmisora = 0;
    int idEmisora = 13; //id en la base de datos de la emisora que se quiere obtener los segmentos actuales

    void Awake() {
        StartCoroutine(GetRequest(DisplaySegments));
        StartCoroutine(GetEmisoras());
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        currentSegmentTransmiting();
        Swiper(contentProvincias, scrollbarProvincias, ref posProvincia, onChangeProvincia);/*paso por referencia*/
        Swiper(contentEmisoras, scrollbarEmisoras, ref posEmisora, onChangeEmisora);/*paso por referencia*/
    }

    IEnumerator GetRequest(Action<SegmentoModel> onSuccess)
    {
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

    IEnumerator GetEmisoras()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("http://oscarp.pythonanywhere.com/api/emisoras/"))
        {
            yield return webRequest.SendWebRequest();
            
            if (webRequest.isNetworkError)
            {
                Debug.Log("Resultado: " + ": Error: " + webRequest.error);
            }
            else
            {
                byte[] result = webRequest.downloadHandler.data;
                string emisorasJSON = System.Text.Encoding.Default.GetString(result);
                EmisorasModel info = JsonUtility.FromJson<EmisorasModel>("{\"emisoras\":" + emisorasJSON +"}");
                emisoras = info.emisoras;
                for (int i = 0; i < info.emisoras.Count; i++)
                {
                    if (!provincias.Contains(info.emisoras[i].provincia))
                    {
                        provincias.Add(info.emisoras[i].provincia);
                    }
                }
                displayProvincias();
                displayEmisoras();
            }
        }
    }

    public void displayProvincias(){
        // provincias = new List<string>(new string[] { "Guayas", "Carchi", "Azuay", "Cañar" });/*para pruebas*/
        for (int i = 0; i < provincias.Count; i++)
        {
            GameObject provinciaItem = Instantiate(provinciaPrefab) as GameObject;
            provinciaItem.transform.SetParent(contentProvincias.transform, false);
            provinciaItem.transform.Find("nombre").GetComponent<TextMeshProUGUI>().text = provincias[i];
            provinciaItem.SetActive(true);
        }
    }

    public void displayEmisoras(){
        string currentProvincia = provincias[posProvincia];
        for (int i = 0; i < emisoras.Count; i++)
        {
            if (emisoras[i].provincia == currentProvincia)
            {
                GameObject emisoraItem = Instantiate(emisoraPrefab) as GameObject;
                emisoraItem.transform.SetParent(contentEmisoras.transform, false);
                emisoraItem.transform.Find("NombreRadio").GetComponent<Text>().text = emisoras[i].nombre;
                emisoraItem.transform.Find("emisora").GetComponent<Text>().text = emisoras[i].frecuencia_dial+" "+emisoras[i].tipo;
                emisoraItem.SetActive(true);
                
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
            segmentItem.transform.SetParent(contentSegments.transform, false);
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
        float prefabHeight = 65f;
        float spacing = 0f;
        float containerHeight = (prefabHeight + spacing) * (segmentsCount);
        contentSegments.sizeDelta = new Vector2(contentSegments.sizeDelta.x, containerHeight);
        // Destroy(segmentPrefab);
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

    public void onChangeEmisora(){
        idEmisora = emisoras[posEmisora].id;
        segmentObjects.Clear();
        destroyAllChildrens(contentSegments);
        StartCoroutine(GetRequest(DisplaySegments));
    }

    public void onChangeProvincia(){
        destroyAllChildrens(contentEmisoras);
        displayEmisoras();
    }

    public void destroyAllChildrens(RectTransform content){
        foreach (Transform child in content.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void Swiper(RectTransform content, GameObject scroll, ref int posicion, Action funcion){
        pos = new float[content.transform.childCount];
        float distance = 1f / (pos.Length - 1f);
        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }

        if (Input.GetMouseButton(0))
        {
            scroll_pos = scroll.GetComponent<Scrollbar>().value;
        }
        else
        {
            for (int i = 0; i < pos.Length; i++)
            {
                if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
                {
                    scroll.GetComponent<Scrollbar>().value = Mathf.Lerp(scroll.GetComponent<Scrollbar>().value, pos[i], 0.1f);
                }
            }
        }


        for (int i = 0; i < pos.Length; i++)
        {
            if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
            {
                // Debug.LogWarning("Current Selected Level" + i);
                if (posicion != i)
                {/*Aquí es cuando hay un cambio de provincia*/
                    posicion = i;
                    funcion();
                }
                content.transform.GetChild(i).localScale = Vector2.Lerp(content.transform.GetChild(i).localScale, new Vector2(1.2f, 1.2f), 0.1f);
                for (int j = 0; j < pos.Length; j++)
                {
                    if (j!=i)
                    {
                        content.transform.GetChild(j).localScale = Vector2.Lerp(content.transform.GetChild(j).localScale, new Vector2(0.8f, 0.8f), 0.1f);
                    }
                }
            }
        }

    }

}
