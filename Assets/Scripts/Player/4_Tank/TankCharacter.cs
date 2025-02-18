using Fusion;
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

    // ガードフラグ
    private ReactiveProperty<bool> _isBlockReactive = new(false);

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
        // 回避処理をガードに置き換える
        CurrentState = CharacterStateEnum.AVOIDANCE;

        _isBlockReactive.Value = true;

        _passive.Passive(this);
    }

    public override void RPC_ReceiveDamage(int damageValue)
    {
        if (CurrentState == CharacterStateEnum.DEATH) return;

        // 無敵中はリターン
        if (_isInvincible) return;

        CurrentState = CharacterStateEnum.DAMAGE_REACTION;


        // ダメージ量に防御力を適応して最終ダメージを算出
        float damage = (damageValue - (damageValue * _characterStatusStruct._defensePower * 0.01f));

        // 現在HPから最終ダメージを引く
        NetworkedHP = Mathf.Clamp(NetworkedHP - damage, 0, _characterStatusStruct._playerStatus.MaxHp);

        _sound.ProduceSE(_characterSoundStruct._audioSource, _characterSoundStruct._getHit, _characterSoundStruct._playBackSpeed_GetHit, _characterSoundStruct._audioVolume_GetHit, _characterSoundStruct._delay_GetHit);
        
        // ガード中なら盾受けアニメーションを再生
        if (_isBlockReactive.Value)
        {
            RPC_PlayAnimation(_blockReactionAnimation.name);
            return;
        }

        // 被弾時のリアクション
        float animationDuration;
        if (damage <= 30)
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