using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum UIEvent 
    { 
        Click,
        Press
    }

    public enum SceneType 
    {
        Unknown,
        GameScene,
        Lobby,
        TitleScene
    }

    // Ǯ���� �׶��� ����.
    public const int PoolGroundSize = 4;
    // �׶��� ���� y�� ��ġ 
    public const float GroundPosY = -5f;
    public StringData stringData;

}
