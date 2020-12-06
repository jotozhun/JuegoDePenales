using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using System;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InfoSegmento : MonoBehaviour
{

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



    private void Awake() {
        StartCoroutine(GetSegmento(MostrarSegmento));
        StartCoroutine(GetLocutores(MostrarLocutores));
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
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
