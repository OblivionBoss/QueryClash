using UnityEngine;

public interface IBuildingState
{
    void Endstate();
    void OnAction(Vector3Int gridPosition);
    void UpdateState(Vector3Int gridPosition);
}