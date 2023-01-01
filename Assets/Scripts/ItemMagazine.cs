using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMagazine : ItemBase {
    [SerializeField]
    private GameObject magazineEffectPrefab;
    [SerializeField]
    private int increaseMagazine = 1;
    [SerializeField]
    private float rotateSpeed = 50;


    private IEnumerator Start() {
        while (true) {
            transform.Rotate(Vector3.up * this.rotateSpeed * Time.deltaTime);

            yield return null;
        }
    }

    public override void Use(GameObject entity) {
        entity.GetComponentInChildren<WeaponAR>().IncreaseMagazine(this.increaseMagazine);
        Instantiate(this.magazineEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

}