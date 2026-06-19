using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;

namespace PlatformerGame.Components
{
    /// <summary>
    /// Player class that handles player movement, animations, and collision.
    /// </summary>
    internal class Player : Component, IUpdatable, ITriggerListener
    {
        private bool _isPaused = false;

        public Vector2 RespawnPosition;

        // Player Data
        public int DeathCount = 0;
        public int ItemScore = 0;
        public bool Staircase = false;
        public string Teleport = "item";

        // Physics
        public float MoveSpeed = 8f;
        public float Gravity = 1500;
        public float JumpHeight = 40;
        public float TimeSinceGrounded = 0; // Coyote Time
        public int MaxJumpCount = 2; // Double Jumps
        private int _jumps;

        private SpriteAnimator _animator;
        private TiledMapMover _mover;
        private BoxCollider _boxCollider;
        private TiledMapMover.CollisionState _collisionState = new TiledMapMover.CollisionState();
        private ColliderTriggerHelper _triggerHelper;
        private Vector2 _velocity;
        private VirtualButton _jumpInput;
        private VirtualIntegerAxis _xAxisInput;

        /// <summary>
        /// Constructor for the player class.
        /// </summary>
        /// <param name="respawnPosition"></param>
        public Player(Vector2 respawnPosition)
        {
            RespawnPosition = respawnPosition;
        }

        /// <summary>
        /// Set the spawnpoint of the player.
        /// </summary>
        /// <param name="spawnpoint"></param>
        public void SetSpawnpoint(string spawnpoint)
        {
            RespawnPosition = new Vector2(0, 0);
        }

        /// <summary>
        /// Trigger the teleporter.
        /// </summary>
        /// <param name="teleporterName"></param>
        public void TriggerTeleport(string teleporterName)
        {
            Debug.Log($"Player triggered teleporter: {teleporterName}");

            Teleport = teleporterName;
        }

        /// <summary>
        /// Kill the player.
        /// </summary>
        public void KillPlayer()
        {
            DeathCount++;
            Entity.Position = RespawnPosition;
        }

        /// <summary>
        /// Add a staircase to the player.
        /// </summary>
        public void AddStaircase()
        {
            Staircase = true;
        }

        /// <summary>
        /// Add an item score to the player.
        /// </summary>
        /// <param name="itemName"></param>
        public void AddItemScore(string itemName) // Item logic
        {
            switch (itemName)
            {
                case "5-score":
                    ItemScore += 5;
                    break;
                case "10-score":
                    ItemScore += 10;
                    break;
                case "double-score":
                    ItemScore *= 2;
                    break;
                case "triple-score":
                    ItemScore *= 3;
                    break;
                default:
                    break;
            }
        }

        // Not used, but required for ITriggerListener
        public void OnTriggerEnter(Collider other, Collider local)
        {
        }

        // Not used, but required for ITriggerListener
        public void OnTriggerExit(Collider other, Collider local)
        { }

        /// <summary>
        /// Called when the component is added to an entity. Initialises animations and input.
        /// </summary>
        public override void OnAddedToEntity()
        {
            #region Animation
            Entity.SetScale(new Vector2(0.75f, 0.75f));

            var texture = Entity.Scene.Content.LoadTexture("Player/CharacterSheet");
            var sprites = Sprite.SpritesFromAtlas(texture, 32, 32);

            _boxCollider = Entity.GetComponent<BoxCollider>();
            _mover = Entity.GetComponent<TiledMapMover>();
            _animator = Entity.AddComponent(new SpriteAnimator(sprites[0]));

            // Allows trigger listeners to be used.
            _triggerHelper = new ColliderTriggerHelper(Entity);

            // Extract the animations from the atlas
            _animator.AddAnimation("Walk", new[]
            {
                sprites[16],
                sprites[17],
                sprites[18],
                sprites[19]
            });

            _animator.AddAnimation("Run", new[]
            {
                sprites[24],
                sprites[25],
                sprites[26],
                sprites[27],
                sprites[28],
                sprites[29],
                sprites[30],
                sprites[31]
            });

            _animator.AddAnimation("Idle", new[]
            {
                sprites[0],
            });

            _animator.AddAnimation("Attack", new[]
            {
                sprites[64],
                sprites[65],
                sprites[66],
                sprites[67],
                sprites[68],
                sprites[69],
                sprites[70],
                sprites[71],
            });

            _animator.AddAnimation("Death", new[]
            {
                sprites[48],
                sprites[49],
                sprites[50]
            });

            _animator.AddAnimation("Falling", new[]
            {
                sprites[44]
            });

            _animator.AddAnimation("Hurt", new[]
            {
                sprites[48],
                sprites[49]
            });

            _animator.AddAnimation("Jumping", new[]
            {
                sprites[40],
                sprites[41],
                sprites[42],
                sprites[43],
                sprites[44],
                sprites[45],
                sprites[46],
                sprites[47]
            });

            #endregion

            SetupInput();
        }

        /// <summary>
        /// Called when the component is removed from an entity. Deregisters input.
        /// </summary>
        public override void OnRemovedFromEntity()
        {
            // Deregister virtual input
            _jumpInput.Deregister();
            _xAxisInput.Deregister();
        }

        /// <summary>
        /// Sets up the input for the player.
        /// </summary>
        private void SetupInput()
        {
            // Jumping Input, the sample uses Z for jumping, who in the world does that.
            _jumpInput = new VirtualButton();
            _jumpInput.Nodes.Add(new VirtualButton.KeyboardKey(Keys.Space));
            _jumpInput.Nodes.Add(new VirtualButton.GamePadButton(0, Buttons.A));

            // Horizontal Input
            _xAxisInput = new VirtualIntegerAxis();
            _xAxisInput.Nodes.Add(new VirtualAxis.GamePadDpadLeftRight());
            _xAxisInput.Nodes.Add(new VirtualAxis.GamePadLeftStickX());
            _xAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.A, Keys.D));
        }

        /// <summary>
        /// Updates the player's pause state.
        /// </summary>
        /// <param name="isPaused"></param>
        public void Update(bool isPaused)
        {
            _isPaused = isPaused;
        }

        /// <summary>
        /// Updates the player's movement and animations.
        /// </summary>
        void IUpdatable.Update()
        {
            // Movement and Animations
            var moveDir = new Vector2(_xAxisInput.Value, 0);
            string animation = null;

            if (!_isPaused)
            {
                if (moveDir.X < 0)
                {
                    if (_collisionState.Below)
                        animation = "Run";
                    _animator.FlipX = true;
                    _velocity.X = 1000 * -MoveSpeed * Time.DeltaTime;
                }
                else if (moveDir.X > 0)
                {
                    if (_collisionState.Below)
                        animation = "Run";
                    _animator.FlipX = false;
                    _velocity.X = 1000 * MoveSpeed * Time.DeltaTime;
                }
                else
                {
                    _velocity.X = 0;
                    if (_collisionState.Below)
                        animation = "Idle";
                }

                // Coyote time implementation
                if (_collisionState.Below)
                    TimeSinceGrounded = 0;
                else
                    TimeSinceGrounded += Time.DeltaTime;

                if (((_collisionState.Below | TimeSinceGrounded < 0.05) || _jumps < MaxJumpCount) && _jumpInput.IsPressed)
                {
                    animation = "Jumping";
                    _velocity.Y = -Mathf.Sqrt(2f * JumpHeight * Gravity);
                    _jumps++;
                }
                if (!_collisionState.Below && _velocity.Y > 0)
                    animation = "Falling";

                // Apply gravity and move.
                _velocity.Y += Gravity * Time.DeltaTime;
                if (_velocity.Y > 1000)
                    _velocity.Y = 1000;
                _mover.Move(_velocity * Time.DeltaTime, _boxCollider, _collisionState);

                // Update triggerHelper for collisions.
                _triggerHelper.Update();

                // Jump reset and head boinking
                if (_collisionState.Below)
                {
                    _velocity.Y = 0;
                    _jumps = 0;
                }
                if (_collisionState.Above)
                {
                    _velocity.Y = 0;
                }

                // Update the animation
                if (animation != null && !_animator.IsAnimationActive(animation))
                    _animator.Play(animation);
            }
        }
    }
}

