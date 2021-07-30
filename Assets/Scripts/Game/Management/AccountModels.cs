using System;
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

    [System.Serializable]
    public class Torneo
    {
        public int id;
        public string nombre_torneo;
        //public DateTime fecha_inicio;
        public string fecha_inicio;
        public bool is_close;
        public bool is_finish;
        //public DateTime fecha_fin;
        public string fecha_fin;
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
        public bool istorneo;
    }

    [System.Serializable]
    public class DueloTorneo
    {
        public int id;
        public Jugador ganador;
        public Jugador perdedor;
        public int goles_ganador;
        public int goles_perdedor;
        public string fecha;
        public int goles_atajados_ganador;
        public int goles_atajados_perdedor;
        public int goles_recibidos_ganador;
        public int goles_recibidos_perdedor;
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
        public DueloAgendado duelo_agendado;

        public UserLogin()
        {
            name = "Offline User";
            username = "Offline User";
            email = "";
        }

        public bool isFirstLogin()
        {
            return (total_partidos == 0) && (emblema == 0) && (haircut_player == 0) && (haircut_goalkeeper == 0) && (player == 0);
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
        [SerializeField]
        public int id;
        [SerializeField]
        public string nombre;
        //public Sprite sprite;
    }

    [System.Serializable]
    public class Publicidad
    {
        [SerializeField]
        public int id;
        [SerializeField]
        public string marca;
        [SerializeField]
        public string tipo;
        [SerializeField]
        public string tipo_imagen;
        [SerializeField]
        public string descripcion;
        [SerializeField]
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

    [System.Serializable]
    public class DueloAgendado
    {
        public int id;
        public Jugador jugador1;
        public Jugador jugador2;
        public string fecha_hora_inicio;
        public string fecha_hora_fin;
        public int numero_inicial_goles;
        public int tiempo_patear_segundos;
        public int tiempo_prorroga;
        public int ronda;
        //public Duelo duelo;
        public int duelo;

        /*
        public DueloAgendado()
        {
            id = 0;
            jugador1 = new Jugador()
            {
                id = 0,
                username = "player1"
            };

            jugador2 = new Jugador()
            {
                id = 1,
                username = "player2"
            };
            numero_inicial_goles = 3;
            tiempo_patear_segundos = 7;
            tiempo_prorroga = 2;
            ronda = 1;
            duelo = 1;
            fecha_hora_inicio = DateTime.Now;//.AddSeconds(30);
            fecha_hora_fin = fecha_hora_inicio.AddSeconds(15);//AddMinutes(tiempo_prorroga);

        }
        */
    }
    [System.Serializable]
    public class PublicidadesGame
    {
        public List<PublicidadGame> gol = new List<PublicidadGame>();
        public List<PublicidadGame> vertical = new List<PublicidadGame>();
        public List<PublicidadGame> horizontal = new List<PublicidadGame>();
    }

    [System.Serializable]
    public class PublicidadGame
    {
        public int id;
        public string marca;
        public string descripcion;
        public List<ImageGame> imagenes = new List<ImageGame>();

        public PublicidadGame(int _id, string _marca, string _descripcion)
        {
            id = _id;
            marca = _marca;
            descripcion = _descripcion;
        }
        public PublicidadGame()
        {

        }
    }

    [System.Serializable]
    public class ImageGame
    {
        [SerializeField]
        public int id;
        [SerializeField]
        public Sprite sprite;

        public ImageGame(int _id, Sprite _sprite)
        {
            id = _id;
            sprite = _sprite;
        }
        
        public ImageGame()
        {

        }
    }


    [System.Serializable]
    public class DueloAgendadoRequest
    {
        public int duelo;
        public int ganador;
        public int perdedor;
    }
}



