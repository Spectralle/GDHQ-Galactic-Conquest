﻿using System.Collections;
using UnityEngine;


public class BossGeneratorGun : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float _attackFrequency;

    private Transform _projectileContainer;
    private bool _readyToShoot;
    private AudioSource _audioSource;


    void Awake()
    {
        _audioSource = transform.parent.GetComponent<AudioSource>();
        _projectileContainer = GameObject.Find("Projectile Container").transform;
        StartCoroutine(ShotCooldown());
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 180);

        if (BossFightManager._canGensShoot && _readyToShoot)
            StartCoroutine(MakeAnAttack(AttackLibrary.Laser.Boss.ThreeForward30()));
    }

    private IEnumerator MakeAnAttack(AttackTemplate attackData)
    {
        _readyToShoot = false;

        float angleStep = attackData.Degrees / attackData.Number;
        float angle = attackData.Degrees != 360 ? -(attackData.Degrees - angleStep) / 2 : 0f;
        float transformUpAngle = Mathf.Atan2(transform.up.x, transform.up.y);
        float PIx2 = Mathf.PI * 2;
        Vector3 originPoint = transform.position;

        for (int i = 0; i < attackData.Number; i++)
        {
            Vector2 startPosition = new Vector2(
                Mathf.Sin(((angle * Mathf.PI) / 180) + transformUpAngle),
                Mathf.Cos(((angle * Mathf.PI) / 180) + transformUpAngle)
            );

            Vector2 relativeStartPosition = (Vector2)originPoint + startPosition * attackData.Radius;
            float rotationZ = (360 - angle) - (angle * PIx2 + transformUpAngle) * Mathf.Rad2Deg;
            Vector2 shotMovementVector = (relativeStartPosition - (Vector2)originPoint).normalized * attackData.Speed;

            Vector2 relativeCurrentStartPosition = (Vector2)transform.position + startPosition * attackData.Radius;
            Vector2 shotCurrentMovementVector = (relativeCurrentStartPosition - (Vector2)transform.position).normalized * attackData.Speed;

            GameObject shot = Instantiate(
                attackData.Prefab,
                attackData.Delay == 0 ? relativeStartPosition : relativeCurrentStartPosition,
                Quaternion.Euler(0, 0, rotationZ),
                _projectileContainer);
            shot.tag = "Enemy Projectile";
            shot.GetComponent<Rigidbody2D>().velocity = attackData.Delay == 0 ? shotMovementVector : shotCurrentMovementVector;

            angle += angleStep;

            if (attackData.Delay > 0)
            {
                if (attackData.Delay >= 0.1f && _audioSource && attackData.AudioClip)
                    _audioSource.PlayOneShot(attackData.AudioClip);
                yield return new WaitForSeconds(attackData.Delay);
            }
        }

        if (attackData.Delay < 0.1f && _audioSource && attackData.AudioClip)
            _audioSource.PlayOneShot(attackData.AudioClip);
        StartCoroutine(ShotCooldown());
    }

    private IEnumerator ShotCooldown()
    {
        yield return new WaitForSeconds(_attackFrequency);
        _readyToShoot = true;
    }

    private void OnDrawGizmos() => Gizmos.DrawRay(transform.position, transform.up);
}