using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using AccountModels;
using TMPro;
using System;

public class NetworkAPICalls : MonoBehaviour
{
    [Header("UI elements")]
    public TextMeshProUGUI loginStatus;
    public TextMeshProUGUI registerStatus;

    
    public NetworkManager manager;
    public static NetworkAPICalls instance;

    private void Awake()
    {
        instance = this;
    }


    public IEnumerator CallUserToken(string username, string password, Action<string> res, Action<int> err)
    {
        string testUri = "https://willymedi.pythonanywhere.com/usuarios/usuario/auth/";
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(testUri, form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                err(600);
            }
            else
            {
                if (webRequest.responseCode == 200)
                {
                    string resp = webRequest.downloadHandler.text;
                    res(resp);
                }
                else
                {
                    err((int)webRequest.responseCode);
                }
            }
        }
    }

    public IEnumerator GetLoginInfo(int id, string token, Action<string> res, Action<int> err)
    {
        string testUri = "https://willymedi.pythonanywhere.com/usuarios/usuario/" + id;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(testUri))
        {
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", "Bearer " + token);
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                err(600);
            }
            else
            {
                if (webRequest.responseCode == 200)
                {
                    string resp = webRequest.downloadHandler.text;
                    res(resp);
                }
                else
                {
                    err((int)webRequest.responseCode);
                }
            }
        }
    }

    public IEnumerator UpdateInterfazData(int emblemaIndex, int haircut_kicker, int kicker_index, int id, string token)
    {
        string testUri = "https://willymedi.pythonanywhere.com/usuarios/usuario/" + id;
        InterfazInfo interfazInfo = new InterfazInfo { emblema = emblemaIndex, haircut_player = haircut_kicker, player = kicker_index };
        string rawInterfazInfo = JsonUtility.ToJson(interfazInfo);
        using (UnityWebRequest webRequest = UnityWebRequest.Put(testUri, System.Text.Encoding.UTF8.GetBytes(rawInterfazInfo)))
        {
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", "Bearer " + token);
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {

            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
            }
        }
    }

    public IEnumerator AddMatchResultsToPlayer(bool isWin, int id, int goles_anotados, int goles_atajados, int goles_recibidos, string token)
    {
        string testUri = "https://willymedi.pythonanywhere.com/usuarios/usuario/" + id;

        //manager.AddLocalMatchResult(isWin, goles_anotados, goles_atajados, goles_recibidos);

        UserLogin tmpUser = manager.userLogin;

        MatchResult matchResult = new MatchResult() {
            total_partidos = tmpUser.total_partidos,
            partidos_ganados = tmpUser.partidos_ganados,
            partidos_perdidos = tmpUser.partidos_perdidos,
            goles_anotados = tmpUser.goles_anotados,
            goles_atajados = tmpUser.goles_atajados,
            goles_recibidos = tmpUser.goles_recibidos
        };

        string rawMatchResult = JsonUtility.ToJson(matchResult);
        using (UnityWebRequest webRequest = UnityWebRequest.Put(testUri, System.Text.Encoding.UTF8.GetBytes(rawMatchResult)))
        {
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", "Bearer " + token);
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {

            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
            }
        }
    }

    public IEnumerator RegisterUser(string name, string username, string email, string password, Action<string> res, Action<int> err)
    {
        string testUri = "https://willymedi.pythonanywhere.com/usuarios/register/";

        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("username", username);
        form.AddField("email", email);
        form.AddField("password", password);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(testUri, form))
        {
            yield return webRequest.SendWebRequest();
            if(webRequest.isNetworkError)
            {
                Debug.Log("Estoy en un error");
                err(600);
            }
            else
            {
                Debug.Log("HOLI");
                if(webRequest.responseCode == 200)
                {
                    res("Usuario Registrado Correctamente!");
                }
                else
                {
                    err((int)webRequest.responseCode);
                }
            }

        }
    }

    public IEnumerator GetPublicidad(Action<string> res, Action<int> err)
    {
        string publicidadUri = "https://willymedi.pythonanywhere.com/publicidad/publicidad/";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(publicidadUri))
        {
            yield return webRequest.SendWebRequest();
            if(webRequest.isNetworkError)
            {
                err(600);
            }
            else
            {
                if(webRequest.responseCode == 200)
                {
                    
                    string rawResponse = webRequest.downloadHandler.text;
                    
                    res(rawResponse);
                }
                else
                {
                    Debug.Log(webRequest.error);
                    err((int)webRequest.responseCode);
                }
            }
        }
    }

    public IEnumerator GetTexture(string url, Action<Texture2D> res, Action<int> err)
    {
        using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(url))
        {
            yield return unityWebRequest.SendWebRequest();

            if(unityWebRequest.isNetworkError)
            {
                err(600);
            }
            else
            {
                DownloadHandlerTexture downloadHandlerTexture = unityWebRequest.downloadHandler as DownloadHandlerTexture;
                res(downloadHandlerTexture.texture);
            }
        }
    }

    public IEnumerator CreateDueloNormal(int id_w, int id_l, int goles_w, int goles_l, int goles_atajados_ganador, int goles_atajados_perdedor, int goles_recibidos_ganador, int goles_recibidos_perdedor, Action<string> res, Action<int> respCode)
    {
        string testUrl = "https://willymedi.pythonanywhere.com/duelos/duelo_normal/";
        WWWForm form = new WWWForm();
        form.AddField("ganador", id_w);
        form.AddField("perdedor", id_l);
        form.AddField("goles_ganador", goles_w);
        form.AddField("goles_perdedor", goles_l);
        form.AddField("goles_atajados_ganador", goles_atajados_ganador);
        form.AddField("goles_atajados_perdedor", goles_atajados_perdedor);
        form.AddField("goles_recibidos_ganador", goles_recibidos_ganador);
        form.AddField("goles_recibidos_perdedor", goles_recibidos_perdedor);
        using (UnityWebRequest webRequest = UnityWebRequest.Post(testUrl, form))
        {
            yield return webRequest.SendWebRequest();
            if(webRequest.isNetworkError)
            {
                respCode(600);
            }
            else
            {
                if(webRequest.responseCode == 200)
                {
                    string rawDueloCreado = webRequest.downloadHandler.text;
                    res(rawDueloCreado);
                }    
            }
        }

    }
}
