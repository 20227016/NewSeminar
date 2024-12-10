
public interface IComboCounter
{
    /// <summary>
    /// コンボ取得
    /// </summary>
    /// <returns>現在のコンボ数</returns>
    float GetComboMultiplier();

    /// <summary>
    /// コンボ加算
    /// </summary>
    void AddCombo();
}