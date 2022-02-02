using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using UnityEngine.XR;
using UnityEngine;

namespace DashMonke
{
    public class DashManager : MonoBehaviour
    {
        private Rigidbody RB;
        private GorillaLocomotion.Player P;
        private float CachedMagnitude = 0;
        private bool Released = true;
        private bool Buffer = false;

        public Dash Pdash;
        public bool Dashing { get; private set; }
        public bool CanDash { get; private set; }
        public Vector3 Direction { get; private set; }
        void Awake()
        {
            RB = GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody;
            P = GorillaLocomotion.Player.Instance;
        }

        void OnEnable()
        {
            Console.WriteLine("DashManager Enabled");
            Pdash = DashSettings.dash;

            CanDash = true;
        }

        void OnDisable()
        {
            Console.WriteLine("DashManager Disabled");
            Pdash = new Dash();

            CanDash = false;
        }

        void Update()
        {
            bool pressed = false;

            XRNode Controller = DashSettings.UseRight ? XRNode.RightHand : XRNode.LeftHand;
            InputFeatureUsage<bool> Button = DashSettings.UsePrimary ? CommonUsages.primaryButton : CommonUsages.secondaryButton;

            if (!Buffer) InputDevices.GetDeviceAtXRNode(Controller).TryGetFeatureValue(Button, out pressed);
            InputDevices.GetDeviceAtXRNode(Controller).TryGetFeatureValue(Button, out Buffer);

            if (CanDash && pressed && !P.disableMovement)
            {
                Dash();
                Released = false;
            }

            if(Released && !pressed)
            {
                Released = true;
            }

            if (Dashing)
            {
                if(Pdash.cancelable && pressed && Released)
                {
                    EndDash(true);
                }
                  
                if (Pdash.controlled) Direction = P.headCollider.transform.forward;
                
                float targMag = Pdash.power + (Pdash.pm * CachedMagnitude);
                Vector3 targVel = Direction * (Mathf.Min(targMag * Pdash.duration, 30) / Pdash.duration); 

                RB.velocity = targVel;
            }

            if(P.IsHandTouching(false) || P.IsHandTouching(true))
            {
                if(Dashing) EndDash(false);
                
                if(Pdash.wait < 2) CanDash = true;
            }

            if (P.disableMovement)
            {
                if(Dashing) EndDash(false);
            }
        }

        void Dash()
        {
            CachedMagnitude = RB.velocity.magnitude;
            Direction = P.headCollider.transform.forward;
            CanDash = false;
            Dashing = true;

            StartCoroutine(DashTimer());
        }

        IEnumerator DashTimer()
        {
            yield return new WaitForSeconds(Pdash.duration);
            if(Dashing) EndDash(true);
        }

        void EndDash(bool ApplyBoost)
        {
            StopCoroutine(DashTimer());
            Dashing = false;

            if (ApplyBoost)
            {
                Vector3 targVel = Direction * (Pdash.ending + (Pdash.em * CachedMagnitude));
                targVel = Vector3.ClampMagnitude(targVel, 200);

                RB.velocity = targVel;
            }

            if (Pdash.wait >= 2) StartCoroutine(WaitTimer());
        }

        IEnumerator WaitTimer()
        {
            float wait = Pdash.wait;
            if (!RB.useGravity && Pdash.wait < 8) wait = 8f;
            yield return new WaitForSeconds(wait);
            CanDash = true;
        }
    }

    public class Dash
    {
        public Dash(float power, float ending, float duration, bool controlled, bool cancelable, float pm, float em, float wait)
        {
            this.power = power;
            this.ending = ending;
            this.duration = duration;

            this.controlled = controlled;
            this.cancelable = cancelable;

            this.pm = pm;
            this.em = em;

            this.wait = wait;
        }

        public Dash()
        {
            this.power = 0;
            this.ending = 0;
            this.duration = 0;

            this.controlled = false;
            this.cancelable = false;

            this.pm = 0;
            this.em = 0;

            this.wait = 0;
        }

        public float power;
        public float ending;
        public float duration;

        public bool controlled;
        public bool cancelable;

        public float pm;
        public float em;

        public float wait;
    }
}
