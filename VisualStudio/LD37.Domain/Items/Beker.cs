namespace LD37.Domain.Items
{
    public sealed class Beker : Item
    {
        private static volatile Beker instance;
        private static object syncRoot = new object();

        public static Beker Instance
        {
            get
            {
                if(instance == null)
                {
                    lock(syncRoot)
                    {
                        if(instance == null)
                        {
                            instance = new Beker();
                        }
                    }
                }

                return instance;
            }
        }

        private Beker() : base("Beker") {}
    }
}