namespace Micro.Climb {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MicroClimbPlayer : MonoBehaviour
    {
        public MicroClimb microgame;
        public SFXManager sfx;
        public Rigidbody2D mainRb, hingeRb, maceRb;
        public HingeJoint2D hinge;
        public SliderJoint2D slider;
        public Transform maceHead;
        public SpriteRenderer spriteRenderer, maceSpriteRenderer;
        public float maxAngleForCalc = 720f;
        JointMotor2D hingeMotor, sliderMotor;
        float angleToCursor, distanceToCursor, cursorAngleDiff, lastDistanceToCursor, maceOffset, distanceLowerBound, distanceUpperBound;
        Vector2 macePosition, lastMousePosition;

        void Awake()
        {
            hingeMotor = hinge.motor;
            sliderMotor = slider.motor;
            maceOffset = ((Vector2)maceHead.transform.position - hingeRb.position).magnitude;
            distanceLowerBound = maceOffset - slider.limits.max;
            distanceUpperBound = maceOffset - slider.limits.min;
            lastMousePosition = Utils.GetMousePosition();
        }

        void Update()
        {
            spriteRenderer.flipX = Utils.ConvertReflexAngle(hingeRb.rotation - mainRb.rotation) <= -90f || Utils.ConvertReflexAngle(hingeRb.rotation - mainRb.rotation) > 90f ? true : false;
            maceSpriteRenderer.flipY = Utils.ConvertReflexAngle(hingeRb.rotation - mainRb.rotation) <= -90f || Utils.ConvertReflexAngle(hingeRb.rotation - mainRb.rotation) > 90f ? true : false;
        }

        void FixedUpdate()
        {
            distanceToCursor = Vector2.Dot(Utils.GetMousePosition() - (Vector2)maceHead.position, maceHead.right);
            sliderMotor.motorSpeed = 0;
            if ((distanceToCursor - lastDistanceToCursor > 0 && slider.jointTranslation > slider.limits.min + 0.1f && (Utils.GetMousePosition() - hingeRb.position).magnitude >= distanceLowerBound) || 
                (distanceToCursor - lastDistanceToCursor < 0 && slider.jointTranslation < slider.limits.max - 0.1f && (Utils.GetMousePosition() - hingeRb.position).magnitude <= distanceUpperBound))
            {
                sliderMotor.motorSpeed = -(distanceToCursor - lastDistanceToCursor) / Time.fixedDeltaTime;
            }
            slider.motor = sliderMotor;

            cursorAngleDiff = Vector2.Angle(Utils.GetMousePosition() - hingeRb.position, lastMousePosition - hingeRb.position);
            angleToCursor = Vector2.SignedAngle(Utils.GetMousePosition() - hingeRb.position, hinge.transform.right);
            hingeMotor.motorSpeed = angleToCursor / Time.fixedDeltaTime * Mathf.Clamp01(cursorAngleDiff / maxAngleForCalc / Time.fixedDeltaTime);
            hinge.motor = hingeMotor;

            lastDistanceToCursor = distanceToCursor;
            lastMousePosition = Utils.GetMousePosition();
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag != "Goal" || microgame.cleared || microgame.timer <= 0) return;
            microgame.cleared = true;
            microgame.sfx.PlaySFX(0);
            microgame.avatars[0].SetExpression(1);
        }
    }
}