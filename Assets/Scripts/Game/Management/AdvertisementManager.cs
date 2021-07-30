using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AccountModels;

public class AdvertisementManager : MonoBehaviour
{
    [SerializeField]
    private AdvertisementController[] horizontalAdds;
    [SerializeField]
    private AdvertisementController[] verticalAdds;
    [SerializeField]
    private AdvertisementController[] goalAdds;
    private PublicidadesGame publicidades;

    private void Awake()
    {
        publicidades = NetworkManager.instance.publicidadesGame;
    }
    // Start is called before the first frame update
    void Start()
    {
        int timeToChange = Random.Range(7, 13);
        foreach(AdvertisementController hAdds in horizontalAdds)
        {
            hAdds.Initialize(publicidades.horizontal, timeToChange);
        }
        foreach(AdvertisementController vAdds in verticalAdds)
        {
            vAdds.Initialize(publicidades.vertical, timeToChange);
        }
        foreach(AdvertisementController gAdds in goalAdds)
        {
            gAdds.Initialize(publicidades.gol);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

namespace AdvertisementType
{
    public enum Advertisement { GOL, VERTICAL, HORIZONTAL };
}
