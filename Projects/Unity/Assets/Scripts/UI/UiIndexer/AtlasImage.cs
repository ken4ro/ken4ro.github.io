using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

namespace UI {
    /// <summary>
    /// SpriteAtlasを使用したImageクラス
    /// SpriteAtlas内のSprite切り替えをするなら必須
    /// 基本的にはImageではなくこちらを使用する
    /// </summary>
    public class AtlasImage : Image {
        [SerializeField] SpriteAtlas m_Atlas;
        public SpriteAtlas atlas { get { return m_Atlas; } set { m_Atlas = value; } }
        [SerializeField] string m_SpriteName;

        // Editor拡張機能以外で使用しないでください。
        public string spriteName {
            get { return m_SpriteName; }
            set { m_SpriteName = value; }
        }
    }
}