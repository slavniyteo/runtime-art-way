namespace RuntimeArtWay.Circuit
{
    public class Candidate
    {
        public Point Point { get; private set; }
        public float Angle { get; private set; }

        public Candidate(Point point, float angle)
        {
            Point = point;
            Angle = angle;
        }

        public override string ToString()
        {
            return $"{Point} => {Angle}";
        }
    }
}