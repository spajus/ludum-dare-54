using Godot;
using System;
using System.ComponentModel;

public partial class PlayerController : CharacterBody3D {
    private Vector3 crouchOffset;
    private Vector3 standOffset;
    private float holdDistance = Consts.BookHoldDist;

    private float mouseSensitivity = 0.1f;

    private Vector3 mouseRot;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    private float shootDistance = 1000f;
    private Camera3D camera;
    private RayCast3D sightRay;
    private Book carriedBook;
    private bool isRotMode;

    public override void _Ready() {
        camera = GetNode<Camera3D>(Nodes.Camera);
        standOffset = camera.Position;
        crouchOffset = standOffset * new Vector3(1f, 0.5f, 1f);
        sightRay = GetNode<RayCast3D>(Nodes.SightRay);
        Runtime.Timer.OnDone += DropBook;
    }

    public override void _PhysicsProcess(double delta) {
        if (Runtime.GameState == Game.State.Game) {
            isRotMode = Input.IsActionPressed("rot_mode");
            Velocity = HandleInput(Velocity, (float) delta);
            MoveAndSlide();
        }
        MoveCarriedBook();
        RaycastSight();
    }

    private void MoveCarriedBook() {
        if (carriedBook == null) { return; }
        var sightDir = sightRay.ToGlobal(sightRay.TargetPosition);
        var rayPos = sightRay.GlobalPosition;

        var wantedPos = rayPos + sightDir.Normalized() * holdDistance;

        Vector3 distanceToTarget = wantedPos - carriedBook.GlobalTransform.Origin;
        Vector3 force = Vector3.Zero;
        if (distanceToTarget.Length() > 0.001f)  {
            Vector3 direction = distanceToTarget;
            var len = distanceToTarget.Length();
            direction = direction.Normalized() * (len * len);
            force = direction * Consts.BookForceSpeed;
        }

        mouseRot.Z += Input.GetAxis("rot_left", "rot_right");
        if (mouseRot.LengthSquared() > 0.001f) {
            carriedBook.ApplyTorque(mouseRot * Consts.BookRotateSpeed);
            mouseRot = Vector3.Zero;
        }
        carriedBook.ApplyForce(force);
    }

    private void RaycastSight() {
        if (sightRay.IsColliding()) {
            Runtime.UI.SetCrosshairTint(Colors.Green);
        } else {
            Runtime.UI.SetCrosshairTint(Colors.White);
        }
    }

    private Vector3 HandleInput(Vector3 velocity, float delta) {

        if (!Runtime.IsFocused) { return velocity; }
        // Add the gravity.
        if (!IsOnFloor()) {
            velocity.Y -= gravity * delta;
        }

        var isOnFloor = IsOnFloor();

        // Handle Jump.
        if (isOnFloor && Input.IsActionJustPressed("jump")) {
            velocity.Y = Consts.JumpVelocity;
        }

        if (Input.IsActionPressed("spawn_book")) {
            DebugSpawnBook();
        }
        if (Input.IsActionJustPressed("interact")) {
            Interact();
        }

        if (isOnFloor) {
            if (Input.IsActionPressed("crouch")) {
                camera.Position = camera.Position.Lerp(crouchOffset, delta * Consts.CrouchSpeed);
            } else {
                camera.Position = camera.Position.Lerp(standOffset, delta * Consts.CrouchSpeed);
            }
        }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        if (isOnFloor) {
            Vector2 inputDir = Input.GetVector("left", "right", "forward", "back");
            Vector3 direction = (Transform.Basis *
                new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
            if (direction != Vector3.Zero) {
                velocity.X = direction.X * Consts.MoveSpeed;
                velocity.Z = direction.Z * Consts.MoveSpeed;
            } else {
                velocity.X = Mathf.MoveToward(Velocity.X, 0, Consts.MoveSpeed);
                velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Consts.MoveSpeed);
            }
        }
        return velocity;
    }
    private void Interact() {
        if (carriedBook != null) {
            DropBook();
            return;
        }
        if (!sightRay.IsColliding()) { return; }
        var bookCol = (Node3D) sightRay.GetCollider();
        var book = bookCol.GetNode<Book>(bookCol.GetPath());
        CarryBook(book);
        GD.Print(book);
    }

    private void CarryBook(Book book) {
        book.Carry();
        carriedBook = book;
    }

    private void DropBook() {
        carriedBook.Drop();
        carriedBook = null;
    }

    private void DebugSpawnBook() {
        var book = Runtime.BookPool.Spawn();
        var sightDir = sightRay.ToGlobal(sightRay.TargetPosition);
        var rayPos = sightRay.GlobalPosition;
        Runtime.Root.AddChild(book);
        book.GlobalPosition = rayPos;
        var rand = new Random();
        book.GlobalRotation = new Vector3(
                rand.NextSingle() * 180f,
                rand.NextSingle() * 180f,
                rand.NextSingle() * 180f);
        book.LinearVelocity = sightDir.Normalized() * Consts.BookDropSpeed;
    }


    public override void _Input(InputEvent evt) {
        if (!Runtime.IsFocused) { return; }
        if (Runtime.GameState != Game.State.Game) { return; }

        if (evt is InputEventMouseButton emb) {
            if (emb.IsPressed()) {
                if (carriedBook != null) {
                    var dir = 0f;
                    if (emb.ButtonIndex == MouseButton.WheelUp) {
                        dir = Consts.BookHoldDistChangeSpeed;
                    }
                    if (emb.ButtonIndex == MouseButton.WheelDown) {
                        dir = -Consts.BookHoldDistChangeSpeed;
                    }
                    if (dir != 0f) {
                        holdDistance = Mathf.Clamp(holdDistance + dir,
                            Consts.MinBookHoldDist, Consts.MaxBookHoldDist);
                    }
                }
            }
        }
        if (evt is InputEventMouseMotion eventMouseMotion) {
            if (isRotMode) {
                mouseRot = new Vector3(
                    eventMouseMotion.Relative.Y * mouseSensitivity,
                    eventMouseMotion.Relative.X * mouseSensitivity,
                    0f);
                return;
            }
            mouseRot = Vector3.Zero;
            // Rotate the player left and right
            RotationDegrees = new Vector3(
                RotationDegrees.X,
                RotationDegrees.Y - eventMouseMotion.Relative.X * mouseSensitivity,
                RotationDegrees.Z
            );

            // Access the Camera3D node to rotate it up and down
            camera.RotationDegrees = new Vector3(
                Mathf.Clamp(camera.RotationDegrees.X -
                    eventMouseMotion.Relative.Y * mouseSensitivity, -80, 80),
                camera.RotationDegrees.Y,
                camera.RotationDegrees.Z
            );
        }
    }
}
