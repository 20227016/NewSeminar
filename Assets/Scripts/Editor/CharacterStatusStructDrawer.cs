using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CharacterStatusStruct))]
public class CharacterStatusStructDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // インデント設定
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 1;

        // ラインの高さ
        var lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        // 区切り線の描画
        DrawSeparator(ref rect, position.width);

        // プロパティのラベルを表示
        EditorGUI.LabelField(rect, "キャラクターステータス");
        rect.y += lineHeight;

        // 各フィールドの日本語ラベルを順番に描画
        DrawPropertyWithLabel(property, rect, "_walkSpeed", "通常移動速度", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_runSpeed", "ダッシュ移動速度", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_avoidanceDistance", "回避距離", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_avoidanceDuration", "回避持続時間", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_attackMultipiler", "攻撃倍率", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_attackSpeed", "攻撃速度", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_defensePower", "防御力", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_skillTime", "スキル継続時間", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_skillCoolTime", "スキルクールタイム", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_skillPointUpperLimit", "スキルポイント上限値", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_recoveryStamina", "スタミナ自動回復量", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_runStamina", "ダッシュ時スタミナ消費量", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_avoidanceStamina", "回避時スタミナ消費量", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_counterTime", "カウンター受付時間", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_ressurectionTime", "蘇生所要時間", ref rect, lineHeight);
        DrawSeparator(ref rect, position.width);

        // インデントを元に戻す
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    private void DrawPropertyWithLabel(SerializedProperty property, Rect rect, string propertyName, string label, ref Rect currentRect, float lineHeight)
    {
        var prop = property.FindPropertyRelative(propertyName);
        if (prop != null)
        {
            EditorGUI.PropertyField(currentRect, prop, new GUIContent(label));
            currentRect.y += lineHeight;
        }
    }

    private void DrawSeparator(ref Rect rect, float width)
    {
        // 区切り線の高さと位置
        float lineHeight = 2f;
        Rect separatorRect = new Rect(rect.x, rect.y, width, lineHeight);

        // 区切り線の描画（灰色）
        EditorGUI.DrawRect(separatorRect, Color.black);

        // 区切り線の高さを調整
        rect.y += lineHeight + EditorGUIUtility.standardVerticalSpacing;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 子プロパティの数に基づいて高さを計算
        int fieldCount = property.CountInProperty();
        float separatorHeight = 2f + EditorGUIUtility.standardVerticalSpacing; // 区切り線の高さ
        return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * fieldCount + separatorHeight;
    }
}