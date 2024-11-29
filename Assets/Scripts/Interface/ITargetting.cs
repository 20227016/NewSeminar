
using UnityEngine;

public interface ITargetting
{
    void InitializeSetting(Camera camera);

    /// <summary>
    /// ターゲッティング
    /// </summary>
    void Targetting();
}