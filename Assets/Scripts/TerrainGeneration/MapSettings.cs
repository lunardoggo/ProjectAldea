using UnityEngine;

[CreateAssetMenu(menuName = "Project Aldea/MapSettings", fileName = "MapSettings")]
public class MapSettings : ScriptableObject
{
    [SerializeField]
    private string displayName;
    [SerializeField]
    private Vector2Int mapSize = new Vector2Int(256, 256);
    [SerializeField]
    private int minRiverCount = 10;
    [SerializeField]
    private int riverCarvingTrys = 20;
    [SerializeField]
    private float minRiverLength = 10.0f;

    public int RiverCarvingTrys { get => this.riverCarvingTrys; }
    public int MinRiverCount { get => this.minRiverCount; }
    public string Name { get => this.displayName; }
    public Vector2Int MapSize { get => mapSize;} 
}