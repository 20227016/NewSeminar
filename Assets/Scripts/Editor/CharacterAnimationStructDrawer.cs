using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CharacterAnimationStruct))]
public class CharacterAnimationStructDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // JÂÂ\ÈFoldout
        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, "Aj[VÝè");

        if (!property.isExpanded)
            return;

        // Cfgð1iK[­·é
        EditorGUI.indentLevel++;

        // etB[hÌ`æ
        float fieldY = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // úYÊuðÝè

        // Draw each field in the struct
        fieldY = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        fieldY = DrawAnimationField(position, fieldY, property, "_idleAnimation", "Ò@");
        fieldY = DrawAnimationField(position, fieldY, property, "_walkAnimation", "à«");
        fieldY = DrawAnimationField(position, fieldY, property, "_runAnimation", "è");
        fieldY = DrawAnimationField(position, fieldY, property, "_attackLightAnimation1", "ãU1");
        fieldY = DrawAnimationField(position, fieldY, property, "_attackLightAnimation2", "ãU2");
        fieldY = DrawAnimationField(position, fieldY, property, "_attackLightAnimation3", "ãU3");
        fieldY = DrawAnimationField(position, fieldY, property, "_attackStrongAnimation", "­U");
        fieldY = DrawAnimationField(position, fieldY, property, "_avoidanceActionAnimation", "ñðs®");
        fieldY = DrawAnimationField(position, fieldY, property, "_skillAnimation", "XL");
        fieldY = DrawAnimationField(position, fieldY, property, "_damageReactionLightAnimation", "yíe");
        fieldY = DrawAnimationField(position, fieldY, property, "_damageReactionHeavyAnimation", "díe");
        fieldY = DrawAnimationField(position, fieldY, property, "_deathAnimation", "S");
        fieldY = DrawAnimationField(position, fieldY, property, "_reviveAnimation", "íh¶");

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