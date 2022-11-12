using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryPool : MonoBehaviour {
    private class PoolItem {
        public bool isActive;
        public GameObject gameObject;
    }

    private int increaseCount = 5;
    private int maxCount;
        public int MaxCount => maxCount;
    private int activeCount;
        public int ActiveCount => activeCount;

    private GameObject poolObject;
    private List<PoolItem> poolItemList;


    public MemoryPool(GameObject poolObject) {
        this.maxCount = 0;
        this.activeCount = 0;
        this.poolObject = poolObject;
        this.poolItemList = new List<PoolItem>();

        InstantiateObjects();
    }

    public void InstantiateObjects() {
        this.maxCount += this.increaseCount;

        for (int i = 0; i < this.increaseCount; ++i) {
            PoolItem poolItem = new PoolItem();

            poolItem.isActive = false;
            poolItem.gameObject = GameObject.Instantiate(this.poolObject);
            poolItem.gameObject.SetActive(false);

            poolItemList.Add(poolItem);
        }
    }
    
    public void DestroyObjects() {
        if (this.poolItemList == null) {
            return;
        }

        int count = this.poolItemList.Count;

        for (int i = 0; i < count; ++i) {
            GameObject.Destroy(this.poolItemList[i].gameObject);
        }
        this.poolItemList.Clear();
    }

    public GameObject ActivatePoolItem() {
        if (this.poolItemList == null) {
            return null;
        }

        if (this.maxCount == this.activeCount) {
            InstantiateObjects();
        }

        int count = this.poolItemList.Count;

        for (int i = 0; i < count; ++i) {
            PoolItem poolItem = this.poolItemList[i];

            if (poolItem.isActive == false) {
                this.activeCount++;
                
                poolItem.isActive = true;
                poolItem.gameObject.SetActive(true);

                return poolItem.gameObject;
            }
        }

        return null;
    }

    public void DeactivatePoolItem(GameObject removeObject) {
        if (this.poolItemList == null || removeObject == null) {
            return;
        }

        int count = this.poolItemList.Count;

        for (int i = 0; i < count; ++i) {
            PoolItem poolItem = this.poolItemList[i];

            if (poolItem.gameObject == removeObject) {
                this.activeCount--;

                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);

                return;
            }
        }
    }

    public void DeactivateAllPoolItems() {
        if (this.poolItemList == null) {
            return;
        }

        int count = this.poolItemList.Count;

        for (int i = 0; i < count; ++i) {
            PoolItem poolItem = this.poolItemList[i];

            if (poolItem.gameObject != null && poolItem.isActive == true) {
                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);
            }
        }

        this.activeCount = 0;
    }
}