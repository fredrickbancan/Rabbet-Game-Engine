namespace RabbetGameEngine
{
    /*List of all possible player/EntityLiving actions requestable via keyboard and mouse input or game logic*/
    public enum EntityAction
    {
        none,
        fowards,
        strafeLeft,
        backwards,
        strafeRight,
        jump,
        attack,
        duck,
        sprint,
        interact,
    };
    class EntityLiving
    {
        public static readonly int actionsCount = System.Enum.GetNames(typeof(EntityAction)).Length;
    }
}
