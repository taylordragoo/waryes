using UnityEngine;
namespace SurvivorBase.Scripts
{
    [CreateAssetMenu(fileName = "ContainerMaterialSet", menuName = "SurvivorBase/Container Material Set")]
public class ContainerMaterialSet : ScriptableObject
{
    public Material FrameMaterial;
    public Material DetailsMaterial;
    public Material ValvesMaterial;
    public Material FloorMaterial;
    public Material WallInsideMaterial;
    public Material StairsMaterial;
    public Material DoorMaterial;
    public Texture2D Icon;
}
}