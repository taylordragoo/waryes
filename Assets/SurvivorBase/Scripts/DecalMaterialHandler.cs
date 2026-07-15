using UnityEngine;

[ExecuteInEditMode]
public class DecalMaterialHandler : MonoBehaviour
{
    [SerializeField] private Renderer decalRenderer;
    [SerializeField] public Material[] decalMaterials;

    private int _currentMaterialIndex = -1;
    private bool _isActive = false;

    public void ToggleDecal(int materialIndex)
    {
        _isActive = !_isActive;
        gameObject.SetActive(_isActive);

        if (_isActive)
        {
            ApplyMaterial(materialIndex);
        }
    }

    private void OnEnable()
    {
        if (_currentMaterialIndex >= 0)
        {
            ApplyMaterial(_currentMaterialIndex);
        }
    }

    public void ApplyMaterial(int index)
    {
        if (decalMaterials.Length == 0 || decalRenderer == null || index < 0 || index >= decalMaterials.Length)
            return;

        _currentMaterialIndex = index;
        decalRenderer.sharedMaterial = decalMaterials[_currentMaterialIndex];
    }
}