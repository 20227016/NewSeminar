
using UnityEngine;

public interface IAvoidance
{
    /// <summary>
    /// ‰ñ”ğƒƒ\ƒbƒh
    /// </summary>
    /// <param name="avoidanceDirection">‰ñ”ğ•ûŒü</param>
    /// <param name="avoidanceDistance">‰ñ”ğ‹——£</param>
    /// <param name="avoidanceDuration">‰ñ”ğŠÔ</param>
    void Avoidance(Transform transform, Rigidbody rigidbody, Vector2 avoidanceDirection, float avoidanceDistance, float avoidanceDuration);
}