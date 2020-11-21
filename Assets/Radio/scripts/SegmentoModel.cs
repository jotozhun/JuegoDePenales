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
    public string nombre, descripcion;
    public List<Horario> horarios;
    public Emisora emisora;
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