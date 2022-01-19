using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhenomenalViborg.MUCOSDK
{
    [RequireComponent(typeof(CharacterController))]
    public class MUCODesktopFPPController : MonoBehaviour
    {
        // public AnimationCurve Bobcurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f), new Keyframe(1.5f, -1f), new Keyframe(2f, 0f)); // sin curve for head bob
        [SerializeField] private float m_WalkSpeed = 5.0f;
        [SerializeField] private Camera m_Camera;
        [SerializeField] private float m_Sensitivity = 2.0f; // This should properly be moved to some sort of player settings system.

        private CharacterController m_CharacterController;

        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            // Update rotation
            float inputX = Input.GetAxis("Mouse Y") * m_Sensitivity;
            float inputY = Input.GetAxis("Mouse X") * m_Sensitivity;

            transform.rotation *= Quaternion.Euler(0.0f, inputY, 0.0f);
            m_Camera.transform.rotation *= Quaternion.Euler(-inputX, 0.0f, 0.0f);

            // Update translation
            Vector3 moveDirection = m_Camera.transform.forward * Input.GetAxis("Vertical") + m_Camera.transform.right * Input.GetAxis("Horizontal");
            moveDirection.y = 0.0f;
            moveDirection = moveDirection.normalized * m_WalkSpeed;

            CollisionFlags m_CollisionFlags = m_CharacterController.Move(moveDirection * Time.deltaTime);
        }
    }
}