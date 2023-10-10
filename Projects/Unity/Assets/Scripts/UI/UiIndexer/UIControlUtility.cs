using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace UI
{
    /// <summary>
    /// UI操作ユーティリティ
    /// </summary>
    public static class UIControlUtility
    {
        /// <summary>
        /// スプライトリストの後始末
        /// SpriteAtlasから取得したSpriteは後で必ずDestroyする必要がある
        /// </summary>
        public static void DestroyAtlasSpriteList(List<Sprite> list)
        {
            foreach (var spr in list)
            {
                GameObject.Destroy(spr);
            }
            list.Clear();
        }
    }

    /// <summary>
    /// SpriteAtlasから生成したSpriteリスト
    /// 使用後はDestroyを呼ぶ
    /// </summary>
    public class AtlasSpriteList
    {
        private List<Sprite> m_SpriteList;

        public AtlasSpriteList(int count = 0)
        {
            m_SpriteList = new List<Sprite>(count);
        }
        /// <summary>
        /// Sprite直接追加
        /// </summary>
        public void Add(Sprite spr)
        {
            m_SpriteList.Add(spr);
        }
        public void AddRange(List<Sprite> list)
        {
            m_SpriteList.AddRange(list);
        }
        /// <summary>
        /// SpriteAtlasから追加
        /// </summary>
        public void Add(SpriteAtlas atlas, string name)
        {
            var spr = atlas.GetSprite(name);
            m_SpriteList.Add(spr);
        }
        /// <summary>
        /// Sprite取得
        /// </summary>

        public Sprite this[int index]
        {
            get { return m_SpriteList[index]; }
        }
        /// <summary>
        /// 登録数
        /// </summary>
        public int Count { get { return m_SpriteList.Count; } }
        /// <summary>
        /// 後始末
        /// </summary>
        public void Destroy()
        {
            UIControlUtility.DestroyAtlasSpriteList(m_SpriteList);
        }
    }
}
