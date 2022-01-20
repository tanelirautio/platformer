using Unity;

namespace pf
{
    public class Timer
    {
        /// Elapsed time in seconds.
        public float Elapsed { get; private set; }
        public bool Running { get; private set; }

        public void Reset()
        {
            Elapsed = 0.0f;
            Running = false;
        }

        public void Start()
        {
            Elapsed = 0.0f;
            Running = true;
        }

        public void Stop()
        {
            Running = false;
        }

        public void Update(float deltaTime)
        {
            if (Running)
            {
                Elapsed += deltaTime;
            }
        }
    }

}
