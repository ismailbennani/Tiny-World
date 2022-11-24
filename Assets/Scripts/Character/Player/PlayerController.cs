namespace Character.Player
{
    public class PlayerController: ThirdPersonController
    {
        public static PlayerController Instance { get; private set; }

        void OnEnable()
        {
            Instance = this;
        }
    }
}
