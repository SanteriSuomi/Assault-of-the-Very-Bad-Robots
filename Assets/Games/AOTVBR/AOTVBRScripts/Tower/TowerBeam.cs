using UnityEngine;

namespace AOTVBR
{
    public class TowerBeam : TowerBase
    {
        private LineRenderer lineRenderer;
        [SerializeField]
        private Vector3 laserOnPositionOffset = new Vector3(0, 0.4f, 0);

        private void Awake()
            => lineRenderer = GetComponentInChildren<LineRenderer>();

        protected override void EnemyDetectedEvent(Vector3 enemyPosition)
        {
            ActivateLineRenderer(true);
            ActivateLaser(enemyPosition);
        }

        protected override void ResetTower()
        {
            base.ResetTower();
            ActivateLineRenderer(false);
        }

        private void ActivateLineRenderer(bool value) 
            => lineRenderer.enabled = value;

        private void ActivateLaser(Vector3 target)
        {
            lineRenderer.SetPosition(0, turret.position + laserOnPositionOffset);
            lineRenderer.SetPosition(1, target);
        }

        protected override void PlayAttackAudio()
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }
}