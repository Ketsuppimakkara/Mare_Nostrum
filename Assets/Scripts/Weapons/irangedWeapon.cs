using UnityEngine;

public interface IRangedWeapon
{
    public GameObject gameObject { get ; }
    public void fire();
    public void reload();
    public bool targetIsInRange();
    public void nextAmmoType();
    public void setTarget(GameObject target);
    public float maxWeaponRange();
}