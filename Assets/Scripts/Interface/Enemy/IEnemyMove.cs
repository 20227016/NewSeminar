using UnityEngine;

public interface IEnemyMove
{
    public void Execute(Vector3 startPos,Vector3 endPos, float speed);

}