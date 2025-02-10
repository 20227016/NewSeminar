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
                RPC_BoolAnimation(_characterAnimationStruct._avoidanceActionAnimation.name, isBlock);
            })
            .AddTo(this);
    }


    protected override void ProcessInput(PlayerNetworkInput input)
    {
        // ガード解除入力
        if (_isBlockReactive.Value && input.IsAvoidance)
        {
            CurrentState = CharacterStateEnum.IDLE;

            _isBlockReactive.Value = false;

            _passive.Passive(this);

            return;
        }
        base.ProcessInput(input);
    }


    protected override void Avoidance(Transform transform)
    {
        // 回避処理をガードに置き換える
        CurrentState = CharacterStateEnum.AVOIDANCE;

        _isBlockReactive.Value = true;

        _passive.Passive(this);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public override void RPC_ReceiveDamage(int damageValue)
    {
        if (!Object.HasStateAuthority) return;

        // 被弾中は無敵
        if (CurrentState == CharacterStateEnum.DAMAGE_REACTION) return;

        CurrentState = CharacterStateEnum.DAMAGE_REACTION;

        // ダメージ量に防御力を適応して最終ダメージを算出
        float damage = (damageValue - _characterStatusStruct._defensePower);

        // 現在HPから最終ダメージを引く
        NetworkedHP = Mathf.Clamp(NetworkedHP - damageValue, 0, _characterStatusStruct._playerStatus.MaxHp);

        if (NetworkedHP <= 0)
        {
            RPC_Death();
            return;
        }

        // ガード中なら盾受けアニメーションを再生
        if (_isBlockReactive.Value)
        {
            RPC_PlayAnimation(_blockReactionAnimation.name);
            return;
        }

        // 被弾時のリアクション
        float animationDuration;
        if (damage <= _characterStatusStruct._playerStatus.MaxHp / 2)
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

    protected override void Skill(CharacterBase characterBase, float skillTime, float skillCoolTime)
    {
        base.Skill(characterBase, skillTime, skillCoolTime);
    }
}