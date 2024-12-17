using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CharacterStatusStruct))]
public class CharacterStatusStructDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 開閉可能なFoldout
        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, "キャラクターステータス");

        if (!property.isExpanded)
            return;

        // インデントを1段階深くする
        EditorGUI.indentLevel++;

        // 各フィールドの描画
        float fieldY = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // 初期Y位置を設定

        // 項目ごとにラベルを配置
        fieldY = DrawLabel(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), "基礎ステータス");
        fieldY = DrawLabelField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), "最大HP量", "100");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_walkSpeed", "通常移動速度");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_runSpeed", "ダッシュ移動速度");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_avoidanceDistance", "回避距離");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_attackPower", "攻撃力");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_attackLightMultiplier", "弱攻撃ダメージ倍率");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_attackStrongMultiplier", "強攻撃ダメージ倍率");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_attackSpeed", "攻撃速度倍率 ( × 攻撃速度 )");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_defensePower", "防御力");

        fieldY = DrawLabel(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), "スタミナ関連");
        fieldY = DrawLabelField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), "最大スタミナ量", "100");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_recoveryStamina", "スタミナ自動回復量 ( / 秒 )");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_runStamina", "ダッシュ移動時スタミナ消費量 ( / 秒 )");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_avoidanceStamina", "回避時スタミナ消費量");

        fieldY = DrawLabel(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), "スキル関連");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_skillPointUpperLimit", "スキルポイント上限値");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_skillDuration", "スキル持続時間 ( 秒 )");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_skillCoolTime", "スキルクールタイム ( 秒 )");

        fieldY = DrawLabel(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), "その他");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_counterTime", "カウンター受付時間 ( 秒 )");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_ressurectionTime", "蘇生所要時間 ( 秒 )");

        // インデントを戻す
        EditorGUI.indentLevel--;
    }

    private float DrawField(Rect position, SerializedProperty property, string fieldName, string displayName)
    {
        SerializedProperty field = property.FindPropertyRelative(fieldName);
        if (field != null)
        {
            EditorGUI.PropertyField(position, field, new GUIContent(displayName));
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // 次の項目に備えてYを調整
        }
        return position.y; // 次のY位置を返す
    }

    private float DrawLabel(Rect position, string label)
    {
        // ラベル描画
        EditorGUI.LabelField(position, label, EditorStyles.boldLabel);
        return position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    }

    private float DrawLabelField(Rect position, string label, string value)
    {
        // ラベルフィールド描画
        EditorGUI.LabelField(position, label, value);
        return position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded)
            return EditorGUIUtility.singleLineHeight;

        // 各項目のラベルを含めた高さを計算
        int fieldCount = property.CountInProperty() + 2; // 操作できない2つのフィールドを追加
        int labelCount = 4; // 項目ラベルの数（例: 移動速度、回避など）

        return EditorGUIUtility.singleLineHeight * (fieldCount + labelCount) + EditorGUIUtility.standardVerticalSpacing * (fieldCount + labelCount - 1);
    }
}