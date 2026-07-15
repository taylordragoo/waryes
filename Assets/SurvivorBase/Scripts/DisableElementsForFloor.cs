using UnityEngine;

public class DisableElementsForFloor : MonoBehaviour
{
    [SerializeField] private GameObject frontElement;
    [SerializeField] private GameObject leftElement;
    [SerializeField] private GameObject rightElement;
    
    [SerializeField] private bool frontElementButton;
    [SerializeField] private bool leftElementButton;
    [SerializeField] private bool rightElementButton;

    private void OnValidate()
    {
        if (frontElement != null)
            frontElement.SetActive(frontElementButton);
        
        if (leftElement != null)
            leftElement.SetActive(leftElementButton);
        
        if (rightElement != null)
            rightElement.SetActive(rightElementButton);
    }
}