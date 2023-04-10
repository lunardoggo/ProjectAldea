using UnityEngine;

[CreateAssetMenu(menuName = "Project Aldea/MapSettings", fileName = "MapSettings")]
public class MapSettings : ScriptableObject
{
    [SerializeField]
    private string displayName;
    [SerializeField]
    private Vector2Int mapSize = new Vector2Int(256, 256);

    public string Name { get => this.displayName; }
    public Vector2Int MapSize { get => mapSize;} 
}