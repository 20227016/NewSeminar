
/// <summary>
/// IDLE : 止まっている<br />
/// WALKING : 歩いている<br />
/// RUNNING : 走っている<br />
/// FALLING : //降下中
/// FRYING  : //上昇中
/// DOWNED : ダウンしている<br />
///  STUNNED : のけぞり中<br />
/// </summary>
public enum EnemyMovementState
{
    IDLE,        // 止まっている
    WALKING,     // 歩いている
    RUNNING,     // 走っている
    FALLING,    //降下中
    FRYING,     //上昇中
    DOWNED,      // ダウンしている
    STUNNED,     // のけぞり中
    DIE,         // 死亡
}