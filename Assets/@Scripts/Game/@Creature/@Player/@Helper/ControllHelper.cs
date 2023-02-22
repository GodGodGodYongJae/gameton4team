using Assets._Scripts.Game._Creature._Player._Helper.HelperInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Player;

namespace Assets._Scripts.Game._Creature._Player._Helper
{

    internal class ControllHelper : PlayerHelper, IControllerHelper
    {
        private SwipeController swipeController;
        private Rigidbody2D rigid;
        private Vector2 directionVector; // 방향

        float jumpForce = 0;
        public ControllHelper(Player player) : base(player)
        {
            swipeController = new SwipeController();

            rigid = transform.GetComponent<Rigidbody2D>();
            Managers.FixedUpdateAction += FUpdate;
            Managers.UpdateAction += swipeController.MUpdate;

            player.PlayerActionAdd(PlayerActionKey.Direction, ChangeDirection);
            player.PlayerActionAdd(PlayerActionKey.Jump, Jump);
            directionVector = Vector2.right;
        }
        /// <summary>
        /// FixedUpdate
        /// </summary>
        private void FUpdate()
        {
            //MOVE
            transform.Translate(directionVector * (  playerData.Speed) * Time.deltaTime);
            //JUMP
            if(jumpForce != 0)
            {
                rigid.AddForce(Vector2.up * jumpForce);
                jumpForce = 0;
            }
                
        }

        /// <summary>
        /// 방향을 바꿔주는 메소드
        /// </summary>
        private void ChangeDirection()
        {
            if (Time.timeScale == 1)
            {
                directionVector = directionVector * -1;
                transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
            }
        }

        /// <summary>
        /// 점프하는 메소드
        /// </summary>
        /// 
        private void Jump()
        {
            if (rigid.velocity.y == 0)
            {
                jumpForce = playerData.JumpForce;
            }
        }

        #region interface
        public Vector2 returnToDirection() => directionVector;
        #endregion
    }
}
