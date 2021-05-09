using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AccountModels
{
    [System.Serializable]
    public class UserToken
    {
        public string username;
        public string token;
        public int id;
        public string email;


        public override string ToString()
        {
            return "Username: " + username + "token: " + token;
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

    [System.Serializable]
    public class Duelo
    {
        public int id;
        public Jugador ganador;
        public Jugador perdedor;
        public int goles_ganador;
        public int goles_perdedor;
        public string fecha;
    }

    [System.Serializable]
    public class Jugador
    {
        public int id;
        public string username;
    }

    [System.Serializable]
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
        //public Duelo[] duelos = new Duelo[5];
        public List<Duelo> duelos;

        public UserLogin()
        {
            name = "Offline User";
            username = "Offline User";
            email = "";
        }

        public void AddDuelo(Duelo duelo)
        {
            if (duelos.Count == 5)
            {
                ShiftDueloArr(duelo);
            }
            else
            {
                int tmpIndex = duelos.Count;
                duelos.Add(duelo);
                //duelos[tmpIndex] = duelo;
            }
        }

        private void ShiftDueloArr(Duelo duelo)
        {
            for (int i = 1; i < duelos.Count; i++)
            {
                duelos[i - 1] = duelos[i];
            }
            duelos[4] = duelo;
        }
    }

    [System.Serializable]
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

    [System.Serializable]
    public class Imagen
    {
        public int id;
        public string nombre;
        public Sprite sprite;
    }

    [System.Serializable]
    public class Publicidad
    {
        public int id;
        public string marca;
        public string tipo;
        public string descripcion;
        public Imagen[] imagenes;


        public override string ToString()
        {
            return "id: " + id + " marca: " + marca + " tipo: " + tipo;//+ " Length: " + imagenes.ToString();
        }
    }

    [System.Serializable]
    public class Publicidades
    {
        public Publicidad[] publicidades;
    }
}



