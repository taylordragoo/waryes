using UnityEngine;

public class DisableElementsForStairs : MonoBehaviour
{
    [SerializeField] private GameObject leftRailings;
    [SerializeField] private GameObject rightRailings;
    [SerializeField] private GameObject floor;
    [SerializeField] private GameObject floorRailings;
    
    [SerializeField] private bool leftRailingsButton;
    [SerializeField] private bool rightRailingsButton;
    [SerializeField] private bool floorButton;
    [SerializeField] private bool floorRailingsButton;

    private void OnValidate()
    { 
        leftRailings.gameObject.SetActive(leftRailingsButton);
        rightRailings.gameObject.SetActive(rightRailingsButton);
        floor.gameObject.SetActive(floorButton);
        floorRailings.gameObject.SetActive(floorRailingsButton);
       
    }
}