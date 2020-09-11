using Coictus.Models;
using Coictus.VFX;
using OpenTK;
using System;

namespace Coictus
{
    public class EntityTank : EntityVehicle
    {
        public static readonly double projectileHopupAngle = 7.2F;
        private double bodyYaw;
        private double barrelPitch;
        public static readonly double barrelLength = 6.75D;
        Random rand = new Random();
        public EntityTank() : base()
        {
            this.entityModel = new EntityTankModel(this);
            this.hasModel = true;
            this.setCollider(new AABBCollider(new Vector3d(-2.5, -1.25, -2.5), new Vector3d(2.5, 1.25, 2.5), this), 2);
        }
        public EntityTank(Vector3d initialPos) : base(initialPos)
        {
            this.entityModel = new EntityTankModel(this);
            this.hasModel = true;
            this.setCollider(new AABBCollider(new Vector3d(-2.5, -1.25, -2.5), new Vector3d(2.5, 1.25, 2.5), this), 2);
        }

        public override void onTick()
        {
            base.onTick();//do first
            if (mountingEntity != null && !playerDriving)
            {
                bodyYaw = mountingEntity.getYaw() + 90;
                barrelPitch = mountingEntity.getHeadPitch() + projectileHopupAngle;
            }
        }

        public override void onFrame()
        {
            //set body rotation to player cam yaw and barrel to cam pitch for frame perfect accuracy
            if(playerDriving)
            {
                bodyYaw = ((EntityPlayer)mountingEntity).getCamera().getYaw() + 90;
                barrelPitch = ((EntityPlayer)mountingEntity).getCamera().getPitch() + projectileHopupAngle;
            }
            base.onFrame();
        }

        /*Called by base ontick()*/
        /*When called, aligns vectors according to the entities state and rotations.*/
        protected override void alignVectors()
        {
            /*correcting front vector based on new pitch and yaw*/
            frontVector.X = (double)(Math.Cos(MathUtil.radians(yaw)));
            frontVector.Z = (double)(Math.Sin(MathUtil.radians(yaw)));
            frontVector.Normalize();
        }

        /*Called by base ontick()*/
        /*Changes velocity based on state and movement vector, movement vector is changed by movement functions such as walkFowards()*/
        protected override void moveByMovementVector()
        {
            //modify walk speed here i.e slows, speed ups etc
            double walkSpeedModified = driveSpeed;

            if (!isGrounded)walkSpeedModified = 0.02D;//reduce movespeed when jumping or mid air 


            //change velocity based on movement
            //movement vector is a unit vector.
            if(movementVector.Length > 0)
            movementVector.Normalize();//normalize vector so vehicle is same speed in any direction
            rotateYaw((movementVector.X > 0 ? 1 : (movementVector.X < 0 ? -1 : 0))  * turnRate);//rotate wheels, if reversing then rotate tank in opposite direction
            velocity += frontVector * movementVector.Z * walkSpeedModified;//fowards and backwards movement
            movementVector *= 0;//reset movement vector
        }

        /*called when player left clicks while driving this vehicle*/
        public override void onLeftClick()
        {
            Vector3d muzzleLocation = getMuzzleLocation();
            currentPlanet.spawnEntityInWorld(new EntityTankProjectile(muzzleLocation, getMuzzleFrontVector(), barrelPitch, bodyYaw));
            VFXUtil.doSmallSmokePuffEffect(currentPlanet, muzzleLocation, (float)barrelPitch, (float)bodyYaw);
        }

        private Vector3d getMuzzleLocation()
        {
            Matrix4 barrelLengthTranslationMatrix = Matrix4.CreateTranslation(new Vector3(0, 0, -(float)barrelLength)) * ((EntityTankModel)entityModel).getBarrelModelMatrix();
            Vector3d result = new Vector3d();
            result.X += barrelLengthTranslationMatrix.Row3.X;
            result.Y += barrelLengthTranslationMatrix.Row3.Y;
            result.Z += barrelLengthTranslationMatrix.Row3.Z;
            return result;
        }

        private Vector3d getMuzzleFrontVector()
        {
            Vector3d result = new Vector3d();
            result.X = (double)(Math.Cos(MathUtil.radians(mountingEntity.getYaw()))) * (double)(Math.Cos(MathUtil.radians(barrelPitch)));
            result.Y = (double)Math.Sin(MathUtil.radians(barrelPitch));
            result.Z = (double)(Math.Sin(MathUtil.radians(mountingEntity.getYaw()))) * (double)(Math.Cos(MathUtil.radians(barrelPitch)));
            result.Normalize();
            return result;
        }
        public override void rotateYaw(double amount)
        {
            base.rotateYaw(amount);
            this.mountingEntity.rotateYaw(amount);
        }
        public double getBodyYaw { get => bodyYaw;}
        public double getBarrelPitch { get => barrelPitch;}
    }
}
