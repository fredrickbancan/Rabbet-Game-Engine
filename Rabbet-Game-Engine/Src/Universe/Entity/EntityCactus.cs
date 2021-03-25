using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using RabbetGameEngine.Physics;
namespace RabbetGameEngine
{
    public class EntityCactus : EntityLiving
    {
        private bool turningRight = false;
        private bool turningLeft = false;
        private bool walkingFowards = false;
		private float rotationSpeed = 2.5F;
        public EntityCactus(Planet planet) : base(planet)
        {
            this.walkSpeed = 0.025F;
            this.entityModel = new EntityCactusModel(this);
            this.hasModel = true;
            this.collider = new AABB(new Vector3(-0.5F, -0.5F, -0.5F), new Vector3(0.5F, 0.5F, 0.5F), this);
            this.hasCollider = true;
            yaw = (float)currentPlanet.rand.NextDouble() * 360;
            walkFowards();
        }
        public EntityCactus(Planet planet, Vector3 pos) : base(planet, pos)
        {
            this.walkSpeed = 0.025F;
            this.entityModel = new EntityCactusModel(this);
            this.hasModel = true;
            this.collider = new AABB(new Vector3(-0.5F, -0.5F, -0.5F), new Vector3(0.5F, 0.5F, 0.5F), this);
            this.hasCollider = true;
            yaw = (float)currentPlanet.rand.NextDouble() * 360;
            walkFowards();
        }

        public override void onTick()
        {
			if (currentPlanet.rand.Next() % 100 == 0)
			{
				walkingFowards = true;
			}

			if (walkingFowards && currentPlanet.rand.Next() % 20 == 0)
			{
				walkingFowards = false;
			}
			if (walkingFowards)
			{
				turningLeft = turningRight = false;
			}
			else
			{
				if (currentPlanet.rand.Next() % 100 == 0)
				{
					turningLeft = true;
				}
				if (turningLeft && currentPlanet.rand.Next() % 10 == 0)
				{
					turningLeft = false;
				}
				if (currentPlanet.rand.Next() % 100 == 0)
				{
					turningRight = true;
				}
				if (turningRight && currentPlanet.rand.Next() % 10 == 0)
				{
					turningRight = false;
				}
			}
			if (walkingFowards)
			{
				walkFowards();
			}

			if (turningLeft)
			{
				yaw -= rotationSpeed;
			}

			if (turningRight)
			{
				yaw += rotationSpeed;
			}
			base.onTick();//do last
        }
    }
}
