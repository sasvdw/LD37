namespace LD37.Domain.Movement
{
    public struct Direction
    {
        private readonly DirectionEnum direction;

        private Direction(DirectionEnum direction)
        {
            this.direction = direction;
        }
        public static Direction North => new Direction(DirectionEnum.North);
        public static Direction East => new Direction(DirectionEnum.East);
        public static Direction West => new Direction(DirectionEnum.South);
        public static Direction South => new Direction(DirectionEnum.West);

        public Direction Opposite
        {
            get
            {
                var current = (int)this.direction;

                var opposite = current - 2;

                if(opposite < 0)
                {
                    return new Direction((DirectionEnum)(opposite + 4));
                }

                return new Direction((DirectionEnum)opposite);
            }
        }

        private enum DirectionEnum
        {
            North,
            East,
            South,
            West
        }

        public override bool Equals(object obj)
        {
            return obj is Direction && this == (Direction)obj;
        }

        public static bool operator ==(Direction left, Direction right)
        {
            return left.direction == right.direction;
        }

        public static bool operator !=(Direction left, Direction right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return this.direction.GetHashCode();
        }

        public override string ToString()
        {
            return this.direction.ToString();
        }
    }
}