using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using System;

using UnityEngine.UI;

using TMPro;

public class ProgramacionController : MonoBehaviour
{
    public GameObject segmentPrefab;
    private List<GameObject> segmentObjects = new List<GameObject>();

    public int idSegmento;
    public TMP_Text titulo;
    public TMP_Text descripcion;
    public TMP_Text horarios;
    public TMP_Text noLocutores;
    public RawImage segmentoImage;
    public RectTransform locutoresContainer;
    public RectTransform imagenes;
    public RectTransform content;
    public GameObject locutorPrefab;
    public GameObject objetoInfoPrograma;

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

    public void mostrarInfoPrograma(){
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
            int idActual = info.segmentos[i].id;
            string horarioFormat = info.segmentos[i].horarios[0].fecha_inicio.Substring(0, 5) + " - " + info.segmentos[i].horarios[0].fecha_fin.Substring(0, 5);
            segmentItem.transform.Find("horario").GetComponent<TextMeshProUGUI>().text = horarioFormat;
            segmentItem.transform.Find("programaBtn").GetComponent<Button>().onClick.AddListener(() => {
                objetoInfoPrograma.SetActive(true);
                idSegmento = idActual;
                StartCoroutine(GetSegmento(MostrarSegmento));
                StartCoroutine(GetLocutores(MostrarLocutores));
            });
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

     IEnumerator GetSegmento(Action<Segmento> onSuccess)
    {
        //TODO hacer un endpoint para solo consultar por un segmento por su id, con este endpoint se obtienen todos los segmentos
        using (UnityWebRequest webRequest = UnityWebRequest.Get("http://oscarp.pythonanywhere.com/api/segmentos/"))
        {
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
                for (int i = 0; i < info.segmentos.Count; i++)
                {
                    if (info.segmentos[i].id == idSegmento)
                    {                        
                        onSuccess(info.segmentos[i]);
                        break;
                    }
                }
            }
        }
    }

    public void MostrarSegmento(Segmento segmento){
        titulo.text = segmento.nombre;
        descripcion.text = segmento.descripcion;
        horarios.text = segmento.HorariosToString();
        StartCoroutine(DownloadImage(segmento.imagen, segmentoImage));
        //TODO reconocer overflow y aumentar tamaño
    }

    IEnumerator DownloadImage(string url, RawImage imagen) {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            imagen.texture = myTexture;
        }
    }

    IEnumerator GetLocutores(Action<LocutoresModel> onSuccess)
    {
        //TODO hacer un endpoint para solo consultar por un segmento por su id, con este endpoint se obtienen todos los segmentos
        using (UnityWebRequest webRequest = UnityWebRequest.Get(String.Format("http://oscarp.pythonanywhere.com/api/segmentos/{0}/locutores", idSegmento)))
        {
            yield return webRequest.SendWebRequest();
            
            if (webRequest.isNetworkError)
            {
                Debug.Log("Resultado: " + ": Error: " + webRequest.error);
            }
            else
            {
                byte[] result = webRequest.downloadHandler.data;
                string locutoresJSON = System.Text.Encoding.Default.GetString(result);
                LocutoresModel info = JsonUtility.FromJson<LocutoresModel>("{\"locutores\":" + locutoresJSON +"}");
                onSuccess(info);
            }
        }
    }

    public void MostrarLocutores(LocutoresModel info){
        int cantLocutores = info.locutores.Count;
        if (cantLocutores == 0)
        {
            Destroy(locutorPrefab);
            noLocutores.gameObject.SetActive(true);
        }else
        {
            // locutoresContainer
            // imagenes
            float n = cantLocutores/2;
            if (cantLocutores%2 == 0)
            {
                n-=1;
            }
            n = n * (350f);
            // locutoresContainer.sizeDelta = new Vector2(locutoresContainer.sizeDelta.x, locutoresContainer.sizeDelta.y + n);
            // imagenes.sizeDelta = new Vector2(imagenes.sizeDelta.x, imagenes.sizeDelta.y + n);
            content.sizeDelta = new Vector2(content.sizeDelta.x, content.sizeDelta.y + n);
            for (int i = 0; i < cantLocutores; i++)
            {  
                GameObject locutorItem = Instantiate(locutorPrefab) as GameObject;
                locutorItem.transform.Find("nombre").GetComponent<TextMeshProUGUI>().text = info.locutores[i].NombresCompletos();
                RawImage imagenLocutor = locutorItem.transform.Find("imagen").GetComponent<RawImage>();
                StartCoroutine(DownloadImage(info.locutores[i].imagen, imagenLocutor));
                locutorItem.SetActive(true);
                locutorItem.transform.SetParent(locutorPrefab.transform.parent, false);
            }
            Destroy(locutorPrefab);
        }
    }

}