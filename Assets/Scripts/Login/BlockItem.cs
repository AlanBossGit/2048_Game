using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BlockItem : MonoBehaviour
{
    //方块的X坐标下标
    public int posX;
    //方块的Y坐标下标
    public int posY;
    //方块的乘方
    public int power;
    //方块的图片
    public Image image;

    //方块是否在移动
    public bool isMove;
    //方块是否已经合成过
    public bool hadCompound;
    //方块移动完成后是否需要被删除
    public bool isNeedDestory;
    
    //方块的移动动画
    public Tweener tweener;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    void Start()
    {
        //乘方随机得到 1 或者 2
        power = Random.value > 0.3f ? 1 : 2;
        if (PlayInfo.Instance.GetLevelType().Equals(LevelType.SHuiGuo))
        {
            //获取图片
            image.sprite = GameManager.NumSpriteList[power - 1];
        }
        else
        {
            //获取图片
            image.sprite = GameManager.BlockSpriteList[power - 1];
        }


        
        //随机位置
        do
        {
            posX = Random.Range(0, 4);
            posY = Random.Range(0, 4);
        } while (GameManager.BlockList[posX, posY] != null);

        //初始化位置
        transform.localPosition = new Vector2(-369 + posX * 246, -368 + posY * 246);

        //赋值给方块数组
        GameManager.BlockList[posX, posY] = this;

        //检测游戏是否失败
        if (GameManager.GameFailure())
        {
            //重置最高分
            int bestScore = PlayerPrefs.GetInt("BestScore");
            bestScore = bestScore > GameManager.CurrentScore ? bestScore : GameManager.CurrentScore;
            GameManager.bestScore = bestScore;
            PlayerPrefs.SetInt("BestScore", GameManager.bestScore);

            GameManager.isFailure = true;

            //打开失败场景
            GameManager.PlayGameOverAudio();
            StartCoroutine(GameOver());
        }
    }
    
    void Update()
    {
        //方块移动
        if (!isMove)
        {
            //如果方块的位置不是正确的位置
            if (transform.localPosition != new Vector3(-369 + posX * 246, -368 + posY * 246, 0))
            {
                isMove = true;

                //移动动画
                tweener = transform.DOLocalMove(new Vector3(-369 + posX * 246, -368 + posY * 246, 0), 0.1f);
                //动画完成后重置
                tweener.OnComplete(AnimOnComplete);
            }
        }
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1f);
        //延时 1 秒打开结束场景
        MessageManager.TriggerEvent(DefineManager.OpenOverScene);
    }

    /// <summary>
    /// 移动动画完成后调用的方法，用来重置信息
    /// </summary>
    public void AnimOnComplete()
    {
        //合并后需要进行销毁
        if (isNeedDestory)
        {
            //加分
            GameManager.CurrentScore += (int)Mathf.Pow(2,power) * 10;
            //销毁该对象
            Destroy(this.gameObject);

            if (PlayInfo.Instance.GetLevelType().Equals(LevelType.SHuiGuo))
            {
                //更换合并完成的图片
                GameManager.BlockList[posX, posY].image.sprite = GameManager.NumSpriteList[GameManager.BlockList[posX, posY].power - 1];
            }
            else
            {
                //更换合并完成的图片
                GameManager.BlockList[posX, posY].image.sprite = GameManager.BlockSpriteList[GameManager.BlockList[posX, posY].power - 1];
            }

            
        }
        isMove = false;

        //从方块数组移除该对象
        GameManager.MovingBlockList.Remove(this);
    }

    /// <summary>
    /// 方块移动的方块，外界调用
    /// </summary>
    /// <param name="directionX"></param>
    /// <param name="directionY"></param>
    public void Move(int directionX, int directionY)
    {
        //向右移动
        if (directionX == 1)
        {
            int index = 1;

            //得到寻找的方向上，空方块的个数
            while (GameManager.isEmpty(posX + index, posY))
            {
                index++;
            }

            //修改坐标，更新数组
            if (index > 1)
            {
                GameManager.hadMove = true;

                //判定移动数组中是否该对象
                if (!GameManager.MovingBlockList.Contains(this))
                {
                    GameManager.MovingBlockList.Add(this);
                }
                //置空方块数组的原来位置
                GameManager.BlockList[posX, posY] = null;

                //修改方块坐标并重新赋值方块数组
                posX = posX + index - 1;
                GameManager.BlockList[posX, posY] = this;
            }

            //判断数字是否相同并且没有合成过，相同合并
            if (power < 11 && posX < GameManager.fatory - 1 && power == GameManager.BlockList[posX + 1, posY].power && !GameManager.BlockList[posX + 1, posY].hadCompound)
            {
                GameManager.hadMove = true;
                //标记为已经合成
                GameManager.BlockList[posX + 1, posY].hadCompound = true;
                
                //判定移动数组中是否该对象
                if (!GameManager.MovingBlockList.Contains(this))
                {
                    GameManager.MovingBlockList.Add(this);
                }

                //销毁该对象
                isNeedDestory = true;
                //更新合成对象的乘方
                GameManager.BlockList[posX + 1, posY].power++;
                //置空方块数组的原来位置
                GameManager.BlockList[posX, posY] = null;
                //更新坐标
                posX += 1;
            }
        }
        //向左移动
        else if (directionX == -1)
        {
            int index = 1;

            //得到寻找的方向上，空方块的个数
            while (GameManager.isEmpty(posX - index, posY))
            {
                index++;
            }

            //修改坐标，更新数组
            if (index > 1)
            {
                GameManager.hadMove = true;
                if (!GameManager.MovingBlockList.Contains(this))
                {
                    GameManager.MovingBlockList.Add(this);
                }
                GameManager.BlockList[posX, posY] = null;
                posX = posX - index + 1;
                GameManager.BlockList[posX, posY] = this;
            }

            //判断数字是否相同，相同合并
            if (power < 11 && posX > 0 && power == GameManager.BlockList[posX - 1, posY].power && !GameManager.BlockList[posX - 1, posY].hadCompound)
            {
                GameManager.hadMove = true;
                GameManager.BlockList[posX - 1, posY].hadCompound = true;
                if (!GameManager.MovingBlockList.Contains(this))
                {
                    GameManager.MovingBlockList.Add(this);
                }
                isNeedDestory = true;
                GameManager.BlockList[posX - 1, posY].power++;
                GameManager.BlockList[posX, posY] = null;
                posX -= 1;
            }
        }
        //向上移动
        else if (directionY == 1)
        {
            int index = 1;

            //得到寻找的方向上，空方块的个数
            while (GameManager.isEmpty(posX, posY + index))
            {
                index++;
            }

            //修改坐标，更新数组
            if (index > 1)
            {
                GameManager.hadMove = true;
                if (!GameManager.MovingBlockList.Contains(this))
                {
                    GameManager.MovingBlockList.Add(this);
                }
                GameManager.BlockList[posX, posY] = null;
                posY = posY + index - 1;
                GameManager.BlockList[posX, posY] = this;
            }

            //判断数字是否相同，相同合并
            if (power < 11 && posY < GameManager.fatory - 1 && power == GameManager.BlockList[posX, posY + 1].power && !GameManager.BlockList[posX, posY + 1].hadCompound)
            {
                GameManager.hadMove = true;
                GameManager.BlockList[posX, posY + 1].hadCompound = true;
                if (!GameManager.MovingBlockList.Contains(this))
                {
                    GameManager.MovingBlockList.Add(this);
                }
                isNeedDestory = true;
                GameManager.BlockList[posX, posY + 1].power++;
                GameManager.BlockList[posX, posY] = null;
                posY += 1;
            }
        }
        //向下移动
        else if (directionY == -1)
        {
            int index = 1;

            //得到寻找的方向上，空方块的个数
            while (GameManager.isEmpty(posX, posY - index))
            {
                index++;
            }

            //修改坐标，更新数组
            if (index > 1)
            {
                GameManager.hadMove = true;
                if (!GameManager.MovingBlockList.Contains(this))
                {
                    GameManager.MovingBlockList.Add(this);
                }
                GameManager.BlockList[posX, posY] = null;
                posY = posY - index + 1;
                GameManager.BlockList[posX, posY] = this;
            }

            //判断数字是否相同，相同合并
            if (power < 11 && posY > 0 && power == GameManager.BlockList[posX, posY - 1].power && !GameManager.BlockList[posX, posY - 1].hadCompound)
            {
                GameManager.hadMove = true;
                GameManager.BlockList[posX, posY - 1].hadCompound = true;
                if (!GameManager.MovingBlockList.Contains(this))
                {
                    GameManager.MovingBlockList.Add(this);
                }
                isNeedDestory = true;
                GameManager.BlockList[posX, posY - 1].power++;
                GameManager.BlockList[posX, posY] = null;
                posY -= 1;
            }
        }
    }
}
