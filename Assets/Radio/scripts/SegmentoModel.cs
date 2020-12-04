using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

[Serializable]
public class SegmentoModel
{
    public List<Segmento> segmentos;
}

[Serializable]
public class Segmento
{
    public int id;
    public string nombre, descripcion, imagen;
    public List<Horario> horarios;
    public Emisora emisora;

    public string HorariosToString(){
        string result = "";
        string[] dias = {"Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo"};
        string[] inicios = new string[7];
        string[] finales = new string[7];
        for (int i = 0; i < horarios.Count; i++)
        {
            for (int j = 0; j < dias.Length; j++)
            {
                if (horarios[i].dia==dias[j])
                {
                    inicios[j] = horarios[i].fecha_inicio.Substring(0,5);
                    finales[j] = horarios[i].fecha_fin.Substring(0,5);
                    break;
                }
            }
        }

        if (/*Lunes a domingo*/
            inicios[0]!= null && 
            inicios[0]==inicios[1] && inicios[0] == inicios[2] && inicios[0] == inicios[3] && inicios[0] == inicios[4] && inicios[0] == inicios[5] && inicios[0] == inicios[6] && 
            finales[0]==finales[1] && finales[0] == finales[2] && finales[0] == finales[3] && finales[0] == finales[4] && finales[0] == finales[5] && finales[0] == finales[6]
        )
        {
            result = "Lunes a Domingo "+inicios[0]+" a "+finales[0];
        }else if(/*Lunes a viernes*/
            inicios[0]!= null && 
            inicios[0]==inicios[1] && inicios[0] == inicios[2] && inicios[0] == inicios[3] && inicios[0] == inicios[4] && 
            finales[0]==finales[1] && finales[0] == finales[2] && finales[0] == finales[3] && finales[0] == finales[4]
        )
        {
            result = "Lunes a Viernes "+inicios[0]+" a "+finales[0];
        }else if (/*Sábados y Domingos*/
            inicios[5]!= null && 
            inicios[5]==inicios[6] &&
            finales[5]==finales[6]
        )
        {
            result = "Sábados y Domingos "+inicios[5]+" a "+finales[5];
        }else/*Cada día abajo de otro*/
        {
            for (int i = 0; i < inicios.Length; i++)
            {
                if (inicios[i]!=null)
                {
                    result+= dias[i]+" "+inicios[i]+" a "+finales[i]+"\n";
                }
            }
        }
        return result;
    }
}


[Serializable]
public class Horario
{
    public string fecha_inicio, fecha_fin, dia;
}

[Serializable]
public class Emisora
{
    public int id;
    public string nombre;
}