using UnityEngine;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Input
{
    
    public class TouchInputReader
    {
        #region PrimaryFinger
        public Vector2 PrimaryFingerPosition { get; private set; }
        public void SetPrimaryFingerPosition(Vector2 position) => PrimaryFingerPosition = position;
        public Vector2 PrimaryFingerPreviousFramePosition { get; private set; }
        public void SetPrimaryFingerPreviousFramePosition(Vector2 position) => PrimaryFingerPreviousFramePosition = position;
        
        public TouchPhase PrimaryFingerPhase { get; private set; }
        public void SetPrimaryFingerPhase(TouchPhase phase) => PrimaryFingerPhase = phase;
        
        public Vector2 DeltaPrimaryFingerPosition { get => PrimaryFingerPosition - PrimaryFingerPreviousFramePosition; }
        
        #endregion

        #region SecondaryFinger

        public Vector2 SecondaryFingerPosition { get; private set; }
        public void SetSecondaryFingerPosition(Vector2 position) => SecondaryFingerPosition = position;
        public Vector2 SecondaryFingerPreviousFramePosition { get; private set; }
        public void SetSecondaryFingerPreviousFramePosition(Vector2 position) => SecondaryFingerPreviousFramePosition = position;
        
        public TouchPhase SecondaryFingerPhase { get; private set; }
        public void SetSecondaryFingerPhase(TouchPhase phase) => SecondaryFingerPhase = phase;
        
        public Vector2 DeltaSecondaryFingerPosition { get => SecondaryFingerPosition - SecondaryFingerPreviousFramePosition; }

        #endregion

        #region Misc
        public float DistanceBetweenFingers { get => Vector2.Distance(PrimaryFingerPosition, SecondaryFingerPosition); }
        public float PreviousFrameDistanceBetweenFingers { get => Vector2.Distance(PrimaryFingerPreviousFramePosition, SecondaryFingerPreviousFramePosition); }
        public float DeltaDistanceBetweenFingers { get => DistanceBetweenFingers - PreviousFrameDistanceBetweenFingers; }
        public bool IsTouchingInteractable { get; private set; }
        public void SetIsTouchingInteractable(bool value) => IsTouchingInteractable = value;

        public bool IsAppCurrentlyInteractable { get; private set; } = true;
        public void SetIsAppCurrentlyInteractable(bool value) => IsAppCurrentlyInteractable = value;
        #endregion

        public delegate void TouchUpdate();
        public TouchUpdate OnTouchUpdate;
    }
}