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
    public GameObject locutorPrefab;
    public GameObject provinciaEmisoraPrefab;   //no necesita estar instanciado en unity
    public GameObject emisora2Prefab;           //no necesita estar instanciado en unity
    private List<GameObject> segmentObjects = new List<GameObject>();
    public List<Emisora> emisoras;
    public List<string> provincias;
    public Dictionary<string, List<Emisora>> emisorasxProv = new Dictionary<string, List<Emisora>>();

    public int idSegmento;
    public int idEmisora; //id en la base de datos de la emisora que se quiere obtener los segmentos actuales
    public TMP_Text titulo;
    public TMP_Text descripcion;
    public TMP_Text horarios;
    public TMP_Text noLocutores;
    public Text nombreEmisora;
    public Text ciudadEmisora;
    public Text frecuenciaEmisora;
    public RawImage segmentoImage;
    public RectTransform locutoresContainer;
    public RectTransform imagenes;
    public RectTransform contentSegmento;
    public RectTransform contentEmisorasProvincia;
    
    public GameObject EmisorasProvinciasContainer;
    public GameObject EmisoraContainer;
    public GameObject InfoSegmentoContainer;

    void Awake() {
        StartCoroutine(GetEmisoras(DisplayEmisorasxProvincia));
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

    IEnumerator GetEmisoras(Action<EmisorasModel> onSuccess)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("https://oscarp.pythonanywhere.com/api/emisoras/"))
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
                for (int i = 0; i < info.emisoras.Count; i++)
                {
                    string key = info.emisoras[i].provincia;
                    List<Emisora> value;
                    if (!emisorasxProv.TryGetValue(key, out value))
                    {
                        value = new List<Emisora>();
                        emisorasxProv[key] = value;
                    }
                    value.Add(info.emisoras[i]);
                }
                onSuccess(info);
            }
        }
    }

    public void DisplayEmisorasxProvincia(EmisorasModel info){
        emisoras = info.emisoras;
        float height  = 0f;
        foreach( KeyValuePair<string, List<Emisora>> kvp in emisorasxProv )
        {
            Debug.Log(kvp.Key);            
            GameObject provEmiItem = Instantiate(provinciaEmisoraPrefab) as GameObject;
            provEmiItem.transform.SetParent(contentEmisorasProvincia, false);
            provEmiItem.transform.Find("ProvinciaLabel").GetComponent<TextMeshProUGUI>().text = kvp.Key;
            GameObject emisorasGameObject = provEmiItem.transform.Find("Emisoras").gameObject;
            height += provEmiItem.GetComponent<RectTransform>().sizeDelta.y; /*sumar el tamaño del prefab ProvinciaEmisoras al content*/
            int cantEmisoras = kvp.Value.Count;
            float n = cantEmisoras/2;
            if (cantEmisoras%2 == 0)
            {
                n-=1;
            }
            n = n * (emisora2Prefab.GetComponent<RectTransform>().sizeDelta.y); 
            height += n; /*sumar el tamaño de los prefabs Emisora2 al content*/
            height += 50f; /*sumar el spacing entre prefabs*/
            provEmiItem.GetComponent<RectTransform>().sizeDelta = new Vector2(provEmiItem.GetComponent<RectTransform>().sizeDelta.x, provEmiItem.GetComponent<RectTransform>().sizeDelta.y + n);            
            emisorasGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(emisorasGameObject.GetComponent<RectTransform>().sizeDelta.x, emisorasGameObject.GetComponent<RectTransform>().sizeDelta.y + n);            
            foreach (var item in kvp.Value)
            {
                Debug.Log(item.nombre);
                GameObject emisora2Item = Instantiate(emisora2Prefab) as GameObject;
                emisora2Item.transform.SetParent(emisorasGameObject.transform, false);
                emisora2Item.transform.Find("emisora").GetComponent<TextMeshProUGUI>().text = item.nombre;
                emisora2Item.transform.Find("frecuencia").GetComponent<TextMeshProUGUI>().text = item.frecuencia_dial+" "+item.tipo;
                emisora2Item.transform.Find("btn").GetComponent<Button>().onClick.AddListener(() => {
                    idEmisora = item.id;
                    nombreEmisora.text = item.nombre;
                    ciudadEmisora.text = item.ciudad;
                    frecuenciaEmisora.text = item.frecuencia_dial+" "+item.tipo;
                    StartCoroutine(GetRequest(DisplaySegments));
                    EmisoraContainer.SetActive(true);
                    EmisorasProvinciasContainer.SetActive(false);
                });
                emisora2Item.SetActive(true);
            }
            provEmiItem.SetActive(true);
        }
        contentEmisorasProvincia.sizeDelta = new Vector2(contentEmisorasProvincia.sizeDelta.x, height);
    }


    IEnumerator GetRequest(Action<SegmentoModel> onSuccess)
    {
        // using (UnityWebRequest webRequest = UnityWebRequest.Get(String.Format("http://127.0.0.1:8000/api/emisoras/{0}/segmentos/today?format=json", idEmisora)))
        using (UnityWebRequest webRequest = UnityWebRequest.Get(String.Format("https://oscarp.pythonanywhere.com/api/emisoras/{0}/segmentos/today?format=json", idEmisora)))
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
                idSegmento = idActual;
                StartCoroutine(GetSegmento(MostrarSegmento));   //Mostrar información del segmento que seleccionó
                StartCoroutine(GetLocutores(MostrarLocutores)); //Mostrar locutores del segmento que seleccionó
                InfoSegmentoContainer.SetActive(true);
                EmisoraContainer.SetActive(false);
            });
            GameObject transmisionGameObject = segmentItem.transform.Find("entransmision").gameObject;
            Horario horarioSegmento = info.segmentos[i].horarios[0];
            transmisionGameObject.SetActive(isTransmiting(horarioSegmento));
            segmentObjects.Add(segmentItem);
            // TODO: ACTIVAR EL OBJETO FAVORITO DE LOS QUE EL USUARIO TIENE COMO FAVORITOS
        }

        /*Dar espaciado entre segmentos*/
        RectTransform container;
        float prefabHeight = segmentPrefab.GetComponent<RectTransform>().sizeDelta.y;
        float spacing = 1f;
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
        using (UnityWebRequest webRequest = UnityWebRequest.Get("https://oscarp.pythonanywhere.com/api/segmentos/"))
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
        using (UnityWebRequest webRequest = UnityWebRequest.Get(String.Format("https://oscarp.pythonanywhere.com/api/segmentos/{0}/locutores", idSegmento)))
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
            contentSegmento.sizeDelta = new Vector2(contentSegmento.sizeDelta.x, contentSegmento.sizeDelta.y + n);
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