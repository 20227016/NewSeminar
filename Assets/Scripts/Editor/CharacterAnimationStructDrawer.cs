using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CharacterAnimationStruct))]
public class CharacterAnimationStructDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // �J�\��Foldout
        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, "�A�j���[�V�����ݒ�");

        if (!property.isExpanded)
            return;

        // �C���f���g��1�i�K�[������
        EditorGUI.indentLevel++;

        // �e�t�B�[���h�̕`��
        float fieldY = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // ����Y�ʒu��ݒ�

        // Draw each field in the struct
        fieldY = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        fieldY = DrawAnimationField(position, fieldY, property, "_idleAnimation", "�ҋ@");
        fieldY = DrawAnimationField(position, fieldY, property, "_walkAnimation", "����");
        fieldY = DrawAnimationField(position, fieldY, property, "_runAnimation", "����");
        fieldY = DrawAnimationField(position, fieldY, property, "_attackLightAnimation1", "��U��1");
        fieldY = DrawAnimationField(position, fieldY, property, "_attackLightAnimation2", "��U��2");
        fieldY = DrawAnimationField(position, fieldY, property, "_attackLightAnimation3", "��U��3");
        fieldY = DrawAnimationField(position, fieldY, property, "_attackStrongAnimation", "���U��");
        fieldY = DrawAnimationField(position, fieldY, property, "_avoidanceActionAnimation", "����s��");
        fieldY = DrawAnimationField(position, fieldY, property, "_skillAnimation", "�X�L��");
        fieldY = DrawAnimationField(position, fieldY, property, "_damageReactionLightAnimation", "�y��e");
        fieldY = DrawAnimationField(position, fieldY, property, "_damageReactionHeavyAnimation", "�d��e");
        fieldY = DrawAnimationField(position, fieldY, property, "_deathAnimation", "���S");
        fieldY = DrawAnimationField(position, fieldY, property, "_reviveAnimation", "��h��");

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