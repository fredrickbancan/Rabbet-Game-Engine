﻿using OpenTK;

namespace RabbetGameEngine.Physics
{
    public struct PointCollider : ICollider
    {
        public Vector3 pos;
        public PositionalObject parent;

        public PointCollider(Vector3 pos, PositionalObject parent = null)
        {
            this.pos = pos;
            this.parent = parent;
        }
     
        public bool getHasParent()
        {
            return parent != null;
        }
        public void setParent(PositionalObject parent)
        {
            this.parent = parent;
        }
        public ICollider getNextTickPredictedHitbox()
        {
            if (parent != null)
            {
                PointCollider result = new PointCollider(pos);
                result.pos += parent.getPredictedNextTickPos();
                return result;
            }
            return this;
        }

        public int getCollisionWeight()
        {
            if (getHasParent())
            {
                return parent.getCollisionWeight();
            }

            return 0;
        }

        public PositionalObject getParent()
        {
            return parent;
        }

        public void onTick()
        {
            //replaced by new tryToMoveEntity()
           /* pos = parent.getPosition();*/
        }

        public Vector3 getCenterVec()
        {
            return pos;
        }

        public void offset(Vector3 vec)
        {
            pos += vec;
        }
        public ColliderType getType()
        {
            return ColliderType.point;
        }
    }
}
