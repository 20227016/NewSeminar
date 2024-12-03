using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CharacterStatusStruct))]
public class CharacterStatusStructDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // �C���f���g�ݒ�
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 1;

        // ���C���̍���
        var lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        // ��؂���̕`��
        DrawSeparator(ref rect, position.width);

        // �v���p�e�B�̃��x����\��
        EditorGUI.LabelField(rect, "�L�����N�^�[�X�e�[�^�X");
        rect.y += lineHeight;

        // �e�t�B�[���h�̓��{�ꃉ�x�������Ԃɕ`��
        DrawPropertyWithLabel(property, rect, "_walkSpeed", "�ʏ�ړ����x", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_runSpeed", "�_�b�V���ړ����x", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_avoidanceDistance", "�������", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_avoidanceDuration", "�����������", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_attackMultipiler", "�U���{��", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_attackSpeed", "�U�����x", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_defensePower", "�h���", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_skillTime", "�X�L���p������", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_skillCoolTime", "�X�L���N�[���^�C��", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_skillPointUpperLimit", "�X�L���|�C���g����l", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_recoveryStamina", "�X�^�~�i�����񕜗�", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_runStamina", "�_�b�V�����X�^�~�i�����", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_avoidanceStamina", "������X�^�~�i�����", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_counterTime", "�J�E���^�[��t����", ref rect, lineHeight);
        DrawPropertyWithLabel(property, rect, "_ressurectionTime", "�h�����v����", ref rect, lineHeight);
        DrawSeparator(ref rect, position.width);

        // �C���f���g�����ɖ߂�
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
        // ��؂���̍����ƈʒu
        float lineHeight = 2f;
        Rect separatorRect = new Rect(rect.x, rect.y, width, lineHeight);

        // ��؂���̕`��i�D�F�j
        EditorGUI.DrawRect(separatorRect, Color.black);

        // ��؂���̍����𒲐�
        rect.y += lineHeight + EditorGUIUtility.standardVerticalSpacing;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // �q�v���p�e�B�̐��Ɋ�Â��č������v�Z
        int fieldCount = property.CountInProperty();
        float separatorHeight = 2f + EditorGUIUtility.standardVerticalSpacing; // ��؂���̍���
        return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * fieldCount + separatorHeight;
    }
}