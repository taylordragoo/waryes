using System.Collections.Generic;
using UnityEngine;

namespace SurvivorBase.Scripts
{
  public class PillarIdentifier : MonoBehaviour
  {
    [SerializeField] private GameObject Pillar;
    public List<SideType> SideTypes;

    public void SetActive(bool state)
    {
      Pillar.SetActive(state);
    }
  
  }
}
