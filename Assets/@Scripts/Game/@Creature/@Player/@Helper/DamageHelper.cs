using Assets._Scripts.Game._Creature._Player._Helper.HelperInterface;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;
using static UnityEngine.GraphicsBuffer;

namespace Assets._Scripts.Game._Creature._Player._Helper
{
    internal class DamageHelper : PlayerHelper, IDamageHelper
    {
        #region property
        private float currentHp = 0;
        private bool isDamage = false;
        public SPUM_SpriteList root;
        private const int blinkCount = 3;
        private Color blinkRedColor = Color.red;
        private Dictionary<SpriteRenderer,Color> blinkOriginColorList = new Dictionary<SpriteRenderer,Color>();
        private AudioClip respawnClip;
        private Color returnToBlinkColor(bool cur,SpriteRenderer spriteId)
        {
            Color color = Color.white;
            if (blinkOriginColorList.ContainsKey(spriteId))
            {
                color = blinkOriginColorList[spriteId];
            }
           
            return cur ? blinkRedColor : color ;
        }
        #endregion
        public DamageHelper(Player player) : base(player)
        {
            root = player.GetSPUMSpriteList;
            PostEventHp();
        }

        public void InitResource()
        {
            List<List<SpriteRenderer>> AllSpriteList = root.AllSpriteList;
            foreach (var item in AllSpriteList)
            {
                List<SpriteRenderer> spriteList = item;
                foreach (var item2 in spriteList)
                {

                    try
                    {
                        blinkOriginColorList.Add(item2, item2.color);
                    }
                    catch
                    {
                    }
                   
                }
            }
        }
        #region 데미지 이벤트

        public void Damage(float Dmg, Creature Sender)
        {
            if (isDamage == true) return;
          
            InvincibilityDealy().Forget();
            BlinkObject().Forget();
            player.KnockBack(Sender.gameObject);

            currentHp = (currentHp - Dmg < 0) ? 0 : currentHp - Dmg;
            if(currentHp <= 0)
             Death();
           
            PostEventHp();
        }

        /// <summary>
        /// 피격 무적시간
        /// </summary>
        /// <returns></returns>
        private async UniTaskVoid InvincibilityDealy()
        {
            isDamage = true;
            await UniTask.Delay(playerData.InvinvibilityDuration);
            isDamage = false;
        }
        /// <summary>
        /// 피격시 블링크
        /// </summary>
        /// <returns></returns>
        public async UniTaskVoid BlinkObject()
        {
            List<List<SpriteRenderer>> AllSpriteList = root.AllSpriteList;
            bool spriteColor = false;
            for (int i = 0; i < blinkCount * 2; i++)
            {
                spriteColor = !spriteColor;
                foreach (var item in AllSpriteList)
                {
                    List<SpriteRenderer> spriteList = item;
                    foreach (var item2 in spriteList)
                    {
                        item2.color = returnToBlinkColor(spriteColor,item2);
                    }
                }
                await UniTask.Delay(150);
            }

        }

        private void Death()
        {
            Time.timeScale = 0;
            respawnClip = Managers.Sound.CurrentMusicClip;
            Managers.Events.PostNotification(Define.GameEvent.playerEvents, player, PlayerActionKey.Death);
            Managers.Sound.PlaySFX("Death");
            Managers.Sound.PlayBGM("Death_bgm");
        }
        #endregion

        #region 체력관련
        public void AddHp(float add)
        {
            currentHp = (currentHp + add >= playerData.MaxHP) ? playerData.MaxHP : currentHp + add;
            PostEventHp();
        }


        private void PostEventHp()
        {
            Define.PlayerEvent_HPData data = new Define.PlayerEvent_HPData();
            data.maxHp = playerData.MaxHP;
            data.curHp = currentHp;
            Managers.Events.PostNotification(Define.GameEvent.playerHealthChange, player, data);
        }



        #endregion

        public void Respawn()
        {
            Time.timeScale = 1;
            AddHp(playerData.MaxHP);
            Managers.Sound.PlayBGM(respawnClip);
        }


    }
}