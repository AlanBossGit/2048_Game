using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    //对象池对象
    private GameObject poolObj;
    //对象池链表
    private List<GameObject> poolObjsList;
    //是否自动扩充对象池
    private bool isAutoExpand;


    /// <summary>
    /// 构造函数，参数为：对象池对象、对象池大小、是否自动扩充对象池
    /// </summary>
    /// <param name="_obj"></param>
    /// <param name="_poolSize"></param>
    /// <param name="_isAutoExpand"></param>
    public ObjectPool(GameObject _obj, int _poolSize, bool _isAutoExpand)
    {
        poolObjsList = new List<GameObject>();
        poolObj = _obj;
        isAutoExpand = _isAutoExpand;

        //根据对象池大小生成对象，并设置为失活状态
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject obj = GameObject.Instantiate(poolObj);
            obj.SetActive(false);

            //设置父物体
            if (poolObj.name == "Bullet")
            {
                obj.transform.parent = ObjectPoolManager.Instance.objectPool.transform;
            }
            else if (poolObj.tag == "Effects")
            {
                obj.transform.parent = ObjectPoolManager.Instance.effectObjectPool.transform;
            }
            else
            {
                obj.transform.parent = ObjectPoolManager.Instance.enemyObjectPool.transform;
                ObjectPoolManager.Instance.enemyObjectPoolList.Add(obj);
            }
            //obj.transform.parent = transform;

            //将实例化的对象添加到对象池链表中
            poolObjsList.Add(obj);
        }
    }

    /// <summary>
    /// 获取对象池的对象
    /// </summary>
    /// <returns></returns>
    public GameObject GetPoolGameObject()
    {
        //从对象池中获取对象
        for (int i = 0; i < poolObjsList.Count; i++)
        {
            if (!poolObjsList[i].activeSelf)
            {
                poolObjsList[i].SetActive(true);
                return poolObjsList[i];
            }
        }

        //对象池中所有对象都是激活状态时，根据是否可以自动扩充，扩充或者返回空
        if (isAutoExpand)
        {
            GameObject obj = GameObject.Instantiate(poolObj);
            //obj.transform.parent = transform;
            poolObjsList.Add(obj);
            return obj;
        }

        return null;
    }
}
