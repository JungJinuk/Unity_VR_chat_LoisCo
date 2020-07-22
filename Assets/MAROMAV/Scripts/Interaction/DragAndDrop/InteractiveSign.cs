namespace MAROMAV.CoworkSpace
{

    using UnityEngine;
    using System.Collections;

    // A script that teleports an interactive object to
    // its initial position and rotation on release, and
    // allows the object to maintain facing to another game object.
    public class InteractiveSign : MonoBehaviour
    {

        [Tooltip("The interactive object component on this game object.")]
        public BaseInteractiveObject interactiveObject;

        [Tooltip("The rigidbody attached to this object.")]
        public Rigidbody rigidbodyCmp;

        [Tooltip("Sound played when the object is selected.")]
        public AudioClip selectSound;

        public Vector3 lookPos = new Vector3(0, 4, -10);

        private Quaternion targetRotation;

        private bool isSelected;

        private BaseInteractiveObject.ObjectState state;
        private BaseInteractiveObject.ObjectState stateLastFrame;

        private float lastBounceTime;

        private const float BOUNCE_SOUND_DELAY = 0.5f;
        private const float MIN_BOUNCE_VELOCITY = 0.5f;

        public bool isControllable = false;

        void OnValidate()
        {
            if (interactiveObject == null)
            {
                interactiveObject = gameObject.GetComponent<BaseInteractiveObject>();
            }
            if (rigidbodyCmp == null)
            {
                rigidbodyCmp = gameObject.GetComponent<Rigidbody>();
            }
        }

        private void ToggleISControllable()
        {
            isControllable = isControllable ? false : true;
        }

        void Awake()
        {
            isSelected = false;
        }

        void Update()
        {
            state = interactiveObject.State;

            if (state == BaseInteractiveObject.ObjectState.Selected)
            {
                isSelected = true;
            }
            else
            {
                isSelected = false;
            }

            if (isSelected && state != stateLastFrame)
            {
                if (selectSound != null)
                {
                    GameSound.Instance.PlayOneShot(selectSound);
                }
            }

            stateLastFrame = interactiveObject.State;
        }

        void FixedUpdate()
        {
            if (isControllable)
            {
                if (state == BaseInteractiveObject.ObjectState.Selected)
                {
                    Vector3 relativePos = lookPos - rigidbodyCmp.position;
                    Quaternion playerFacing = Quaternion.LookRotation(-relativePos);
                    targetRotation = rigidbodyCmp.rotation;
                    targetRotation = Quaternion.Slerp(targetRotation, playerFacing, Time.deltaTime);
                    rigidbodyCmp.MoveRotation(targetRotation);
                }
            }
        }
    }
}
