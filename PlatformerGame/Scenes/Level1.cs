namespace PlatformerGame.Scenes
{
    /// <summary>
    /// Level 1 scene
    /// </summary>
    internal class Level1 : GameScene
    {
        // The name of the level
        public string levelName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="levelName"></param>
        public Level1(string levelName) : base()
        {
            this.levelName = levelName;
        }

        /// <summary>
        /// Called when the scene is initialized
        /// </summary>
        public override void Begin()
        {
            base.Begin();

            // Call the setup of the GameScene
            SetupScene($"Content/Maps/{levelName}.tmx");
        }
    }
}
