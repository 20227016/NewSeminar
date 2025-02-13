
using UnityEngine;

public interface IResurrection
{

    void Resurrection(Transform thisTransform, float resurrectionTime);

    void CancelResurrection(Transform thisTransform);
}