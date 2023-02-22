using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Game._Creature._Player._Helper
{
    internal class LevelHelper : PlayerHelper
    {

        private float currentExp = 0;
        /// <summary>
        /// 생성과 동시에 리스너 등록.
        /// </summary>
        public LevelHelper(Player player) : base(player)
        {
            Managers.Events.AddListener(Define.GameEvent.monsterDestroy, MonsterKill);
            LevelUP();
        }

        /// <summary>
        /// 몬스터를 죽였을 때 실행되는 함수.
        /// </summary>
        private void MonsterKill(Define.GameEvent eventType, Component Sender, object param)
        {
            if (eventType == Define.GameEvent.monsterDestroy)
            {
                Monster monster = (Monster)Sender;
                currentExp += monster.MonsterData.Exp;
                if (currentExp >= playerData.ExperiencePoint && playerData.ExperiencePoint != int.MaxValue)
                {

                    LevelUpText().Forget();
                    LevelUP();
                }
                PostChangeExp();
            }
        }

        /// <summary>
        /// 레벨업 텍스트 띄어주는 메서드
        /// </summary>
        /// <returns></returns>
        private async UniTaskVoid LevelUpText()
        {
            Managers.Object.InstantiateAsync("LevelUpText", transform.position);
        }

        /// <summary>
        /// 데이터 수정 및 체력 회복, UI에게 포스트 전달
        /// </summary>
        private void LevelUP()
        {
            playerData.LevelUp();
            player.AddHp(playerData.MaxHP);
            currentExp = 0;
            PostChangeExp();
        }

        private void PostChangeExp()
        {
          Managers.Events.PostNotification(Define.GameEvent.playerExpChange, player ,currentExp);
        }
    }
}
