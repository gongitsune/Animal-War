using System;
using Projects.Scripts.Domains.Input;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Projects.Scripts.Applications.Input
{
    public class PlayerInputUseCase : IDisposable
    {
        private readonly InputMap _input = new();

        private readonly Subject<InputAction.CallbackContext>
            _onFire = new(),
            _onJump = new(),
            _onSprint = new();

        public bool Enabled
        {
            get => _input.Player.enabled;
            set
            {
                if (value) _input.Player.Enable();
                else _input.Player.Disable();
            }
        }
        
        public PlayerInputUseCase()
        {
            _input.Player.Fire.started += _onFire.OnNext;
            _input.Player.Jump.started += _onJump.OnNext;
            _input.Player.Dash.started += _onSprint.OnNext;
        }

        public IObservable<InputAction.CallbackContext> OnFire => _onFire;
        public IObservable<InputAction.CallbackContext> OnJump => _onJump;
        public IObservable<InputAction.CallbackContext> OnSprint => _onSprint;
        public Vector2 Move => _input.Player.Move.ReadValue<Vector2>();

        #region 破棄処理

        public void Dispose()
        {
            _input.Player.Fire.started -= _onFire.OnNext;
            _input.Player.Jump.started -= _onJump.OnNext;
            _input.Player.Dash.started -= _onSprint.OnNext;

            _input.Dispose();
            _onFire.Dispose();
            _onJump.Dispose();
            _onSprint.Dispose();
        }

        ~PlayerInputUseCase()
        {
            Dispose();
        }

        #endregion
    }
}