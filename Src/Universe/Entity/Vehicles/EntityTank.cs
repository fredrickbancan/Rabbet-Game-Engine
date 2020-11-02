using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using RabbetGameEngine.Physics;
using RabbetGameEngine.Sound;
using RabbetGameEngine.VisualEffects;
using System;
namespace RabbetGameEngine
{
    public class EntityTank : EntityVehicle
    {
        public static readonly float projectileHopupAngle = 7.2F;
        public static readonly float roundsPerSecond = 6F;//fire rate of tank
        private float bodyYaw;
        private float barrelPitch;
        public static readonly float barrelLength = 6.75F;
        private TickTimer fireTimer;
        public EntityTank(Planet p) : base(p)
        {
            this.entityModel = new EntityTankModel(this);
            this.hasModel = true;
            this.collider = new AABB(new Vector3(-2.5F, -1.25F, -2.5F), new Vector3(2.5F, 1.25F, 2.5F), this);
            this.hasCollider = true;
            fireTimer = new TickTimer(1F / roundsPerSecond);
        }
        public EntityTank(Planet p, Vector3 initialPos) : base(p, initialPos)
        {
            this.entityModel = new EntityTankModel(this);
            this.hasModel = true;
            this.collider = new AABB(new Vector3(-2.5F, -1.25F, -2.5F), new Vector3(2.5F, 1.25F, 2.5F), this);
            this.hasCollider = true;
            fireTimer = new TickTimer(1F / roundsPerSecond);
        }

        public override void onTick()
        {
            if (mountingEntity != null)
            {
                if (!playerDriving)
                {
                    bodyYaw = mountingEntity.getYaw() + 90;
                    barrelPitch = mountingEntity.getHeadPitch() + projectileHopupAngle;
                }
                fireTimer.doFunctionAtIntervalOnTick(onDriverAttack, mountingEntity.doingAction(Action.attack));
            }
            base.onTick();
        }

        public override void onFrame()
        {
            //set body rotation to player cam yaw and barrel to cam pitch for frame perfect accuracy
            if(playerDriving)
            {
                bodyYaw = ((EntityPlayer)mountingEntity).getCamera().getYaw() + 90;
                barrelPitch = ((EntityPlayer)mountingEntity).getCamera().getPitch() + projectileHopupAngle;

                if(barrelPitch >= 90)//clamp barrel pitch to only be able to look directly up
                {
                    barrelPitch = 89.999F;
                }
            }
            base.onFrame();
        }

        /*Called by base ontick()*/
        /*When called, aligns vectors according to the entities state and rotations.*/
        protected override void alignVectors()
        {
            /*correcting front vector based on new pitch and yaw*/
            frontVector.X = (float)(Math.Cos(MathUtil.radians(yaw)));
            frontVector.Z = (float)(Math.Sin(MathUtil.radians(yaw)));
            frontVector.Normalize();
        }

        /*Called by base ontick()*/
        /*Changes velocity based on state and movement vector, movement vector is changed by movement functions such as walkFowards()*/
        protected override void moveByMovementVector()
        {
            //modify walk speed here i.e slows, speed ups etc
            float walkSpeedModified = driveSpeed;

            if (!isGrounded)walkSpeedModified = 0.02F;//reduce movespeed when jumping or mid air 


            //change velocity based on movement
            //movement vector is a unit vector.
            if(movementVector.Length > 0)
            movementVector.Normalize();//normalize vector so vehicle is same speed in any direction
            rotateYaw((movementVector.X > 0 ? 1 : (movementVector.X < 0 ? -1 : 0))  * turnRate);//rotate wheels, if reversing then rotate tank in opposite direction
            velocity += frontVector * movementVector.Z * walkSpeedModified;//fowards and backwards movement
            movementVector *= 0;//reset movement vector
        }

        /*called when player left clicks while driving this vehicle*/
        public void onDriverAttack()
        {
            Vector3 muzzleLocation = getMuzzleLocation();
            SoundManager.playSound("tankfire", 1.0F, 1.0F - (float)GameInstance.rand.NextDouble() * 0.1F);
            currentPlanet.spawnEntityInWorld(new EntityTankProjectile(currentPlanet, muzzleLocation, getMuzzleFrontVector(), barrelPitch, bodyYaw));
            VFXUtil.doSmallSmokePuffEffect(currentPlanet, muzzleLocation, (float)barrelPitch, (float)bodyYaw);
        }

        private Vector3 getMuzzleLocation()
        {
            Matrix4 barrelLengthTranslationMatrix = Matrix4.CreateTranslation(new Vector3(0, 0, -(float)barrelLength)) * ((EntityTankModel)entityModel).getBarrelModelMatrix();
            Vector3 result = new Vector3();
            result.X += barrelLengthTranslationMatrix.Row3.X;
            result.Y += barrelLengthTranslationMatrix.Row3.Y;
            result.Z += barrelLengthTranslationMatrix.Row3.Z;
            return result;
        }

        private Vector3 getMuzzleFrontVector()
        {
            Vector3 result = new Vector3();
            result.X = (float)(Math.Cos(MathUtil.radians(mountingEntity.getYaw()))) * (float)(Math.Cos(MathUtil.radians(barrelPitch)));
            result.Y = (float)Math.Sin(MathUtil.radians(barrelPitch));
            result.Z = (float)(Math.Sin(MathUtil.radians(mountingEntity.getYaw()))) * (float)(Math.Cos(MathUtil.radians(barrelPitch)));
            result.Normalize();
            return result;
        }
        public override void rotateYaw(float amount)
        {
            base.rotateYaw(amount);
            this.mountingEntity.rotateYaw(amount);
        }
        public float getBodyYaw { get => bodyYaw;}
        public float getBarrelPitch { get => barrelPitch;}
    }
}
