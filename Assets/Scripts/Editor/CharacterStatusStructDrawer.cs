using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CharacterStatusStruct))]
public class CharacterStatusStructDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // �J�\��Foldout
        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, "�L�����N�^�[�X�e�[�^�X");

        if (!property.isExpanded)
            return;

        // �C���f���g��1�i�K�[������
        EditorGUI.indentLevel++;

        // �e�t�B�[���h�̕`��
        float fieldY = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // ����Y�ʒu��ݒ�

        // ���ڂ��ƂɃ��x����z�u
        fieldY = DrawLabel(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), "��b�X�e�[�^�X");
        fieldY = DrawLabelField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), "�ő�HP��", "100");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_walkSpeed", "�ʏ�ړ����x");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_runSpeed", "�_�b�V���ړ����x");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_avoidanceDistance", "�������");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_attackPower", "�U����");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_attackLightMultiplier", "��U���_���[�W�{��");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_attackStrongMultiplier", "���U���_���[�W�{��");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_attackSpeed", "�U�����x�{�� ( �~ �U�����x )");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_defensePower", "�h���");

        fieldY = DrawLabel(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), "�X�^�~�i�֘A");
        fieldY = DrawLabelField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), "�ő�X�^�~�i��", "100");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_recoveryStamina", "�X�^�~�i�����񕜗� ( / �b )");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_runStamina", "�_�b�V���ړ����X�^�~�i����� ( / �b )");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_avoidanceStamina", "������X�^�~�i�����");

        fieldY = DrawLabel(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), "�X�L���֘A");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_skillPointUpperLimit", "�X�L���|�C���g����l");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_skillDuration", "�X�L���������� ( �b )");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_skillCoolTime", "�X�L���N�[���^�C�� ( �b )");

        fieldY = DrawLabel(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), "���̑�");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_counterTime", "�J�E���^�[��t���� ( �b )");
        fieldY = DrawField(new Rect(position.x, fieldY, position.width, EditorGUIUtility.singleLineHeight), property, "_ressurectionTime", "�h�����v���� ( �b )");

        // �C���f���g��߂�
        EditorGUI.indentLevel--;
    }

    private float DrawField(Rect position, SerializedProperty property, string fieldName, string displayName)
    {
        SerializedProperty field = property.FindPropertyRelative(fieldName);
        if (field != null)
        {
            EditorGUI.PropertyField(position, field, new GUIContent(displayName));
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // ���̍��ڂɔ�����Y�𒲐�
        }
        return position.y; // ����Y�ʒu��Ԃ�
    }

    private float DrawLabel(Rect position, string label)
    {
        // ���x���`��
        EditorGUI.LabelField(position, label, EditorStyles.boldLabel);
        return position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    }

    private float DrawLabelField(Rect position, string label, string value)
    {
        // ���x���t�B�[���h�`��
        EditorGUI.LabelField(position, label, value);
        return position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded)
            return EditorGUIUtility.singleLineHeight;

        // �e���ڂ̃��x�����܂߂��������v�Z
        int fieldCount = property.CountInProperty() + 2; // ����ł��Ȃ�2�̃t�B�[���h��ǉ�
        int labelCount = 4; // ���ڃ��x���̐��i��: �ړ����x�A����Ȃǁj

        return EditorGUIUtility.singleLineHeight * (fieldCount + labelCount) + EditorGUIUtility.standardVerticalSpacing * (fieldCount + labelCount - 1);
    }
}