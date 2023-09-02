using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对象池信息结构体
/// </summary>
[System.Serializable]
public struct ObjectPoolInfo
{
    public GameObject obj;
    public int poolSize;
    public bool isAutoExpand;
}

/// <summary>
/// 对象池管理器，用法：创建ObjectPools，挂载该脚本，其下创建Enemy和Effect空物体
/// </summary>
public class ObjectPoolManager : SingletonMono<ObjectPoolManager>
{

    //对象池
    public GameObject objectPool;
    //敌人对象池
    public GameObject enemyObjectPool;
    //特效对象池
    public GameObject effectObjectPool;
    //敌人对象池链表
    public List<GameObject> enemyObjectPoolList;
    //对象池容器，存放对象池
    public Dictionary<string, ObjectPool> objectPools;
    //对象池信息数组
    public ObjectPoolInfo[] ObjectPoolInfo;

    void Awake()
    {
        enemyObjectPool = transform.Find("Enemy").gameObject;
    }

    void Start()
    {
        objectPools = new Dictionary<string, ObjectPool>();

        //遍历对象池信息数组，创建相应的对象池
        for (int i = 0; i < ObjectPoolInfo.Length; i++)
        {
            print(enemyObjectPool.name);
            CreateObjectPool(ObjectPoolInfo[i].obj, ObjectPoolInfo[i].poolSize, ObjectPoolInfo[i].isAutoExpand);
        }
    }

    /// <summary>
    /// 创建对象池
    /// </summary>
    /// <param name="_obj">对象池生成的对象</param>
    /// <param name="_poolSize">对象池大小</param>
    /// <param name="_isAutoExpand">是否自动扩充</param>
    public void CreateObjectPool(GameObject _obj, int _poolSize, bool _isAutoExpand)
    {
        //判断对象池是否存在
        if (objectPools.ContainsKey(_obj.name))
        {
            Debug.LogError("对象池已经存在，直接获取就可以");
        }
        else
        {
            ObjectPool objectPool = new ObjectPool(_obj, _poolSize, _isAutoExpand);
            objectPools.Add(_obj.name, objectPool);
        }
    }

    /// <summary>
    /// 通过对象池的名字获取对应对象
    /// </summary>
    /// <param name="objName"></param>
    /// <returns></returns>
    public GameObject GetGameObject(string objName)
    {
        GameObject obj = objectPools[objName].GetPoolGameObject();
        if (objName == "Bullet")
        {
            obj.transform.parent = objectPool.transform;
        }
        else if (objName == "Effects01")
        {
            obj.transform.parent = effectObjectPool.transform;
        }
        else if (objName == "Effects02")
        {
            obj.transform.parent = effectObjectPool.transform;
        }
        else
        {
            obj.transform.parent = enemyObjectPool.transform;
            enemyObjectPoolList.Add(obj);
        }
        return obj;
    }
    
    /// <summary>
    /// 新建一个类，用于对象的回收处理
    /// </summary>
    public void DestroyGameObject()
    {
        
    //    void OnEnable()
    //    {
    //        Invoke("DestroyPoolGameObject", 2f);
    //    }

    //    void DestroyPoolGameObject()
    //    {
    //        gameObject.SetActive(false);
    //    }

    //    void OnDisable()
    //    {
    //        CancelInvoke();
    //    }
    }
    
}
