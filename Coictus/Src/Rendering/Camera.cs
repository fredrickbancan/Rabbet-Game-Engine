using OpenTK;
using OpenTK.Input;
using System;

namespace Coictus
{
    /*This class represents a virtual point at which the world should be rendered from. It takes in a player and will be bound to the players head.*/
    public class Camera
    {
        private float pitch, yaw;
        private int mouseDeltaX;
        private int mouseDeltaY;
        private Matrix4 viewMatrix;
        private Vector3 camUpVector;
        private Vector3 up;
        private Vector3 camFrontVector;
        private Vector3 camRightVector;
        private Vector3 camTargetVector;
        private Vector3 camDirectionVector;
        private EntityPlayer child;

        /*Class for a camera contpitched by a mouse. The camera will be attached to a player entity. The 
          camera movement will control the players yaw and head yaw, which will then contpitch which
          direction the player moves in.*/
        public Camera(EntityPlayer childEntity)
        {
            this.child = childEntity;
            this.yaw = child.getYaw();
            camTargetVector = new Vector3(0.0F);
            camDirectionVector = Vector3.Normalize(child.getEyePosition() - camTargetVector);
            up = new Vector3(0.0F, 1.0F, 0.0F);
            camRightVector = Vector3.Normalize(Vector3.Cross(up, camDirectionVector));
            camFrontVector = new Vector3(0.0F, 0.0F, -1.0F);
            camUpVector = Vector3.Cross(camDirectionVector, camRightVector);
            viewMatrix = Matrix4.LookAt(child.getEyePosition(), camTargetVector, up);
        }

        /*Called every FRAME (not tick), will update view matrix depending on interpolated player position.
          also updates mouse input and rotates camera dependingly.*/
        public void onUpdate()
        {
            updateMouseDeltas();
            Input.centerMouse();
            pitch -= mouseDeltaY * GameSettings.mouseSensitivity;
            yaw += mouseDeltaX * GameSettings.mouseSensitivity;

          //  if (yaw > 360.0F) { yaw = 0.0F; }
          //  if (yaw < -360.0F) { yaw = 0.0F; }
            /*cap yaw so cam and entity can not flip*/
            if (pitch >= 90.0F)
            {
                pitch = 89.999F;
            }
            else if (pitch <= -90.0F)
            {
                pitch = -89.999F;
            }

            //the camera controls the players entity yaw and headpitch
            child.setYaw(yaw);
            child.setHeadPitch(pitch);

            camDirectionVector.X =(float) (Math.Cos(MathUtil.radians(yaw)) * Math.Cos(MathUtil.radians(pitch)));
            camDirectionVector.Y =(float) Math.Sin(MathUtil.radians(pitch));
            camDirectionVector.Z =(float) (Math.Sin(MathUtil.radians(yaw)) * Math.Cos(MathUtil.radians(pitch)));
            camFrontVector = Vector3.Normalize(camDirectionVector);
            camRightVector = Vector3.Normalize(Vector3.Cross(up, camDirectionVector));
            camUpVector = Vector3.Cross(camDirectionVector, camRightVector);

            Vector3 parentLerpPos = child.getLerpEyePos();
            viewMatrix = Matrix4.LookAt(parentLerpPos, parentLerpPos + camFrontVector, camUpVector);
        }

        public void onTick()
        {
        }

        /*updates mouse input for camera*/
        private void updateMouseDeltas()
        {
            MouseState mouseState = Mouse.GetCursorState();

            /*compares the current curor pos to the center of screen location each frame. The cursor will then be reset to cetner after taking value.*/
            mouseDeltaX = mouseState.X - GameInstance.windowCenterX;
            mouseDeltaY = mouseState.Y - GameInstance.windowCenterY;
        }
        public Matrix4 getViewMatrix()
        {
            return this.viewMatrix;
        }

        public float getYaw()
        {
            return yaw;
        }
        public float getPitch()
        {
            return pitch;
        }
    }
}
