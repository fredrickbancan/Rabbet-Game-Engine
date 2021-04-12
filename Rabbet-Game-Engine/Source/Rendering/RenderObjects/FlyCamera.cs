using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace RabbetGameEngine
{
    public class FlyCamera : Camera
    {
        private float moveSpeed = 5.0F;
        private Vector3 prevTickMovePos;
        private Vector3 movePos;
        private Vector3 moveVec;

        public FlyCamera(Vector3 pos) : base(pos)
        {
            movePos = pos;
            prevTickMovePos = pos;
        }

        public override void onFrame(float ptnt)
        { 
            //get input and update pitch,yaw and movevec here
            if (moveVec.X == 0.0F) moveVec.X = System.Convert.ToInt32(Input.keyIsDown(Keys.D)) - System.Convert.ToInt32(Input.keyIsDown(Keys.A));
            if (moveVec.Z == 0.0F) moveVec.Z = System.Convert.ToInt32(Input.keyIsDown(Keys.W)) - System.Convert.ToInt32(Input.keyIsDown(Keys.S));
            if (moveVec.Y == 0.0F) moveVec.Y = System.Convert.ToInt32(Input.keyIsDown(Keys.Space)) - System.Convert.ToInt32(Input.keyIsDown(Keys.LeftControl));
            if (moveVec != Vector3.Zero) moveVec.Normalize();

            pitch -= Input.getGrabbedMouseDelta().Y * GameSettings.mouseSensetivity.floatValue;
            yaw += Input.getGrabbedMouseDelta().X * GameSettings.mouseSensetivity.floatValue;

            camPos = MathUtil.lerp(prevTickMovePos, movePos, ptnt);
            base.onFrame(ptnt);
        }

        public override void onTick(float ts)
        {
            prevTickMovePos = movePos;
            movePos += camFrontVector * moveVec.Z * moveSpeed * ts;
            movePos += camRightVector * moveVec.X * moveSpeed * ts;
            movePos.Y += moveVec.Y * moveSpeed * ts;
            moveVec = Vector3.Zero;
            base.onTick(ts);
        }
    }
}
