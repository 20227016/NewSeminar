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
                _animation.BoolAnimation(_animator, _characterAnimationStruct._avoidanceActionAnimation, isBlock);
            })
            .AddTo(this);
    }


    protected override void ProcessInput(PlayerNetworkInput input)
    {
        // ガード解除入力
        if (_isBlockReactive.Value && input.IsAvoidance)
        {
            _currentState = CharacterStateEnum.IDLE;

            _isBlockReactive.Value = false;

            _passive.Passive(this);

            return;
        }
        base.ProcessInput(input);
    }


    protected override void Avoidance(Transform transform)
    {
        // 回避処理をガードに置き換える
        _currentState = CharacterStateEnum.AVOIDANCE;

        _isBlockReactive.Value = true;

        _passive.Passive(this);
    }


    public override void ReceiveDamage(int damageValue)
    {
        if (!Object.HasStateAuthority) return;

        // 被弾中は無敵
        if (_currentState == CharacterStateEnum.DAMAGE_REACTION) return;

        _currentState = CharacterStateEnum.DAMAGE_REACTION;

        // ダメージ量に防御力を適応して最終ダメージを算出
        float damage = (damageValue - _characterStatusStruct._defensePower);

        // 現在HPから最終ダメージを引く
        _networkedHP = Mathf.Clamp(_networkedHP - damageValue, 0, _characterStatusStruct._playerStatus.MaxHp);

        if (_networkedHP <= 0)
        {
            Death();
            return;
        }

        // ガード中なら盾受けアニメーションを再生
        if (_isBlockReactive.Value)
        {
            _animation.PlayAnimation(_animator, _blockReactionAnimation);
            return;
        }

        // 被弾時のリアクション
        float animationDuration;
        if (damage <= _characterStatusStruct._playerStatus.MaxHp / 2)
        {
            // 怯み
            animationDuration = _animation.PlayAnimation(_animator, _characterAnimationStruct._damageReactionLightAnimation);
        }
        else
        {
            // 吹っ飛び
            animationDuration = _animation.PlayAnimation(_animator, _characterAnimationStruct._damageReactionHeavyAnimation);
            // ノックバック
            _avoidance.Avoidance(transform, new Vector2(-transform.forward.x, -transform.forward.z), _characterStatusStruct._avoidanceDistance, animationDuration / 5);
        }

        ResetState(animationDuration);
    }
}