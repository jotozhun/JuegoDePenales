using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

[Serializable]
public class LocutoresModel
{
    public List<Locutor> locutores;
}

[Serializable]
public class Locutor
{
    public int id;
    public string first_name, last_name, imagen;

    public string NombresCompletos(){
        return first_name+" "+last_name;
    }
}