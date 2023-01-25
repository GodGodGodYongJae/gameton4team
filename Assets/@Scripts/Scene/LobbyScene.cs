using System.Collections;
using UnityEngine;

namespace Assets._Scripts.Scene
{
    public class LobbyScene : BaseScene
    {

        UI_Lobby _lobbySceneUI;
        protected override bool Init()
        {
            if (base.Init() == false)
                return false;

            SceneType = Define.SceneType.Lobby;

            Managers.UI.ShowSceneUI<UI_Lobby>(callback: (LobbyScene) =>
            {
                _lobbySceneUI = LobbyScene;
            });
            return true;
        }
    }
}