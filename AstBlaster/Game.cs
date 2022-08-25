using Godot;
using System;

namespace AstBlaster
{
    /// <summary>
    /// Global game class
    /// </summary>
    public class Game : Node
    {
        /// <summary>
        /// Default place area size
        /// </summary>
        private static readonly Vector2 DefaultSize = new Vector2(1920f, 1080f);

        /// <summary>
        /// Game instance
        /// </summary>
        public static Game Instance;

        /// <summary>
        /// Play bounds
        /// </summary>
        [Export]
        public Rect2 Bounds { get; private set; } = new(-1f * DefaultSize / 2f, DefaultSize);

        /// <summary>
        /// ENTER TREE
        /// </summary>
        public override void _EnterTree()
        {
            if (Instance is not null)
            {
                GD.PrintErr($"{this} {Name} :: Tried to add second {nameof(Game)} instance");
            }
            else
            {
                Instance = this;
            }
        }

        /// <summary>
        /// EXIT TREE
        /// </summary>
        public override void _ExitTree()
        {
            Instance = null;
        }
    }
}
