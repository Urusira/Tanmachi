using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShiroGe.CharacterController
{
    [DefaultExecutionOrder(-2)]
    public class PlayerActionsController : MonoBehaviour, PlayerControls.IPlayerActionsActions
    {
        public PlayerControls PlayerControls { get; private set; }
        public bool AttackInput { get; private set; }
        public bool InteractInput { get; private set; }

        private void OnEnable()
        {
            PlayerControls = new PlayerControls();
            PlayerControls.Enable();

            PlayerControls.PlayerActions.Enable();
            PlayerControls.PlayerActions.SetCallbacks(this);
        }

        private void OnDisable()
        {
            PlayerControls.PlayerActions.Disable();
            PlayerControls.PlayerActions.RemoveCallbacks(this);
        }

        /*private void LateUpdate()
        {
            AttackInput = false;
            InteractInput = false;
        }*/

        public void SetAttackPressedFalse()
        {
            AttackInput = false;
        }

        public void SetInteractPressedFalse()
        {
            InteractInput = false;
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            AttackInput = true;
        }

        /*
         * TODO: Продумать эту срань. При подборе в конце анимки сбрасывается интеракт вручную,
         * но без анимки работает только этот костыль. Проблема в том, что при подборе предмета
         * врубается интеракт, но при отжатии клавиши он сбрасывается, что может произойти раньше,
         * чем закончится анимация. А при взаимодействии у меня идёт проверка как раз прожат ли
         * интеракт и идёт ли он сейчас, что означает возможность нового интеракта
         * до окончания текущего. Тут либо надо делать анимации на все взаимодействия и чтобы они
         * сбрасывали интеракт, либо тащить в код каждого предмета контроллер этот и сбрасывать вручную интеракт,
         * либо сделать отдельно кнопку взаимодействия с одной клавишей на каждый вид взаимодействия, но это не сработает.
         * Уже в процессе написания понял, что последний вариант не прокатит, там куча действий за раз будет тригериться
         * и ситуация будет ровной той же самой, а то и хуже.
         * Изначальная проблема в том, что в общем контроллере игрока перед пусканием луча идёт проверка с участием интеракта,
         * где и происходил изначальный сброс ластфрейма после взаимодействия, однако при добавлении данного контроллера действий
         * интеракт ничто не сбрасывало. Если действие не пикап, который в самой анимации вызывает сброс интеракта, то интеракт
         * оставался трушным, из-за чего проверка ластфрейма не проходила и сброс его соответственно не происходил, алгоритм клинило буквально.
         * Такой костыль работает, но есть вышеописанные минусы.
        */
        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            InteractInput = true;
        }
    }
}