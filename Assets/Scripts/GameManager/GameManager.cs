
using System;
using UniRx;
using UnityEngine;

/// <summary>
/// GameManager.cs
/// �N���X����
/// �_�B���̃N���X�͐_�ł��B�����߂�
///
/// �쐬��: 10/3
/// �쐬��: �������h
/// </summary>
public class GameManager : MonoBehaviour
{

    private Subject<Unit> OnGameStart = new Subject<Unit>();
    public IObservable<Unit> GameStart => OnGameStart;

    private void Awake()
    {
        print("�w�ǂ��J�n���܂�");
        // �Q�[���J�n�C�x���g���w��
        GameInitializer.Instance.InitializationComplete.Subscribe(_ => StartGame());

        print("GameInitializer���Ăяo���A�Q�[���̏����������J�n�����܂�");
        // �Q�[�������ݒ���J�n
        GameInitializer.Instance.StartInitialization();
    }

    /// <summary>
    /// �Q�[���J�n�C�x���g�𔭍s����(�Q�[���J�n�g���K�[���~�����N���X�͂��̃C�x���g���w�ǂ��Ă�)
    /// </summary>
    private void StartGame()
    {
        print("���ׂĂ̏����������������܂����B�Q�[�����X�^�[�g���܂�");
        OnGameStart.OnNext(Unit.Default);
    }
}