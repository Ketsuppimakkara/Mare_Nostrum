using UnityEngine;

public interface IDamageable
{
    public GameObject gameObject { get ; }

    public void handleDamage(Collision collision, GameObject projectile);
    public void handleDamageOverTime(Collider collider, float damagePerSecond);

    public void lodgeProjectile(Collision collision, GameObject projectile);
    
    public void lodgeProjectile(Collision collision, GameObject projectile, RaycastHit hit);

    public bool isEnabled();
}