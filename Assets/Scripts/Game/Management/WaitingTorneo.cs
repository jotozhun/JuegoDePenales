using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using AccountModels;
using System;

public class WaitingTorneo : MonoBehaviour
{
    NetworkManager network_manager;
    DueloAgendado duelo_agendado;
    DateTime fecha_fin;

    void Start()
    {
        network_manager = NetworkManager.instance;
        duelo_agendado = network_manager.userLogin.duelo_agendado;
        fecha_fin = DateTime.Parse(duelo_agendado.fecha_hora_fin);
    }

    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 1 && DateTime.Now > fecha_fin)
        {
            int ganador = 0;
            int perdedor = 0;

            if(network_manager.userLogin.id == duelo_agendado.jugador1.id)
            {
                ganador = duelo_agendado.jugador1.id;
                perdedor = duelo_agendado.jugador2.id;
            }
            else
            {
                ganador = duelo_agendado.jugador2.id;
                perdedor = duelo_agendado.jugador1.id;
            }
            network_manager.SetDueloAgendadoResult(-1, ganador, perdedor);
            Menu.instance.OnCancelButton_WR();
        }
    }
}
