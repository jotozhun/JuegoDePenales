using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AccountModels;

public class ShowMatches : MonoBehaviour
{
    NetworkManager manager;
    public GameObject matchResultPrefab;
    public GameObject matchTorneoPrefab;
    private Transform container;
    private List<Duelo> duelos;
    private int playerId;

    private void Awake()
    {
        container = GetComponent<Transform>();
        manager = NetworkManager.instance;
        duelos = manager.userLogin.duelos;
        playerId = manager.userLogin.id;
    }
    // Start is called before the first frame update
    void Start()
    {
        LoadMatches();
    }

    void LoadMatches()
    {
        foreach(Duelo tmpDuelo in duelos)
        {
            GameObject tmpElement;
            if (tmpDuelo.istorneo)
                tmpElement = Instantiate<GameObject>(matchTorneoPrefab);
            else
                tmpElement = Instantiate<GameObject>(matchResultPrefab);
            bool isWin = false;
            if(tmpDuelo.ganador.id == playerId)
            {
                isWin = true;
            }
            tmpElement.GetComponent<MatchController>().Initialize(tmpDuelo, isWin);
            tmpElement.transform.SetParent(container);
        }
    }

    
}
