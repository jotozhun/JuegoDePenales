using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AccountModels
{
    public class UserToken
    {
        public string username;
        public string token;
        public int id;
        public string email;
        public int statusCode;

        public UserToken(int badCode)
        {
            statusCode = badCode;
        }

        public override string ToString()
        {
            return "Username: " + username + "token: " + token + "status: " + statusCode;
        }
    }

    public class Torneo
    {
        public int id;
        public string nombre_torneo;
        public string fecha_inicio;
        public bool is_close;
        public bool is_finish;
        public string fecha_fin;
        public int num_maximo;
        public int num_participantes;
        public int num_goles;
        public int tiempo_espera;
        public int tiempo_patear;
        public int num_grupos;
    }

    public class UserLogin
    {
        public int id;
        public Torneo torneo;
        public string email;
        public string username;
        public string name;
        public string telefono;
        public int total_partidos;
        public int partidos_ganados;
        public int partidos_perdidos;
        public int goles_anotados;
        public int goles_atajados;
        public int goles_recibidos;
        public int posicion_ranking;
        public int emblema;
        public int haircut_player;
        public int haircut_goalkeeper;
        public int player;
        public int statusCode;

        public UserLogin()
        {
            name = "Offline User";
            username = "Offline User";
            email = "";
        }

        public UserLogin(int badCode)
        {
            statusCode = badCode;
        }
    }

    public class SavedAccount
    {
        public string username;
        public string password;
        public bool isLoggedIn;
    }

    public class InterfazInfo
    {
        public int emblema;
        public int haircut_player;
        public int player;
    }

    public class MatchResult
    {
        public int total_partidos;
        public int partidos_ganados;
        public int partidos_perdidos;
        public int goles_anotados;
        public int goles_atajados;
        public int goles_recibidos;
    }

    public class StatusCodes
    {
        public int registerCode;

    }
}
