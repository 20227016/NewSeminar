using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CharacterAnimationStruct))]
public class CharacterAnimationStructDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 開閉可能なFoldout
        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, "アニメーション設定");

        if (!property.isExpanded)
            return;

        // インデントを1段階深くする
        EditorGUI.indentLevel++;

        // 各フィールドの描画
        float fieldY = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // 初期Y位置を設定

        // Draw each field in the struct
        fieldY = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        fieldY = DrawAnimationField(position, fieldY, property, "_idleAnimation", "待機");
        fieldY = DrawAnimationField(position, fieldY, property, "_walkAnimation", "歩き");
        fieldY = DrawAnimationField(position, fieldY, property, "_runAnimation", "走り");
        fieldY = DrawAnimationField(position, fieldY, property, "_attackLightAnimation1", "弱攻撃1");
        fieldY = DrawAnimationField(position, fieldY, property, "_attackLightAnimation2", "弱攻撃2");
        fieldY = DrawAnimationField(position, fieldY, property, "_attackLightAnimation3", "弱攻撃3");
        fieldY = DrawAnimationField(position, fieldY, property, "_attackStrongAnimation", "強攻撃");
        fieldY = DrawAnimationField(position, fieldY, property, "_avoidanceActionAnimation", "回避行動");
        fieldY = DrawAnimationField(position, fieldY, property, "_skillAnimation", "スキル");
        fieldY = DrawAnimationField(position, fieldY, property, "_damageReactionLightAnimation", "軽被弾");
        fieldY = DrawAnimationField(position, fieldY, property, "_damageReactionHeavyAnimation", "重被弾");
        fieldY = DrawAnimationField(position, fieldY, property, "_deathAnimation", "死亡");
        fieldY = DrawAnimationField(position, fieldY, property, "_reviveAnimation", "被蘇生");

        // Reset indentation
        EditorGUI.indentLevel--;
    }

    private float DrawAnimationField(Rect position, float fieldY, SerializedProperty property, string fieldName, string displayName)
    {
        SerializedProperty field = property.FindPropertyRelative(fieldName);
        if (field != null)
        {
            EditorGUI.PropertyField(
                new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight),
                field,
                new GUIContent(displayName)
            );
            fieldY += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
        return fieldY;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded)
            return EditorGUIUtility.singleLineHeight;

        int fieldCount = property.CountInProperty() - 1; // Number of fields in the struct
        return EditorGUIUtility.singleLineHeight * fieldCount + EditorGUIUtility.standardVerticalSpacing * (fieldCount - 1);
    }
}