using Fusion;
using System;
using UniRx;
using UnityEngine;

/// <summary>
/// TankCharacter.cs
/// クラス説明
///
///
/// 作成日: 9/13
/// 作成者: 山田智哉
/// </summary>
public class TankCharacter : CharacterBase
{
    [SerializeField, Tooltip("ガード成功時のリアクションアニメーション")]
    private AnimationClip _blockReactionAnimation = default;

    [SerializeField, Tooltip("ガード時のスキルポイント取得量")]
    private float _blockSPHealValue = 2f;

    // ガードフラグ
    private ReactiveProperty<bool> _isBlockReactive = new(false);

    private float guardDefencePower = default;

    public float GuardDefencePower { get => guardDefencePower; set => guardDefencePower = value; }

    public override void Spawned()
    {
        base.Spawned();

        // ガード中にアニメーション
        _isBlockReactive
            .DistinctUntilChanged()
            .Subscribe(isBlock =>
            {
                // パラメーターを配列に取得
                AnimatorControllerParameter[] parameters = _animator.parameters;

                // 各パラメーターを調べてBool型の場合、リセットする
                foreach (AnimatorControllerParameter parameter in parameters)
                {
                    if (parameter.type == AnimatorControllerParameterType.Bool)
                    {
                        _animator.SetBool(parameter.name, false);
                    }
                }
                RPC_BoolAnimation(_characterAnimationStruct._avoidanceActionAnimation.name, isBlock);
                if (!isBlock)
                {
                    ResetState(0.5f);
                }
            })
            .AddTo(this);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if(_currentStamina.Value <= 0)
        {
            _isBlockReactive.Value = false;

            _passive.Passive(this);
        }
    }

    protected override void HandleStaminaRecovery()
    {
        Observable.Interval(TimeSpan.FromSeconds(STAMINA_UPDATE_INTERVAL))
            // 回避状態ではない
            .Where(_ => CurrentState != CharacterStateEnum.AVOIDANCE)
            // 走っていない or スタミナ切れ or 移動していない
            .Where(_ => !_isRun || _isOutOfStamina || _moveDirection == Vector2.zero)
            // スタミナが最大値以下
            .Where(_ => _currentStamina.Value < _characterStatusStruct._playerStatus.MaxStamina)
            .Subscribe(_ =>
            {
                // スタミナ切れ時は回復速度が半減
                float recoveryRate = _isOutOfStamina ? 2.0f : 1.0f;
                recoveryRate = _isBlockReactive.Value ? 2.0f : 1.0f;
                _currentStamina.Value += _characterStatusStruct._recoveryStamina * STAMINA_UPDATE_INTERVAL / recoveryRate;
            })
            .AddTo(this);
    }

    protected override void Move(Transform transform, Vector2 moveDirection, float moveSpeed, Rigidbody rigidbody, CharacterStateEnum characterState)
    {
        if(_isBlockReactive.Value == true)
        {
            return;
        }

        base.Move(transform, moveDirection, moveSpeed, rigidbody, characterState);
    }

    protected override void ProcessInput(PlayerNetworkInput input)
    {
        // ガード解除入力
        if (_isBlockReactive.Value && input.IsAvoidance)
        {
            _isBlockReactive.Value = false;

            _passive.Passive(this);

            return;
        }
        base.ProcessInput(input);
        
    }


    protected override void Avoidance(Transform transform, PlayerNetworkInput input)
    {
        if (_isOutOfStamina)
        {
            return;
        }

        _isBlockReactive.Value = true;

        _passive.Passive(this);

    }

    public override void ReceiveDamage(int damageValue)
    {
        if (CurrentState == CharacterStateEnum.DEATH) return;

        // 無敵中はリターン
        if (_isInvincible) return;

        CurrentState = CharacterStateEnum.DAMAGE_REACTION;

        // 防御力の合計値を0以上100以下に制限
        float defensePower = Mathf.Clamp(_characterStatusStruct._defensePower + GuardDefencePower, 0, 100);

        // ダメージ量を計算
        float damage = damageValue - (damageValue * (defensePower * 0.01f));

        if (damage <= 0)
        {
            return;
        }

        // 現在HPから最終ダメージを引く
        NetworkedHP = Mathf.Clamp(NetworkedHP - damage, 0, _characterStatusStruct._playerStatus.MaxHp);

        _sound.ProduceSE(_characterSoundStruct._audioSource, _characterSoundStruct._getHit, _characterSoundStruct._playBackSpeed_GetHit, _characterSoundStruct._audioVolume_GetHit, _characterSoundStruct._delay_GetHit);
        
        // ガード中なら盾受けアニメーションを再生
        if (_isBlockReactive.Value)
        {
            _currentSkillPoint.Value += _blockSPHealValue;
            _currentStamina.Value -= damageValue / 2;
            RPC_PlayAnimation(_blockReactionAnimation.name);
            return;
        }

        // 被弾時のリアクション
        float animationDuration;
        if (damage <= FATAL_DAMAGE_REACTION)
        {
            animationDuration = _animation.GetAnimationLength(_animator, _characterAnimationStruct._damageReactionLightAnimation.name);
            // 怯み
            RPC_PlayAnimation(_characterAnimationStruct._damageReactionLightAnimation.name);
        }
        else
        {
            animationDuration = _animation.GetAnimationLength(_animator, _characterAnimationStruct._damageReactionHeavyAnimation.name);
            // 吹っ飛び
            RPC_PlayAnimation(_characterAnimationStruct._damageReactionHeavyAnimation.name);

            // ノックバック
            _avoidance.Avoidance(transform, _rigidbody, new Vector2(-transform.forward.x, -transform.forward.z), _characterStatusStruct._avoidanceDistance, animationDuration / 5);
        }
        
        Invincible(animationDuration * 2f);
        ResetState(animationDuration);
    }
}