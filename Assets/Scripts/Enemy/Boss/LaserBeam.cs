using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;

/// <summary>
/// LaserBeam.cs
/// レーザービームを制御する
/// 作成日: 11/17
/// 作成者: 石井直人
/// </summary>
public class LaserBeam : MonoBehaviour
{
    [SerializeField]
    private int damageOverTime = 30;

    [SerializeField]
    private GameObject HitEffect;
    private float HitOffset = 0;
    private bool useLaserRotation = false;

    [SerializeField]
    private float MaxLength;
    private LineRenderer Laser;

    private float MainTextureLength = 1f;
    private float NoiseTextureLength = 1f;
    private Vector4 Length = new Vector4(1, 1, 1, 1);
    //private Vector4 LaserSpeed = new Vector4(0, 0, 0, 0); {DISABLED AFTER UPDATE}
    //private Vector4 LaserStartSpeed; {DISABLED AFTER UPDATE}
    //One activation per shoot
    private bool LaserSaver = false;
    private bool UpdateSaver = false;

    private ParticleSystem[] Effects;
    private ParticleSystem[] Hit;

    private LineRenderer lineRenderer;

    private BoxCollider _laserCollider = default; // 当たり判定

    private float _targetWidth = 3f; // 最終的な太さ
    private float _growSpeed = 1f;   // 成長速度

    private float _activationTimer = 3f; // レーザー発射ラグ

    /// <summary>
    /// 初期化
    /// </summary>
    private void Awake()
    {
        //Get LineRender and ParticleSystem components from current prefab;  
        Laser = GetComponent<LineRenderer>();
        Effects = GetComponentsInChildren<ParticleSystem>();
        Hit = HitEffect.GetComponentsInChildren<ParticleSystem>();
        _laserCollider = GetComponent<BoxCollider>();
        //if (Laser.material.HasProperty("_SpeedMainTexUVNoiseZW")) LaserStartSpeed = Laser.material.GetVector("_SpeedMainTexUVNoiseZW");
        //Save [1] and [3] textures speed
        //{ DISABLED AFTER UPDATE}
        //LaserSpeed = LaserStartSpeed;
    }

    /// <summary>
    /// ビーム攻撃が終わったらエフェクト類を消す
    /// </summary>
    private void OnDisable()
    {
        LaserSaver = false;
        Laser.enabled = false;
        HitEffect.transform.position = transform.position; // レーザーの先を初期化

        _laserCollider.enabled = false; // 判定オフ
        _activationTimer = 3f;
    }

    /// <summary>
    /// ビーム処理
    /// </summary>
    private void Update()
    {
        // アクティブ化から3秒経過していなければ何もしない
        if (_activationTimer >= 0)
        {
            _activationTimer -= Time.deltaTime;
            return;
        }

        _laserCollider.enabled = true; // 判定オン

        //if (Laser.material.HasProperty("_SpeedMainTexUVNoiseZW")) Laser.material.SetVector("_SpeedMainTexUVNoiseZW", LaserSpeed);
        //SetVector("_TilingMainTexUVNoiseZW", Length); - old code, _TilingMainTexUVNoiseZW no more exist
        Laser.material.SetTextureScale("_MainTex", new Vector2(Length[0], Length[1]));
        Laser.material.SetTextureScale("_Noise", new Vector2(Length[2], Length[3]));
        //To set LineRender position
        if (Laser != null && UpdateSaver == false)
        {
            Laser.SetPosition(0, transform.position);
            RaycastHit hit; //DELETE THIS IF YOU WANT USE LASERS IN 2D
            int layerMask = ~LayerMask.GetMask("Player", "UI"); // 自分のレイヤーを無視するマスク
            //ADD THIS IF YOU WANNT TO USE LASERS IN 2D: RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.forward, MaxLength);       
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, MaxLength, layerMask))//CHANGE THIS IF YOU WANT TO USE LASERRS IN 2D: if (hit.collider != null)
            {
                // ビームが当たった場合の処理
                //End laser position if collides with object
                Laser.SetPosition(1, hit.point);

                HitEffect.transform.position = hit.point + hit.normal * HitOffset;
                if (useLaserRotation)
                    HitEffect.transform.rotation = transform.rotation;
                else
                    HitEffect.transform.LookAt(hit.point + hit.normal);

                foreach (var AllPs in Effects)
                {
                    if (!AllPs.isPlaying) AllPs.Play();
                }
                //Texture tiling
                Length[0] = MainTextureLength * (Vector3.Distance(transform.position, hit.point));
                Length[2] = NoiseTextureLength * (Vector3.Distance(transform.position, hit.point));
                //Texture speed balancer {DISABLED AFTER UPDATE}
                //LaserSpeed[0] = (LaserStartSpeed[0] * 4) / (Vector3.Distance(transform.position, hit.point));
                //LaserSpeed[2] = (LaserStartSpeed[2] * 4) / (Vector3.Distance(transform.position, hit.point));
                //Destroy(hit.transform.gameObject); // destroy the object hit
                //hit.collider.SendMessage("SomeMethod"); // example
                /*if (hit.collider.tag == "Enemy")
                {
                    hit.collider.GetComponent<HittedObject>().TakeDamage(damageOverTime * Time.deltaTime);
                }*/

                // BoxCollider のサイズと位置を更新
                float distance = Vector3.Distance(transform.position, hit.point);
                _laserCollider.size = new Vector3(_laserCollider.size.x, _laserCollider.size.y, distance);
                _laserCollider.center = new Vector3(0, 0, distance / 2f);
            }
            else
            {
                // ビームが何も当たらない場合の処理
                //End laser position if doesn't collide with object
                var EndPos = transform.position + transform.forward * MaxLength;
                Laser.SetPosition(1, EndPos);
                HitEffect.transform.position = EndPos;
                foreach (var AllPs in Hit)
                {
                    if (AllPs.isPlaying) AllPs.Stop();
                }
                //Texture tiling
                Length[0] = MainTextureLength * (Vector3.Distance(transform.position, EndPos));
                Length[2] = NoiseTextureLength * (Vector3.Distance(transform.position, EndPos));
                //LaserSpeed[0] = (LaserStartSpeed[0] * 4) / (Vector3.Distance(transform.position, EndPos)); {DISABLED AFTER UPDATE}
                //LaserSpeed[2] = (LaserStartSpeed[2] * 4) / (Vector3.Distance(transform.position, EndPos)); {DISABLED AFTER UPDATE}

                // BoxCollider のサイズと位置を更新
                _laserCollider.size = new Vector3(_laserCollider.size.x, _laserCollider.size.y, MaxLength);
                _laserCollider.center = new Vector3(0, 0, MaxLength / 2f);
            }
            //Insurance against the appearance of a laser in the center of coordinates!
            if (Laser.enabled == false && LaserSaver == false)
            {
                LaserSaver = true;
                Laser.enabled = true;
            }
        }

        // 現在の太さを取得
        float currentWidth = Laser.startWidth;

        // 太さを徐々に増加
        float newWidth = Mathf.MoveTowards(currentWidth, _targetWidth, _growSpeed * Time.deltaTime);

        // LineRendererの太さを更新
        Laser.startWidth = newWidth;
        Laser.endWidth = newWidth;
    }

    public void DisablePrepare()
    {
        if (Laser != null)
        {
            Laser.enabled = false;
        }
        UpdateSaver = true;
        //Effects can = null in multiply shooting
        if (Effects != null)
        {
            foreach (var AllPs in Effects)
            {
                if (AllPs.isPlaying) AllPs.Stop();
            }
        }
    }
}
